import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import {
  Invoice,
  CreateInvoiceCommand,
  UpdateInvoiceCommand,
  InvoiceSearchParams,
  PaginatedInvoiceResponse,
  DeleteInvoiceCommand,
} from '../models/invoice';
@Injectable({
  providedIn: 'root',
})
export class InvoiceService {
  private httpService = inject(HttpService);
  private baseUrl = '/Invoices';
  searchInvoices(
    params?: InvoiceSearchParams
  ): Observable<PaginatedInvoiceResponse> {
    return this.httpService.get<PaginatedInvoiceResponse>(
      `${this.baseUrl}/search`,
      {
        params: params as any,
      }
    );
  }
  getInvoiceById(id: string): Observable<Invoice> {
    return this.httpService.get<Invoice>(`${this.baseUrl}/${id}`);
  }
  createInvoice(command: CreateInvoiceCommand): Observable<Invoice> {
    return this.httpService.post<Invoice>(this.baseUrl, command);
  }
  updateInvoice(command: UpdateInvoiceCommand): Observable<Invoice> {
    return this.httpService.put<Invoice>(this.baseUrl, command);
  }
  deleteInvoice(id: string): Observable<void> {
    const command: DeleteInvoiceCommand = { id };
    return this.httpService.delete<void>(this.baseUrl, {
      params: command as any,
    });
  }
}