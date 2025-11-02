using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Insurances.DeleteInsurance;

public class DeleteInsuranceHandler : IRequestHandler<DeleteInsuranceCommand, ServiceResult<DeleteInsuranceResponse>>
{
    private readonly IInsuranceRepository _insuranceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInsuranceHandler(IInsuranceRepository insuranceRepository, IUnitOfWork unitOfWork)
    {
        _insuranceRepository = insuranceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<DeleteInsuranceResponse>> Handle(DeleteInsuranceCommand request, CancellationToken cancellationToken)
    {
        var insurance = await _insuranceRepository.GetByIdAsync(request.Id);
        if (insurance == null)
        {
            return ServiceResult<DeleteInsuranceResponse>.Fail($"Insurance with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        // Soft delete insurance
        await _insuranceRepository.DeleteAsync(insurance);
        await _unitOfWork.SaveChangesAsync();

        var response = new DeleteInsuranceResponse
        {
            Id = request.Id,
            Message = "Insurance deleted successfully."
        };

        return ServiceResult<DeleteInsuranceResponse>.Success(response);
    }
}

