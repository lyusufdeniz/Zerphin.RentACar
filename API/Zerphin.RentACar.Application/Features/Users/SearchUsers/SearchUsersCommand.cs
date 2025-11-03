using MediatR;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Users.SearchUsers;

public class SearchUsersCommand : IRequest<ServiceResult<SearchUsersResponse>>
{
    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    // Filters
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public UserRole? Role { get; set; }
    public bool? IsActive { get; set; }
    public string? IdentityNumber { get; set; }
    public string? LicenseNumber { get; set; }
    public bool? IsVerified { get; set; }
    
    // Sorting
    public string? OrderBy { get; set; } = "Id";
    public bool IsDescending { get; set; } = false;
}

