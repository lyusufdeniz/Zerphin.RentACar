using AutoMapper;
using MediatR;
using System.Linq.Expressions;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Users.SearchUsers;

public class SearchUsersHandler : IRequestHandler<SearchUsersCommand, ServiceResult<SearchUsersResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public SearchUsersHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<SearchUsersResponse>> Handle(SearchUsersCommand request, CancellationToken cancellationToken)
    {
        Expression<Func<User, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim();
            Expression<Func<User, bool>> emailFilter = u => u.Email.Contains(email);
            predicate = predicate == null ? emailFilter : CombineExpressions(predicate, emailFilter);
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            var firstName = request.FirstName.Trim();
            Expression<Func<User, bool>> firstNameFilter = u => u.FirstName.Contains(firstName);
            predicate = predicate == null ? firstNameFilter : CombineExpressions(predicate, firstNameFilter);
        }

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            var lastName = request.LastName.Trim();
            Expression<Func<User, bool>> lastNameFilter = u => u.LastName.Contains(lastName);
            predicate = predicate == null ? lastNameFilter : CombineExpressions(predicate, lastNameFilter);
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phoneNumber = request.PhoneNumber.Trim();
            Expression<Func<User, bool>> phoneFilter = u => u.PhoneNumber.Contains(phoneNumber);
            predicate = predicate == null ? phoneFilter : CombineExpressions(predicate, phoneFilter);
        }

        if (request.Role.HasValue)
        {
            var role = request.Role.Value;
            Expression<Func<User, bool>> roleFilter = u => u.Role == role;
            predicate = predicate == null ? roleFilter : CombineExpressions(predicate, roleFilter);
        }

        if (request.IsActive.HasValue)
        {
            var isActive = request.IsActive.Value;
            Expression<Func<User, bool>> isActiveFilter = u => u.IsActive == isActive;
            predicate = predicate == null ? isActiveFilter : CombineExpressions(predicate, isActiveFilter);
        }

        if (!string.IsNullOrWhiteSpace(request.IdentityNumber))
        {
            var identityNumber = request.IdentityNumber.Trim();
            Expression<Func<User, bool>> identityFilter = u => u.IdentityNumber != null && u.IdentityNumber.Contains(identityNumber);
            predicate = predicate == null ? identityFilter : CombineExpressions(predicate, identityFilter);
        }

        if (!string.IsNullOrWhiteSpace(request.LicenseNumber))
        {
            var licenseNumber = request.LicenseNumber.Trim();
            Expression<Func<User, bool>> licenseFilter = u => u.LicenseNumber != null && u.LicenseNumber.Contains(licenseNumber);
            predicate = predicate == null ? licenseFilter : CombineExpressions(predicate, licenseFilter);
        }

        if (request.IsVerified.HasValue)
        {
            var isVerified = request.IsVerified.Value;
            Expression<Func<User, bool>> isVerifiedFilter = u => u.IsVerified == isVerified;
            predicate = predicate == null ? isVerifiedFilter : CombineExpressions(predicate, isVerifiedFilter);
        }

        Expression<Func<User, object>>? orderByExpression = GetOrderByExpression(request.OrderBy);

        PagedResult<User> pagedResult;
        if (predicate != null && orderByExpression != null)
        {
            pagedResult = await _userRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate,
                orderByExpression,
                request.IsDescending
            );
        }
        else if (predicate != null)
        {
            pagedResult = await _userRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate
            );
        }
        else if (orderByExpression != null)
        {
            pagedResult = await _userRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                u => true,
                orderByExpression,
                request.IsDescending
            );
        }
        else
        {
            pagedResult = await _userRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize
            );
        }

        var response = new SearchUsersResponse
        {
            Users = _mapper.Map<List<SearchUsersResponse.UserDto>>(pagedResult.Data),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages
        };

        return ServiceResult<SearchUsersResponse>.Success(response);
    }

    private static Expression<Func<User, bool>> CombineExpressions(
        Expression<Func<User, bool>> first,
        Expression<Func<User, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(User), "u");
        var body = Expression.AndAlso(
            Expression.Invoke(first, parameter),
            Expression.Invoke(second, parameter)
        );
        return Expression.Lambda<Func<User, bool>>(body, parameter);
    }

    private static Expression<Func<User, object>>? GetOrderByExpression(string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return u => (object)u.Id;

        return orderBy.ToLower() switch
        {
            "id" => u => (object)u.Id,
            "email" => u => (object)u.Email,
            "firstname" => u => (object)u.FirstName,
            "lastname" => u => (object)u.LastName,
            "createdat" => u => (object)u.CreatedAt,
            "role" => u => (object)u.Role,
            _ => u => (object)u.Id
        };
    }
}

