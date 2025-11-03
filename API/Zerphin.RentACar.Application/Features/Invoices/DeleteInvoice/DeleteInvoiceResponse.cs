namespace Zerphin.RentACar.Application.Features.Invoices.DeleteInvoice;

public class DeleteInvoiceResponse
{
    public Guid Id { get; set; }
    public string Message { get; set; } = "Invoice deleted successfully.";
}



