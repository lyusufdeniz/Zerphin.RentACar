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

  private getHeaders(): HttpHeaders {
    let headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    try {

      const rememberMe = localStorage.getItem('rememberMe') === 'true' || 
                         sessionStorage.getItem('rememberMe') === 'false';

      const token = rememberMe
        ? localStorage.getItem('authToken')
        : sessionStorage.getItem('authToken');

      const tokenValue = token || 
                        localStorage.getItem('authToken') || 
                        sessionStorage.getItem('authToken');

      if (tokenValue) {
        headers = headers.set('Authorization', `Bearer ${tokenValue}`);
      }
    } catch (error) {
      console.error('Error getting auth token from storage:', error);
    }

    return headers;
  }

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

  private extractServiceResult<T>(response: ServiceResult<T>): T {

    if (response && typeof response === 'object' && 'errorMessage' in response) {
      const serviceResult = response as ServiceResult<T>;

      if (serviceResult.errorMessage && serviceResult.errorMessage.length > 0) {
        const error = new Error(serviceResult.errorMessage.join(', '));
        (error as any).errorMessage = serviceResult.errorMessage;
        throw error;
      }

      if (serviceResult.data !== null && serviceResult.data !== undefined) {
        return serviceResult.data;
      }

      throw new Error('İşlem başarısız oldu');
    }

    return response as T;
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'Bir hata oluştu';
    let errorMessages: string[] | null = null;

    if (error.error instanceof ErrorEvent) {

      errorMessage = `Hata: ${error.error.message}`;
      errorMessages = [errorMessage];
    } else {

      if (error.error?.errorMessage && Array.isArray(error.error.errorMessage)) {
        errorMessages = error.error.errorMessage;
        errorMessage = errorMessages ? errorMessages.join(', ') : 'Bir hata oluştu';
      } else {

        switch (error.status) {
          case 400:
            errorMessage = error.error?.message || 'Geçersiz istek';
            break;
          case 401:
            errorMessage = error.error?.message || 'Yetkisiz erişim';

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

  setBaseUrl(url: string): void {
    this.baseUrl = url;
  }

  getBaseUrl(): string {
    return this.baseUrl;
  }
}
