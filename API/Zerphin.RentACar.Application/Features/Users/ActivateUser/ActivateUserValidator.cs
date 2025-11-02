using FluentValidation;

namespace Zerphin.RentACar.Application.Features.Users.ActivateUser;

public class ActivateUserValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("User ID must be greater than 0");
    }
}

