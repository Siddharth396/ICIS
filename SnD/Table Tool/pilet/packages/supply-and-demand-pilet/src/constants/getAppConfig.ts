import { settings } from '@icis/app-shell-apis';

import { AppConfig } from 'types';
import { dev, nonDev } from './global';

/**
 * Gets the config for this app. There is one config for dev and another for all other environments that is ascertained based on context.
 * The current environment is requested from the app shell.
 */
const getAppConfig = (): AppConfig => (settings.environment === 'dev' ? dev : nonDev);

export default getAppConfig;
