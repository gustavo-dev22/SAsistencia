import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideZonelessChangeDetection(),
    provideHttpClient(withInterceptors([jwtInterceptor])),
    providePrimeNG({
      ripple: true,
      license: 'eyJpZCI6ImRiMzg1OGNkLWJmYzAtNDMxNS1iM2U2LWY2ZjYxM2Q5MjNjNSIsInByb2R1Y3QiOiJwcmltZXVpIiwidGllciI6ImNvbW11bml0eSIsInR5cGUiOiJkZXYiLCJpYXQiOjE3ODg2NzQxMjMsImV4cCI6MTgyMDIxMDEyM30.Hd0WO1dibMRjMCf-WtqrLDwa0npyQZpQGng4OCywKg_MvZo5ajMQxs7oFQt3bXIrBnYr83Kosqk5s9kq1WfuAQ',
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: '.p-dark'
        }
      }
    })
  ],
};
