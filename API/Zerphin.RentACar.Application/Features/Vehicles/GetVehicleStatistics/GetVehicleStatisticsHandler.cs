using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Vehicles.GetVehicleStatistics;

public class GetVehicleStatisticsHandler : IRequestHandler<GetVehicleStatisticsCommand, ServiceResult<GetVehicleStatisticsResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IRentalRepository _rentalRepository;

    public GetVehicleStatisticsHandler(
        IVehicleRepository vehicleRepository,
        IRentalRepository rentalRepository)
    {
        _vehicleRepository = vehicleRepository;
        _rentalRepository = rentalRepository;
    }

    public async Task<ServiceResult<GetVehicleStatisticsResponse>> Handle(GetVehicleStatisticsCommand request, CancellationToken cancellationToken)
    {
        var response = new GetVehicleStatisticsResponse();

        // Toplam araç sayısı
        response.TotalVehicles = await _vehicleRepository.CountAsync();

        // Duruma göre araç sayıları
        response.AvailableVehicles = await _vehicleRepository.CountAsync(v => v.Status == VehicleStatus.Available);
        response.RentedVehicles = await _vehicleRepository.CountAsync(v => v.Status == VehicleStatus.Rented);
        response.MaintenanceVehicles = await _vehicleRepository.CountAsync(v => v.Status == VehicleStatus.Maintenance);
        response.OutOfServiceVehicles = await _vehicleRepository.CountAsync(v => v.Status == VehicleStatus.OutOfService);

        // Durum istatistikleri
        response.StatusStatistics = new List<VehicleStatusStatistics>
        {
            new() { Status = VehicleStatus.Available, StatusName = "Müsait", Count = response.AvailableVehicles },
            new() { Status = VehicleStatus.Rented, StatusName = "Kiralanmış", Count = response.RentedVehicles },
            new() { Status = VehicleStatus.Maintenance, StatusName = "Bakımda", Count = response.MaintenanceVehicles },
            new() { Status = VehicleStatus.OutOfService, StatusName = "Hizmet Dışı", Count = response.OutOfServiceVehicles }
        };

        // Kategoriye göre araç sayıları
        var categories = Enum.GetValues<VehicleCategory>();
        var categoryStats = new List<VehicleCategoryStatistics>();

        foreach (var category in categories)
        {
            var count = await _vehicleRepository.CountAsync(v => v.Category == category);
            categoryStats.Add(new VehicleCategoryStatistics
            {
                Category = category,
                CategoryName = category.ToString(),
                Count = count
            });
        }

        response.CategoryStatistics = categoryStats;

        return ServiceResult<GetVehicleStatisticsResponse>.Success(response);
    }
}

