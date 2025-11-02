using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Insurances.CreateInsurance;

public class CreateInsuranceHandler : IRequestHandler<CreateInsuranceCommand, ServiceResult<CreateInsuranceResponse>>
{
    private readonly IInsuranceRepository _insuranceRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateInsuranceHandler(
        IInsuranceRepository insuranceRepository,
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _insuranceRepository = insuranceRepository;
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<CreateInsuranceResponse>> Handle(CreateInsuranceCommand request, CancellationToken cancellationToken)
    {
        // Check if policy number already exists
        if (await _insuranceRepository.IsPolicyNumberExistsAsync(request.PolicyNumber))
        {
            return ServiceResult<CreateInsuranceResponse>.Fail($"Insurance with policy number '{request.PolicyNumber}' already exists.", HttpStatusCode.Conflict);
        }

        // Check if vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId);
        if (vehicle == null)
        {
            return ServiceResult<CreateInsuranceResponse>.Fail($"Vehicle with ID {request.VehicleId} not found.", HttpStatusCode.NotFound);
        }

        // Validate dates
        if (request.StartDate >= request.EndDate)
        {
            return ServiceResult<CreateInsuranceResponse>.Fail("Start date must be before end date.", HttpStatusCode.BadRequest);
        }

        // Create insurance entity
        var insurance = _mapper.Map<Domain.Entities.Insurance>(request);
        var createdInsurance = await _insuranceRepository.AddAsync(insurance);

        await _unitOfWork.SaveChangesAsync();

        // Reload insurance
        createdInsurance = await _insuranceRepository.GetByIdAsync(createdInsurance.Id);

        // Map to response
        var response = _mapper.Map<CreateInsuranceResponse>(createdInsurance);

        return ServiceResult<CreateInsuranceResponse>.Success(response, HttpStatusCode.Created);
    }
}


