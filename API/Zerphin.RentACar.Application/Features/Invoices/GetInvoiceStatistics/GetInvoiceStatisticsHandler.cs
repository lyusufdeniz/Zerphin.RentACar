using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Invoices.GetInvoiceStatistics;

public class GetInvoiceStatisticsHandler : IRequestHandler<GetInvoiceStatisticsCommand, ServiceResult<GetInvoiceStatisticsResponse>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceStatisticsHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<ServiceResult<GetInvoiceStatisticsResponse>> Handle(GetInvoiceStatisticsCommand request, CancellationToken cancellationToken)
    {
        var response = new GetInvoiceStatisticsResponse();

        response.TotalInvoices = await _invoiceRepository.CountAsync();
        response.PaidInvoices = await _invoiceRepository.CountAsync(i => i.IsPaid);
        response.UnpaidInvoices = await _invoiceRepository.CountAsync(i => !i.IsPaid);

        var invoices = await _invoiceRepository.FindAsync(i => true);
        var invoiceList = invoices.ToList();

        if (invoiceList.Any())
        {
            response.TotalAmount = invoiceList.Sum(i => i.TotalAmount);
            response.PaidAmount = invoiceList.Where(i => i.IsPaid).Sum(i => i.TotalAmount);
            response.UnpaidAmount = invoiceList.Where(i => !i.IsPaid).Sum(i => i.TotalAmount);
            
            var now = DateTime.UtcNow;
            var overdueInvoices = invoiceList.Where(i => !i.IsPaid && i.DueDate < now).ToList();
            response.OverdueInvoices = overdueInvoices.Count;
            response.OverdueAmount = overdueInvoices.Sum(i => i.TotalAmount);
        }

        return ServiceResult<GetInvoiceStatisticsResponse>.Success(response);
    }
}

