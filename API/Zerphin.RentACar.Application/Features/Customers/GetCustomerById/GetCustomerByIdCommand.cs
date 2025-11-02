using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Customers.GetCustomerById;

public class GetCustomerByIdCommand : IRequest<ServiceResult<GetCustomerByIdResponse>>
{
    public Guid Id { get; set; }
}


