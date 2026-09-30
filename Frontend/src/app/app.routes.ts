import { Routes } from '@angular/router';

import { Login } from './auth/login/login';
import { Register } from './auth/register/register';

import { Dashboard as CustomerDashboard }
  from './customer/dashboard/dashboard';

import { Dashboard as WasherDashboard }
  from './washer/dashboard/dashboard';

import { Dashboard as AdminDashboard }
  from './admin/dashboard/dashboard';

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
    component: CustomerDashboard
  },

  {
    path: 'washer',
    component: WasherDashboard
  },

  {
    path: 'admin',
    component: AdminDashboard
  }

];