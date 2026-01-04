import type { CalificacionDestinoDto, ResumenCalificacionDto } from './models';
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
  

  editarCalificacion = (calificacionId: string, nuevaPuntuacion?: number, nuevoComentario?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: `/api/app/calificacion-destino/editar-calificacion/${calificacionId}`,
      params: { nuevaPuntuacion, nuevoComentario },
    },
    { apiName: this.apiName,...config });
  

  eliminarCalificacion = (calificacionId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: `/api/app/calificacion-destino/eliminar-calificacion/${calificacionId}`,
    },
    { apiName: this.apiName,...config });
  

  obtenerComentariosPorDestino = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificacionDestinoDto[]>({
      method: 'POST',
      url: `/api/app/calificacion-destino/obtener-comentarios-por-destino/${destinoId}`,
    },
    { apiName: this.apiName,...config });
  

  obtenerMisCalificaciones = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificacionDestinoDto[]>({
      method: 'POST',
      url: '/api/app/calificacion-destino/obtener-mis-calificaciones',
    },
    { apiName: this.apiName,...config });
  

  obtenerPromedioDestino = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ResumenCalificacionDto>({
      method: 'POST',
      url: `/api/app/calificacion-destino/obtener-promedio-destino/${destinoId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
