import type { CrearEventoDestinoDto, NotificacionDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class NotificacionesService {
  apiName = 'Default';
  

  marcarComoLeida = (idNotificacion: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/notificaciones/marcar-como-leida',
      params: { idNotificacion },
    },
    { apiName: this.apiName,...config });
  

  marcarComoNoLeida = (idNotificacion: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/notificaciones/marcar-como-no-leida',
      params: { idNotificacion },
    },
    { apiName: this.apiName,...config });
  

  obtenerMisNotificaciones = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, NotificacionDto[]>({
      method: 'POST',
      url: '/api/app/notificaciones/obtener-mis-notificaciones',
    },
    { apiName: this.apiName,...config });
  

  reportarEventoEnDestino = (input: CrearEventoDestinoDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/notificaciones/reportar-evento-en-destino',
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
