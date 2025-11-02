namespace Zerphin.RentACar.Application.Features.Vehicles.GetVehicleStatistics;

public class GetVehicleStatisticsResponse
{
    public int TotalVehicles { get; set; }
    public int AvailableVehicles { get; set; }
    public int RentedVehicles { get; set; }
    public int MaintenanceVehicles { get; set; }
    public int OutOfServiceVehicles { get; set; }
    
    public List<VehicleCategoryStatistics> CategoryStatistics { get; set; } = new();
    public List<VehicleStatusStatistics> StatusStatistics { get; set; } = new();
}

