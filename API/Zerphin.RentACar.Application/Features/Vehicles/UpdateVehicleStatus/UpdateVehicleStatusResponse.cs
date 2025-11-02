using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Vehicles.UpdateVehicleStatus;

public class UpdateVehicleStatusResponse
{
    public Guid Id { get; set; }
    public VehicleStatus Status { get; set; }
    public string Message { get; set; } = "Vehicle status updated successfully.";
}


