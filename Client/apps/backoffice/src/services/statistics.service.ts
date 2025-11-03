import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import {
  VehicleStatisticsResponse,
  RentalStatisticsResponse,
  PaymentStatisticsResponse,
  InvoiceStatisticsResponse,
  InsuranceStatisticsResponse,
  UserStatisticsResponse,
} from '../models/statistics';

@Injectable({
  providedIn: 'root',
})
export class StatisticsService {
  private httpService = inject(HttpService);

  /**
   * Get vehicle statistics
   */
  getVehicleStatistics(): Observable<VehicleStatisticsResponse> {
    return this.httpService.get<VehicleStatisticsResponse>(
      '/Vehicles/statistics'
    );
  }

  /**
   * Get rental statistics
   */
  getRentalStatistics(): Observable<RentalStatisticsResponse> {
    return this.httpService.get<RentalStatisticsResponse>(
      '/Rentals/statistics'
    );
  }


  /**
   * Get payment statistics
   */
  getPaymentStatistics(): Observable<PaymentStatisticsResponse> {
    return this.httpService.get<PaymentStatisticsResponse>(
      '/Payments/statistics'
    );
  }

  /**
   * Get invoice statistics
   */
  getInvoiceStatistics(): Observable<InvoiceStatisticsResponse> {
    return this.httpService.get<InvoiceStatisticsResponse>(
      '/Invoices/statistics'
    );
  }

  /**
   * Get insurance statistics
   */
  getInsuranceStatistics(): Observable<InsuranceStatisticsResponse> {
    return this.httpService.get<InsuranceStatisticsResponse>(
      '/Insurances/statistics'
    );
  }

  /**
   * Get user statistics
   */
  getUserStatistics(): Observable<UserStatisticsResponse> {
    return this.httpService.get<UserStatisticsResponse>(
      '/Users/statistics'
    );
  }
}

