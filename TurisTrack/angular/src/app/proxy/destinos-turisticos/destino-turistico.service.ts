import type { DestinoTuristicoDto, DestinoTuristicoDtoPersistido, SaveResultDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DestinoTuristicoService {
  apiName = 'Default';
  

  buscarDestinos = (nombre: string, pais?: string, region?: string, poblacionMinima?: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoTuristicoDto[]>({
      method: 'POST',
      url: '/api/app/destino-turistico/buscar-destinos',
      params: { nombre, pais, region, poblacionMinima },
    },
    { apiName: this.apiName,...config });
  

  guardarDestino = (destinoExterno: DestinoTuristicoDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SaveResultDto>({
      method: 'POST',
      url: '/api/app/destino-turistico/guardar-destino',
      body: destinoExterno,
    },
    { apiName: this.apiName,...config });
  

  listarDestinosGuardados = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoTuristicoDtoPersistido[]>({
      method: 'POST',
      url: '/api/app/destino-turistico/listar-destinos-guardados',
    },
    { apiName: this.apiName,...config });
  

  listarDestinosPopulares = (limit: number = 10, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoTuristicoDto[]>({
      method: 'POST',
      url: '/api/app/destino-turistico/listar-destinos-populares',
      params: { limit },
    },
    { apiName: this.apiName,...config });
  

  obtenerDestinoPorId = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoTuristicoDto>({
      method: 'POST',
      url: `/api/app/destino-turistico/${id}/obtener-destino-por-id`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
