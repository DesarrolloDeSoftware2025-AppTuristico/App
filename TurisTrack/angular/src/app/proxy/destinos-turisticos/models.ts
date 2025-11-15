import type { EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CalificacionDestinoDto extends EntityDto<string> {
  userId?: string;
  destinoTuristicoId?: string;
  puntuacion: number;
  comentario?: string;
}

export interface DestinoTuristicoDto extends EntityDto<string> {
  idAPI: number;
  tipo?: string;
  nombre?: string;
  pais?: string;
  codigoPais?: string;
  region?: string;
  codigoRegion?: string;
  metrosDeElevacion: number;
  latitud: number;
  longitud: number;
  poblacion: number;
  zonaHoraria?: string;
  foto?: string;
  eliminado: boolean;
}

export interface DestinoTuristicoDtoPersistido extends EntityDto<string> {
  id?: string;
  idAPI: number;
  tipo?: string;
  nombre?: string;
  pais?: string;
  codigoPais?: string;
  region?: string;
  codigoRegion?: string;
  metrosDeElevacion: number;
  latitud: number;
  longitud: number;
  poblacion: number;
  zonaHoraria?: string;
  foto?: string;
  eliminado: boolean;
}

export interface SaveResultDto {
  success: boolean;
  message?: string;
  idInterno?: string;
  idApi: number;
}


