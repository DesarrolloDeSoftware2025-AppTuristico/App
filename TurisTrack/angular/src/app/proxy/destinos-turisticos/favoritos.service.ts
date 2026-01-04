import type { DestinoFavoritoDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class FavoritosService {
  apiName = 'Default';
  

  agregarFavorito = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: `/api/app/favoritos/agregar-favorito/${destinoId}`,
    },
    { apiName: this.apiName,...config });
  

  eliminarFavorito = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: `/api/app/favoritos/eliminar-favorito/${destinoId}`,
    },
    { apiName: this.apiName,...config });
  

  getListaFavoritos = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoFavoritoDto[]>({
      method: 'GET',
      url: '/api/app/favoritos/a-favoritos',
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
