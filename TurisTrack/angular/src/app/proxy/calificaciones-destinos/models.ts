import type { EntityDto } from '@abp/ng.core';

export interface CalificacionDestinoDto extends EntityDto<string> {
  userId?: string;
  userName?: string;
  destinoTuristicoId?: string;
  puntuacion: number;
  comentario?: string;
  creationTime?: string;
}

export interface ResumenCalificacionDto {
  promedio: number;
  totalCalificaciones: number;
}
