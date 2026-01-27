import { Environment } from '@abp/ng.core';

const baseUrl = 'https://app-zeta-snowy-53.vercel.app/';

const oAuthConfig = {
  issuer: 'https://turistrack.onrender.com',
  redirectUri: baseUrl,
  clientId: 'TurisTrack_Swagger',
  responseType: 'code',
  scope: 'TurisTrack',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'TurisTrack',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://turistrack.onrender.com',
      rootNamespace: 'TurisTrack',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge',
  },
} as Environment;
