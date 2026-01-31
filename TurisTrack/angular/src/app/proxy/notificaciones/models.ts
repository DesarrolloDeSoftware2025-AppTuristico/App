import type { TipoNotificacion } from './tipo-notificacion.enum';
import type { EntityDto } from '@abp/ng.core';

export interface CrearEventoDestinoDto {
  destinoId?: string;
  titulo?: string;
  mensaje?: string;
  tipo?: TipoNotificacion;
}

export interface NotificacionDto extends EntityDto<string> {
  destinoTuristicoId?: string;
  nombreDestino?: string;
  titulo?: string;
  mensaje?: string;
  tipo?: TipoNotificacion;
  leido: boolean;
  creationTime?: string;
}
