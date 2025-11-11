export interface Customer {
  id: string;
  userId: string;
  userName?: string;
  userEmail?: string;
  userPhone?: string;
  userFirstName?: string;
  userLastName?: string;
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
  createdAt?: string;
  updatedAt?: string;
}