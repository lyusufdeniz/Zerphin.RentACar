import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import {
  provideHttpClient,
  withInterceptorsFromDi,
  withInterceptors,
} from '@angular/common/http';
import { appRoutes } from './app.routes';
import { authInterceptor } from './interceptors/auth.interceptor';
import { exceptionInterceptor } from './interceptors/exception.interceptor';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(appRoutes),
    provideHttpClient(
      withInterceptorsFromDi(),
      withInterceptors([authInterceptor, exceptionInterceptor])
    ),
    provideCharts(withDefaultRegisterables()),
  ],
};
