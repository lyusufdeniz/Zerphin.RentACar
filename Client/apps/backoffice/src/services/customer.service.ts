import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import {
  Customer,
  CreateCustomerCommand,
  UpdateCustomerCommand,
  CustomerSearchParams,
  PaginatedCustomerResponse,
} from '../models/customer';

@Injectable({
  providedIn: 'root',
})
export class CustomerService {
  private httpService = inject(HttpService);
  private baseUrl = '/Customers';

  /**
   * Search customers with filters
   */
  searchCustomers(
    params?: CustomerSearchParams
  ): Observable<PaginatedCustomerResponse> {
    return this.httpService.get<PaginatedCustomerResponse>(
      `${this.baseUrl}/search`,
      {
        params: params as any,
      }
    );
  }

  /**
   * Get customer by ID
   */
  getCustomerById(id: string): Observable<Customer> {
    return this.httpService.get<Customer>(`${this.baseUrl}/id`, {
      params: { id },
    });
  }

  /**
   * Create new customer
   */
  createCustomer(command: CreateCustomerCommand): Observable<Customer> {
    return this.httpService.post<Customer>(this.baseUrl, command);
  }

  /**
   * Update customer
   */
  updateCustomer(command: UpdateCustomerCommand): Observable<Customer> {
    return this.httpService.put<Customer>(this.baseUrl, command);
  }

  /**
   * Delete customer
   */
  deleteCustomer(id: string): Observable<void> {
    return this.httpService.delete<void>(this.baseUrl, { params: { id } });
  }
}

