using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Rentals.GetRentalById;

public class GetRentalByIdHandler : IRequestHandler<GetRentalByIdCommand, ServiceResult<GetRentalByIdResponse>>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IMapper _mapper;

    public GetRentalByIdHandler(IRentalRepository rentalRepository, IMapper mapper)
    {
        _rentalRepository = rentalRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<GetRentalByIdResponse>> Handle(GetRentalByIdCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.Id);
        
        if (rental == null)
        {
            return ServiceResult<GetRentalByIdResponse>.Fail($"Rental with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        var response = _mapper.Map<GetRentalByIdResponse>(rental);
        return ServiceResult<GetRentalByIdResponse>.Success(response);
    }
}

