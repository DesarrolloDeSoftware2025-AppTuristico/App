import { mapEnumToOptions } from '@abp/ng.core';

export enum TipoNotificacion {
  Evento = 1,
  Clima = 2,
  General = 3,
}

export const tipoNotificacionOptions = mapEnumToOptions(TipoNotificacion);
