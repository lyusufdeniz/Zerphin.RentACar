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

  getRentalById(id: string): Observable<Rental> {
    return this.httpService.get<Rental>(`${this.baseUrl}/id`, {
      params: { id },
    });
  }

  createRental(command: CreateRentalCommand): Observable<Rental> {
    return this.httpService.post<Rental>(this.baseUrl, command);
  }

  updateRental(command: UpdateRentalCommand): Observable<Rental> {
    return this.httpService.put<Rental>(this.baseUrl, command);
  }

  updateRentalStatus(command: UpdateRentalStatusCommand): Observable<Rental> {
    return this.httpService.patch<Rental>(this.baseUrl, command);
  }

  deleteRental(id: string): Observable<void> {
    return this.httpService.delete<void>(this.baseUrl, { params: { id } });
  }
}
