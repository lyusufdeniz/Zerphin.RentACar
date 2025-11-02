using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Insurances.GetInsuranceStatistics;

public class GetInsuranceStatisticsHandler : IRequestHandler<GetInsuranceStatisticsCommand, ServiceResult<GetInsuranceStatisticsResponse>>
{
    private readonly IInsuranceRepository _insuranceRepository;

    public GetInsuranceStatisticsHandler(IInsuranceRepository insuranceRepository)
    {
        _insuranceRepository = insuranceRepository;
    }

    public async Task<ServiceResult<GetInsuranceStatisticsResponse>> Handle(GetInsuranceStatisticsCommand request, CancellationToken cancellationToken)
    {
        var response = new GetInsuranceStatisticsResponse();

        response.TotalInsurances = await _insuranceRepository.CountAsync();
        response.ActiveInsurances = await _insuranceRepository.CountAsync(i => i.IsActive && i.EndDate >= DateTime.UtcNow);
        
        var now = DateTime.UtcNow;
        response.ExpiredInsurances = await _insuranceRepository.CountAsync(i => i.EndDate < now);
        response.ExpiringSoonInsurances = await _insuranceRepository.CountAsync(i => i.EndDate >= now && i.EndDate <= now.AddDays(30));

        var insurances = await _insuranceRepository.FindAsync(i => true);
        var insuranceList = insurances.ToList();
        if (insuranceList.Any())
        {
            response.TotalPremiumAmount = insuranceList.Sum(i => i.PremiumAmount);
        }

        return ServiceResult<GetInsuranceStatisticsResponse>.Success(response);
    }
}

