import { Routes } from '@angular/router';

import { Login } from './auth/login/login';
import { Register } from './auth/register/register';

import { Dashboard as CustomerDashboard }
  from './customer/dashboard/dashboard';

import { Dashboard as WasherDashboard }
  from './washer/dashboard/dashboard';

import { Dashboard as AdminDashboard }
  from './admin/dashboard/dashboard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [

  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  {
    path: 'login',
    component: Login
  },

  {
    path: 'register',
    component: Register
  },

  {
    path: 'customer',
    component: CustomerDashboard,
    canActivate: [roleGuard],
    data: { role: 'Customer' }
  },

  {
    path: 'washer',
    component: WasherDashboard,
    canActivate: [roleGuard],
    data: { role: 'Washer' }
  },

  {
    path: 'admin',
    component: AdminDashboard,
    canActivate: [roleGuard],
    data: { role: 'Admin' }
  }

];
