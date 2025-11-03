using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Rentals.UpdateRentalStatus;

public class UpdateRentalStatusHandler : IRequestHandler<UpdateRentalStatusCommand, ServiceResult<UpdateRentalStatusResponse>>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRentalStatusHandler(
        IRentalRepository rentalRepository,
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork)
    {
        _rentalRepository = rentalRepository;
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<UpdateRentalStatusResponse>> Handle(UpdateRentalStatusCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.Id);
        if (rental == null)
        {
            return ServiceResult<UpdateRentalStatusResponse>.Fail($"Rental with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        var previousStatus = rental.Status;
        rental.Status = request.Status;
        await _rentalRepository.UpdateAsync(rental);

        // Update vehicle status based on rental status
        var vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId);
        if (vehicle != null)
        {
            // If rental becomes Active, set vehicle to Rented
            if (request.Status == RentalStatus.Active)
            {
                vehicle.Status = VehicleStatus.Rented;
                await _vehicleRepository.UpdateAsync(vehicle);
            }
            // If rental becomes Completed or Cancelled, set vehicle back to Available
            else if (request.Status == RentalStatus.Completed || 
                     request.Status == RentalStatus.Cancelled)
            {
                // Check if there are other active rentals for this vehicle
                var activeRentals = await _rentalRepository.FindAsync(r => 
                    r.VehicleId == rental.VehicleId && 
                    r.Id != rental.Id && 
                    !r.IsDeleted &&
                    r.Status == RentalStatus.Active);

                // Only set to Available if no other active rentals exist
                if (!activeRentals.Any())
                {
                    vehicle.Status = VehicleStatus.Available;
                    await _vehicleRepository.UpdateAsync(vehicle);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();

        var response = new UpdateRentalStatusResponse
        {
            Id = request.Id,
            Status = request.Status,
            Message = "Rental status updated successfully."
        };

        return ServiceResult<UpdateRentalStatusResponse>.Success(response);
    }
}

