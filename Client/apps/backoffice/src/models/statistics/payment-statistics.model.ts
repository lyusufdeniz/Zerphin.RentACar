
export interface PaymentStatisticsResponse {
  totalPayments: number;
  totalAmount: number;
  completedAmount: number;
  pendingAmount: number;
  failedAmount: number;
  statusStatistics: PaymentStatusStatistics[];
  methodStatistics: PaymentMethodStatistics[];
}
export interface PaymentStatusStatistics {
  status: number; 
  statusName: string;
  count: number;
  totalAmount: number;
}
export interface PaymentMethodStatistics {
  method: number; 
  methodName: string;
  count: number;
  totalAmount: number;
}