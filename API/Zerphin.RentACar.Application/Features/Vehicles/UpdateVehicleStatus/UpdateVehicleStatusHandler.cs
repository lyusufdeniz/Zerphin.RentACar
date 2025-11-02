using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Vehicles.UpdateVehicleStatus;

public class UpdateVehicleStatusHandler : IRequestHandler<UpdateVehicleStatusCommand, ServiceResult<UpdateVehicleStatusResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVehicleStatusHandler(IVehicleRepository vehicleRepository, IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<UpdateVehicleStatusResponse>> Handle(UpdateVehicleStatusCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(request.Id);
        if (vehicle == null)
        {
            return ServiceResult<UpdateVehicleStatusResponse>.Fail($"Vehicle with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        vehicle.Status = request.Status;
        await _vehicleRepository.UpdateAsync(vehicle);
        await _unitOfWork.SaveChangesAsync();

        var response = new UpdateVehicleStatusResponse
        {
            Id = request.Id,
            Status = request.Status,
            Message = "Vehicle status updated successfully."
        };

        return ServiceResult<UpdateVehicleStatusResponse>.Success(response);
    }
}


