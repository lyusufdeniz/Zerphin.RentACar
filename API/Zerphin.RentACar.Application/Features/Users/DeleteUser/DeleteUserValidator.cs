using FluentValidation;

namespace Zerphin.RentACar.Application.Features.Users.DeleteUser;

public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("User ID must be greater than 0");
    }
}

