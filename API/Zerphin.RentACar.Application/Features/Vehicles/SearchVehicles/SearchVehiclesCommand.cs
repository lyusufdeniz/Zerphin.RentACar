using MediatR;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Vehicles.SearchVehicles;

public class SearchVehiclesCommand : IRequest<ServiceResult<SearchVehiclesResponse>>
{
    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    // Filters (optional - all nullable)
    public VehicleCategory? Category { get; set; }
    public VehicleStatus? Status { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    
    // Search
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? LicensePlate { get; set; }
    
    // Sorting
    public string? OrderBy { get; set; } = "Id";
    public bool IsDescending { get; set; } = false;
}

