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

  getVehicleStatistics(): Observable<VehicleStatisticsResponse> {
    return this.httpService.get<VehicleStatisticsResponse>(
      '/Vehicles/statistics'
    );
  }

  getRentalStatistics(): Observable<RentalStatisticsResponse> {
    return this.httpService.get<RentalStatisticsResponse>(
      '/Rentals/statistics'
    );
  }

  getPaymentStatistics(): Observable<PaymentStatisticsResponse> {
    return this.httpService.get<PaymentStatisticsResponse>(
      '/Payments/statistics'
    );
  }

  getInvoiceStatistics(): Observable<InvoiceStatisticsResponse> {
    return this.httpService.get<InvoiceStatisticsResponse>(
      '/Invoices/statistics'
    );
  }

  getInsuranceStatistics(): Observable<InsuranceStatisticsResponse> {
    return this.httpService.get<InsuranceStatisticsResponse>(
      '/Insurances/statistics'
    );
  }

  getUserStatistics(): Observable<UserStatisticsResponse> {
    return this.httpService.get<UserStatisticsResponse>(
      '/Users/statistics'
    );
  }
}
