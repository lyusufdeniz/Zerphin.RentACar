using FluentValidation;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Vehicles.CreateVehicle;

public class CreateVehicleValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleValidator()
    {
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required")
            .MaximumLength(50).WithMessage("Brand cannot exceed 50 characters");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required")
            .MaximumLength(50).WithMessage("Model cannot exceed 50 characters");

        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("License plate is required")
            .MaximumLength(20).WithMessage("License plate cannot exceed 20 characters");

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, DateTime.Now.Year + 1)
            .WithMessage($"Year must be between 1900 and {DateTime.Now.Year + 1}");

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Color is required")
            .MaximumLength(30).WithMessage("Color cannot exceed 30 characters");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid vehicle category");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid vehicle status");

        RuleFor(x => x.DailyRentalPrice)
            .GreaterThan(0).WithMessage("Daily rental price must be greater than 0");

        RuleFor(x => x.SeatingCapacity)
            .InclusiveBetween(1, 50).WithMessage("Seating capacity must be between 1 and 50");

        RuleFor(x => x.FuelType)
            .MaximumLength(30).WithMessage("Fuel type cannot exceed 30 characters")
            .When(x => !string.IsNullOrEmpty(x.FuelType));

        RuleFor(x => x.Transmission)
            .MaximumLength(30).WithMessage("Transmission cannot exceed 30 characters")
            .When(x => !string.IsNullOrEmpty(x.Transmission));

        RuleFor(x => x.Km)
            .GreaterThanOrEqualTo(0).WithMessage("Km cannot be negative")
            .When(x => x.Km.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

