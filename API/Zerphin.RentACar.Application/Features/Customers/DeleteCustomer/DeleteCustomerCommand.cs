using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Customers.DeleteCustomer;

public class DeleteCustomerCommand : IRequest<ServiceResult<DeleteCustomerResponse>>
{
    public Guid Id { get; set; }
}

