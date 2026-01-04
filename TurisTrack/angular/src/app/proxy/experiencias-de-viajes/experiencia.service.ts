import type { SentimientoExperiencia } from './sentimiento-experiencia.enum';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { ExperienciaDeViajeDto } from '../experiencias/dtos/models';

@Injectable({
  providedIn: 'root',
})
export class ExperienciaService {
  apiName = 'Default';
  

  buscarPorPalabraClave = (palabraClave: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienciaDeViajeDto[]>({
      method: 'POST',
      url: '/api/app/experiencia/buscar-por-palabra-clave',
      params: { palabraClave },
    },
    { apiName: this.apiName,...config });
  

  crearExperiencia = (destinoId: string, comentario: string, fechaVisita: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: `/api/app/experiencia/crear-experiencia/${destinoId}`,
      params: { comentario, fechaVisita },
    },
    { apiName: this.apiName,...config });
  

  editarExperiencia = (experienciaId: string, comentario?: string, fechaVisita?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: `/api/app/experiencia/editar-experiencia/${experienciaId}`,
      params: { comentario, fechaVisita },
    },
    { apiName: this.apiName,...config });
  

  eliminarExperiencia = (experienciaId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: `/api/app/experiencia/eliminar-experiencia/${experienciaId}`,
    },
    { apiName: this.apiName,...config });
  

  filtrarPorSentimiento = (destinoId: string, sentimiento: SentimientoExperiencia, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienciaDeViajeDto[]>({
      method: 'POST',
      url: `/api/app/experiencia/filtrar-por-sentimiento/${destinoId}`,
      params: { sentimiento },
    },
    { apiName: this.apiName,...config });
  

  obtenerExperienciasDeOtros = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienciaDeViajeDto[]>({
      method: 'POST',
      url: `/api/app/experiencia/obtener-experiencias-de-otros/${destinoId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
