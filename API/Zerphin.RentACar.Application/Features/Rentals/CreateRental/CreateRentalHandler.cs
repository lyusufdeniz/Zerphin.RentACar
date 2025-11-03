using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Rentals.CreateRental;

public class CreateRentalHandler : IRequestHandler<CreateRentalCommand, ServiceResult<CreateRentalResponse>>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IRepository<Domain.Entities.User> _userRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateRentalHandler(
        IRentalRepository rentalRepository,
        IRepository<Domain.Entities.User> userRepository,
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _rentalRepository = rentalRepository;
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<CreateRentalResponse>> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        // Validate dates
        if (request.StartDate >= request.EndDate)
        {
            return ServiceResult<CreateRentalResponse>.Fail("Start date must be before end date.", HttpStatusCode.BadRequest);
        }

        if (request.StartDate < DateTime.UtcNow.Date)
        {
            return ServiceResult<CreateRentalResponse>.Fail("Start date cannot be in the past.", HttpStatusCode.BadRequest);
        }

        // Check if customer exists
        var customer = await _userRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            return ServiceResult<CreateRentalResponse>.Fail($"Customer with ID {request.CustomerId} not found.", HttpStatusCode.NotFound);
        }

        // Check if vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId);
        if (vehicle == null)
        {
            return ServiceResult<CreateRentalResponse>.Fail($"Vehicle with ID {request.VehicleId} not found.", HttpStatusCode.NotFound);
        }

        // Check if vehicle is available (not currently rented)
        var activeRental = await _rentalRepository.GetActiveRentalByVehicleIdAsync(request.VehicleId);
        if (activeRental != null)
        {
            return ServiceResult<CreateRentalResponse>.Fail($"Vehicle with ID {request.VehicleId} is currently rented.", HttpStatusCode.Conflict);
        }

        // Create rental entity
        var rental = _mapper.Map<Domain.Entities.Rental>(request);
        var createdRental = await _rentalRepository.AddAsync(rental);

        // Update vehicle status based on rental status
        // If rental is Active, set vehicle to Rented
        if (rental.Status == RentalStatus.Active)
        {
            vehicle.Status = VehicleStatus.Rented;
            await _vehicleRepository.UpdateAsync(vehicle);
        }

        await _unitOfWork.SaveChangesAsync();

        // Reload rental
        createdRental = await _rentalRepository.GetByIdAsync(createdRental.Id);

        // Map to response
        var response = _mapper.Map<CreateRentalResponse>(createdRental);

        return ServiceResult<CreateRentalResponse>.Success(response, HttpStatusCode.Created);
    }
}


