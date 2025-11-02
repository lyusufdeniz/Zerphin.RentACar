using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Rentals.UpdateRentalStatus;

public class UpdateRentalStatusResponse
{
    public Guid Id { get; set; }
    public RentalStatus Status { get; set; }
    public string Message { get; set; } = "Rental status updated successfully.";
}


