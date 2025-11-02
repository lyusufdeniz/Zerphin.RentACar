using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Contracts.Services;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Vehicles.CreateVehicle;

public class CreateVehicleHandler : IRequestHandler<CreateVehicleCommand, ServiceResult<CreateVehicleResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICloudinaryService _cloudinaryService;

    public CreateVehicleHandler(
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

    public async Task<ServiceResult<CreateVehicleResponse>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        // Check if license plate already exists
        if (await _vehicleRepository.IsLicensePlateExistsAsync(request.LicensePlate))
        {
            return ServiceResult<CreateVehicleResponse>.Fail($"Vehicle with license plate '{request.LicensePlate}' already exists.", HttpStatusCode.Conflict);
        }

        // Upload image if provided
        string? imageUrl = null;
        if (!string.IsNullOrWhiteSpace(request.ImageBase64))
        {
            imageUrl = await _cloudinaryService.UploadImageAsync(request.ImageBase64, "vehicles", $"{request.LicensePlate.Replace(" ", "_")}");
        }

        // Create vehicle entity
        var vehicle = _mapper.Map<Vehicle>(request);
        vehicle.ImageUrl = imageUrl;
        
        var createdVehicle = await _vehicleRepository.AddAsync(vehicle);

        await _unitOfWork.SaveChangesAsync();

        // Reload vehicle
        createdVehicle = await _vehicleRepository.GetByIdAsync(createdVehicle.Id);

        // Map to response
        var response = _mapper.Map<CreateVehicleResponse>(createdVehicle);

        return ServiceResult<CreateVehicleResponse>.Success(response, HttpStatusCode.Created);
    }
}

