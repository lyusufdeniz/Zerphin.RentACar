using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Rentals.GetRentalStatistics;

public class GetRentalStatisticsHandler : IRequestHandler<GetRentalStatisticsCommand, ServiceResult<GetRentalStatisticsResponse>>
{
    private readonly IRentalRepository _rentalRepository;

    public GetRentalStatisticsHandler(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<ServiceResult<GetRentalStatisticsResponse>> Handle(GetRentalStatisticsCommand request, CancellationToken cancellationToken)
    {
        var response = new GetRentalStatisticsResponse();

        // Temel sayılar
        response.TotalRentals = await _rentalRepository.CountAsync();

        // Duruma göre sayılar
        response.ActiveRentals = await _rentalRepository.CountAsync(r => r.Status == RentalStatus.Active);
        response.CompletedRentals = await _rentalRepository.CountAsync(r => r.Status == RentalStatus.Completed);
        response.CancelledRentals = await _rentalRepository.CountAsync(r => r.Status == RentalStatus.Cancelled);

        // Toplam gelir ve ortalama
        var rentals = await _rentalRepository.FindAsync(r => true);
        var rentalList = rentals.ToList();
        
        if (rentalList.Any())
        {
            response.TotalRevenue = rentalList.Sum(r => r.TotalAmount);
            response.AverageRentalAmount = rentalList.Average(r => r.TotalAmount);
            
            var completedRentals = rentalList.Where(r => r.Status == RentalStatus.Completed && r.ActualReturnDate.HasValue).ToList();
            if (completedRentals.Any())
            {
                var totalDays = completedRentals.Sum(r => (r.ActualReturnDate!.Value - r.StartDate).TotalDays);
                response.AverageRentalDays = totalDays / completedRentals.Count;
            }
        }

        // Durum istatistikleri
        response.StatusStatistics = new List<RentalStatusStatistics>
        {
            new() 
            { 
                Status = RentalStatus.Active, 
                StatusName = "Aktif", 
                Count = response.ActiveRentals,
                TotalAmount = rentalList.Where(r => r.Status == RentalStatus.Active).Sum(r => r.TotalAmount)
            },
            new() 
            { 
                Status = RentalStatus.Completed, 
                StatusName = "Bitti", 
                Count = response.CompletedRentals,
                TotalAmount = rentalList.Where(r => r.Status == RentalStatus.Completed).Sum(r => r.TotalAmount)
            },
            new() 
            { 
                Status = RentalStatus.Cancelled, 
                StatusName = "İptal Edildi", 
                Count = response.CancelledRentals,
                TotalAmount = rentalList.Where(r => r.Status == RentalStatus.Cancelled).Sum(r => r.TotalAmount)
            }
        };

        // Aylık istatistikler (son 12 ay) - LINQ to Objects ile
        var last12MonthsRentals = rentalList
            .Where(r => r.CreatedAt >= DateTime.UtcNow.AddMonths(-12))
            .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
            .Select(g => new RentalMonthlyStatistics
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                Count = g.Count(),
                TotalRevenue = g.Sum(r => r.TotalAmount)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToList();

        response.MonthlyStatistics = last12MonthsRentals;

        return ServiceResult<GetRentalStatisticsResponse>.Success(response);
    }
}

