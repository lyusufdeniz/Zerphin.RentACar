import { Insurance } from './insurance.model';
import { InsuranceStatus } from './insurance-status.model';

export interface CreateInsuranceCommand {
  vehicleId: string;
  insuranceCompany?: string;
  policyNumber?: string;
  startDate: string;
  endDate: string;
  premiumAmount: number;
  coverageType?: string;
  coverageLimit?: number;
  deductible?: number;
  coverageDetails?: string;
  contactInfo?: string;
}

export interface UpdateInsuranceCommand {
  id: string;
  insuranceCompany?: string;
  policyNumber?: string;
  startDate: string;
  endDate: string;
  premiumAmount: number;
  coverageType?: string;
  coverageLimit?: number;
  deductible?: number;
  coverageDetails?: string;
  contactInfo?: string;
}

export interface InsuranceSearchParams {
  PageNumber?: number;
  PageSize?: number;
  LicensePlate?: string;
  StartDateFrom?: string;
  StartDateTo?: string;
  EndDateFrom?: string;
  EndDateTo?: string;
  PolicyNumber?: string;
  OrderBy?: string;
  IsDescending?: boolean;
}

export interface PaginatedInsuranceResponse {
  insurances: Insurance[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}

