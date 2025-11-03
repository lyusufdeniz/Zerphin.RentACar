import { InsuranceStatus } from './insurance-status.model';

export interface Insurance {
  id: string;
  vehicleId: string;
  vehicleBrand?: string;
  vehicleModel?: string;
  vehicleLicensePlate?: string;
  policyNumber: string;
  insuranceCompany: string;
  startDate: string;
  endDate: string;
  premiumAmount: number;
  coverageType?: string;
  coverageLimit?: number;
  deductible?: number;
  coverageDetails?: string;
  contactInfo?: string;
  status?: InsuranceStatus; // Optional - can be calculated from dates
  notes?: string;
  createdAt?: string;
  updatedAt?: string;
}

