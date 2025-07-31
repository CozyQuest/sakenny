using sakenny.Application.DTO;

namespace sakenny.Application.Interfaces
{
    public interface ICheckoutService
    {
        Task<PropertyCheckoutDTO> getPropertyByIdAsync(int id);
        Task<string> CreateCheckoutSessionAsync(RentingCheckoutDTO rentingCheckoutDTO, string userId);
        Task<bool> ProcessWebhookAsync(string json, string stripeSignature, string endpointSecret);
    }
}
