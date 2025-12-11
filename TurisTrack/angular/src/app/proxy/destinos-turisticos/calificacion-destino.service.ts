import type { CalificacionDestinoDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CalificacionDestinoService {
  apiName = 'Default';
  

  crearCalificacion = (destinoId: string, puntuacion: number, comentario?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: `/api/app/calificacion-destino/crear-calificacion/${destinoId}`,
      params: { puntuacion, comentario },
    },
    { apiName: this.apiName,...config });
  

  obtenerMisCalificaciones = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificacionDestinoDto[]>({
      method: 'POST',
      url: '/api/app/calificacion-destino/obtener-mis-calificaciones',
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
