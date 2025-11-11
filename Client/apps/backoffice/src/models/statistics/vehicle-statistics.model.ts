
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
  category: number; 
  categoryName: string;
  count: number;
}
export interface VehicleStatusStatistics {
  status: number; 
  statusName: string;
  count: number;
}