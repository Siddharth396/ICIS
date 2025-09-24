// istanbul ignore file
import { useCallback, useRef, useMemo, useState, memo, useEffect } from 'react';
import { Modal, Text, theme } from '@icis/ui-kit';
import withClient, { ApolloClientProps } from 'components/HOCs/withClient';
import { isEqual } from 'lodash';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import { IPriceEntrySeriesItem, PriceSeries, PriceSeriesDetails, RowInput, PriceSeriesValidationResult } from 'apollo/queries';
import PESeriesList from 'components/PESeriesList';
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
  FooterWrapper,
} from './PriceSeriesSelectModal.style';
import PESeriesSelectModalFooter from './PriceSeriesSelectModal.Footer';
import LeftSidebarMenu from './LeftSidebarMenu';
import ErrorFooter from './ErrorFooter';
import GridPlaceholder from '../GridPlaceholder/GridPlaceholder';
import { IOption } from 'utils/types';

type GridApi = any; // Replace with actual grid API type if known

type Props = {
  showModal: boolean;
  selectedPriceSeries?: RowInput[];
  data: PriceSeries[];
  loading: boolean;
  selectionState: {
    commodity: IOption | undefined;
    priceCategory: IOption | undefined;
    region: IOption | undefined;
    priceSettlementType: IOption | undefined;
    itemFrequency: IOption | undefined;
    setCommodity: (value: IOption | undefined) => void;
    setPriceCategory: (value: IOption | undefined) => void;
    setRegion: (value: IOption | undefined) => void;
    setPriceSettlementType: (value: IOption | undefined) => void;
    setItemFrequency: (value: IOption | undefined) => void;
    hasSelectedAllFilters: boolean;
  };
  dropdownOptions: {
    commodities: IOption[];
    priceCategories: IOption[];
    regions: IOption[];
    priceSettlementTypes: IOption[];
    itemFrequencies: IOption[];
  };
  errorCodes: string[];
  setShowModal: React.Dispatch<React.SetStateAction<boolean>>;
  onUpdateContentBlock: (updatedSelectedSeries: RowInput[], e?: any) => void;
  selectedPriceSeriesInOtherGrids: IPriceEntrySeriesItem[];
};

const ModalFooter = memo(
  ({
    errorCodes,
    canSave,
    onClose,
    onSave,
  }: {
    errorCodes: string[];
    canSave: boolean;
    onClose: () => void;
    onSave: () => void;
  }) => (
    <FooterWrapper>
      <ErrorFooter errorCodes={errorCodes} />
      <PESeriesSelectModalFooter
        canSave={canSave}
        cancelButtonTestId='price-entry-modal-cancel-button'
        saveButtonTestId='price-entry-modal-save-button'
        onClose={onClose}
        onSave={onSave}
      />
    </FooterWrapper>
  ),
);

const PriceSeriesSelectModal = ({
  showModal,
  selectedPriceSeries = [],
  dropdownOptions,
  data,
  loading,
  selectionState,
  errorCodes,
  setShowModal,
  onUpdateContentBlock,
  selectedPriceSeriesInOtherGrids
}: Props & ApolloClientProps) => {
  const {
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
  } = selectionState;

  const modalContentRef = useRef<HTMLDivElement>(null);
  const messages = useLocaleMessages();
  const [gridApis, setGridApis] = useState<Record<string, GridApi>>({});
  const [selectedGrid, setSelectedGrid] = useState<string | null>(null);
  const [updatedSelectedSeries, setUpdatedSelectedSeries] = useState<RowInput[]>(
    selectedPriceSeries || [],
  );

  const [lockedScheduleId, setLockedScheduleId] = useState<string | null>(null);

  const onDismissModal = useCallback(() => {
    setShowModal(false);
  }, [setShowModal]);

  // Pre-flatten series for schedule lookups
  const allSeries: PriceSeriesDetails[] = useMemo(
    () => (data || []).flatMap((d) => d.priceSeriesDetails || []),
    [data],
  );

  const priceSeriesIdsSelectedInOtherGridsSet = useMemo<Set<string>>(() =>
    new Set(selectedPriceSeriesInOtherGrids.flatMap(ps => ps.id)),
    [selectedPriceSeriesInOtherGrids]);

  const selectedScheduleIdInOtherGrid = useMemo<string | null>(() =>
    selectedPriceSeriesInOtherGrids[0]?.publicationScheduleId ?? null,
    [selectedPriceSeriesInOtherGrids]);

  /*
    * If lockedScheduleId is null, set it once based on available data. It will be null by default or when all price series are unselected and no selected price series is there in other grid.
    * Set the locked schedule ID first from another grid's selection if available, otherwise from the first selected series in the current grid.
  */
  useEffect(() => {
    if (updatedSelectedSeries.length == 0 && !selectedScheduleIdInOtherGrid) {
      setLockedScheduleId(null);
      return;
    }

    // If the lockedScheduleId is already set then don't set it again.
    if (lockedScheduleId) {
      return;
    }

    if (selectedScheduleIdInOtherGrid) {
      setLockedScheduleId(selectedScheduleIdInOtherGrid);
      return;
    }

    if (
      showModal &&
      allSeries.length > 0
    ) {
      const selectedIds = new Set(updatedSelectedSeries.map((s) => s.priceSeriesId));
      const firstWithSchedule = allSeries.find((s) => selectedIds.has(s.id) && s.scheduleId);
      setLockedScheduleId(firstWithSchedule?.scheduleId ?? null);
    }
  }, [showModal, selectedScheduleIdInOtherGrid, updatedSelectedSeries, allSeries]);

  const validatePriceSeries = useCallback(
    (priceSeriesDetail: PriceSeriesDetails): PriceSeriesValidationResult => {

      let validationResult: PriceSeriesValidationResult = { isValid: true };

      const isNotOfSameSchedule =
        lockedScheduleId &&
        priceSeriesDetail.scheduleId &&
        priceSeriesDetail.scheduleId !== lockedScheduleId;

      if (isNotOfSameSchedule) {
        validationResult = {
          isValid: false,
          message: messages.General.DifferentScheduleTooltipMessage,
        };
      }

      if (priceSeriesIdsSelectedInOtherGridsSet.has(priceSeriesDetail.id)) {
        validationResult = {
          isValid: false,
          message: messages.General.PriceSeriesAlreadySelectedTooltipMessage,
        };
      }

      return validationResult;
    },
    [lockedScheduleId, priceSeriesIdsSelectedInOtherGridsSet]
  );

  const validatedData = useMemo<PriceSeries[]>(() => {
    return data.map((priceSeries): PriceSeries => ({
      ...priceSeries,
      priceSeriesDetails: priceSeries.priceSeriesDetails.map((detail): PriceSeriesDetails => ({
        ...detail,
        priceSeriesValidationResult: validatePriceSeries(detail),
      })),
    }));
  }, [data, validatePriceSeries]);

  useEffect(() => {
    if (showModal) {
      // reset both the series list & selected grid on open
      setUpdatedSelectedSeries(selectedPriceSeries);
      setSelectedGrid(null);
    }
  }, [showModal, selectedPriceSeries]);

  const onSubmitModal = useCallback(() => {
    setShowModal(false);
  }, [setShowModal]);

  const handleAddUpdateGrid = useCallback(
    (onSubmit?: (e?: any) => void) => {
      onUpdateContentBlock(updatedSelectedSeries, onSubmit);
    },
    [updatedSelectedSeries, onUpdateContentBlock],
  );

  const handleSeriesSelect = useCallback(
    (selectedSeriesIds: RowInput[], gridId: string) => {
      // If user just cleared all selections (empty array) we should not attempt to derive a new lock here.
      if (selectedSeriesIds.length === 0) {
        setUpdatedSelectedSeries([]);
        return;
      }
      if (selectedGrid && selectedGrid !== gridId) {
        setUpdatedSelectedSeries(selectedSeriesIds);
      } else {
        setUpdatedSelectedSeries((prev = []) => {
          const newSelectedSeries = prev.filter((prevSeries) =>
            selectedSeriesIds.some((selectedSeries) => isEqual(selectedSeries, prevSeries)),
          );
          selectedSeriesIds.forEach((selectedSeries) => {
            if (!newSelectedSeries.some((s) => isEqual(s, selectedSeries))) {
              newSelectedSeries.push(selectedSeries);
            }
          });
          return newSelectedSeries;
        });
      }

      Object.entries(gridApis).forEach(([id, api]) => {
        if (id !== gridId) {
          api.forEachNode((node: any) => node.setSelected(false));
        }
      });

      setSelectedGrid(gridId);
    },
    [selectedGrid, gridApis],
  );

  const onGridReady = useCallback((id: string, api: GridApi) => {
    setGridApis((prevApis) => ({ ...prevApis, [id]: api }));
  }, []);

  const canSave = useMemo(() => {
    const originalPriceSeriesIds = selectedPriceSeries.map((series) => series.priceSeriesId).sort();

    const updatedPriceSeriesIds = updatedSelectedSeries
      .map((series) => series.priceSeriesId)
      .sort();

    const hasAnyChanges =
      updatedPriceSeriesIds.length !== originalPriceSeriesIds.length ||
      !isEqual(originalPriceSeriesIds, updatedPriceSeriesIds);

    return showModal && updatedPriceSeriesIds.length > 0 && hasAnyChanges;
  }, [updatedSelectedSeries, selectedPriceSeries, showModal]);

  const renderListContent = useCallback(() => {
    if (!hasSelectedAllFilters) {
      return <GridPlaceholder />;
    }

    if (!data || data.length === 0) {
      return (
        <div style={{ textAlign: 'center', padding: '20px' }}>
          <Text.Body>No price series available</Text.Body>
        </div>
      );
    }

    return (
      <div style={{ display: 'flex', height: '100%' }}>
        {validatedData.map((gd, i) => (
          <PESeriesList
            key={i}
            id={gd?.seriesItemTypeCode}
            seriesData={gd.priceSeriesDetails}
            width={data.length > 1 ? '50%' : '100%'}
            selectedNodes={updatedSelectedSeries}
            lockedScheduleId={lockedScheduleId}
            onSeriesSelect={handleSeriesSelect}
            onGridReady={onGridReady}
            clearLock={() => {
              setUpdatedSelectedSeries([]);
            }}
          />
        ))}
      </div>
    );
  }, [
    hasSelectedAllFilters,
    data,
    updatedSelectedSeries,
    handleSeriesSelect,
    onGridReady,
    lockedScheduleId,
  ]);

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
          <ModalFooter
            errorCodes={errorCodes}
            canSave={canSave}
            onClose={() => setShowModal(false)}
            onSave={() => handleAddUpdateGrid(onSubmit)}
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
            <MainContainer data-testid='leftside-maincontainer'>
              <LeftSidebarMenu
                disabled={updatedSelectedSeries.length > 0}
                dropdownOptions={dropdownOptions}
                commodity={commodity}
                priceCategory={priceCategory}
                region={region}
                priceSettlementType={priceSettlementType}
                itemFrequency={itemFrequency}
                setCommodity={setCommodity}
                setPriceCategory={setPriceCategory}
                setRegion={setRegion}
                setPriceSettlementType={setPriceSettlementType}
                setItemFrequency={setItemFrequency}
              />
            </MainContainer>
            {loading ? (
              <SkeletonWrapper data-testid='grid-loading'>
                <GridPreviewSkeleton />
              </SkeletonWrapper>
            ) : (
              <ListContainer isEmpty={!hasSelectedAllFilters || !data || data.length === 0}>
                {renderListContent()}
              </ListContainer>
            )}
          </ContentWrapper>
        </TableArea>
      </Wrapper>
    </Modal>
  );
};

export default withClient(memo(PriceSeriesSelectModal));
