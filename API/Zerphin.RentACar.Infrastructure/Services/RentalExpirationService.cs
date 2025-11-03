using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.ValueObjects;
using Zerphin.RentACar.Domain.Common;

namespace Zerphin.RentACar.Infrastructure.Services;

public class RentalExpirationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RentalExpirationService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Her 1 dk bir kontrol et

    public RentalExpirationService(
        IServiceProvider serviceProvider,
        ILogger<RentalExpirationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Rental Expiration Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredRentalsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing expired rentals");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessExpiredRentalsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var rentalRepository = scope.ServiceProvider.GetRequiredService<IRentalRepository>();
        var vehicleRepository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var now = DateTime.UtcNow;
        
        // Find rentals that have passed their end date and are still active
        var expiredRentals = await rentalRepository.FindAsync(r =>
            !r.IsDeleted &&
            r.EndDate < now &&
            r.Status == RentalStatus.Active);

        foreach (var rental in expiredRentals)
        {
            try
            {
                // Mark rental as completed
                rental.Status = RentalStatus.Completed;
                if (rental.ActualReturnDate == null)
                {
                    rental.ActualReturnDate = now;
                }
                await rentalRepository.UpdateAsync(rental);

                // Update vehicle status if no other active rentals exist
                var vehicle = await vehicleRepository.GetByIdAsync(rental.VehicleId);
                if (vehicle != null)
                {
                    // Check if there are other active rentals for this vehicle
                    var activeRentals = await rentalRepository.FindAsync(r =>
                        r.VehicleId == rental.VehicleId &&
                        r.Id != rental.Id &&
                        !r.IsDeleted &&
                        r.Status == RentalStatus.Active);

                    // Only set to Available if no other active rentals exist
                    if (!activeRentals.Any())
                    {
                        vehicle.Status = VehicleStatus.Available;
                        await vehicleRepository.UpdateAsync(vehicle);
                        _logger.LogInformation("Vehicle {VehicleId} set to Available after rental {RentalId} expired", 
                            vehicle.Id, rental.Id);
                    }
                }

                _logger.LogInformation("Rental {RentalId} marked as Completed (expired)", rental.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing rental {RentalId}", rental.Id);
            }
        }

        if (expiredRentals.Any())
        {
            await unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Processed {ExpiredCount} expired rental(s)", expiredRentals.Count());
        }
    }
}

