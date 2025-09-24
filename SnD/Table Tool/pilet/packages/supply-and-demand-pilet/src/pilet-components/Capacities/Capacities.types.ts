import IBaseProps from '../../Common/Model/IBaseProps';

export interface Props extends IBaseProps {
  commodities: string
  regions: string
};

export type CapacitiesData = {
  capacityDevelopmentsByCommoditiesAndRegions: Capacity[];
};

type Capacity = {
  country: string,
  company: string,
  site: string,
  plantNo: number,
  type: string,
	estimatedStart: string,
	newAnnualCapacity: number,
  capacityChange: number,
	percentChange: number | string,
	lastUpdated: string
};
