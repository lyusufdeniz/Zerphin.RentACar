import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import {
  Insurance,
  CreateInsuranceCommand,
  UpdateInsuranceCommand,
  InsuranceSearchParams,
  PaginatedInsuranceResponse,
} from '../models/insurance';

@Injectable({
  providedIn: 'root',
})
export class InsuranceService {
  private httpService = inject(HttpService);
  private baseUrl = '/Insurances';

  /**
   * Search insurances with filters
   */
  searchInsurances(
    params?: InsuranceSearchParams
  ): Observable<PaginatedInsuranceResponse> {
    return this.httpService.get<PaginatedInsuranceResponse>(
      `${this.baseUrl}/search`,
      {
        params: params as any,
      }
    );
  }

  /**
   * Get insurance by ID
   */
  getInsuranceById(id: string): Observable<Insurance> {
    return this.httpService.get<Insurance>(`${this.baseUrl}/id`, {
      params: { id },
    });
  }

  /**
   * Create new insurance
   */
  createInsurance(command: CreateInsuranceCommand): Observable<Insurance> {
    return this.httpService.post<Insurance>(this.baseUrl, command);
  }

  /**
   * Update insurance
   */
  updateInsurance(command: UpdateInsuranceCommand): Observable<Insurance> {
    return this.httpService.put<Insurance>(this.baseUrl, command);
  }

  /**
   * Delete insurance
   */
  deleteInsurance(id: string): Observable<void> {
    return this.httpService.delete<void>(this.baseUrl, { params: { id } });
  }
}

