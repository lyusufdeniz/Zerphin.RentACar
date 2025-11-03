/**
 * Customer Statistics Response Models
 */
export interface CustomerStatisticsResponse {
  totalCustomers: number;
  verifiedCustomers: number;
  unverifiedCustomers: number;
  customersWithInsurance: number;
  newCustomersLast30Days: number;
  averageCreditScore: number;
}


