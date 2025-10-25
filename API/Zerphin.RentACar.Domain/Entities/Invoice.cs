namespace Zerphin.RentACar.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    
    public DateTime DueDate { get; set; }
    
    public decimal SubTotal { get; set; }
    
    public decimal TaxAmount { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public string? TaxNumber { get; set; }
    
    public string? BillingAddress { get; set; }
    
    public string? CustomerName { get; set; }
    
    public string? CustomerTaxNumber { get; set; }
    
    public bool IsPaid { get; set; } = false;
    
    public DateTime? PaidDate { get; set; }
    
    public string? Notes { get; set; }
    
    public int RentalId { get; set; }
    
    public virtual Rental Rental { get; set; } = null!;
}
