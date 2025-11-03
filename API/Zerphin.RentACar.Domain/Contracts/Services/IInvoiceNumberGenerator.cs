namespace Zerphin.RentACar.Domain.Contracts.Services;

public interface IInvoiceNumberGenerator
{
    Task<string> GenerateInvoiceNumberAsync();
}

