import { Rental } from './rental.model';
import { RentalStatus } from './rental-status.model';

export interface CreateRentalCommand {
  customerId: string;
  vehicleId: string;
  startDate: string;
  endDate: string;
  dailyRate: number;
  totalAmount: number;
  status: RentalStatus;
  notes?: string;
  pickupLocation?: string;
  returnLocation?: string;
  kmAtStart?: number;
}

export interface UpdateRentalCommand {
  id: string;
  startDate: string;
  endDate: string;
  actualReturnDate?: string;
  dailyRate: number;
  totalAmount: number;
  lateFee?: number;
  damageFee?: number;
  notes?: string;
  pickupLocation?: string;
  returnLocation?: string;
  kmAtStart?: number;
  kmAtReturn?: number;
}

export interface UpdateRentalStatusCommand {
  id: string;
  status: RentalStatus;
}

export interface RentalSearchParams {
  PageNumber?: number;
  PageSize?: number;
  CustomerId?: string;
  VehicleId?: string;
  Status?: RentalStatus;
  StartDateFrom?: string;
  StartDateTo?: string;
  EndDateFrom?: string;
  EndDateTo?: string;
  OrderBy?: string;
  IsDescending?: boolean;
}

export interface PaginatedRentalResponse {
  rentals: Rental[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}

