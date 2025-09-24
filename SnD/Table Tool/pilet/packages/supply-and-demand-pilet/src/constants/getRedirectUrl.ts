// istanbul ignore file

import { subPath } from './global';

const getRedirectUrl = (isAuthoring: boolean): string => {
  const url = window.location.href;
  let redirectUrl;
  const authoringUrl = `${subPath}/authoring`;
  const subscriberUrl = subPath;

  if (url.includes('localhost')) {
    const subscriberPageUrl = new URL(url);
    subscriberPageUrl.port = '8085';
    redirectUrl = subscriberPageUrl.toString();
    if (isAuthoring) {
      redirectUrl = url.replace(authoringUrl, subscriberUrl);
    } else {
      redirectUrl = url.replace(subscriberUrl, authoringUrl);
    }
  } else if (isAuthoring) {
    redirectUrl = url.replace('authoring.', 'subscriber.');
    redirectUrl = redirectUrl.replace('.cha', '.genesis.cha');
    redirectUrl = redirectUrl.replace(authoringUrl, subscriberUrl);
  } else {
    redirectUrl = url.replace('subscriber.', 'authoring.');
    redirectUrl = redirectUrl.replace('.genesis', '');
    redirectUrl = redirectUrl.replace(subscriberUrl, authoringUrl);
  }
  return redirectUrl;
}

export default getRedirectUrl;
