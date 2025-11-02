using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Rentals.UpdateRental;

public class UpdateRentalHandler : IRequestHandler<UpdateRentalCommand, ServiceResult<UpdateRentalResponse>>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateRentalHandler(
        IRentalRepository rentalRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _rentalRepository = rentalRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<UpdateRentalResponse>> Handle(UpdateRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.Id);
        if (rental == null)
        {
            return ServiceResult<UpdateRentalResponse>.Fail($"Rental with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        // Validate dates
        if (request.StartDate >= request.EndDate)
        {
            return ServiceResult<UpdateRentalResponse>.Fail("Start date must be before end date.", HttpStatusCode.BadRequest);
        }

        // Update rental properties
        _mapper.Map(request, rental);
        await _rentalRepository.UpdateAsync(rental);
        await _unitOfWork.SaveChangesAsync();

        // Reload rental
        rental = await _rentalRepository.GetByIdAsync(rental.Id);

        var response = _mapper.Map<UpdateRentalResponse>(rental);
        return ServiceResult<UpdateRentalResponse>.Success(response);
    }
}


