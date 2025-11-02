using FluentValidation;

namespace Zerphin.RentACar.Application.Features.Users.GetUsersByRole;

public class GetUsersByRoleValidator : AbstractValidator<GetUsersByRoleCommand>
{
    public GetUsersByRoleValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("Role name is required");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");
    }
}

