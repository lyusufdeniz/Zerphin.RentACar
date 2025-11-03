using AutoMapper;
using MediatR;
using System.Linq.Expressions;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Insurances.SearchInsurances;

public class SearchInsurancesHandler : IRequestHandler<SearchInsurancesCommand, ServiceResult<SearchInsurancesResponse>>
{
    private readonly IInsuranceRepository _insuranceRepository;
    private readonly IMapper _mapper;

    public SearchInsurancesHandler(IInsuranceRepository insuranceRepository, IMapper mapper)
    {
        _insuranceRepository = insuranceRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<SearchInsurancesResponse>> Handle(SearchInsurancesCommand request, CancellationToken cancellationToken)
    {
        Expression<Func<Insurance, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(request.LicensePlate))
        {
            var licensePlate = request.LicensePlate.Trim();
            Expression<Func<Insurance, bool>> licensePlateFilter = i => i.Vehicle != null && i.Vehicle.LicensePlate.Contains(licensePlate);
            predicate = predicate == null ? licensePlateFilter : CombineExpressions(predicate, licensePlateFilter);
        }

        if (request.StartDateFrom.HasValue)
        {
            var dateFrom = request.StartDateFrom.Value;
            Expression<Func<Insurance, bool>> dateFromFilter = i => i.StartDate >= dateFrom;
            predicate = predicate == null ? dateFromFilter : CombineExpressions(predicate, dateFromFilter);
        }

        if (request.StartDateTo.HasValue)
        {
            var dateTo = request.StartDateTo.Value;
            Expression<Func<Insurance, bool>> dateToFilter = i => i.StartDate <= dateTo;
            predicate = predicate == null ? dateToFilter : CombineExpressions(predicate, dateToFilter);
        }

        if (request.EndDateFrom.HasValue)
        {
            var dateFrom = request.EndDateFrom.Value;
            Expression<Func<Insurance, bool>> dateFromFilter = i => i.EndDate >= dateFrom;
            predicate = predicate == null ? dateFromFilter : CombineExpressions(predicate, dateFromFilter);
        }

        if (request.EndDateTo.HasValue)
        {
            var dateTo = request.EndDateTo.Value;
            Expression<Func<Insurance, bool>> dateToFilter = i => i.EndDate <= dateTo;
            predicate = predicate == null ? dateToFilter : CombineExpressions(predicate, dateToFilter);
        }

        if (!string.IsNullOrWhiteSpace(request.PolicyNumber))
        {
            var policyNumber = request.PolicyNumber;
            Expression<Func<Insurance, bool>> policyFilter = i => i.PolicyNumber.Contains(policyNumber);
            predicate = predicate == null ? policyFilter : CombineExpressions(predicate, policyFilter);
        }

        Expression<Func<Insurance, object>>? orderByExpression = GetOrderByExpression(request.OrderBy);

        PagedResult<Insurance> pagedResult;
        if (predicate != null && orderByExpression != null)
        {
            pagedResult = await _insuranceRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate,
                orderByExpression,
                request.IsDescending
            );
        }
        else if (predicate != null)
        {
            pagedResult = await _insuranceRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate
            );
        }
        else if (orderByExpression != null)
        {
            pagedResult = await _insuranceRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                i => true,
                orderByExpression,
                request.IsDescending
            );
        }
        else
        {
            pagedResult = await _insuranceRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize
            );
        }

        var response = new SearchInsurancesResponse
        {
            Insurances = _mapper.Map<List<SearchInsurancesResponse.InsuranceDto>>(pagedResult.Data),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages
        };

        return ServiceResult<SearchInsurancesResponse>.Success(response);
    }

    private static Expression<Func<Insurance, bool>> CombineExpressions(
        Expression<Func<Insurance, bool>> first,
        Expression<Func<Insurance, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(Insurance), "i");
        var body = Expression.AndAlso(
            Expression.Invoke(first, parameter),
            Expression.Invoke(second, parameter)
        );
        return Expression.Lambda<Func<Insurance, bool>>(body, parameter);
    }

    private static Expression<Func<Insurance, object>>? GetOrderByExpression(string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return i => i.Id;

        return orderBy.ToLower() switch
        {
            "id" => i => (object)i.Id,
            "startdate" => i => (object)i.StartDate,
            "enddate" => i => (object)i.EndDate,
            "policynumber" => i => (object)i.PolicyNumber,
            _ => i => (object)i.Id
        };
    }
}



