import { mapEnumToOptions } from '@abp/ng.core';

export enum SentimientoExperiencia {
  Neutral = 0,
  Positiva = 1,
  Negativa = 2,
}

export const sentimientoExperienciaOptions = mapEnumToOptions(SentimientoExperiencia);
