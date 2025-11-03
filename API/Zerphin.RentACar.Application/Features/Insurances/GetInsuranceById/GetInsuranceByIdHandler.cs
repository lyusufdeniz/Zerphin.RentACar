using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Insurances.GetInsuranceById;

public class GetInsuranceByIdHandler : IRequestHandler<GetInsuranceByIdCommand, ServiceResult<GetInsuranceByIdResponse>>
{
    private readonly IInsuranceRepository _insuranceRepository;
    private readonly IMapper _mapper;

    public GetInsuranceByIdHandler(IInsuranceRepository insuranceRepository, IMapper mapper)
    {
        _insuranceRepository = insuranceRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<GetInsuranceByIdResponse>> Handle(GetInsuranceByIdCommand request, CancellationToken cancellationToken)
    {
        var insurance = await _insuranceRepository.GetByIdAsync(request.Id);
        
        if (insurance == null)
        {
            return ServiceResult<GetInsuranceByIdResponse>.Fail($"Insurance with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        var response = _mapper.Map<GetInsuranceByIdResponse>(insurance);
        return ServiceResult<GetInsuranceByIdResponse>.Success(response);
    }
}



