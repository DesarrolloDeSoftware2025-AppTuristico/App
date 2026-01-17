import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
      {
        path: '/',
        name: '::Menu:Home',
        iconClass: 'fas fa-home',
        order: 1,
        layout: eLayoutType.application,
      },
      {
        path: '/destinos',
        name: '::Menu:Destinos',
        iconClass: 'fas fa-map-marked-alt',
        order: 2,
        layout: eLayoutType.application,
      },
      {
        path: '/metricas',
        name: '::Menu:Metricas',
        iconClass: 'fas fa-chart-line',
        order: 3,
        layout: eLayoutType.application,
        requiredPolicy: 'AbpIdentity.Users'   // Para que lo vea solo el Admin
      },
      {
        path: '/buscar-usuarios',
        name: '::Menu:Buscar Usuarios',
        iconClass: 'fas fa-users',
        order: 4,
        layout: eLayoutType.application,
      },
  ]);
}
