import type { ApiMetricaResumenDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AdminMetricasService {
  apiName = 'Default';
  

  obtenerMetricasUso = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ApiMetricaResumenDto>({
      method: 'POST',
      url: '/api/app/admin-metricas/obtener-metricas-uso',
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
