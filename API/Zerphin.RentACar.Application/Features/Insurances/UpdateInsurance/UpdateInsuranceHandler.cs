using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Insurances.UpdateInsurance;

public class UpdateInsuranceHandler : IRequestHandler<UpdateInsuranceCommand, ServiceResult<UpdateInsuranceResponse>>
{
    private readonly IInsuranceRepository _insuranceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateInsuranceHandler(
        IInsuranceRepository insuranceRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _insuranceRepository = insuranceRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<UpdateInsuranceResponse>> Handle(UpdateInsuranceCommand request, CancellationToken cancellationToken)
    {
        var insurance = await _insuranceRepository.GetByIdAsync(request.Id);
        if (insurance == null)
        {
            return ServiceResult<UpdateInsuranceResponse>.Fail($"Insurance with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        // Check if policy number is being changed and if new one already exists
        if (insurance.PolicyNumber != request.PolicyNumber)
        {
            if (await _insuranceRepository.IsPolicyNumberExistsAsync(request.PolicyNumber))
            {
                return ServiceResult<UpdateInsuranceResponse>.Fail($"Insurance with policy number '{request.PolicyNumber}' already exists.", HttpStatusCode.Conflict);
            }
        }

        // Validate dates
        if (request.StartDate >= request.EndDate)
        {
            return ServiceResult<UpdateInsuranceResponse>.Fail("Start date must be before end date.", HttpStatusCode.BadRequest);
        }

        // Update insurance properties
        _mapper.Map(request, insurance);
        await _insuranceRepository.UpdateAsync(insurance);
        await _unitOfWork.SaveChangesAsync();

        // Reload insurance
        insurance = await _insuranceRepository.GetByIdAsync(insurance.Id);

        var response = _mapper.Map<UpdateInsuranceResponse>(insurance);
        return ServiceResult<UpdateInsuranceResponse>.Success(response);
    }
}

