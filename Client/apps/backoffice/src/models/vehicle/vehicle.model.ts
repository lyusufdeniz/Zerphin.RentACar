import { VehicleCategory } from './vehicle-category.model';
import { VehicleStatus } from './vehicle-status.model';

export interface Vehicle {
  id: string;
  brand: string;
  model: string;
  licensePlate: string;
  year: number;
  color?: string;
  category: VehicleCategory;
  status: VehicleStatus;
  dailyRentalPrice: number;
  seatingCapacity: number;
  fuelType?: string;
  transmission?: string;
  km?: number;
  description?: string;
  imageBase64?: string;
  imageUrl?: string;
  hasAirConditioning: boolean;
  hasGPS: boolean;
  hasBluetooth: boolean;
  createdAt?: string;
  updatedAt?: string;
}
