// PriceEntrySelectModalContainer.tsx
// istanbul ignore file
import withClient, { ApolloClientProps } from 'components/HOCs/withClient';
import { useLazyQuery } from '@apollo/client';
import { GET_PRICE_SERIES, IPriceEntrySeriesItem, PriceSeriesResponse, RowInput } from 'apollo/queries';
import PriceSeriesSelectModal from './PriceSeriesSelectModal';
import usePriceEntryModalState from 'utils/hooks/usePriceEntryModalState';
import { useEffect, useMemo } from 'react';

export type Props = {
  showModal: boolean;
  selectedPriceSeries?: RowInput[];
  errorCodes: string[];
  setShowModal: React.Dispatch<React.SetStateAction<boolean>>;
  onUpdateContentBlock: (updatedSelectedSeries: RowInput[]) => void;
  selectedPriceSeriesInOtherGrids: IPriceEntrySeriesItem[];
};

const PriceEntrySelectModalContainer = ({
  client,
  showModal,
  selectedPriceSeries = [],
  errorCodes,
  setShowModal,
  onUpdateContentBlock,
  selectedPriceSeriesInOtherGrids
}: Props & ApolloClientProps) => {

  const priceSeriesIds = useMemo<string[]>(() => {
    return selectedPriceSeries.map((x) => x.priceSeriesId);
  }, [selectedPriceSeries]);

  const {
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
    loading: filtersLoading,
    commodityOptions,
    priceCategoryOptions,
    regionOptions,
    priceSettlementTypeOptions,
    itemFrequencyOptions,
    hasSelectedAllFilters,
  } = usePriceEntryModalState(client, priceSeriesIds);

  const [fetchPriceSeries, { data, loading: seriesLoading }] = useLazyQuery<PriceSeriesResponse>(
    GET_PRICE_SERIES,
    {
      client,
      fetchPolicy: 'no-cache',
    },
  );

  useEffect(() => {
    if (hasSelectedAllFilters) {
      fetchPriceSeries({
        variables: {
          commodityId: commodity?.value ?? undefined, // Handle null
          priceCategoryId: priceCategory?.value ?? undefined, // Handle null
          regionId: region?.value ?? undefined, // Handle null
          priceSettlementTypeId: priceSettlementType?.value ?? undefined, // Handle null
          itemFrequencyId: itemFrequency?.value ?? undefined, // Handle null
        },
      });
    }
  }, [
    commodity,
    priceCategory,
    region,
    priceSettlementType,
    itemFrequency,
    hasSelectedAllFilters,
    fetchPriceSeries,
  ]);

  const selectionState = {
    commodity,
    priceCategory,
    region,
    priceSettlementType,
    itemFrequency,
    setCommodity,
    setPriceCategory,
    setRegion,
    setPriceSettlementType,
    setItemFrequency,
    hasSelectedAllFilters,
  };

  const dropdownOptions = {
    commodities: commodityOptions,
    priceCategories: priceCategoryOptions,
    regions: regionOptions,
    priceSettlementTypes: priceSettlementTypeOptions,
    itemFrequencies: itemFrequencyOptions,
  };

  return (
    <PriceSeriesSelectModal
      errorCodes={errorCodes}
      showModal={showModal}
      selectedPriceSeries={selectedPriceSeries}
      data={data?.priceSeries ?? []}
      loading={filtersLoading || seriesLoading}
      selectionState={selectionState}
      dropdownOptions={dropdownOptions}
      setShowModal={setShowModal}
      onUpdateContentBlock={onUpdateContentBlock}
      selectedPriceSeriesInOtherGrids={selectedPriceSeriesInOtherGrids}
    />
  );
};

export default withClient(PriceEntrySelectModalContainer);
