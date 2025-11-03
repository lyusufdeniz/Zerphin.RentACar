import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';

// Track shown errors to prevent duplicates
const errorCache = new Map<string, number>();
const ERROR_CACHE_DURATION = 3000; // 3 seconds
let lastCacheCleanup = 0;
const CACHE_CLEANUP_INTERVAL = 10000; // Clean up every 10 seconds

/**
 * Get error key for caching
 */
function getErrorKey(status: number, message: string, url: string): string {
  // For server errors (500+), group by status only to catch multiple requests
  if (status >= 500) {
    return `server-error-${status}`;
  }
  // For other errors, use status + message + url
  return `${status}-${message}-${url}`;
}

/**
 * Clean old entries from cache
 */
function cleanCache() {
  const now = Date.now();
  const cutoff = now - ERROR_CACHE_DURATION * 2;
  for (const [k, v] of errorCache.entries()) {
    if (v < cutoff) {
      errorCache.delete(k);
    }
  }
}

/**
 * Check if error should be shown (not in cache or cache expired)
 */
function shouldShowError(key: string): boolean {
  const cachedTime = errorCache.get(key);
  const now = Date.now();
  
  // Periodically clean cache
  if (now - lastCacheCleanup > CACHE_CLEANUP_INTERVAL) {
    cleanCache();
    lastCacheCleanup = now;
  }
  
  if (!cachedTime || (now - cachedTime) > ERROR_CACHE_DURATION) {
    errorCache.set(key, now);
    return true;
  }
  return false;
}

/**
 * Get error message from error response
 */
function getErrorMessage(errorResponse: any, defaultMessage: string): string {
  if (errorResponse?.errorMessage && Array.isArray(errorResponse.errorMessage)) {
    return errorResponse.errorMessage.join(', ');
  }
  if (errorResponse?.errorMessage && typeof errorResponse.errorMessage === 'string') {
    return errorResponse.errorMessage;
  }
  if (errorResponse?.message) {
    return errorResponse.message;
  }
  return defaultMessage;
}

export const exceptionInterceptor: HttpInterceptorFn = (req, next) => {
  const toastService = inject(ToastService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Handle different error scenarios
      if (error.error instanceof ErrorEvent) {
        // Client-side error (network, CORS, etc.)
        const errorKey = getErrorKey(0, 'Bağlantı hatası oluştu', req.url);
        if (shouldShowError(errorKey)) {
          toastService.error('Bağlantı hatası oluştu');
        }
        console.error('Client-side error:', error.error);
      } else {
        // Server-side error
        const status = error.status;
        const errorResponse = error.error;
        let errorMessage = '';
        let shouldShow = true;

        switch (status) {
          case 0:
            // Network error, connection failed
            errorMessage = 'Sunucuya bağlanılamadı. Lütfen internet bağlantınızı kontrol edin.';
            break;

          case 400:
            // Bad Request - Show validation errors
            errorMessage = getErrorMessage(errorResponse, 'Geçersiz istek');
            if (errorResponse?.errorMessage && Array.isArray(errorResponse.errorMessage)) {
              const errorKey = getErrorKey(status, errorMessage, req.url);
              if (shouldShowError(errorKey)) {
                toastService.showErrorMessages(errorResponse.errorMessage);
              }
              shouldShow = false; // Already shown via showErrorMessages
            }
            break;

          case 401:
            // Unauthorized - Already handled by auth interceptor, but we can show a message
            // Don't show toast here as auth interceptor handles redirect
            shouldShow = false;
            break;

          case 403:
            // Forbidden
            errorMessage = getErrorMessage(errorResponse, 'Bu işlem için yetkiniz bulunmamaktadır');
            if (errorResponse?.errorMessage && Array.isArray(errorResponse.errorMessage)) {
              const errorKey = getErrorKey(status, errorMessage, req.url);
              if (shouldShowError(errorKey)) {
                toastService.showErrorMessages(errorResponse.errorMessage);
              }
              shouldShow = false; // Already shown via showErrorMessages
            }
            break;

          case 404:
            // Not Found
            errorMessage = getErrorMessage(errorResponse, 'Kayıt bulunamadı');
            if (errorResponse?.errorMessage && Array.isArray(errorResponse.errorMessage)) {
              const errorKey = getErrorKey(status, errorMessage, req.url);
              if (shouldShowError(errorKey)) {
                toastService.showErrorMessages(errorResponse.errorMessage);
              }
              shouldShow = false; // Already shown via showErrorMessages
            }
            break;

          case 500:
          case 502:
          case 503:
          case 504:
            // Server errors - Show connection error message (grouped by status)
            errorMessage = 'Sunucuya bağlanılamadı. Lütfen daha sonra tekrar deneyin.';
            break;

          default:
            // Other errors
            errorMessage = getErrorMessage(errorResponse, `Bir hata oluştu (${status})`);
            if (errorResponse?.errorMessage && Array.isArray(errorResponse.errorMessage)) {
              const errorKey = getErrorKey(status, errorMessage, req.url);
              if (shouldShowError(errorKey)) {
                toastService.showErrorMessages(errorResponse.errorMessage);
              }
              shouldShow = false; // Already shown via showErrorMessages
            }
            break;
        }

        // Show error message if not already shown via showErrorMessages
        if (shouldShow && errorMessage) {
          const errorKey = getErrorKey(status, errorMessage, req.url);
          if (shouldShowError(errorKey)) {
            toastService.error(errorMessage);
          }
        }

        console.error('HTTP Error:', {
          status,
          url: req.url,
          error: errorResponse,
        });
      }

      // Re-throw the error so components can handle it if needed
      return throwError(() => error);
    })
  );
};

