using MediatR;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Rentals.SearchRentals;

public class SearchRentalsCommand : IRequest<ServiceResult<SearchRentalsResponse>>
{
    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    // Filters
    public Guid? CustomerId { get; set; }
    public Guid? VehicleId { get; set; }
    public RentalStatus? Status { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    
    // Sorting
    public string? OrderBy { get; set; } = "Id";
    public bool IsDescending { get; set; } = false;
}


