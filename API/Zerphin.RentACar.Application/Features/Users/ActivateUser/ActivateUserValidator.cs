using FluentValidation;

namespace Zerphin.RentACar.Application.Features.Users.ActivateUser;

public class ActivateUserValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");
    }
}

