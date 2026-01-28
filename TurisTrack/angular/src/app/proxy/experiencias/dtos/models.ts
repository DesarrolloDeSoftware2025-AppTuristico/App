import type { EntityDto } from '@abp/ng.core';
import type { SentimientoExperiencia } from '../../experiencias-de-viajes/sentimiento-experiencia.enum';

export interface ExperienciaDeViajeDto extends EntityDto<string> {
  destinoId?: string;
  comentario?: string;
  fechaVisita?: string;
  sentimiento?: SentimientoExperiencia;
  creationTime?: string;
  creatorId?: string;
}
