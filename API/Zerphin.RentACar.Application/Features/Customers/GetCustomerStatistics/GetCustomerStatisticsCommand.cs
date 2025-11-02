using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Customers.GetCustomerStatistics;

public class GetCustomerStatisticsCommand : IRequest<ServiceResult<GetCustomerStatisticsResponse>>
{
}

