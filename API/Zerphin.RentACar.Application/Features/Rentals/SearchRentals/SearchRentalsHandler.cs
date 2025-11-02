using AutoMapper;
using MediatR;
using System.Linq.Expressions;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Rentals.SearchRentals;

public class SearchRentalsHandler : IRequestHandler<SearchRentalsCommand, ServiceResult<SearchRentalsResponse>>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IMapper _mapper;

    public SearchRentalsHandler(IRentalRepository rentalRepository, IMapper mapper)
    {
        _rentalRepository = rentalRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<SearchRentalsResponse>> Handle(SearchRentalsCommand request, CancellationToken cancellationToken)
    {
        Expression<Func<Rental, bool>>? predicate = null;

        if (request.CustomerId.HasValue)
        {
            var customerId = request.CustomerId.Value;
            Expression<Func<Rental, bool>> customerFilter = r => r.CustomerId == customerId;
            predicate = predicate == null ? customerFilter : CombineExpressions(predicate, customerFilter);
        }

        if (request.VehicleId.HasValue)
        {
            var vehicleId = request.VehicleId.Value;
            Expression<Func<Rental, bool>> vehicleFilter = r => r.VehicleId == vehicleId;
            predicate = predicate == null ? vehicleFilter : CombineExpressions(predicate, vehicleFilter);
        }

        if (request.Status.HasValue)
        {
            var status = request.Status.Value;
            Expression<Func<Rental, bool>> statusFilter = r => r.Status == status;
            predicate = predicate == null ? statusFilter : CombineExpressions(predicate, statusFilter);
        }

        if (request.StartDateFrom.HasValue)
        {
            var startDateFrom = request.StartDateFrom.Value;
            Expression<Func<Rental, bool>> startFromFilter = r => r.StartDate >= startDateFrom;
            predicate = predicate == null ? startFromFilter : CombineExpressions(predicate, startFromFilter);
        }

        if (request.StartDateTo.HasValue)
        {
            var startDateTo = request.StartDateTo.Value;
            Expression<Func<Rental, bool>> startToFilter = r => r.StartDate <= startDateTo;
            predicate = predicate == null ? startToFilter : CombineExpressions(predicate, startToFilter);
        }

        if (request.EndDateFrom.HasValue)
        {
            var endDateFrom = request.EndDateFrom.Value;
            Expression<Func<Rental, bool>> endFromFilter = r => r.EndDate >= endDateFrom;
            predicate = predicate == null ? endFromFilter : CombineExpressions(predicate, endFromFilter);
        }

        if (request.EndDateTo.HasValue)
        {
            var endDateTo = request.EndDateTo.Value;
            Expression<Func<Rental, bool>> endToFilter = r => r.EndDate <= endDateTo;
            predicate = predicate == null ? endToFilter : CombineExpressions(predicate, endToFilter);
        }

        Expression<Func<Rental, object>>? orderByExpression = GetOrderByExpression(request.OrderBy);

        PagedResult<Rental> pagedResult;
        if (predicate != null && orderByExpression != null)
        {
            pagedResult = await _rentalRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate,
                orderByExpression,
                request.IsDescending
            );
        }
        else if (predicate != null)
        {
            pagedResult = await _rentalRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate
            );
        }
        else if (orderByExpression != null)
        {
            pagedResult = await _rentalRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                r => true,
                orderByExpression,
                request.IsDescending
            );
        }
        else
        {
            pagedResult = await _rentalRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize
            );
        }

        var response = new SearchRentalsResponse
        {
            Rentals = _mapper.Map<List<SearchRentalsResponse.RentalDto>>(pagedResult.Data),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages
        };

        return ServiceResult<SearchRentalsResponse>.Success(response);
    }

    private static Expression<Func<Rental, bool>> CombineExpressions(
        Expression<Func<Rental, bool>> first,
        Expression<Func<Rental, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(Rental), "r");
        var body = Expression.AndAlso(
            Expression.Invoke(first, parameter),
            Expression.Invoke(second, parameter)
        );
        return Expression.Lambda<Func<Rental, bool>>(body, parameter);
    }

    private static Expression<Func<Rental, object>>? GetOrderByExpression(string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return r => r.Id;

        return orderBy.ToLower() switch
        {
            "id" => r => (object)r.Id,
            "startdate" => r => (object)r.StartDate,
            "enddate" => r => (object)r.EndDate,
            "totalamount" => r => (object)r.TotalAmount,
            "status" => r => (object)r.Status,
            _ => r => (object)r.Id
        };
    }
}


