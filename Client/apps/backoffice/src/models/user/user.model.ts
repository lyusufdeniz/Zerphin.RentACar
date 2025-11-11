export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  identityNumber?: string;
  birthDate?: string;
  roleName: string;
  role: number;
  isActive?: boolean;
  createdAt?: string;
  updatedAt?: string;

  licenseNumber?: string;
  licenseExpiryDate?: string;
  licenseClass?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  specialNotes?: string;
  isVerified?: boolean;
  verificationDate?: string;
  verificationDocument?: string;
  creditScore?: number;
  hasInsurance?: boolean;
  insuranceCompany?: string;
  insurancePolicyNumber?: string;
}
