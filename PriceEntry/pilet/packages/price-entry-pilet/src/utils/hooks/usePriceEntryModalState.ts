// istanbul ignore file
import { useQuery, ApolloClient, ApolloError } from '@apollo/client';
import { LOAD_FILTERS } from 'apollo/queries';
import { useState, useEffect, useMemo } from 'react';
import { IOption } from 'utils/types';

type FilterDetail = {
  id: string | null;
  name: string;
  isDefault?: boolean;
};

type Filter = {
  name: string;
  filterDetails: FilterDetail[];
};

type FiltersData = {
  filters: Filter[];
};

type UsePriceEntryModalStateReturn = {
  commodity: IOption | undefined;
  setCommodity: (value: IOption | undefined) => void;
  priceCategory: IOption | undefined;
  setPriceCategory: (value: IOption | undefined) => void;
  region: IOption | undefined;
  setRegion: (value: IOption | undefined) => void;
  priceSettlementType: IOption | undefined;
  setPriceSettlementType: (value: IOption | undefined) => void;
  itemFrequency: IOption | undefined;
  setItemFrequency: (value: IOption | undefined) => void;
  loading: boolean;
  error: ApolloError | undefined;
  commodityOptions: IOption[];
  priceCategoryOptions: IOption[];
  regionOptions: IOption[];
  priceSettlementTypeOptions: IOption[];
  itemFrequencyOptions: IOption[];
  hasSelectedAllFilters: boolean;
};

function transformFilterDetails(
  filters: Filter[],
  filterName: string,
): { options: IOption[]; defaultOption: IOption | undefined } {
  const filterObj = filters.find((f) => f.name === filterName);
  if (!filterObj) return { options: [], defaultOption: undefined };

  const options = filterObj.filterDetails.map((d) => ({
    label: d.name,
    value: d.id,
    isDefault: d.isDefault || false,
  }));

  const defaultOption = options.find((o) => o.isDefault);

  return { options, defaultOption };
}

const usePriceEntryModalState = (
  client: ApolloClient<any>,
  selectedPriceSeriesIds: string[],
): UsePriceEntryModalStateReturn => {
  const { data, loading, error } = useQuery<FiltersData>(LOAD_FILTERS, {
    client,
    fetchPolicy: 'no-cache',
    variables: {
      selectedPriceSeriesIds,
    },
  });

  const [commodity, setCommodity] = useState<IOption | undefined>(undefined);
  const [priceCategory, setPriceCategory] = useState<IOption | undefined>(undefined);
  const [region, setRegion] = useState<IOption | undefined>(undefined);
  const [priceSettlementType, setPriceSettlementType] = useState<IOption | undefined>(undefined);
  const [itemFrequency, setItemFrequency] = useState<IOption | undefined>(undefined);

  const [commodityOptions, setCommodityOptions] = useState<IOption[]>([]);
  const [priceCategoryOptions, setPriceCategoryOptions] = useState<IOption[]>([]);
  const [regionOptions, setRegionOptions] = useState<IOption[]>([]);
  const [priceSettlementTypeOptions, setPriceSettlementTypeOptions] = useState<IOption[]>([]);
  const [itemFrequencyOptions, setItemFrequencyOptions] = useState<IOption[]>([]);

  useEffect(() => {
    if (data && data.filters) {
      const commodityData = transformFilterDetails(data.filters, 'commodity');
      const priceCategoryData = transformFilterDetails(data.filters, 'price-category');
      const regionData = transformFilterDetails(data.filters, 'region');
      const priceSettlementTypeData = transformFilterDetails(data.filters, 'price-settlement-type');
      const itemFrequencyData = transformFilterDetails(data.filters, 'frequency');

      setCommodityOptions(commodityData.options);
      setPriceCategoryOptions(priceCategoryData.options);
      setRegionOptions(regionData.options);
      setPriceSettlementTypeOptions(priceSettlementTypeData.options);
      setItemFrequencyOptions(itemFrequencyData.options);

      if (!commodity && commodityData.defaultOption) {
        setCommodity(commodityData.defaultOption);
      }
      if (!priceCategory && priceCategoryData.defaultOption) {
        setPriceCategory(priceCategoryData.defaultOption);
      }
      if (!region && regionData.defaultOption) {
        setRegion(regionData.defaultOption);
      }
      if (!priceSettlementType && priceSettlementTypeData.defaultOption) {
        setPriceSettlementType(priceSettlementTypeData.defaultOption);
      }
      if (!itemFrequency && itemFrequencyData.defaultOption) {
        setItemFrequency(itemFrequencyData.defaultOption);
      }
    }
  }, [data, commodity, priceCategory, region, priceSettlementType, itemFrequency]);

  const hasSelectedAllFilters = useMemo(() => {
    return (
      !!commodity?.value &&
      !!priceCategory?.value &&
      !!region?.value &&
      !!priceSettlementType?.value &&
      !!itemFrequency?.value
    );
  }, [commodity, priceCategory, region, priceSettlementType, itemFrequency]);

  return {
    commodity,
    setCommodity,
    priceCategory,
    setPriceCategory,
    region,
    setRegion,
    priceSettlementType,
    setPriceSettlementType,
    itemFrequency,
    setItemFrequency,
    loading,
    error,
    commodityOptions,
    priceCategoryOptions,
    regionOptions,
    priceSettlementTypeOptions,
    itemFrequencyOptions,
    hasSelectedAllFilters,
  };
};

export default usePriceEntryModalState;
