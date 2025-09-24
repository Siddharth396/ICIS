import IBaseProps from '../../Common/Model/IBaseProps';

export interface Props extends IBaseProps {
  commodities: string
  regions: string
};

export type OutagesData = {
  outagesByCommoditiesAndRegions: Outage[];
};

type Outage = {
  outageStart: string,
  outageEnd: string,
  country: string,
  company: string,
  site: string,
  plantNo: number,
  cause: string,
  capacityLoss: string,
  totalAnnualCapacity: number,
  lastUpdated: string,
  comments: string,
};
