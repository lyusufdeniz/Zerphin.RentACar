using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Rentals.GetRentalById;

public class GetRentalByIdResponse
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? LateFee { get; set; }
    public decimal? DamageFee { get; set; }
    public RentalStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? PickupLocation { get; set; }
    public string? ReturnLocation { get; set; }
    public int? KmAtStart { get; set; }
    public int? KmAtReturn { get; set; }
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public Guid VehicleId { get; set; }
    public string? VehicleBrand { get; set; }
    public string? VehicleModel { get; set; }
    public string? VehicleLicensePlate { get; set; }
}


