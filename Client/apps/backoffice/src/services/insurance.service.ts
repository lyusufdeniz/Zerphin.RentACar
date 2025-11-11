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
  getInsuranceById(id: string): Observable<Insurance> {
    return this.httpService.get<Insurance>(`${this.baseUrl}/id`, {
      params: { id },
    });
  }
  createInsurance(command: CreateInsuranceCommand): Observable<Insurance> {
    return this.httpService.post<Insurance>(this.baseUrl, command);
  }
  updateInsurance(command: UpdateInsuranceCommand): Observable<Insurance> {
    return this.httpService.put<Insurance>(this.baseUrl, command);
  }
  deleteInsurance(id: string): Observable<void> {
    return this.httpService.delete<void>(this.baseUrl, { params: { id } });
  }
}