using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Vehicles.GetVehicleStatistics;

public class VehicleCategoryStatistics
{
    public VehicleCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int Count { get; set; }
}

