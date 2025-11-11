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

  getVehicleById(id: string): Observable<Vehicle> {
    return this.httpService.get<Vehicle>(`/Vehicles/id?Id=${id}`);
  }

  createVehicle(command: CreateVehicleCommand): Observable<Vehicle> {
    return this.httpService.post<Vehicle>('/Vehicles', command);
  }

  updateVehicle(command: UpdateVehicleCommand): Observable<Vehicle> {
    return this.httpService.put<Vehicle>('/Vehicles', command);
  }

  deleteVehicle(id: string): Observable<void> {
    return this.httpService.delete<void>(`/Vehicles?Id=${id}`);
  }

  updateVehicleStatus(
    command: UpdateVehicleStatusCommand
  ): Observable<Vehicle> {
    return this.httpService.patch<Vehicle>('/Vehicles', command);
  }
}
