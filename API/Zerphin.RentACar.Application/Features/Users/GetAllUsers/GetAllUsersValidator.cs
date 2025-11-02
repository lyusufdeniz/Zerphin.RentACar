using FluentValidation;

namespace Zerphin.RentACar.Application.Features.Users.GetAllUsers;

public class GetAllUsersValidator : AbstractValidator<GetAllUsersCommand>
{
    public GetAllUsersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");
    }
}

