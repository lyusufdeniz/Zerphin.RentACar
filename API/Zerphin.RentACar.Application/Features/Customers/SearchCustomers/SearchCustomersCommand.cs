using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Customers.SearchCustomers;

public class SearchCustomersCommand : IRequest<ServiceResult<SearchCustomersResponse>>
{
    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    // Filters
    public Guid? UserId { get; set; }
    public string? LicenseNumber { get; set; }
    public bool? IsVerified { get; set; }
    public int? MinCreditScore { get; set; }
    public int? MaxCreditScore { get; set; }
    public bool? HasInsurance { get; set; }
    
    // Sorting
    public string? OrderBy { get; set; } = "Id";
    public bool IsDescending { get; set; } = false;
}

