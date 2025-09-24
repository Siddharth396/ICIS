import { ApolloClient, NormalizedCacheObject } from '@apollo/client';
import { getApolloClient } from '@icis/app-shell-apis';

import { getIsAuthoring } from 'constants/isAuthoring';
import getAppConfig from 'constants/getAppConfig';

const { bffUrl, authoringBffUrl, version, bypassVersionCheck, appName, alwaysRefresh } = getAppConfig();

let instance: ApolloClient<NormalizedCacheObject> | undefined;

// let instance: Promise<ApolloClient<NormalizedCacheObject>> | undefined;

/* Code for caching is retained in case it needs to be enabled in future */
const shouldResetStore = () => {
  const today = new Date();
  const currentDateUTC = today.getTime();
  const dateToExpire = window.localStorage.getItem('dateToExpire');
  const dateToExpireInNumber = dateToExpire ? parseInt(dateToExpire) : currentDateUTC;

  /* istanbul ignore else */
  if (currentDateUTC >= dateToExpireInNumber) {
    const todayPlusOneDay = new Date(today.setDate(today.getDate() + 1));
    const savedDateUTC = todayPlusOneDay.setUTCHours(0, 0, 0, 0);
    window.localStorage.setItem('dateToExpire', savedDateUTC.toString());
    return true;
  }
  return false;
};

/**
 * Gets an apollo client using the getApolloClient from the app-shell-apis.
 * This client is preconfigured to cache to local storage under the key icis.appName
 */
const getClient = async () => {
  /* istanbul ignore else */
  if (!instance) {
    instance = await getApolloClient(appName, version, getIsAuthoring() ? authoringBffUrl : bffUrl, bypassVersionCheck);
    if (alwaysRefresh || (shouldResetStore() && version !== 'testVersion')) {
      await instance?.resetStore();
    }
  }
  return instance;
};

export default getClient;
