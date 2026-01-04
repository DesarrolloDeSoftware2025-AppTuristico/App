import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
  },
  {
    path: 'destinos',
    canActivate: [authGuard],
    loadComponent: () => import('./destinos/lista-destinos/lista-destinos').then(c => c.ListaDestinos),
  },

  {
    path: 'metricas',
    canActivate: [authGuard, permissionGuard],
    loadComponent: () => import('./admin/dashboard-metricas/dashboard-metricas').then(c => c.DashboardMetricas),
  },
];
