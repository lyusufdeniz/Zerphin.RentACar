using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Payments.CreatePayment;

public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, ServiceResult<CreatePaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePaymentHandler(
        IPaymentRepository paymentRepository,
        IRentalRepository rentalRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _rentalRepository = rentalRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<CreatePaymentResponse>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        // Check if rental exists
        var rental = await _rentalRepository.GetByIdAsync(request.RentalId);
        if (rental == null)
        {
            return ServiceResult<CreatePaymentResponse>.Fail($"Rental with ID {request.RentalId} not found.", HttpStatusCode.NotFound);
        }

        // Create payment entity
        var payment = _mapper.Map<Domain.Entities.Payment>(request);
        var createdPayment = await _paymentRepository.AddAsync(payment);

        await _unitOfWork.SaveChangesAsync();

        // Reload payment
        createdPayment = await _paymentRepository.GetByIdAsync(createdPayment.Id);

        // Map to response
        var response = _mapper.Map<CreatePaymentResponse>(createdPayment);

        return ServiceResult<CreatePaymentResponse>.Success(response, HttpStatusCode.Created);
    }
}

