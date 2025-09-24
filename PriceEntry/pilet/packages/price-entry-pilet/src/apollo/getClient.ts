import { ApolloClient, NormalizedCacheObject } from '@apollo/client';
import { getApolloClient } from '@icis/app-shell-apis';
import getAppConfig from 'constants/getAppConfig';

const { bffUrl, version, bypassVersionCheck, appName } = getAppConfig();

let instance: Promise<ApolloClient<NormalizedCacheObject>> | undefined;

/**
 * Gets an apollo client using the getApolloClient from the app-shell-apis.
 * This client is preconfigured to cache to local storage under the key icis.appName
 */
const getClient = () => {
  /* istanbul ignore else */
  if (!instance) {
    instance = getApolloClient(appName, version, bffUrl, bypassVersionCheck);
  }
  return instance;
};

export default getClient;
