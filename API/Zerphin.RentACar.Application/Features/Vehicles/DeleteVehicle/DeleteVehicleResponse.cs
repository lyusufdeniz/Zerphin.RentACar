namespace Zerphin.RentACar.Application.Features.Vehicles.DeleteVehicle;

public class DeleteVehicleResponse
{
    public Guid Id { get; set; }
    public string Message { get; set; } = "Vehicle deleted successfully.";
}

