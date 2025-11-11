import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';
import { LanguageService } from '../services/language.service';

const errorCache = new Map<string, number>();
const ERROR_CACHE_DURATION = 3000;
let lastCacheCleanup = 0;
const CACHE_CLEANUP_INTERVAL = 10000;

function getErrorKey(status: number, message: string, url: string): string {
  if (status >= 500) {
    return `server-error-${status}`;
  }
  return `${status}-${message}-${url}`;
}

function cleanCache() {
  const now = Date.now();
  const cutoff = now - ERROR_CACHE_DURATION * 2;
  for (const [k, v] of errorCache.entries()) {
    if (v < cutoff) {
      errorCache.delete(k);
    }
  }
}

function shouldShowError(key: string): boolean {
  const cachedTime = errorCache.get(key);
  const now = Date.now();

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
  const languageService = inject(LanguageService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.error instanceof ErrorEvent) {
        const errorKey = getErrorKey(0, languageService.translate('messages.errors.http.connectionError'), req.url);
        if (shouldShowError(errorKey)) {
          toastService.error(languageService.translate('messages.errors.http.connectionError'));
        }
        console.error('Client-side error:', error.error);
      } else {
        const status = error.status;
        const errorResponse = error.error;
        let errorMessage = '';
        let shouldShow = true;

        switch (status) {
          case 0:
            errorMessage = languageService.translate('messages.errors.http.serverUnreachable');
            break;

          case 400:
            errorMessage = getErrorMessage(errorResponse, languageService.translate('messages.errors.http.badRequest'));
            if (errorResponse?.errorMessage && Array.isArray(errorResponse.errorMessage)) {
              const errorKey = getErrorKey(status, errorMessage, req.url);
              if (shouldShowError(errorKey)) {
                toastService.showErrorMessages(errorResponse.errorMessage);
              }
              shouldShow = false;
            }
            break;

          case 401:
            shouldShow = false;
            break;

          case 403:
            errorMessage = getErrorMessage(errorResponse, languageService.translate('messages.errors.http.forbidden'));
            if (errorResponse?.errorMessage && Array.isArray(errorResponse.errorMessage)) {
              const errorKey = getErrorKey(status, errorMessage, req.url);
              if (shouldShowError(errorKey)) {
                toastService.showErrorMessages(errorResponse.errorMessage);
              }
              shouldShow = false;
            }
            break;

          case 404:
            errorMessage = getErrorMessage(errorResponse, languageService.translate('messages.errors.http.notFound'));
            if (errorResponse?.errorMessage && Array.isArray(errorResponse.errorMessage)) {
              const errorKey = getErrorKey(status, errorMessage, req.url);
              if (shouldShowError(errorKey)) {
                toastService.showErrorMessages(errorResponse.errorMessage);
              }
              shouldShow = false;
            }
            break;

          case 500:
          case 502:
          case 503:
          case 504:
            errorMessage = languageService.translate('messages.errors.http.serverError');
            break;

          default:
            errorMessage = getErrorMessage(errorResponse, languageService.translateWithParams('messages.errors.http.unknownError', { status: status.toString() }));
            if (errorResponse?.errorMessage && Array.isArray(errorResponse.errorMessage)) {
              const errorKey = getErrorKey(status, errorMessage, req.url);
              if (shouldShowError(errorKey)) {
                toastService.showErrorMessages(errorResponse.errorMessage);
              }
              shouldShow = false;
            }
            break;
        }

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

      return throwError(() => error);
    })
  );
};
