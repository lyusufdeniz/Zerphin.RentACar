using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Payments.GetPaymentStatistics;

public class GetPaymentStatisticsHandler : IRequestHandler<GetPaymentStatisticsCommand, ServiceResult<GetPaymentStatisticsResponse>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentStatisticsHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<ServiceResult<GetPaymentStatisticsResponse>> Handle(GetPaymentStatisticsCommand request, CancellationToken cancellationToken)
    {
        var response = new GetPaymentStatisticsResponse();

        response.TotalPayments = await _paymentRepository.CountAsync();

        var payments = await _paymentRepository.FindAsync(p => true);
        var paymentList = payments.ToList();

        if (paymentList.Any())
        {
            response.TotalAmount = paymentList.Sum(p => p.Amount);
            response.CompletedAmount = paymentList.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
            response.PendingAmount = paymentList.Where(p => p.Status == PaymentStatus.Pending).Sum(p => p.Amount);
            response.FailedAmount = paymentList.Where(p => p.Status == PaymentStatus.Failed).Sum(p => p.Amount);
        }

        // Durum istatistikleri
        var statuses = Enum.GetValues<PaymentStatus>();
        response.StatusStatistics = statuses.Select(status => new PaymentStatusStatistics
        {
            Status = status,
            StatusName = status.ToString(),
            Count = paymentList.Count(p => p.Status == status),
            TotalAmount = paymentList.Where(p => p.Status == status).Sum(p => p.Amount)
        }).ToList();

        // Ödeme yöntemi istatistikleri
        var methods = Enum.GetValues<PaymentMethod>();
        response.MethodStatistics = methods.Select(method => new PaymentMethodStatistics
        {
            Method = method,
            MethodName = method.ToString(),
            Count = paymentList.Count(p => p.Method == method),
            TotalAmount = paymentList.Where(p => p.Method == method).Sum(p => p.Amount)
        }).ToList();

        return ServiceResult<GetPaymentStatisticsResponse>.Success(response);
    }
}

