/**
 * Invoice Statistics Response Models
 */
export interface InvoiceStatisticsResponse {
  totalInvoices: number;
  paidInvoices: number;
  unpaidInvoices: number;
  totalAmount: number;
  paidAmount: number;
  unpaidAmount: number;
  overdueInvoices: number;
  overdueAmount: number;
}


