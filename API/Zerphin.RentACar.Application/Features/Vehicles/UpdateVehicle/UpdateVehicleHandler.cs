using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Contracts.Services;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Vehicles.UpdateVehicle;

public class UpdateVehicleHandler : IRequestHandler<UpdateVehicleCommand, ServiceResult<UpdateVehicleResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICloudinaryService _cloudinaryService;

    public UpdateVehicleHandler(
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICloudinaryService cloudinaryService)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ServiceResult<UpdateVehicleResponse>> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(request.Id);
        if (vehicle == null)
        {
            return ServiceResult<UpdateVehicleResponse>.Fail($"Vehicle with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        // Check if license plate is being changed and if new one already exists
        if (vehicle.LicensePlate != request.LicensePlate)
        {
            if (await _vehicleRepository.IsLicensePlateExistsAsync(request.LicensePlate))
            {
                return ServiceResult<UpdateVehicleResponse>.Fail($"Vehicle with license plate '{request.LicensePlate}' already exists.", HttpStatusCode.Conflict);
            }
        }

        // Handle image upload if provided
        if (!string.IsNullOrWhiteSpace(request.ImageBase64))
        {
            // Delete old image if exists
            if (!string.IsNullOrWhiteSpace(vehicle.ImageUrl))
            {
                await _cloudinaryService.DeleteImageAsync(vehicle.ImageUrl);
            }

            // Upload new image
            var imageUrl = await _cloudinaryService.UploadImageAsync(request.ImageBase64, "vehicles", $"{request.LicensePlate.Replace(" ", "_")}");
            if (imageUrl != null)
            {
                vehicle.ImageUrl = imageUrl;
            }
        }

        // Update vehicle properties
        _mapper.Map(request, vehicle);
        await _vehicleRepository.UpdateAsync(vehicle);
        await _unitOfWork.SaveChangesAsync();

        // Reload vehicle
        vehicle = await _vehicleRepository.GetByIdAsync(vehicle.Id);

        var response = _mapper.Map<UpdateVehicleResponse>(vehicle);
        return ServiceResult<UpdateVehicleResponse>.Success(response);
    }
}

