// istanbul ignore file
import withClient, { ApolloClientProps } from 'components/HOCs/withClient';
import { useQuery } from '@apollo/client';
import {
  GET_PRICE_SERIES_FOR_PRICE_SERIES_DISPLAY_TOOL,
  PriceDisplaySeriesResponse,
  RowInput,
  RowInputPriceDisplayTable,
  PriceSeriesForDisplayTool,
  SelectedFilterInput,
} from 'apollo/queries';
import PriceDisplaySeriesSelectModal from '../PriceDisplaySeriesSelectModal/PriceDisplaySeriesSelectModal';
import usePriceDisplayModalState from 'utils/hooks/usePriceDisplayModalState';
import { useEffect ,useRef } from 'react';

export type Props = {
  showModal: boolean;
  selectedPriceSeries: RowInput[] | [];
  setShowModal: React.Dispatch<React.SetStateAction<boolean>>;
  onUpdateContentBlock: (updatedSelectedSeries: RowInputPriceDisplayTable[]) => void;
  selectedFiltersData?: SelectedFilterInput;
  editMode: boolean;
};

const PriceDisplaySelectModalContainer = ({
  client,
  showModal,
  selectedPriceSeries,
  setShowModal,
  onUpdateContentBlock,
  selectedFiltersData,
  editMode,
}: Props & ApolloClientProps) => {
  const {
    commodity,
    commodityOptions,
    setCommodity,
    priceCategories,
    setPriceCategories,
    regions,
    setRegions,
    itemFrequencies,
    setItemFrequencies,
    priceSettlementTypes,
    setPriceSettlementTypes,
    setInactivePriceSeriesFlag,
    inactivePriceSeriesFlag,
  } = usePriceDisplayModalState(client, selectedFiltersData);

  const commodityUUIDs = commodity?.map((commodity: any) => commodity.id);
  const isFirstRender = useRef(true);

  const { data, loading } = useQuery<PriceDisplaySeriesResponse>(
    GET_PRICE_SERIES_FOR_PRICE_SERIES_DISPLAY_TOOL,
    {
      variables: {
        commodities: commodityUUIDs,
        includeInactivePriceSeries: inactivePriceSeriesFlag,
      },
      client,
      fetchPolicy: 'no-cache',
      skip: !commodityUUIDs || commodityUUIDs.length === 0, // Skip the query if commodityUUIDs is empty or undefined
    },
  );
  const selectionState = {
    commodity,
    commodityOptions,
    priceCategories,
    regions,
    priceSettlementTypes,
    itemFrequencies,
    inactivePriceSeriesFlag,
    setCommodity,
    setPriceCategories,
    setRegions,
    setPriceSettlementTypes,
    setItemFrequencies,
    setInactivePriceSeriesFlag,
  };

  useEffect(() => {
    if (isFirstRender.current && data) {
      // setting preselected filters on first modal render
      const priceSeriesData = data?.priceSeriesForDisplayTool;
      if (editMode && priceSeriesData && selectedFiltersData) {
        const filteredRegions = filterAndMapSelectedFilters(
          priceSeriesData.regions,
          selectedFiltersData.selectedRegions,
        );
        const filteredAssessedFrequencies = filterAndMapSelectedFilters(
          priceSeriesData.assessedFrequencies,
          selectedFiltersData.selectedAssessedFrequencies,
        );
        const filteredPriceCategories = filterAndMapSelectedFilters(
          priceSeriesData.priceCategories,
          selectedFiltersData.selectedPriceCategories,
        );
        const filteredTransactionType = filterAndMapSelectedFilters(
          priceSeriesData.transactionTypes,
          selectedFiltersData.selectedTransactionTypes,
        );
        setRegions(filteredRegions);
        setItemFrequencies(filteredAssessedFrequencies);
        setPriceCategories(filteredPriceCategories);
        setPriceSettlementTypes(filteredTransactionType);
      }
      isFirstRender.current = false;
    }
  }, [data]);

  const filterAndMapSelectedFilters = (data: any[], selectedData: string[]) => {
    return data
      .filter((item: any) => selectedData.includes(item.name))
      .map((item: any) => ({ id: item.id, label: item.name, value: item.name }));
  };

  return (
    // @ts-ignore
    <PriceDisplaySeriesSelectModal
      showModal={showModal}
      selectedPriceSeries={selectedPriceSeries}
      setShowModal={setShowModal}
      onUpdateContentBlock={onUpdateContentBlock}
      data={data?.priceSeriesForDisplayTool as unknown as PriceSeriesForDisplayTool}
      loading={loading}
      selectionState={selectionState}
      editMode={editMode}
    />
  );
};

export default withClient(PriceDisplaySelectModalContainer);
