using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Invoices.CreateInvoice;

public class CreateInvoiceCommand : IRequest<ServiceResult<CreateInvoiceResponse>>
{
    public Guid RentalId { get; set; }
    public DateTime? DueDate { get; set; } // Optional, default: InvoiceDate + 30 days
    public decimal? TaxRate { get; set; } // Optional, default: 0.18 (18% KDV)
    public string? Notes { get; set; }
}
