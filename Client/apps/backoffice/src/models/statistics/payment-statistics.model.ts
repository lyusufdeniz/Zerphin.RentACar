/**
 * Payment Statistics Response Models
 */
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
  status: number; // PaymentStatus enum (1-6)
  statusName: string;
  count: number;
  totalAmount: number;
}

export interface PaymentMethodStatistics {
  method: number; // PaymentMethod enum (1-5)
  methodName: string;
  count: number;
  totalAmount: number;
}


