/**
 * Vehicle Statistics Response Models
 */
export interface VehicleStatisticsResponse {
  totalVehicles: number;
  availableVehicles: number;
  rentedVehicles: number;
  maintenanceVehicles: number;
  outOfServiceVehicles: number;
  categoryStatistics: VehicleCategoryStatistics[];
  statusStatistics: VehicleStatusStatistics[];
}

export interface VehicleCategoryStatistics {
  category: number; // VehicleCategory enum (1-8)
  categoryName: string;
  count: number;
}

export interface VehicleStatusStatistics {
  status: number; // VehicleStatus enum (1-4)
  statusName: string;
  count: number;
}


