import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ChangeDetectorRef,
  inject,
  computed,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartComponent } from '../../components/chart/chart.component';
import { FavoritePagesComponent } from '../../components/favorite-pages/favorite-pages.component';
import { FavoriteButtonComponent } from '../../components/favorite-button/favorite-button.component';
import { StatisticsService } from '../../services/statistics.service';
import { ToastService } from '../../services/toast.service';
import { LanguageService } from '../../services/language.service';
import { toObservable } from '@angular/core/rxjs-interop';
import { Subscription } from 'rxjs';
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
export class DashboardComponent implements OnInit, OnDestroy {
  private statisticsService = inject(StatisticsService);
  private toastService = inject(ToastService);
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);
  private languageSubscription?: Subscription;

  isLoading = true;

  translations = computed(() => {
    const _ = this.languageService.currentLanguage();
    return {
      title: this.languageService.translate('dashboard.title'),
      refresh: this.languageService.translate('dashboard.refresh'),
      loading: this.languageService.translate('dashboard.loading'),
      loadingStats: this.languageService.translate('dashboard.loadingStats'),
      vehicles: {
        title: this.languageService.translate('dashboard.vehicles.title'),
        total: this.languageService.translate('dashboard.vehicles.total'),
        available: this.languageService.translate('dashboard.vehicles.available'),
        rented: this.languageService.translate('dashboard.vehicles.rented'),
        maintenance: this.languageService.translate('dashboard.vehicles.maintenance'),
      },
      rentals: {
        title: this.languageService.translate('dashboard.rentals.title'),
        total: this.languageService.translate('dashboard.rentals.total'),
        totalRevenue: this.languageService.translate('dashboard.rentals.totalRevenue'),
        averageAmount: this.languageService.translate('dashboard.rentals.averageAmount'),
        active: this.languageService.translate('dashboard.rentals.active'),
      },
      insurances: {
        title: this.languageService.translate('dashboard.insurances.title'),
        total: this.languageService.translate('dashboard.insurances.total'),
        active: this.languageService.translate('dashboard.insurances.active'),
        expired: this.languageService.translate('dashboard.insurances.expired'),
        expiringSoon: this.languageService.translate('dashboard.insurances.expiringSoon'),
      },
      users: {
        title: this.languageService.translate('dashboard.users.title'),
        total: this.languageService.translate('dashboard.users.total'),
        active: this.languageService.translate('dashboard.users.active'),
        inactive: this.languageService.translate('dashboard.users.inactive'),
        newLast30Days: this.languageService.translate('dashboard.users.newLast30Days'),
      },
      charts: {
        vehicleStatus: this.languageService.translate('dashboard.charts.vehicleStatus'),
        vehicleCategory: this.languageService.translate('dashboard.charts.vehicleCategory'),
        rentalStatus: this.languageService.translate('dashboard.charts.rentalStatus'),
        userRole: this.languageService.translate('dashboard.charts.userRole'),
      },
    };
  });

  vehicleStats: VehicleStatisticsResponse | null = null;
  rentalStats: RentalStatisticsResponse | null = null;
  insuranceStats: InsuranceStatisticsResponse | null = null;
  userStats: UserStatisticsResponse | null = null;

  vehicleStatusChartData: ChartData<'pie'> = { labels: [], datasets: [] };
  vehicleCategoryChartData: ChartData<'doughnut'> = { labels: [], datasets: [] };
  rentalStatusChartData: ChartData<'bar'> = { labels: [], datasets: [] };
  userRoleChartData: ChartData<'pie'> = { labels: [], datasets: [] };

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
        ticks: {
          display: true, 
          color: '#999999',
          font: {
            size: 12,
          },
        },
        grid: {
          display: false,
        },
        border: {
          display: false,
        },
      },
      y: {
        type: 'linear',
        position: 'left',
        title: {
          display: false, 
        },
        ticks: {
          display: false, 
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
        position: 'right',
        title: {
          display: false, 
        },
        ticks: {
          display: false, 
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

  ngOnInit() {
    this.loadAllStatistics();

    this.languageSubscription = toObservable(this.languageService.currentLanguage).subscribe(() => {
      this.cdr.markForCheck();
    });
  }

  ngOnDestroy() {
    if (this.languageSubscription) {
      this.languageSubscription.unsubscribe();
    }
  }

  loadAllStatistics() {
    this.isLoading = true;
    this.cdr.markForCheck();

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
      this.handleError(this.languageService.translate('messages.errors.statistics.vehicle'), error);
    }
  }

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
      this.handleError(this.languageService.translate('messages.errors.statistics.rental'), error);
    }
  }

  private async loadInsuranceStatistics() {
    try {
      const result = await this.statisticsService
        .getInsuranceStatistics()
        .toPromise();
      this.insuranceStats = result || null;
    } catch (error: any) {
      this.handleError(this.languageService.translate('messages.errors.statistics.insurance'), error);
    }
  }

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
      this.handleError(this.languageService.translate('messages.errors.statistics.user'), error);
    }
  }

  private prepareVehicleCharts() {
    if (!this.vehicleStats) return;

    const statusColors = [
      '#2196f3', 
      '#4caf50', 
      '#ff9800', 
      '#9c27b0', 
      '#f44336', 
      '#00bcd4', 
    ];
    this.vehicleStatusChartData = {
      labels: this.vehicleStats.statusStatistics.map((s) => s.statusName),
      datasets: [
        {
          data: this.vehicleStats.statusStatistics.map((s) => s.count),
          backgroundColor: statusColors.slice(0, this.vehicleStats.statusStatistics.length),
          borderWidth: 0,
        },
      ],
    };

    const categoryColors = [
      '#e91e63', 
      '#3f51b5', 
      '#009688', 
      '#ff5722', 
      '#673ab7', 
      '#8bc34a', 
      '#ffc107', 
      '#607d8b', 
    ];
    this.vehicleCategoryChartData = {
      labels: this.vehicleStats.categoryStatistics.map((c) => c.categoryName),
      datasets: [
        {
          data: this.vehicleStats.categoryStatistics.map((c) => c.count),
          backgroundColor: categoryColors.slice(0, this.vehicleStats.categoryStatistics.length),
          borderWidth: 0,
        },
      ],
    };
  }

  private prepareRentalCharts() {
    if (!this.rentalStats) return;

    this.rentalStatusChartData = {
      labels: this.rentalStats.statusStatistics.map((s) => s.statusName),
      datasets: [
        {
          label: this.languageService.translate('messages.dashboard.chartLabels.rentalCount'),
          data: this.rentalStats.statusStatistics.map((s) => s.count),
          backgroundColor: '#9c27b0', 
          borderRadius: 8,
          borderSkipped: false,
          yAxisID: 'y',
        },
        {
          label: this.languageService.translate('messages.dashboard.chartLabels.totalAmount'),
          data: this.rentalStats.statusStatistics.map((s) => s.totalAmount),
          backgroundColor: '#e91e63', 
          borderRadius: 8,
          borderSkipped: false,
          yAxisID: 'y1',
        },
      ],
    };
  }

  private prepareUserCharts() {
    if (!this.userStats) return;

    const roleColors = [
      '#4caf50', 
      '#e91e63', 
      '#2196f3', 
      '#ff9800', 
      '#9c27b0', 
      '#00bcd4', 
      '#f44336', 
      '#ffc107', 
    ];
    this.userRoleChartData = {
      labels: this.userStats.roleStatistics.map((r) => r.roleName),
      datasets: [
        {
          data: this.userStats.roleStatistics.map((r) => r.count),
          backgroundColor: roleColors.slice(0, this.userStats.roleStatistics.length),
          borderWidth: 0,
        },
      ],
    };
  }

  private handleError(message: string, error: any) {
    console.error(message, error);
    if (error.errorMessage && Array.isArray(error.errorMessage)) {
      this.toastService.showErrorMessages(error.errorMessage);
    } else {
      this.toastService.error(message);
    }
  }
}
