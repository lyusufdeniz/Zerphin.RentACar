namespace Zerphin.RentACar.Application.Features.Invoices.CreateInvoice;

public class CreateInvoiceResponse
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? TaxNumber { get; set; }
    public string? BillingAddress { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerTaxNumber { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? Notes { get; set; }
    public Guid RentalId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerEmail { get; set; }
}

