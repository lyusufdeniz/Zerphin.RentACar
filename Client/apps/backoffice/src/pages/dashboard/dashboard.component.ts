import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ChangeDetectorRef,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartComponent } from '../../components/chart/chart.component';
import { FavoritePagesComponent } from '../../components/favorite-pages/favorite-pages.component';
import { FavoriteButtonComponent } from '../../components/favorite-button/favorite-button.component';
import { StatisticsService } from '../../services/statistics.service';
import { ToastService } from '../../services/toast.service';
import { ChartData, ChartOptions } from 'chart.js';
import {
  VehicleStatisticsResponse,
  RentalStatisticsResponse,
  InsuranceStatisticsResponse,
  UserStatisticsResponse,
} from '../../models/statistics';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    ChartComponent,
    FavoritePagesComponent,
    FavoriteButtonComponent,
  ],
  templateUrl: './dashboard.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardComponent implements OnInit {
  private statisticsService = inject(StatisticsService);
  private toastService = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);

  isLoading = true;

  // Statistics data
  vehicleStats: VehicleStatisticsResponse | null = null;
  rentalStats: RentalStatisticsResponse | null = null;
  insuranceStats: InsuranceStatisticsResponse | null = null;
  userStats: UserStatisticsResponse | null = null;

  // Chart data
  vehicleStatusChartData: ChartData<'pie'> = { labels: [], datasets: [] };
  vehicleCategoryChartData: ChartData<'doughnut'> = { labels: [], datasets: [] };
  rentalStatusChartData: ChartData<'bar'> = { labels: [], datasets: [] };
  rentalMonthlyChartData: ChartData<'line'> = { labels: [], datasets: [] };
  userRoleChartData: ChartData<'pie'> = { labels: [], datasets: [] };

  // Chart options
  pieChartOptions: ChartOptions<'pie'> = {
    plugins: {
      legend: {
        position: 'bottom',
      },
    },
  };
  doughnutChartOptions: ChartOptions<'doughnut'> = {
    plugins: {
      legend: {
        position: 'bottom',
      },
    },
  };
  barChartOptions: ChartOptions<'bar'> = {
    plugins: {
      legend: {
        position: 'top',
      },
    },
    scales: {
      x: {
        grid: {
          display: false,
        },
        border: {
          display: false,
        },
      },
      y: {
        grid: {
          display: false,
        },
        border: {
          display: false,
        },
      },
    },
  };
  lineChartOptions: ChartOptions<'line'> = {
    plugins: {
      legend: {
        position: 'top',
      },
    },
  };

  ngOnInit() {
    this.loadAllStatistics();
  }

  /**
   * Load all statistics
   */
  loadAllStatistics() {
    this.isLoading = true;
    this.cdr.markForCheck();

    // Load all statistics in parallel
    Promise.all([
      this.loadVehicleStatistics(),
      this.loadRentalStatistics(),
      this.loadInsuranceStatistics(),
      this.loadUserStatistics(),
    ])
      .then(() => {
        this.isLoading = false;
        this.cdr.markForCheck();
      })
      .catch((error) => {
        this.isLoading = false;
        this.cdr.markForCheck();
        console.error('Error loading statistics:', error);
      });
  }

  /**
   * Load vehicle statistics
   */
  private async loadVehicleStatistics() {
    try {
      const result = await this.statisticsService
        .getVehicleStatistics()
        .toPromise();
      this.vehicleStats = result || null;
      if (this.vehicleStats) {
        this.prepareVehicleCharts();
      }
    } catch (error: any) {
      this.handleError('Araç istatistikleri yüklenirken hata oluştu', error);
    }
  }

  /**
   * Load rental statistics
   */
  private async loadRentalStatistics() {
    try {
      const result = await this.statisticsService
        .getRentalStatistics()
        .toPromise();
      this.rentalStats = result || null;
      if (this.rentalStats) {
        this.prepareRentalCharts();
      }
    } catch (error: any) {
      this.handleError('Kiralama istatistikleri yüklenirken hata oluştu', error);
    }
  }

  /**
   * Load insurance statistics
   */
  private async loadInsuranceStatistics() {
    try {
      const result = await this.statisticsService
        .getInsuranceStatistics()
        .toPromise();
      this.insuranceStats = result || null;
    } catch (error: any) {
      this.handleError('Sigorta istatistikleri yüklenirken hata oluştu', error);
    }
  }

  /**
   * Load user statistics
   */
  private async loadUserStatistics() {
    try {
      const result = await this.statisticsService
        .getUserStatistics()
        .toPromise();
      this.userStats = result || null;
      if (this.userStats) {
        this.prepareUserCharts();
      }
    } catch (error: any) {
      this.handleError('Kullanıcı istatistikleri yüklenirken hata oluştu', error);
    }
  }

  /**
   * Prepare vehicle charts
   */
  private prepareVehicleCharts() {
    if (!this.vehicleStats) return;

    // Vehicle Status Chart (Pie)
    this.vehicleStatusChartData = {
      labels: this.vehicleStats.statusStatistics.map((s) => s.statusName),
      datasets: [
        {
          data: this.vehicleStats.statusStatistics.map((s) => s.count),
          backgroundColor: [
            '#00d084', // Green - Available
            '#6366f1', // Indigo - Rented
          ],
          borderWidth: 0,
        },
      ],
    };

    // Vehicle Category Chart (Doughnut)
    this.vehicleCategoryChartData = {
      labels: this.vehicleStats.categoryStatistics.map((c) => c.categoryName),
      datasets: [
        {
          data: this.vehicleStats.categoryStatistics.map((c) => c.count),
          backgroundColor: [
            '#00d084', // Green
            '#6366f1', // Indigo
            '#8b5cf6', // Purple
            '#ec4899', // Pink
            '#f59e0b', // Amber
            '#10b981', // Emerald
            '#3b82f6', // Blue
            '#f97316', // Orange
          ],
          borderWidth: 0,
        },
      ],
    };
  }

  /**
   * Prepare rental charts
   */
  private prepareRentalCharts() {
    if (!this.rentalStats) return;

    // Rental Status Chart (Bar)
    this.rentalStatusChartData = {
      labels: this.rentalStats.statusStatistics.map((s) => s.statusName),
      datasets: [
        {
          label: 'Kiralama Sayısı',
          data: this.rentalStats.statusStatistics.map((s) => s.count),
          backgroundColor: '#00d084',
          borderRadius: 8,
          borderSkipped: false,
        },
        {
          label: 'Toplam Tutar (TL)',
          data: this.rentalStats.statusStatistics.map((s) => s.totalAmount),
          backgroundColor: '#6366f1',
          borderRadius: 8,
          borderSkipped: false,
        },
      ],
    };

    // Rental Monthly Chart (Line)
    this.rentalMonthlyChartData = {
      labels: this.rentalStats.monthlyStatistics.map((m) => m.monthName),
      datasets: [
        {
          label: 'Kiralama Sayısı',
          data: this.rentalStats.monthlyStatistics.map((m) => m.count),
          borderColor: '#00d084',
          backgroundColor: 'rgba(0, 208, 132, 0.15)',
          tension: 0.4,
          fill: true,
          borderWidth: 3,
          pointRadius: 5,
          pointHoverRadius: 7,
          pointBackgroundColor: '#00d084',
          pointBorderColor: '#ffffff',
          pointBorderWidth: 2,
        },
        {
          label: 'Toplam Gelir (TL)',
          data: this.rentalStats.monthlyStatistics.map((m) => m.totalRevenue),
          borderColor: '#6366f1',
          backgroundColor: 'rgba(99, 102, 241, 0.15)',
          tension: 0.4,
          fill: true,
          borderWidth: 3,
          pointRadius: 5,
          pointHoverRadius: 7,
          pointBackgroundColor: '#6366f1',
          pointBorderColor: '#ffffff',
          pointBorderWidth: 2,
          yAxisID: 'y1',
        },
      ],
    };

    // Configure line chart with dual y-axis
    this.lineChartOptions = {
      ...this.lineChartOptions,
      scales: {
        x: {
          grid: {
            display: false,
          },
          border: {
            display: false,
          },
        },
        y: {
          type: 'linear',
          display: true,
          position: 'left',
          title: {
            display: true,
            text: 'Kiralama Sayısı',
            color: '#cccccc',
          },
          ticks: {
            color: '#999999',
          },
          grid: {
            display: false,
          },
          border: {
            display: false,
          },
        },
        y1: {
          type: 'linear',
          display: true,
          position: 'right',
          title: {
            display: true,
            text: 'Gelir (TL)',
            color: '#cccccc',
          },
          ticks: {
            color: '#999999',
          },
          grid: {
            display: false,
          },
          border: {
            display: false,
          },
        },
      },
    };
  }

  /**
   * Prepare user charts
   */
  private prepareUserCharts() {
    if (!this.userStats) return;

    // User Role Chart (Pie)
    this.userRoleChartData = {
      labels: this.userStats.roleStatistics.map((r) => r.roleName),
      datasets: [
        {
          data: this.userStats.roleStatistics.map((r) => r.count),
          backgroundColor: [
            '#00d084', // Green
            '#6366f1', // Indigo
            '#8b5cf6', // Purple
          ],
          borderWidth: 0,
        },
      ],
    };
  }

  /**
   * Handle errors
   */
  private handleError(message: string, error: any) {
    console.error(message, error);
    if (error.errorMessage && Array.isArray(error.errorMessage)) {
      this.toastService.showErrorMessages(error.errorMessage);
    } else {
      this.toastService.error(message);
    }
  }
}
