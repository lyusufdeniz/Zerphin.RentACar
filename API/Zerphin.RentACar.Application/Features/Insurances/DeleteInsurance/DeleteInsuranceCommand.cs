using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Insurances.DeleteInsurance;

public class DeleteInsuranceCommand : IRequest<ServiceResult<DeleteInsuranceResponse>>
{
    public Guid Id { get; set; }
}


