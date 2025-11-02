using FluentValidation;

namespace Zerphin.RentACar.Application.Features.Users.DeactivateUser;

public class DeactivateUserValidator : AbstractValidator<DeactivateUserCommand>
{
    public DeactivateUserValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("User ID must be greater than 0");
    }
}

