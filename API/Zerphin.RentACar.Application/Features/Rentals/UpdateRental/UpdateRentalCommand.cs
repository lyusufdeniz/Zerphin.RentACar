using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Rentals.UpdateRental;

public class UpdateRentalCommand : IRequest<ServiceResult<UpdateRentalResponse>>
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? LateFee { get; set; }
    public decimal? DamageFee { get; set; }
    public string? Notes { get; set; }
    public string? PickupLocation { get; set; }
    public string? ReturnLocation { get; set; }
    public int? KmAtStart { get; set; }
    public int? KmAtReturn { get; set; }
}

