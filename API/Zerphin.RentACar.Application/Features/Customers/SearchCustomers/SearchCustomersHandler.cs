using AutoMapper;
using MediatR;
using System.Linq.Expressions;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Customers.SearchCustomers;

public class SearchCustomersHandler : IRequestHandler<SearchCustomersCommand, ServiceResult<SearchCustomersResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public SearchCustomersHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<SearchCustomersResponse>> Handle(SearchCustomersCommand request, CancellationToken cancellationToken)
    {
        Expression<Func<Customer, bool>>? predicate = null;

        if (request.UserId.HasValue)
        {
            var userId = request.UserId.Value;
            Expression<Func<Customer, bool>> userIdFilter = c => c.UserId == userId;
            predicate = predicate == null ? userIdFilter : CombineExpressions(predicate, userIdFilter);
        }

        if (!string.IsNullOrWhiteSpace(request.LicenseNumber))
        {
            var licenseNumber = request.LicenseNumber;
            Expression<Func<Customer, bool>> licenseFilter = c => c.LicenseNumber.Contains(licenseNumber);
            predicate = predicate == null ? licenseFilter : CombineExpressions(predicate, licenseFilter);
        }

        if (request.IsVerified.HasValue)
        {
            var isVerified = request.IsVerified.Value;
            Expression<Func<Customer, bool>> verifiedFilter = c => c.IsVerified == isVerified;
            predicate = predicate == null ? verifiedFilter : CombineExpressions(predicate, verifiedFilter);
        }

        if (request.MinCreditScore.HasValue)
        {
            var minScore = request.MinCreditScore.Value;
            Expression<Func<Customer, bool>> minScoreFilter = c => c.CreditScore >= minScore;
            predicate = predicate == null ? minScoreFilter : CombineExpressions(predicate, minScoreFilter);
        }

        if (request.MaxCreditScore.HasValue)
        {
            var maxScore = request.MaxCreditScore.Value;
            Expression<Func<Customer, bool>> maxScoreFilter = c => c.CreditScore <= maxScore;
            predicate = predicate == null ? maxScoreFilter : CombineExpressions(predicate, maxScoreFilter);
        }

        if (request.HasInsurance.HasValue)
        {
            var hasInsurance = request.HasInsurance.Value;
            Expression<Func<Customer, bool>> insuranceFilter = c => c.HasInsurance == hasInsurance;
            predicate = predicate == null ? insuranceFilter : CombineExpressions(predicate, insuranceFilter);
        }

        Expression<Func<Customer, object>>? orderByExpression = GetOrderByExpression(request.OrderBy);

        PagedResult<Customer> pagedResult;
        if (predicate != null && orderByExpression != null)
        {
            pagedResult = await _customerRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate,
                orderByExpression,
                request.IsDescending
            );
        }
        else if (predicate != null)
        {
            pagedResult = await _customerRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate
            );
        }
        else if (orderByExpression != null)
        {
            pagedResult = await _customerRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                c => true,
                orderByExpression,
                request.IsDescending
            );
        }
        else
        {
            pagedResult = await _customerRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize
            );
        }

        var response = new SearchCustomersResponse
        {
            Customers = _mapper.Map<List<SearchCustomersResponse.CustomerDto>>(pagedResult.Data),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages
        };

        return ServiceResult<SearchCustomersResponse>.Success(response);
    }

    private static Expression<Func<Customer, bool>> CombineExpressions(
        Expression<Func<Customer, bool>> first,
        Expression<Func<Customer, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(Customer), "c");
        var body = Expression.AndAlso(
            Expression.Invoke(first, parameter),
            Expression.Invoke(second, parameter)
        );
        return Expression.Lambda<Func<Customer, bool>>(body, parameter);
    }

    private static Expression<Func<Customer, object>>? GetOrderByExpression(string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return c => c.Id;

        return orderBy.ToLower() switch
        {
            "id" => c => (object)c.Id,
            "licensenumber" => c => (object)c.LicenseNumber,
            "creditscore" => c => (object)c.CreditScore,
            "licenseexpirydate" => c => (object)c.LicenseExpiryDate,
            _ => c => (object)c.Id
        };
    }
}


