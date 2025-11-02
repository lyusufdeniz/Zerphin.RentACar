using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Vehicles.GetVehicleById;

public class GetVehicleByIdHandler : IRequestHandler<GetVehicleByIdCommand, ServiceResult<GetVehicleByIdResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public GetVehicleByIdHandler(IVehicleRepository vehicleRepository, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<GetVehicleByIdResponse>> Handle(GetVehicleByIdCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(request.Id);
        
        if (vehicle == null)
        {
            return ServiceResult<GetVehicleByIdResponse>.Fail($"Vehicle with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        var response = _mapper.Map<GetVehicleByIdResponse>(vehicle);
        return ServiceResult<GetVehicleByIdResponse>.Success(response);
    }
}


