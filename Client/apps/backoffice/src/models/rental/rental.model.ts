import { RentalStatus } from './rental-status.model';
export interface Rental {
  id: string;
  customerId: string;
  customerName?: string;
  customerEmail?: string;
  customerPhone?: string;
  vehicleId: string;
  vehicleBrand?: string;
  vehicleModel?: string;
  vehicleLicensePlate?: string;
  startDate: string;
  endDate: string;
  actualReturnDate?: string;
  dailyRate: number;
  totalAmount: number;
  lateFee?: number;
  damageFee?: number;
  status: RentalStatus;
  notes?: string;
  pickupLocation?: string;
  returnLocation?: string;
  kmAtStart?: number;
  kmAtReturn?: number;
  createdAt?: string;
  updatedAt?: string;
}