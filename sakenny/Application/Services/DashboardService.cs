using Microsoft.EntityFrameworkCore;
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
            var approvedProperties = await _unit.Properties.GetAllAsync(
                filter: p => !p.IsDeleted && p.Status == PropertyStatus.Approved
            );

            return new List<StatDTO>
            {
                new() { Title = "Total Revenue", Value = rentings.Sum(r => r.TotalPrice), Currency = "$", Icon = "currency-usd" },
                new() { Title = "Total Users", Value = users.Count(), Icon = "account-group-outline" },
                new() { Title = "Total Properties", Value = properties.Count(), Icon = "home-city-outline" },
                new() { Title = "Properties for Rent", Value = approvedProperties.Count(), Icon = "home-clock-outline" },
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
                    p => p.PropertySnapshot.PropertyType,
                    p => p.PropertySnapshot.User
                ]
            );

            return pending.Select(p => new PendingRequestDTO
            {
                Id = p.id,
                Img = p.PropertySnapshot.MainImageUrl,
                Title = p.PropertySnapshot.Title,
                Type = p.PropertySnapshot.PropertyType.Name,
                Location = $"{p.PropertySnapshot.Country}, {p.PropertySnapshot.City}",
                Price = p.PropertySnapshot.Price,
                OwnerName = $"{p.PropertySnapshot.User.FirstName} {p.PropertySnapshot.User.LastName}"
            }).ToList();
        }

        public async Task<bool> ApproveRequestAsync(int permitId)
        {
            var permits = await _unit.PropertyPermits.GetAllAsync(
                p => p.id == permitId,
                includes: p => p.Property
            );

            var permit = permits.FirstOrDefault();
            if (permit == null) return false;

            permit.status = PropertyStatus.Approved;

            if (permit.Property != null)
            {
                permit.Property.Status = PropertyStatus.Approved;
            }

            await _unit.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectRequestAsync(int permitId)
        {
            var permits = await _unit.PropertyPermits.GetAllAsync(
                p => p.id == permitId,
                includes: p => p.Property
            );

            var permit = permits.FirstOrDefault();
            if (permit == null) return false;

            permit.status = PropertyStatus.Rejected;

            if (permit.Property != null)
            {
                permit.Property.Status = PropertyStatus.Rejected;
            }

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
            var markers = await _unit.Properties.GetAllAsync(p => !p.IsDeleted && p.Status == PropertyStatus.Approved && p.Latitude != 0 && p.Longitude != 0);

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
            var reviews = await _unit.Reviews.GetAllAsync(
                includes: [r => r.Property]
            );

            var properties = await _unit.Properties.GetAllAsync();

            // Join reviews to properties and calculate average rating
            var top = reviews
                .Where(r => r.Property != null)
                .GroupBy(r => r.PropertyId)
                .Select(g => new
                {
                    PropertyID = g.Key,
                    AvgRating = g.Average(r => r.Rate)
                })
                .Join(properties,
                      rating => rating.PropertyID,
                      property => property.Id,
                      (rating, property) => new { property, rating.AvgRating })
                .Where(x => !x.property.IsDeleted && x.property.Status == PropertyStatus.Approved)
                .OrderByDescending(x => x.AvgRating)
                .Take(5)
                .Select(x => new TopPropertyDTO
                {
                    Name = x.property.Title,
                    Location = $"{x.property.City}, {x.property.Country}",
                    Img = x.property.MainImageUrl,
                    TotalRating = Math.Round(x.AvgRating, 1)
                })
                .ToList();

            return top;
        }

        public async Task<IEnumerable<TransactionDTO>> GetRecentTransactionsAsync(string? hostId = null)
        {
            var filter = hostId is null
                ? (Expression<Func<Renting, bool>>)(r => !r.Property.IsDeleted && r.Property.Status == PropertyStatus.Approved)
                : r => !r.Property.IsDeleted && r.Property.Status == PropertyStatus.Approved && r.Property.UserId == hostId;

            var transactions = await _unit.Rentings.GetAllAsync(
                filter: filter,
                orderBy: q => q.OrderByDescending(r => r.TransactionDate),
                includes: [r => r.User, r => r.Property, r => r.Property.User]
            );

            return transactions.Take(5).Select(r => new TransactionDTO
            {
                Img = r.Property?.MainImageUrl ?? "assets/images/property/default.jpg",
                Title = r.Property?.Title ?? "Untitled",
                Location = $"{r.Property?.City ?? "Unknown"}, {r.Property?.Country ?? ""}".TrimEnd(',', ' '),
                GuestName = $"{r.User?.FirstName ?? "Unknown"} {r.User?.LastName}".Trim(),
                HostName = $"{r.Property?.User?.FirstName ?? "Unknown"} {r.Property?.User?.LastName}".Trim(),
                Price = $"{r.TotalPrice:C0}",
                Date = r.TransactionDate.ToString("dd MMM yyyy")
            }).ToList();
        }
    }
}
