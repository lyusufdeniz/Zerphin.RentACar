using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Invoices.SearchInvoices;

public class SearchInvoicesCommand : IRequest<ServiceResult<SearchInvoicesResponse>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? RentalId { get; set; }
    public bool? IsPaid { get; set; }
    public DateTime? InvoiceDateFrom { get; set; }
    public DateTime? InvoiceDateTo { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? OrderBy { get; set; } = "Id";
    public bool IsDescending { get; set; } = false;
}



