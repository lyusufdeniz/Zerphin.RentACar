
export interface InsuranceStatisticsResponse {
  totalInsurances: number;
  activeInsurances: number;
  expiredInsurances: number;
  expiringSoonInsurances: number;
  totalPremiumAmount: number;
}