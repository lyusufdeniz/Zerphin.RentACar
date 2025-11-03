import { Customer } from './customer.model';

export interface CreateCustomerCommand {
  userId: string;
  licenseNumber?: string;
  licenseExpiryDate: string;
  licenseClass?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  specialNotes?: string;
  isVerified: boolean;
  verificationDate?: string;
  verificationDocument?: string;
  creditScore: number;
  hasInsurance: boolean;
  insuranceCompany?: string;
  insurancePolicyNumber?: string;
}

export interface UpdateCustomerCommand {
  id: string;
  licenseNumber?: string;
  licenseExpiryDate: string;
  licenseClass?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  specialNotes?: string;
  isVerified: boolean;
  verificationDate?: string;
  verificationDocument?: string;
  creditScore: number;
  hasInsurance: boolean;
  insuranceCompany?: string;
  insurancePolicyNumber?: string;
}

export interface CustomerSearchParams {
  PageNumber?: number;
  PageSize?: number;
  UserId?: string;
  LicenseNumber?: string;
  IsVerified?: boolean;
  MinCreditScore?: number;
  MaxCreditScore?: number;
  HasInsurance?: boolean;
  OrderBy?: string;
  IsDescending?: boolean;
}

export interface PaginatedCustomerResponse {
  customers: Customer[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}

