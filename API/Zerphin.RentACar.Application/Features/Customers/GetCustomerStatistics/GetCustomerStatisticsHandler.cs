using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Customers.GetCustomerStatistics;

public class GetCustomerStatisticsHandler : IRequestHandler<GetCustomerStatisticsCommand, ServiceResult<GetCustomerStatisticsResponse>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerStatisticsHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ServiceResult<GetCustomerStatisticsResponse>> Handle(GetCustomerStatisticsCommand request, CancellationToken cancellationToken)
    {
        var response = new GetCustomerStatisticsResponse();

        response.TotalCustomers = await _customerRepository.CountAsync();
        response.VerifiedCustomers = await _customerRepository.CountAsync(c => c.IsVerified);
        response.UnverifiedCustomers = await _customerRepository.CountAsync(c => !c.IsVerified);
        response.CustomersWithInsurance = await _customerRepository.CountAsync(c => c.HasInsurance);
        response.NewCustomersLast30Days = await _customerRepository.CountAsync(c => c.CreatedAt >= DateTime.UtcNow.AddDays(-30));

        var customers = await _customerRepository.FindAsync(c => true);
        var customerList = customers.ToList();
        if (customerList.Any())
        {
            response.AverageCreditScore = customerList.Average(c => c.CreditScore);
        }

        return ServiceResult<GetCustomerStatisticsResponse>.Success(response);
    }
}

