using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Contracts.Services;

namespace Zerphin.RentACar.Application.Features.Vehicles.DeleteVehicle;

public class DeleteVehicleHandler : IRequestHandler<DeleteVehicleCommand, ServiceResult<DeleteVehicleResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;

    public DeleteVehicleHandler(
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        ICloudinaryService cloudinaryService)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ServiceResult<DeleteVehicleResponse>> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(request.Id);
        if (vehicle == null)
        {
            return ServiceResult<DeleteVehicleResponse>.Fail($"Vehicle with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        // Delete image from Cloudinary if exists
        if (!string.IsNullOrWhiteSpace(vehicle.ImageUrl))
        {
            await _cloudinaryService.DeleteImageAsync(vehicle.ImageUrl);
        }

        // Soft delete vehicle
        await _vehicleRepository.DeleteAsync(vehicle);
        await _unitOfWork.SaveChangesAsync();

        var response = new DeleteVehicleResponse
        {
            Id = request.Id,
            Message = "Vehicle deleted successfully."
        };

        return ServiceResult<DeleteVehicleResponse>.Success(response);
    }
}

