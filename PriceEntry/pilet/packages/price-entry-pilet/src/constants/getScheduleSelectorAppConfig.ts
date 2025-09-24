// istanbul ignore file
import { settings } from '@icis/app-shell-apis';
import { getIsAuthoring } from './isAuthoring';

type AppConfig = {
  appName: string;
  version: string;
  bffVersion: string;
  bypassVersionCheck: boolean;
  bffUrl: string;
  bffCallEnabled: boolean;
};

// This may have to change when we decide on the url we'll be using for authoring
/* istanbul ignore next */
const hostname = getIsAuthoring()
  ? window.location.hostname.replace('subscriber', 'authoring')
  : window.location.hostname.replace('authoring', 'subscriber');

const common = {
  appName: 'period-generator',
  bffVersion: 'v1',
  version: process.env.VERSION || 'dev',
  bypassVersionCheck: false,
  // turn this on to test mock bff!
  bffCallEnabled: false,
};

const dev = {
  ...common,
  bffUrl: 'https://localhost:8007/v1/graphql',
};

const nonDev = {
  ...common,
  bffUrl: `${window.location.protocol}//${hostname}/api/${common.appName}/${common.bffVersion}/graphql`,
};

/**
 * Gets the config for this app. There is one config for dev and another for all other environments that is ascertained based on context.
 * The current environment is requested from the app shell.
 */
const getScheduleSelectorAppConfig = (): AppConfig =>
  settings.environment === 'dev' ? dev : nonDev;

export default getScheduleSelectorAppConfig;
