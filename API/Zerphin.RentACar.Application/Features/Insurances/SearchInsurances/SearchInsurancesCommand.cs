using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Insurances.SearchInsurances;

public class SearchInsurancesCommand : IRequest<ServiceResult<SearchInsurancesResponse>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? VehicleId { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    public string? PolicyNumber { get; set; }
    public string? OrderBy { get; set; } = "Id";
    public bool IsDescending { get; set; } = false;
}


