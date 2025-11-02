using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Rentals.DeleteRental;

public class DeleteRentalHandler : IRequestHandler<DeleteRentalCommand, ServiceResult<DeleteRentalResponse>>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRentalHandler(IRentalRepository rentalRepository, IUnitOfWork unitOfWork)
    {
        _rentalRepository = rentalRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<DeleteRentalResponse>> Handle(DeleteRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.Id);
        if (rental == null)
        {
            return ServiceResult<DeleteRentalResponse>.Fail($"Rental with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        // Soft delete rental
        await _rentalRepository.DeleteAsync(rental);
        await _unitOfWork.SaveChangesAsync();

        var response = new DeleteRentalResponse
        {
            Id = request.Id,
            Message = "Rental deleted successfully."
        };

        return ServiceResult<DeleteRentalResponse>.Success(response);
    }
}

