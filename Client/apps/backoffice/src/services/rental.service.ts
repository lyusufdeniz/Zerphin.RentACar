import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import {
  Rental,
  CreateRentalCommand,
  UpdateRentalCommand,
  UpdateRentalStatusCommand,
  RentalSearchParams,
  PaginatedRentalResponse,
} from '../models/rental';

@Injectable({
  providedIn: 'root',
})
export class RentalService {
  private httpService = inject(HttpService);
  private baseUrl = '/Rentals';

  /**
   * Search rentals with filters
   */
  searchRentals(
    params?: RentalSearchParams
  ): Observable<PaginatedRentalResponse> {
    return this.httpService.get<PaginatedRentalResponse>(
      `${this.baseUrl}/search`,
      {
        params: params as any,
      }
    );
  }

  /**
   * Get rental by ID
   */
  getRentalById(id: string): Observable<Rental> {
    return this.httpService.get<Rental>(`${this.baseUrl}/id`, {
      params: { id },
    });
  }

  /**
   * Create new rental
   */
  createRental(command: CreateRentalCommand): Observable<Rental> {
    return this.httpService.post<Rental>(this.baseUrl, command);
  }

  /**
   * Update rental
   */
  updateRental(command: UpdateRentalCommand): Observable<Rental> {
    return this.httpService.put<Rental>(this.baseUrl, command);
  }

  /**
   * Update rental status
   */
  updateRentalStatus(command: UpdateRentalStatusCommand): Observable<Rental> {
    return this.httpService.patch<Rental>(this.baseUrl, command);
  }

  /**
   * Delete rental
   */
  deleteRental(id: string): Observable<void> {
    return this.httpService.delete<void>(this.baseUrl, { params: { id } });
  }
}

