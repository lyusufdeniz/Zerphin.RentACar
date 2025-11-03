import { Injectable, inject } from '@angular/core';
import {
  HttpClient,
  HttpParams,
  HttpHeaders,
  HttpErrorResponse,
  HttpContext,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../environments/environment';
import { ServiceResult } from '../models/api/api-response.model';

export interface HttpOptions {
  headers?: HttpHeaders | { [header: string]: string | string[] };
  params?: HttpParams | { [param: string]: any };
  context?: HttpContext;
  observe?: 'body';
  reportProgress?: boolean;
  responseType?: 'json';
}

export interface ApiResponse<T> {
  data?: T;
  message?: string;
  success?: boolean;
  error?: string;
}

@Injectable({
  providedIn: 'root',
})
export class HttpService {
  private http = inject(HttpClient);
  private baseUrl = environment?.apiUrl || '';

  /**
   * Get default headers with authorization token
   * Note: Token is now handled by authInterceptor, but we keep this for backward compatibility
   */
  private getHeaders(): HttpHeaders {
    let headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    // Try to get token from localStorage
    // Interceptor will handle token refresh if needed
    try {
      const token = localStorage.getItem('authToken');
      if (token) {
        headers = headers.set('Authorization', `Bearer ${token}`);
      }
    } catch (error) {
      console.error('Error getting auth token from localStorage:', error);
    }

    return headers;
  }

  /**
   * GET request
   */
  get<T>(url: string, options?: HttpOptions): Observable<T> {
    const headers = options?.headers || this.getHeaders();
    return this.http
      .get<ServiceResult<T>>(`${this.baseUrl}${url}`, {
        ...options,
        headers,
        observe: 'body',
        responseType: 'json',
      })
      .pipe(
        map((response) => this.extractServiceResult<T>(response)),
        catchError((error) => this.handleError(error))
      );
  }

  /**
   * POST request
   */
  post<T>(url: string, body?: any, options?: HttpOptions): Observable<T> {
    const headers = options?.headers || this.getHeaders();
    return this.http
      .post<ServiceResult<T>>(`${this.baseUrl}${url}`, body, {
        ...options,
        headers,
        observe: 'body',
        responseType: 'json',
      })
      .pipe(
        map((response) => this.extractServiceResult<T>(response)),
        catchError((error) => this.handleError(error))
      );
  }

  /**
   * PUT request
   */
  put<T>(url: string, body?: any, options?: HttpOptions): Observable<T> {
    const headers = options?.headers || this.getHeaders();
    return this.http
      .put<ServiceResult<T>>(`${this.baseUrl}${url}`, body, {
        ...options,
        headers,
        observe: 'body',
        responseType: 'json',
      })
      .pipe(
        map((response) => this.extractServiceResult<T>(response)),
        catchError((error) => this.handleError(error))
      );
  }

  /**
   * PATCH request
   */
  patch<T>(url: string, body?: any, options?: HttpOptions): Observable<T> {
    const headers = options?.headers || this.getHeaders();
    return this.http
      .patch<ServiceResult<T>>(`${this.baseUrl}${url}`, body, {
        ...options,
        headers,
        observe: 'body',
        responseType: 'json',
      })
      .pipe(
        map((response) => this.extractServiceResult<T>(response)),
        catchError((error) => this.handleError(error))
      );
  }

  /**
   * DELETE request
   */
  delete<T>(url: string, options?: HttpOptions): Observable<T> {
    const headers = options?.headers || this.getHeaders();
    return this.http
      .delete<ServiceResult<T>>(`${this.baseUrl}${url}`, {
        ...options,
        headers,
        observe: 'body',
        responseType: 'json',
      })
      .pipe(
        map((response) => this.extractServiceResult<T>(response)),
        catchError((error) => this.handleError(error))
      );
  }

  /**
   * Extract data from ServiceResult response
   */
  private extractServiceResult<T>(response: ServiceResult<T>): T {
    // Check if response is ServiceResult format
    if (response && typeof response === 'object' && 'errorMessage' in response) {
      const serviceResult = response as ServiceResult<T>;
      
      // If there are error messages, throw error
      if (serviceResult.errorMessage && serviceResult.errorMessage.length > 0) {
        const error = new Error(serviceResult.errorMessage.join(', '));
        (error as any).errorMessage = serviceResult.errorMessage;
        throw error;
      }
      
      // Return data if available
      if (serviceResult.data !== null && serviceResult.data !== undefined) {
        return serviceResult.data;
      }
      
      // If data is null but no error message, throw generic error
      throw new Error('İşlem başarısız oldu');
    }
    
    // If response is not ServiceResult format, return as is
    return response as T;
  }

  /**
   * Handle HTTP errors
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'Bir hata oluştu';
    let errorMessages: string[] | null = null;

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Hata: ${error.error.message}`;
      errorMessages = [errorMessage];
    } else {
      // Server-side error - Check if ServiceResult format
      if (error.error?.errorMessage && Array.isArray(error.error.errorMessage)) {
        errorMessages = error.error.errorMessage;
        errorMessage = errorMessages ? errorMessages.join(', ') : 'Bir hata oluştu';
      } else {
        // Server-side error - Standard format
        switch (error.status) {
          case 400:
            errorMessage = error.error?.message || 'Geçersiz istek';
            break;
          case 401:
            errorMessage = error.error?.message || 'Yetkisiz erişim';
            // Clear auth and redirect to login
            localStorage.removeItem('authToken');
            localStorage.removeItem('refreshToken');
            localStorage.removeItem('user');
            break;
          case 403:
            errorMessage = error.error?.message || 'Erişim reddedildi';
            break;
          case 404:
            errorMessage = error.error?.message || 'Kayıt bulunamadı';
            break;
          case 500:
            errorMessage = error.error?.message || 'Sunucu hatası';
            break;
          default:
            errorMessage = error.error?.message || `Hata: ${error.message}`;
        }
        errorMessages = [errorMessage];
      }
    }

    console.error('HTTP Error:', error);
    const errorObj = new Error(errorMessage);
    (errorObj as any).errorMessage = errorMessages;
    (errorObj as any).status = error.status;
    return throwError(() => errorObj);
  }

  /**
   * Set base URL
   */
  setBaseUrl(url: string): void {
    this.baseUrl = url;
  }

  /**
   * Get base URL
   */
  getBaseUrl(): string {
    return this.baseUrl;
  }
}

