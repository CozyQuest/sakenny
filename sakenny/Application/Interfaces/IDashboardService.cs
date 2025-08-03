using sakenny.Application.DTO;

namespace sakenny.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<IEnumerable<StatDTO>> GetStatsAsync();
        Task<RevenueChartDTO> GetRevenueChartAsync(string period);
        Task<IEnumerable<PendingRequestDTO>> GetPendingRequestsAsync();
        Task<bool> ApproveRequestAsync(int permitId);
        Task<bool> RejectRequestAsync(int permitId);
        Task<IEnumerable<SalesBreakdownDTO>> GetSalesBreakdownAsync(string period);
        Task<IEnumerable<MapMarkerDTO>> GetMapMarkersAsync();
        Task<IEnumerable<TopPropertyDTO>> GetTopPropertiesAsync();
        Task<IEnumerable<TransactionDTO>> GetRecentTransactionsAsync(string? hostId = null);
    }
}
