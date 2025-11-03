import { UserRole } from './user-role.model';
import { User } from './user.model';

export interface CreateUserCommand {
  firstName?: string;
  lastName?: string;
  email?: string;
  phoneNumber?: string;
  password?: string;
  role: UserRole;
  address?: string;
  identityNumber?: string;
  birthDate?: string;
  licenseNumber?: string;
  licenseExpiryDate?: string;
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

export interface UpdateUserCommand {
  id: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  phoneNumber?: string;
  role: UserRole;
  address?: string;
  identityNumber?: string;
  birthDate?: string;
  licenseNumber?: string;
  licenseExpiryDate?: string;
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

export interface ChangePasswordCommand {
  userId: string;
  currentPassword?: string;
  newPassword?: string;
  confirmPassword?: string;
}

export interface DeleteUserCommand {
  id: string;
}

export interface UserSearchParams {
  PageNumber?: number;
  PageSize?: number;
  Email?: string;
  FirstName?: string;
  LastName?: string;
  PhoneNumber?: string;
  Role?: UserRole;
  IsActive?: boolean;
  IdentityNumber?: string;
  LicenseNumber?: string;
  IsVerified?: boolean;
  OrderBy?: string;
  IsDescending?: boolean;
}

export interface PaginatedUserResponse {
  users: User[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}


