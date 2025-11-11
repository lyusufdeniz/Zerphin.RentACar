import { Invoice, InvoiceItem } from './invoice.model';
import { InvoiceStatus } from './invoice-status.model';

export interface CreateInvoiceCommand {
  rentalId?: string;
  customerId: string;
  vehicleId?: string;
  issueDate: string;
  dueDate: string;
  subtotal: number;
  taxAmount?: number;
  discountAmount?: number;
  totalAmount: number;
  status: InvoiceStatus;
  notes?: string;
  items?: InvoiceItem[];
}

export interface UpdateInvoiceCommand {
  id: string;
  issueDate?: string;
  dueDate?: string;
  subtotal?: number;
  taxAmount?: number;
  discountAmount?: number;
  totalAmount?: number;
  status?: InvoiceStatus;
  paidDate?: string;
  paidAmount?: number;
  notes?: string;
  items?: InvoiceItem[];
}

export interface InvoiceSearchParams {
  PageNumber?: number;
  PageSize?: number;
  InvoiceNumber?: string;
  CustomerId?: string;
  RentalId?: string;
  VehicleId?: string;
  Status?: InvoiceStatus;
  IssueDateFrom?: string;
  IssueDateTo?: string;
  DueDateFrom?: string;
  DueDateTo?: string;
  OrderBy?: string;
  IsDescending?: boolean;
}

export interface PaginatedInvoiceResponse {
  invoices: Invoice[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}

export interface DeleteInvoiceCommand {
  id: string;
}
