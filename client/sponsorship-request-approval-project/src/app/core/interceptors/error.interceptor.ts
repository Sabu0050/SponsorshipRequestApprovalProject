import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const isLoginRequest = request.url.includes('/auth/login');

  return next(request).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse && !isLoginRequest && (error.status === 401 || error.status === 403)) {
        authService.logout();
        router.navigate(['/login'], {
          queryParams: {
            returnUrl: router.url
          }
        });
      }

      return throwError(() => error);
    })
  );
};
