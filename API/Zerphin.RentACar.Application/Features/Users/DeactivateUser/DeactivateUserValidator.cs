using FluentValidation;

namespace Zerphin.RentACar.Application.Features.Users.DeactivateUser;

public class DeactivateUserValidator : AbstractValidator<DeactivateUserCommand>
{
    public DeactivateUserValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");
    }
}

