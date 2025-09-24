// istanbul ignore file
import { useEffect, useState } from 'react';
import { useQuery, ApolloClient } from '@apollo/client';
import {
  GET_COMMODITIES_FOR_PRICE_SERIES_DISPLAY_TOOL,
  CommoditiesForPriceDisplayTool,
  SelectedFilterInput,
} from 'apollo/queries';
import { IMultiOption } from '../../components/MultiSelectInputRenderer/MultiSelectInputRenderer';

const usePriceDisplayModalState = (
  client: ApolloClient<any>,
  selectedFiltersData?: SelectedFilterInput,
) => {
  const [commodity, setCommodity] = useState<any>();
  const [commodityOptions, setCommodityOptions] = useState<any>();
  const [priceCategories, setPriceCategories] = useState<IMultiOption[]>([]);
  const [regions, setRegions] = useState<IMultiOption[]>([]);
  const [itemFrequencies, setItemFrequencies] = useState<IMultiOption[]>([]);
  const [priceSettlementTypes, setPriceSettlementTypes] = useState<IMultiOption[]>([]);
  const [inactivePriceSeriesFlag, setInactivePriceSeriesFlag] = useState<boolean>(false);

  const { data } = useQuery<CommoditiesForPriceDisplayTool>(
    GET_COMMODITIES_FOR_PRICE_SERIES_DISPLAY_TOOL,
    {
      client,
      fetchPolicy: 'no-cache',
    },
  );

  useEffect(() => {
    if (data && data.commodities) {
      setCommodityOptions(commodityData);
    }

    if (selectedFiltersData) {
      setInactivePriceSeriesFlag(selectedFiltersData.isInactiveIncluded);
      setCommodity(filteredCommodities);
    }
  }, [data]);

  useEffect(() => {
    if (!commodity?.length) {
      setInactivePriceSeriesFlag(false);
    }
  }, [commodity]);

  const commodityData = data?.commodities.map((commodity) => ({
    id: commodity.id,
    label: commodity.name,
    value: commodity.name,
  }));
  const filteredCommodities = commodityData?.filter((commodity: any) =>
    selectedFiltersData?.selectedCommodities.includes(commodity.label),
  );

  return {
    commodity,
    setCommodity,
    commodityOptions,
    priceCategories,
    setPriceCategories,
    regions,
    setRegions,
    itemFrequencies,
    setItemFrequencies,
    priceSettlementTypes,
    setPriceSettlementTypes,
    inactivePriceSeriesFlag,
    setInactivePriceSeriesFlag,
  };
};

export default usePriceDisplayModalState;
