import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const roleGuard: CanActivateFn = (route) => {
  const router = inject(Router);
  const token = localStorage.getItem('token');
  const role = localStorage.getItem('role');
  const requiredRole = route.data['role'];

  if (token && role === requiredRole) {
    return true;
  }

  return router.parseUrl('/login');
};