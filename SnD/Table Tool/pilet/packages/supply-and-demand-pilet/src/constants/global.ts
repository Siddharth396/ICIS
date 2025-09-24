import { AppConfig, Column } from 'types';
import getMessages from 'constants/getMessages';

const subscriberHostname = window.location.hostname.replace('authoring', 'subscriber');
const authoringHostname = window.location.hostname.replace('subscriber', 'authoring');

const capacitiesMessages = getMessages('en').capacities.table;
const outagesMessages = getMessages('en').outages.table;

const common = {
  appName: 'supply-demand',
  bffVersion: 'v1',
  version: process.env.VERSION || 'dev',
  bypassVersionCheck: false,
  alwaysRefresh: true,
  disableGWA: false,
  // turn this on to test mock bff!
  bffCallEnabled: false,
};

export const dev = {
  ...common,
  bffUrl: 'https://localhost:4400/v1/graphql',
  authoringBffUrl: 'https://localhost:4500/v1/graphql',
};

export const nonDev = {
  ...common,
  bffUrl: `${window.location.protocol}//${subscriberHostname}/api/${common.appName}/${common.bffVersion}/graphql`,
  authoringBffUrl: `${window.location.protocol}//${authoringHostname}/api/${common.appName}/${common.bffVersion}/graphql`,
};

/** Mock app config for unit tests  */
export const mockConfig: AppConfig = {
  appName: 'supply-demand',
  bffVersion: 'v1',
  version: 'dev',
  bypassVersionCheck: false,
  bffCallEnabled: true,
  alwaysRefresh: true,
  disableGWA: true,
  bffUrl: 'testBffUrl',
  authoringBffUrl: 'testBffUrl',
};

export const subPath: string = `/mfe/${common.appName}-pilet`;

export const capacityTable: Column[] = [
  { key: 'country', label: capacitiesMessages.location, width: 134 },
  { key: 'company', label: capacitiesMessages.company, width: 141 },
  { key: 'site', label: capacitiesMessages.site, width: 105 },
  { key: 'plantNo', label: capacitiesMessages.plantNo, width: 125, align: 'right' },
  { key: 'type', label: capacitiesMessages.type, width: 126 },
  { key: 'estimatedStart', label: capacitiesMessages.estimatedStart, width: 139 },
  { key: 'newAnnualCapacity', label: capacitiesMessages.newAnnualCapacity, width: 198, align: 'right' },
  { key: 'capacityChange', label: capacitiesMessages.capacityChange, width: 174, align: 'right' },
  { key: 'percentChange', label: capacitiesMessages.percentChange, width: 105, align: 'right' },
  { key: 'lastUpdated', label: capacitiesMessages.lastUpdated, width: 126 }
];

export const outagesTable: Column[] = [
  { key: 'country', label: outagesMessages.location, width: 134 },
  { key: 'company', label: outagesMessages.company, width: 141 },
  { key: 'site', label: outagesMessages.site, width: 105 },
  { key: 'outageStart', label: outagesMessages.outageStart, width: 216 },
  { key: 'outageEnd', label: outagesMessages.outageEnd, width: 201 },
  { key: 'plantNo', label: outagesMessages.plantNo, width: 124, align: 'right' },
  { key: 'cause', label: outagesMessages.cause, width: 131 },
  { key: 'capacityLoss', label: outagesMessages.capacityLoss, width: 132, align: 'right' },
  { key: 'totalAnnualCapacity', label: outagesMessages.totalAnnualCapacity, width: 201, align: 'right' },
  { key: 'lastUpdated', label: outagesMessages.lastUpdated, width: 122 },
];
