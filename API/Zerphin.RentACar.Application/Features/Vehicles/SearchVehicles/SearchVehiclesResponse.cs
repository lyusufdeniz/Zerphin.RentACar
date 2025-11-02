using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Vehicles.SearchVehicles;

public class SearchVehiclesResponse
{
    public List<VehicleDto> Vehicles { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }

    public class VehicleDto
    {
        public Guid Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;
        public VehicleCategory Category { get; set; }
        public VehicleStatus Status { get; set; }
        public decimal DailyRentalPrice { get; set; }
        public int SeatingCapacity { get; set; }
        public string? FuelType { get; set; }
        public string? Transmission { get; set; }
        public int? Km { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool HasAirConditioning { get; set; }
        public bool HasGPS { get; set; }
        public bool HasBluetooth { get; set; }
    }
}

