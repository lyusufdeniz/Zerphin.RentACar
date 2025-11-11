
export interface RentalStatisticsResponse {
  totalRentals: number;
  totalRevenue: number;
  averageRentalAmount: number;
  averageRentalDays: number;
  pendingRentals: number;
  confirmedRentals: number;
  activeRentals: number;
  completedRentals: number;
  cancelledRentals: number;
  overdueRentals: number;
  statusStatistics: RentalStatusStatistics[];
  monthlyStatistics: RentalMonthlyStatistics[];
}
export interface RentalStatusStatistics {
  status: number; 
  statusName: string;
  count: number;
  totalAmount: number;
}
export interface RentalMonthlyStatistics {
  year: number;
  month: number;
  monthName: string;
  count: number;
  totalRevenue: number;
}