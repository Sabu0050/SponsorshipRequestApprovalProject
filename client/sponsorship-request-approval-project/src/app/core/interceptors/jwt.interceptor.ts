import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const jwtInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const token = authService.getToken();
  const isApiRequest = isApiUrl(request.url);
  const isLoginRequest = request.url.includes('/auth/login');

  const requestWithToken = token && isApiRequest
    ? request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    })
    : request;

  return next(requestWithToken).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse
          && (error.status === 401 || error.status === 403)
          && !isLoginRequest) {
        authService.logout(false);
        router.navigate(['/auth/login'], {
          queryParams: {
            returnUrl: router.url
          }
        });
      }

      return throwError(() => error);
    })
  );
};

function isApiUrl(url: string): boolean {
  try {
    const parsed = new URL(url, window.location.origin);
    return parsed.pathname.startsWith('/api/');
  } catch {
    return url.startsWith('/api/');
  }
}
