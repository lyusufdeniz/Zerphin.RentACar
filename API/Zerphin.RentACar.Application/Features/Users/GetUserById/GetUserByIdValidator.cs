using FluentValidation;

namespace Zerphin.RentACar.Application.Features.Users.GetUserById;

public class GetUserByIdValidator : AbstractValidator<GetUserByIdCommand>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");
    }
}

