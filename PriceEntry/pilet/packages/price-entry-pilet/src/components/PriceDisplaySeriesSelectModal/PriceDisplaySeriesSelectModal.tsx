// istanbul ignore file
import { useCallback, useEffect, useRef, useState } from 'react';
import { Modal, Text, theme } from '@icis/ui-kit';
import withClient, { ApolloClientProps } from 'components/HOCs/withClient';
import { isEqual } from 'lodash';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import {
  RowInputPriceDisplayTable,
  PriceSeriesForDisplayTool,
  IPriceSeriesDetailForDisplay,
  SelectedFilterInput,
} from 'apollo/queries';
import PDSeriesList from 'components/PDSeriesList';
import {
  GridPreviewSkeleton,
  HeadingBorder,
  ListContainer,
  SkeletonWrapper,
  TableArea,
  Wrapper,
  ContentWrapper,
  MainContainer,
  HeaderWrapper,
} from '../PriceSeriesSelectModal/PriceSeriesSelectModal.style';
import PESeriesSelectModalFooter from '../PriceSeriesSelectModal/PriceSeriesSelectModal.Footer';
import PriceDisplayLeftSideBarMenu from './PriceDisplayLeftSideBarMenu';
import { IMultiOption } from '../MultiSelectInputRenderer/MultiSelectInputRenderer';
import { Container } from './styled';

export type Props = {
  showModal: boolean;
  selectedPriceSeries: RowInputPriceDisplayTable[];
  setShowModal: React.Dispatch<React.SetStateAction<boolean>>;
  onUpdateContentBlock: (
    updatedSelectedSeries: RowInputPriceDisplayTable[],
    selectedFilters: SelectedFilterInput,
  ) => void;
  data: PriceSeriesForDisplayTool;
  loading: boolean;
  selectionState: {
    commodity: any;
    commodityOptions: any;
    priceCategories: IMultiOption[];
    regions: IMultiOption[];
    priceSettlementTypes: IMultiOption[];
    itemFrequencies: IMultiOption[];
    inactivePriceSeriesFlag: boolean;
    setCommodity: (value: any) => void;
    setPriceCategories: (value: IMultiOption[]) => void;
    setRegions: (value: IMultiOption[]) => void;
    setPriceSettlementTypes: (value: IMultiOption[]) => void;
    setItemFrequencies: (value: IMultiOption[]) => void;
    setInactivePriceSeriesFlag: (value: boolean) => void;
  };
  editMode: boolean;
};

const PriceDisplaySeriesSelectModal = ({
  showModal,
  selectedPriceSeries,
  setShowModal,
  onUpdateContentBlock,
  data,
  loading,
  selectionState,
  editMode,
}: Props & ApolloClientProps) => {
  const {
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
  } = selectionState;
  const modalContentRef = useRef<HTMLDivElement>(null);
  const messages = useLocaleMessages();
  const [updatedSelectedSeries, setUpdatedSelectedSeries] = useState<RowInputPriceDisplayTable[]>(
    selectedPriceSeries ?? [],
  );

  const [selectedFilters, setSelectedFilter] = useState<SelectedFilterInput>();

  const [priceSeries, setPriceSeries] = useState<IPriceSeriesDetailForDisplay[]>();

  useEffect(() => {
    /* istanbul ignore else */
    let priceSeriesDetails = data?.priceSeriesDetails;

    if (regions.length > 0) {
      priceSeriesDetails = priceSeriesDetails?.filter((x) =>
        regions.some((region) => region.id === x.regionId),
      );
    }

    if (priceCategories.length > 0) {
      priceSeriesDetails = priceSeriesDetails?.filter((x) =>
        priceCategories.some((priceCategory) => priceCategory.id === x.priceCategoryId),
      );
    }

    if (priceSettlementTypes.length > 0) {
      priceSeriesDetails = priceSeriesDetails?.filter((x) =>
        priceSettlementTypes.some(
          (priceSettlementType) => priceSettlementType.id === x.priceSettlementTypeId,
        ),
      );
    }

    if (itemFrequencies.length > 0) {
      priceSeriesDetails = priceSeriesDetails?.filter((x) =>
        itemFrequencies.some((itemFrequency) => itemFrequency.id === x.itemFrequencyId),
      );
    }

    setPriceSeries(priceSeriesDetails);

    if (commodity) {
      setSelectedFilter({
        selectedCommodities: commodity.map((c: any) => c.value as string),
        selectedPriceCategories: priceCategories.map((category) => category.value as string),
        selectedRegions: regions.map((region) => region.value as string),
        selectedTransactionTypes: priceSettlementTypes.map(
          (settlementType) => settlementType.value as string,
        ),
        selectedAssessedFrequencies: itemFrequencies.map((frequency) => frequency.value as string),
        isInactiveIncluded: inactivePriceSeriesFlag,
      });
    }

    if (editMode) moveSelectedRowsToTop(priceSeriesDetails);
  }, [data, regions, priceCategories, priceSettlementTypes, itemFrequencies]);

  const moveSelectedRowsToTop = useCallback(
    (priceSeriesDetails: IPriceSeriesDetailForDisplay[] | [IPriceSeriesDetailForDisplay]) => {
      if (priceSeriesDetails) {
        const selectedSeriesIds = updatedSelectedSeries.map((series) => series.priceSeriesId);
        const selectedPriceSeries = priceSeriesDetails
          .filter((series) => selectedSeriesIds.includes(series.id))
          .sort((a, b) =>
            a.name
              .replace(/\s+/g, '')
              .toUpperCase()
              .localeCompare(b.name.replace(/\s+/g, '').toUpperCase()),
          );
        const unselectedPriceSeries = priceSeriesDetails.filter(
          (series) => !selectedSeriesIds.includes(series.id),
        );
        const sortedPriceSeries = [...selectedPriceSeries, ...unselectedPriceSeries];
        setPriceSeries(sortedPriceSeries);
      }
    },
    [updatedSelectedSeries],
  );

  const onDismissModal = useCallback(() => {
    setShowModal(false);
  }, [setShowModal]);

  const onSubmitModal = useCallback(() => {
    setShowModal(false);
  }, [setShowModal]);

  const handleAddUpdateGrid = useCallback(
    (onSubmit: ((e?: any) => void) | undefined) => {
      onUpdateContentBlock(updatedSelectedSeries, selectedFilters!);
      onSubmit?.();
    },
    [updatedSelectedSeries, onUpdateContentBlock, selectedFilters],
  );

  const handleSeriesSelect = (selectedSeriesIds: RowInputPriceDisplayTable[]) => {
    setUpdatedSelectedSeries((prevSelectedSeriesID = []) => {
      const newSelectedSeriesID = prevSelectedSeriesID.filter((selectedSeries) =>
        selectedSeriesIds.includes(selectedSeries),
      );
      selectedSeriesIds.forEach((selectedSeries) => {
        if (!newSelectedSeriesID.includes(selectedSeries)) {
          newSelectedSeriesID.push(selectedSeries);
        }
      });
      return newSelectedSeriesID;
    });
  };

  return (
    <Modal
      onDismiss={onDismissModal}
      onSubmit={onSubmitModal}
      isOpen={showModal}
      ariaLabel='price-entry-series-select-modal'
      testId='price-entry-series-select-modal'
      variant='MediumLarge'
      header={{
        visible: true,
        title: <HeaderWrapper>{messages.Capabilty.SelectPriceSeries}</HeaderWrapper>,
      }}
      footer={{
        visible: true,
        FooterContent: ({ onSubmit }) => (
          <PESeriesSelectModalFooter
            onClose={() => setShowModal(false)}
            onSave={() => handleAddUpdateGrid(onSubmit)}
            canSave={
              (updatedSelectedSeries &&
                updatedSelectedSeries.length > 0 &&
                priceSeries &&
                priceSeries.length > 0 &&
                !isEqual(updatedSelectedSeries, selectedPriceSeries)) ||
              false
            }
            cancelButtonTestId='price-entry-modal-cancel-button'
            saveButtonTestId='price-entry-modal-save-button'
          />
        ),
        style: {
          width: 'calc(100% + 40px)',
          marginRight: '-20px',
          marginLeft: '0',
          padding: '20px',
          background: `${theme.colours.PRIMARY_8}`,
          borderTop: `1px solid ${theme.colours.PRIMARY_5}`,
        },
      }}>
      <Wrapper ref={modalContentRef} data-testid='price-entry-grid-modal-content'>
        <TableArea>
          <HeadingBorder />
          <ContentWrapper>
            <Container>
              <MainContainer data-testid='leftside-maincontainer'>
                <PriceDisplayLeftSideBarMenu
                  commodity={commodity}
                  commodityData={commodityOptions}
                  priceCategories={priceCategories}
                  regions={regions}
                  priceSettlementTypes={priceSettlementTypes}
                  itemFrequencies={itemFrequencies}
                  inactivePriceSeriesFlag={inactivePriceSeriesFlag}
                  setCommodity={setCommodity}
                  setPriceCategories={setPriceCategories}
                  setRegions={setRegions}
                  setPriceSettlementTypes={setPriceSettlementTypes}
                  setItemFrequencies={setItemFrequencies}
                  priceSeriesData={data}
                  setInactivePriceSeriesFlag={setInactivePriceSeriesFlag}
                />
              </MainContainer>
            </Container>
            {loading && (
              <SkeletonWrapper data-testid='grid-loading'>
                <GridPreviewSkeleton />
              </SkeletonWrapper>
            )}
            {!loading && (
              <ListContainer>
                {/* @ts-ignore */}
                {!priceSeries || priceSeries?.length === 0 ? (
                  <div style={{ textAlign: 'center', padding: '20px' }}>
                    <Text.Body>No price series available</Text.Body>
                  </div>
                ) : (
                  <div style={{ display: 'flex', height: '100%' }}>
                    <PDSeriesList
                      seriesData={priceSeries}
                      width={'100%'}
                      selectedNodes={updatedSelectedSeries}
                      onSeriesSelect={handleSeriesSelect}
                    />
                  </div>
                )}
              </ListContainer>
            )}
          </ContentWrapper>
        </TableArea>
      </Wrapper>
    </Modal>
  );
};

export default withClient(PriceDisplaySeriesSelectModal);
