import type { EntityDto } from '@abp/ng.core';

export interface DestinoFavoritoDto extends EntityDto<string> {
  nombre?: string;
  pais?: string;
  latitud?: string;
  longitud?: string;
}
