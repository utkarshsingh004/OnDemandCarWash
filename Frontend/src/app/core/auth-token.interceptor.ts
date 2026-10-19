import { HttpInterceptorFn } from '@angular/common/http';

export const authTokenInterceptor: HttpInterceptorFn = (request, next) => {
  const token = typeof localStorage === 'undefined'
    ? null
    : localStorage.getItem('token');

  if (!token || request.url.includes('/api/auth/')) {
    return next(request);
  }

  return next(request.clone({
    setHeaders: { Authorization: `Bearer ${token}` }
  }));
};
