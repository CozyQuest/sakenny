using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;
using System.Globalization;
using System.Linq.Expressions;

namespace sakenny.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unit;

        public DashboardService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<IEnumerable<StatDTO>> GetStatsAsync()
        {
            var rentings = await _unit.Rentings.GetAllAsync();
            var users = await _unit.Users.GetAllAsync();
            var properties = await _unit.Properties.GetAllAsync();
            var propertyPermits = await _unit.PropertyPermits.GetAllAsync();

            return new List<StatDTO>
            {
                new() { Title = "Total Revenue", Value = rentings.Sum(r => r.TotalPrice), Currency = "$", Icon = "currency-usd" },
                new() { Title = "Total Users", Value = users.Count(), Icon = "account-group-outline" },
                new() { Title = "Total Properties", Value = properties.Count() + propertyPermits.Count(), Icon = "home-city-outline" },
                new() { Title = "Properties for Rent", Value = properties.Count(p => !p.IsDeleted), Icon = "home-clock-outline" },
                new() { Title = "Total Rentings", Value = rentings.Count(), Icon = "home-lock" },
            };
        }

        public async Task<RevenueChartDTO> GetRevenueChartAsync(string period)
        {
            var rentings = await _unit.Rentings.GetAllAsync();

            List<RevenuePointDTO> grouped = (period switch
            {
                "Y" => rentings
                    .GroupBy(r => r.TransactionDate.Year)
                    .Select(g => new RevenuePointDTO
                    {
                        Label = g.Key.ToString(),
                        Count = g.Count(),
                        Total = g.Sum(r => r.TotalPrice)
                    }),

                "M" => rentings
                    .Where(r => r.TransactionDate.Year == DateTime.Now.Year)
                    .GroupBy(r => r.TransactionDate.Month)
                    .Select(g => new RevenuePointDTO
                    {
                        Label = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                        Count = g.Count(),
                        Total = g.Sum(r => r.TotalPrice)
                    }),

                "W" => rentings
                    .Where(r => r.TransactionDate >= DateTime.Now.AddDays(-7))
                    .GroupBy(r => r.TransactionDate.DayOfWeek)
                    .Select(g => new RevenuePointDTO
                    {
                        Label = g.Key.ToString(),
                        Count = g.Count(),
                        Total = g.Sum(r => r.TotalPrice)
                    }),

                "T" => rentings
                    .Where(r => r.TransactionDate.Date == DateTime.Today)
                    .GroupBy(r => r.TransactionDate.Hour)
                    .Select(g => new RevenuePointDTO
                    {
                        Label = $"{g.Key}:00",
                        Count = g.Count(),
                        Total = g.Sum(r => r.TotalPrice)
                    }),

                _ => Enumerable.Empty<RevenuePointDTO>()
            }).OrderBy(p => p.Label).ToList();

            return new RevenueChartDTO
            {
                Labels = grouped.Select(x => x.Label),
                Datasets = new object[]
                {
                    new { label = "No. of Rentings", data = grouped.Select(x => (double)x.Count) },
                    new { label = "Revenue", data = grouped.Select(x => (double)x.Total) }
                }
            };
        }

        public async Task<IEnumerable<PendingRequestDTO>> GetPendingRequestsAsync()
        {
            var pending = await _unit.PropertyPermits.GetAllAsync(
                filter: p => p.status == PropertyStatus.Pending,
                includes:
                [
                    p => p.PropertySnapshot,
                    p => p.PropertySnapshot.PropertyType
                ]
            );

            return pending.Select(p => new PendingRequestDTO
            {
                Id = p.id,
                Img = p.PropertySnapshot.MainImageUrl,
                Title = p.PropertySnapshot.Title,
                Date = p.PropertySnapshot.CreatedAt.ToString("yyyy-MM-dd"),
                Price = p.PropertySnapshot.Price,
                Type = p.PropertySnapshot.PropertyType.Name
            }).ToList();
        }

        public async Task<bool> ApproveRequestAsync(int permitId)
        {
            var permit = await _unit.PropertyPermits.GetByIdAsync(permitId);
            if (permit == null) return false;

            permit.status = PropertyStatus.Approved;
            await _unit.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectRequestAsync(int permitId)
        {
            var permit = await _unit.PropertyPermits.GetByIdAsync(permitId);
            if (permit == null) return false;

            permit.status = PropertyStatus.Rejected;
            await _unit.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SalesBreakdownDTO>> GetSalesBreakdownAsync(string period)
        {
            var rentings = await _unit.Rentings.GetAllAsync();

            var filtered = period switch
            {
                "Y" => rentings,
                "M" => rentings.Where(r => r.TransactionDate.Year == DateTime.Now.Year),
                "W" => rentings.Where(r => r.TransactionDate >= DateTime.Today.AddDays(-7)),
                "T" => rentings.Where(r => r.TransactionDate.Date == DateTime.Today),
                _ => []
            };

            var total = filtered.Count();
            if (total == 0) return [];

            return filtered.GroupBy(r =>
            {
                if (int.TryParse(r.UserId, out var id))
                {
                    if (id < 1000) return "Website";
                    else if (id < 2000) return "Agent";
                }
                return "Other";
            })
            .Select(g => new SalesBreakdownDTO
            {
                Label = $"Via {g.Key}",
                Percent = Math.Round(g.Count() * 100.0 / total, 2)
            }).ToList();
        }

        public async Task<IEnumerable<MapMarkerDTO>> GetMapMarkersAsync()
        {
            var markers = await _unit.Properties.GetAllAsync(p => !p.IsDeleted && p.Latitude != 0 && p.Longitude != 0);

            return markers
                .GroupBy(p => p.City)
                .Select(g => new MapMarkerDTO
                {
                    Name = g.Key,
                    Coords = new[] { (double)g.First().Latitude, (double)g.First().Longitude }
                }).ToList();
        }

        public async Task<IEnumerable<TopPropertyDTO>> GetTopPropertiesAsync()
        {
            var all = await _unit.Properties.GetAllAsync();

            return all
                .Where(p => !p.IsDeleted && p.Reviews?.Any() == true)
                .OrderByDescending(p => p.Reviews!.Average(r => r.Rate))
                .Take(5)
                .Select(p => new TopPropertyDTO
                {
                    Name = p.Title,
                    Location = $"{p.City}, {p.Country}",
                    Img = p.MainImage.Url,
                    TotalRating = p.Reviews!.Average(r => r.Rate)
                }).ToList();
        }

        public async Task<IEnumerable<TransactionDTO>> GetRecentTransactionsAsync()
        {
            var transactions = await _unit.Rentings.GetAllAsync(
                filter: r => !r.Property.IsDeleted,
                orderBy: q => q.OrderByDescending(r => r.TransactionDate),
                includes: [r => r.User, r => r.Property]
            );

            return transactions.Take(5).Select(r => new TransactionDTO
            {
                Img = r.Property?.MainImage.Url ?? "assets/images/property/default.jpg",
                Date = r.TransactionDate.ToString("dd MMM yyyy"),
                Name = $"{r.User?.FirstName ?? "Unknown"} {r.User?.LastName}",
                Price = $"{r.TotalPrice:C0}",
                Type = "Rent",
                Status = r.TotalPrice > 10000 ? "Unpaid" : "Paid"
            }).ToList();
        }
    }
}
