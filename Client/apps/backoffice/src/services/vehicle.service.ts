import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import {
  Vehicle,
  CreateVehicleCommand,
  UpdateVehicleCommand,
  UpdateVehicleStatusCommand,
  VehicleSearchParams,
  PaginatedVehicleResponse,
} from '../models/vehicle';

@Injectable({
  providedIn: 'root',
})
export class VehicleService {
  private httpService = inject(HttpService);

  /**
   * Search vehicles with filters
   */
  searchVehicles(
    params?: VehicleSearchParams
  ): Observable<PaginatedVehicleResponse> {
    return this.httpService.get<PaginatedVehicleResponse>(
      '/Vehicles/search',
      {
        params: params as any,
      }
    );
  }

  /**
   * Get vehicle by ID
   */
  getVehicleById(id: string): Observable<Vehicle> {
    return this.httpService.get<Vehicle>(`/Vehicles/id?Id=${id}`);
  }

  /**
   * Create new vehicle
   */
  createVehicle(command: CreateVehicleCommand): Observable<Vehicle> {
    return this.httpService.post<Vehicle>('/Vehicles', command);
  }

  /**
   * Update vehicle
   */
  updateVehicle(command: UpdateVehicleCommand): Observable<Vehicle> {
    return this.httpService.put<Vehicle>('/Vehicles', command);
  }

  /**
   * Delete vehicle
   */
  deleteVehicle(id: string): Observable<void> {
    return this.httpService.delete<void>(`/Vehicles?Id=${id}`);
  }

  /**
   * Update vehicle status
   */
  updateVehicleStatus(
    command: UpdateVehicleStatusCommand
  ): Observable<Vehicle> {
    return this.httpService.patch<Vehicle>('/Vehicles', command);
  }
}

