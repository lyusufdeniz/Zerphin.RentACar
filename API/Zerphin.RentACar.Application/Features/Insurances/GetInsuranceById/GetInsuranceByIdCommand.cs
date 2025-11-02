using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Insurances.GetInsuranceById;

public class GetInsuranceByIdCommand : IRequest<ServiceResult<GetInsuranceByIdResponse>>
{
    public Guid Id { get; set; }
}


