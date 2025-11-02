using MediatR;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Rentals.CreateRental;

public class CreateRentalCommand : IRequest<ServiceResult<CreateRentalResponse>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Pending;
    public string? Notes { get; set; }
    public string? PickupLocation { get; set; }
    public string? ReturnLocation { get; set; }
    public int? KmAtStart { get; set; }
    public Guid CustomerId { get; set; }
    public Guid VehicleId { get; set; }
}


