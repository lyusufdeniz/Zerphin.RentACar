using AutoMapper;
using MediatR;
using System.Linq.Expressions;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Payments.SearchPayments;

public class SearchPaymentsHandler : IRequestHandler<SearchPaymentsCommand, ServiceResult<SearchPaymentsResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public SearchPaymentsHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<SearchPaymentsResponse>> Handle(SearchPaymentsCommand request, CancellationToken cancellationToken)
    {
        Expression<Func<Payment, bool>>? predicate = null;

        if (request.RentalId.HasValue)
        {
            var rentalId = request.RentalId.Value;
            Expression<Func<Payment, bool>> rentalFilter = p => p.RentalId == rentalId;
            predicate = predicate == null ? rentalFilter : CombineExpressions(predicate, rentalFilter);
        }

        if (request.Status.HasValue)
        {
            var status = request.Status.Value;
            Expression<Func<Payment, bool>> statusFilter = p => p.Status == status;
            predicate = predicate == null ? statusFilter : CombineExpressions(predicate, statusFilter);
        }

        if (request.Method.HasValue)
        {
            var method = request.Method.Value;
            Expression<Func<Payment, bool>> methodFilter = p => p.Method == method;
            predicate = predicate == null ? methodFilter : CombineExpressions(predicate, methodFilter);
        }

        if (request.PaymentDateFrom.HasValue)
        {
            var dateFrom = request.PaymentDateFrom.Value;
            Expression<Func<Payment, bool>> dateFromFilter = p => p.PaymentDate >= dateFrom;
            predicate = predicate == null ? dateFromFilter : CombineExpressions(predicate, dateFromFilter);
        }

        if (request.PaymentDateTo.HasValue)
        {
            var dateTo = request.PaymentDateTo.Value;
            Expression<Func<Payment, bool>> dateToFilter = p => p.PaymentDate <= dateTo;
            predicate = predicate == null ? dateToFilter : CombineExpressions(predicate, dateToFilter);
        }

        Expression<Func<Payment, object>>? orderByExpression = GetOrderByExpression(request.OrderBy);

        PagedResult<Payment> pagedResult;
        if (predicate != null && orderByExpression != null)
        {
            pagedResult = await _paymentRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate,
                orderByExpression,
                request.IsDescending
            );
        }
        else if (predicate != null)
        {
            pagedResult = await _paymentRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate
            );
        }
        else if (orderByExpression != null)
        {
            pagedResult = await _paymentRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                p => true,
                orderByExpression,
                request.IsDescending
            );
        }
        else
        {
            pagedResult = await _paymentRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize
            );
        }

        var response = new SearchPaymentsResponse
        {
            Payments = _mapper.Map<List<SearchPaymentsResponse.PaymentDto>>(pagedResult.Data),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages
        };

        return ServiceResult<SearchPaymentsResponse>.Success(response);
    }

    private static Expression<Func<Payment, bool>> CombineExpressions(
        Expression<Func<Payment, bool>> first,
        Expression<Func<Payment, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(Payment), "p");
        var body = Expression.AndAlso(
            Expression.Invoke(first, parameter),
            Expression.Invoke(second, parameter)
        );
        return Expression.Lambda<Func<Payment, bool>>(body, parameter);
    }

    private static Expression<Func<Payment, object>>? GetOrderByExpression(string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return p => p.Id;

        return orderBy.ToLower() switch
        {
            "id" => p => (object)p.Id,
            "paymentdate" => p => (object)p.PaymentDate,
            "amount" => p => (object)p.Amount,
            "status" => p => (object)p.Status,
            _ => p => (object)p.Id
        };
    }
}


