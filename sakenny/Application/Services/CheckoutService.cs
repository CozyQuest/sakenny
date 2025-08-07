using AutoMapper;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;
using Stripe.Checkout;
using Stripe;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace sakenny.Application.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CheckoutService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PropertyCheckoutDTO> getPropertyByIdAsync(int id)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(id, p => p.Rentings, propa => propa.Reviews);
            if (property == null)
            {
                throw new KeyNotFoundException($"Property with ID {id} was not found.");
            }
            var propertyCheckoutDTO = _mapper.Map<PropertyCheckoutDTO>(property);
            return propertyCheckoutDTO;
        }
        public async Task<string> CreateCheckoutSessionAsync(RentingCheckoutDTO rentingCheckoutDTO, string userId)
        {
            try
            {
                var property = await _unitOfWork.Properties.GetByIdAsync(rentingCheckoutDTO.PropertyId, p => p.Images, p => p.Rentings);
                if (property == null)
                {
                    throw new KeyNotFoundException($"Property with ID {rentingCheckoutDTO.PropertyId} was not found.");
                }
                rentingCheckoutDTO.StartDate = rentingCheckoutDTO.StartDate.AddDays(1);

                // Ensure Start Date and End date does not overlap with existing rentals for the property.Rentings
                var existingRentals = property.Rentings
                    .Where(r => r.StartDate < rentingCheckoutDTO.EndDate && r.EndDate > rentingCheckoutDTO.StartDate)
                    .ToList();
                if (existingRentals.Any())
                {
                    throw new InvalidOperationException("The selected dates overlap with existing rentals for this property.");
                }


                rentingCheckoutDTO.EndDate = rentingCheckoutDTO.EndDate.AddDays(1);

                // Calculate number of days
                var numberOfDays = (rentingCheckoutDTO.EndDate - rentingCheckoutDTO.StartDate).Days;
                if (numberOfDays <= 0)
                {
                    throw new ArgumentException("End date must be after start date.");
                }

                // Calculate total amount
                var totalAmount = property.Price * numberOfDays;

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long) (totalAmount * 100),
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"Property Rental - {property.Title}",
                                    Description = $"Rental period: {rentingCheckoutDTO.StartDate:yyyy-MM-dd} to {rentingCheckoutDTO.EndDate:yyyy-MM-dd} ({numberOfDays} days at ${property.Price}/day)",
                                    Images = property.Images?.Select(img => img.Url).ToList() ?? new List<string>()
                                }
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = "http://localhost:4200/payment-success",
                    CancelUrl = "http://localhost:4200/payment-cancel",


                    // Add metadata for tracking
                    Metadata = new Dictionary<string, string>
                {
                    {"property_id", rentingCheckoutDTO.PropertyId.ToString()},
                    {"user_id", userId},
                    {"start_date", rentingCheckoutDTO.StartDate.ToString("yyyy-MM-dd")},
                    {"end_date", rentingCheckoutDTO.EndDate.AddDays(-1).ToString("yyyy-MM-dd")},
                    {"number_of_days", numberOfDays.ToString()},
                    {"price_per_day", property.Price.ToString()},
                    {"total_amount", totalAmount.ToString()},
                    {"booking_reference", Guid.NewGuid().ToString()}
                },
                    // Billing address collection
                    BillingAddressCollection = "required",

                    // Payment intent data for additional configuration
                    PaymentIntentData = new SessionPaymentIntentDataOptions
                    {
                        Metadata = new Dictionary<string, string>
                    {
                        {"property_rental", "true"},
                        {"property_id", rentingCheckoutDTO.PropertyId.ToString()},
                        {"user_id", userId}
                    }
                    },
                    ExpiresAt = DateTime.UtcNow.AddHours(12),
                };
                var service = new SessionService();
                var session = await service.CreateAsync(options);
                return session.Url;
            }
            catch (StripeException ex)
            {
                throw new InvalidOperationException($"Stripe error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create checkout session. {ex.Message}", ex);
            }
        }
        public async Task<bool> ProcessWebhookAsync(string json, string stripeSignature, string endpointSecret)
        {
            try
            {
                // Verify the webhook signature
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, endpointSecret);

                // Handle the event based on type
                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        await HandleCheckoutSessionCompleted(stripeEvent);
                        return true;

                    default:
                        return true; // Still return true as it's not an error
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
        {
            var session = stripeEvent.Data.Object as Session;

            if (session?.PaymentStatus == "paid")
            {
                try
                {
                    // Extract metadata from the session
                    var metadata = session.Metadata;

                    if (TryExtractRentalDataFromMetadata(metadata, out Renting rentalData))
                    {
                        // Create the rental record
                        try
                        {
                            await _unitOfWork.Rentings.AddAsync(rentalData);
                            await _unitOfWork.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }

                        // TODO: Send confirmation email
                        // await _emailService.SendBookingConfirmationAsync(rentalData.UserId, rentalId);
                    }
                }
                catch (Exception ex)
                {
                    throw; // Re-throw to let calling code handle the error
                }
            }
        }
        private bool TryExtractRentalDataFromMetadata(Dictionary<string, string> metadata, out Renting rentalData)
        {
            rentalData = null;

            if (metadata.TryGetValue("property_id", out var propertyIdStr) &&
                metadata.TryGetValue("user_id", out var userId) &&
                metadata.TryGetValue("start_date", out var startDateStr) &&
                metadata.TryGetValue("end_date", out var endDateStr) &&
                metadata.TryGetValue("total_amount", out var totalAmountStr))
            {
                if (int.TryParse(propertyIdStr, out var propertyId) &&
                    DateTime.TryParse(startDateStr, out var startDate) &&
                    DateTime.TryParse(endDateStr, out var endDate) &&
                    decimal.TryParse(totalAmountStr, out var totalAmount))
                {
                    rentalData = new Renting
                    {
                        PropertyId = propertyId,
                        UserId = userId,
                        StartDate = startDate,
                        EndDate = endDate,
                        TotalPrice = totalAmount,
                    };
                    return true;
                }
            }
            return false;
        }
    }
}
