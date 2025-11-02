using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Application.Features.Invoices.CreateInvoice;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Rentals.UpdateRentalStatus;

public class UpdateRentalStatusHandler : IRequestHandler<UpdateRentalStatusCommand, ServiceResult<UpdateRentalStatusResponse>>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateRentalStatusHandler(
        IRentalRepository rentalRepository, 
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _rentalRepository = rentalRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<ServiceResult<UpdateRentalStatusResponse>> Handle(UpdateRentalStatusCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.Id);
        if (rental == null)
        {
            return ServiceResult<UpdateRentalStatusResponse>.Fail($"Rental with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        var previousStatus = rental.Status;
        rental.Status = request.Status;
        await _rentalRepository.UpdateAsync(rental);
        await _unitOfWork.SaveChangesAsync();

        // If rental is marked as Completed, automatically create invoice
        if (request.Status == RentalStatus.Completed && previousStatus != RentalStatus.Completed)
        {
            var createInvoiceCommand = new CreateInvoiceCommand
            {
                RentalId = request.Id
            };
            await _mediator.Send(createInvoiceCommand, cancellationToken);
        }

        var response = new UpdateRentalStatusResponse
        {
            Id = request.Id,
            Status = request.Status,
            Message = "Rental status updated successfully."
        };

        return ServiceResult<UpdateRentalStatusResponse>.Success(response);
    }
}

