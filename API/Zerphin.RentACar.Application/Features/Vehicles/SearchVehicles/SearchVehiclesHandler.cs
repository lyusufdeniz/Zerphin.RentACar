using AutoMapper;
using MediatR;
using System.Linq.Expressions;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Vehicles.SearchVehicles;

public class SearchVehiclesHandler : IRequestHandler<SearchVehiclesCommand, ServiceResult<SearchVehiclesResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public SearchVehiclesHandler(IVehicleRepository vehicleRepository, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<SearchVehiclesResponse>> Handle(SearchVehiclesCommand request, CancellationToken cancellationToken)
    {
        // Build predicate expression for filtering
        Expression<Func<Vehicle, bool>>? predicate = null;

        // Category filter
        if (request.Category.HasValue)
        {
            var category = request.Category.Value;
            Expression<Func<Vehicle, bool>> categoryFilter = v => v.Category == category;
            predicate = predicate == null ? categoryFilter : CombineExpressions(predicate, categoryFilter);
        }

        // Status filter
        if (request.Status.HasValue)
        {
            var status = request.Status.Value;
            Expression<Func<Vehicle, bool>> statusFilter = v => v.Status == status;
            predicate = predicate == null ? statusFilter : CombineExpressions(predicate, statusFilter);
        }

        // Price range filter
        if (request.MinPrice.HasValue)
        {
            var minPrice = request.MinPrice.Value;
            Expression<Func<Vehicle, bool>> minPriceFilter = v => v.DailyRentalPrice >= minPrice;
            predicate = predicate == null ? minPriceFilter : CombineExpressions(predicate, minPriceFilter);
        }

        if (request.MaxPrice.HasValue)
        {
            var maxPrice = request.MaxPrice.Value;
            Expression<Func<Vehicle, bool>> maxPriceFilter = v => v.DailyRentalPrice <= maxPrice;
            predicate = predicate == null ? maxPriceFilter : CombineExpressions(predicate, maxPriceFilter);
        }

        // Brand filter
        if (!string.IsNullOrWhiteSpace(request.Brand))
        {
            var brand = request.Brand;
            Expression<Func<Vehicle, bool>> brandFilter = v => v.Brand.Contains(brand);
            predicate = predicate == null ? brandFilter : CombineExpressions(predicate, brandFilter);
        }

        // Model filter
        if (!string.IsNullOrWhiteSpace(request.Model))
        {
            var model = request.Model;
            Expression<Func<Vehicle, bool>> modelFilter = v => v.Model.Contains(model);
            predicate = predicate == null ? modelFilter : CombineExpressions(predicate, modelFilter);
        }

        // License plate filter
        if (!string.IsNullOrWhiteSpace(request.LicensePlate))
        {
            var licensePlate = request.LicensePlate;
            Expression<Func<Vehicle, bool>> licensePlateFilter = v => v.LicensePlate.Contains(licensePlate);
            predicate = predicate == null ? licensePlateFilter : CombineExpressions(predicate, licensePlateFilter);
        }

        // Build order by expression
        Expression<Func<Vehicle, object>>? orderByExpression = GetOrderByExpression(request.OrderBy);

        // Get paged results
        PagedResult<Vehicle> pagedResult;
        if (predicate != null && orderByExpression != null)
        {
            pagedResult = await _vehicleRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate,
                orderByExpression,
                request.IsDescending
            );
        }
        else if (predicate != null)
        {
            pagedResult = await _vehicleRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate
            );
        }
        else if (orderByExpression != null)
        {
            pagedResult = await _vehicleRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                v => true,
                orderByExpression,
                request.IsDescending
            );
        }
        else
        {
            pagedResult = await _vehicleRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize
            );
        }

        var response = new SearchVehiclesResponse
        {
            Vehicles = _mapper.Map<List<SearchVehiclesResponse.VehicleDto>>(pagedResult.Data),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages
        };

        return ServiceResult<SearchVehiclesResponse>.Success(response);
    }

    private static Expression<Func<Vehicle, bool>> CombineExpressions(
        Expression<Func<Vehicle, bool>> first,
        Expression<Func<Vehicle, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(Vehicle), "v");
        var body = Expression.AndAlso(
            Expression.Invoke(first, parameter),
            Expression.Invoke(second, parameter)
        );
        return Expression.Lambda<Func<Vehicle, bool>>(body, parameter);
    }

    private static Expression<Func<Vehicle, object>>? GetOrderByExpression(string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return v => v.Id;

        return orderBy.ToLower() switch
        {
            "id" => v => (object)v.Id,
            "brand" => v => (object)v.Brand,
            "model" => v => (object)v.Model,
            "price" => v => (object)v.DailyRentalPrice,
            "year" => v => (object)v.Year,
            "category" => v => (object)v.Category,
            "status" => v => (object)v.Status,
            _ => v => (object)v.Id
        };
    }
}
