import { VehicleCategory } from './vehicle-category.model';
import { VehicleStatus } from './vehicle-status.model';
import { Vehicle } from './vehicle.model';

export interface CreateVehicleCommand {
  brand?: string;
  model?: string;
  licensePlate?: string;
  year: number;
  color?: string;
  category: VehicleCategory;
  dailyRentalPrice: number;
  seatingCapacity: number;
  fuelType?: string;
  transmission?: string;
  km?: number;
  description?: string;
  imageBase64?: string;
  hasAirConditioning: boolean;
  hasGPS: boolean;
  hasBluetooth: boolean;
}

export interface UpdateVehicleCommand extends CreateVehicleCommand {
  id: string;
}

export interface UpdateVehicleStatusCommand {
  id: string;
  status: VehicleStatus;
}

export interface VehicleSearchParams {
  PageNumber?: number;
  PageSize?: number;
  Category?: VehicleCategory;
  Status?: VehicleStatus;
  MinPrice?: number;
  MaxPrice?: number;
  Brand?: string;
  Model?: string;
  LicensePlate?: string;
  OrderBy?: string;
  IsDescending?: boolean;
}

export interface PaginatedVehicleResponse {
  vehicles: Vehicle[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}

