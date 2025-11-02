using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Invoices.GetInvoiceStatistics;

public class GetInvoiceStatisticsCommand : IRequest<ServiceResult<GetInvoiceStatisticsResponse>>
{
}

