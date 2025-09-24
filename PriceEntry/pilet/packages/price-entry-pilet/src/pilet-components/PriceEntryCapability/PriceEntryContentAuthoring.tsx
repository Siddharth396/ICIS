import { useMemo, useState, useCallback, Fragment } from 'react';
import {
  ContentBlockResponse,
  IPriceEntrySeriesItem,
  IPriceSeriesGrid,
  RowInput,
  SeriesItemInputType,
  UpdateUserPreferenceInput,
} from 'apollo/queries';
import { LockedContentDetails, LockInformationType } from '@icis/app-shell';
import { useUser } from '@icis/app-shell-apis';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import { PriceSeriesSelectionCard } from 'components/PriceSeriesSelectionCard';
import PriceEntrySelectModalContainer from 'components/PriceSeriesSelectModal/PriceEntrySelectModalContainer';
import WorkflowAuthoring from 'pilet-components/WorkflowMFE/WorkflowAuthoring';
import { getStartOfDayUTC } from 'utils/date';
import PECapabilitySkeleton from './PECapabilitySkeleton';
import PriceEntryCommentary from './PriceEntryCommentary';
import {
  AddGridButtonWrapper,
  CapabilityContainer,
  DatePickerWrapper,
  HeaderRow,
  HeaderWrapper,
  LockedContentCursorOverlay,
  LockedContentOverlay,
  RowLeft,
  RowRight,
  WorkflowButtonWrapper,
} from './styled';
import { useContentLockStatus } from 'utils/hooks/useContentLockStatus';
import AddGridButton from 'components/AddGridButton';
import GridItem from 'components/GridItem';
import ScheduleSelectorTool from 'pilet-components/ScheduleSelectorTool/ScheduleSelectorTool';

interface IPriceEntryContentAuthoring {
  loading: boolean;
  showPriceSeriesSelectionCard: boolean;
  data: ContentBlockResponse | undefined;
  iconVisibility: boolean;
  selectedDate: Date;
  isReviewMode?: boolean;
  lockInformation: LockInformationType | undefined;
  onSaveTitle: (title: string, gridId?: string) => Promise<void>;
  getReviewURL: () => string;
  setSelectedDate: (date: Date) => void;
  refetchGridData: (includeNotStarted?: boolean) => void;
  onStartEditing: (data?: Array<LockedContentDetails>) => void;
  onFinishEditing: (data?: Array<string>) => void;
  handlePriceSeriesDataUpdate: (
    payload: SeriesItemInputType,
    seriesItemTypeCode: string,
  ) => Promise<void>;
  handleUpdateUserPreference: (params: UpdateUserPreferenceInput) => Promise<void>;
  handleUpdateContentBlock: (params: any) => Promise<any>;
}

const PriceEntryContentAuthoring = ({
  loading,
  showPriceSeriesSelectionCard,
  data,
  lockInformation,
  isReviewMode,
  selectedDate: initialSelectedDate,
  iconVisibility,
  getReviewURL,
  setSelectedDate,
  refetchGridData,
  onSaveTitle,
  onStartEditing,
  onFinishEditing,
  handlePriceSeriesDataUpdate,
  handleUpdateUserPreference,
  handleUpdateContentBlock,
}: IPriceEntryContentAuthoring) => {
  const [userSelectedDate, setUserSelectedDate] = useState<Date | undefined>(undefined);
  const [activeGrid, setActiveGrid] = useState<any>(null);
  const [showModal, setShowModal] = useState(false);
  const [insertIndex, setInsertIndex] = useState<number | null>(null);
  const [modalErrorCodes, setModalErrorCodes] = useState([]);
  const [isNonPublishingDay, setIsNonPublishingDay] = useState<boolean>(false);

  const messages: { Errors: { [key: string]: string } } = useLocaleMessages();
  const user = useUser();

  const isContentLocked = useContentLockStatus({
    lockInformation,
    contentBlockId: data?.contentBlock?.contentBlockId,
    selectedDate: userSelectedDate || initialSelectedDate,
    userId: user?.userId,
  });

  // istanbul ignore next
  const isContentDisabled = useMemo(() => {
    const grids = data?.contentBlock?.priceSeriesGrids || [];
    const allSeries = grids.flatMap((grid: any) => grid.priceSeries || []);
    return allSeries.length > 0
      ? allSeries.every((series: IPriceEntrySeriesItem) => series.readOnly)
      : false;
  }, [data?.contentBlock?.priceSeriesGrids]);

  const scheduleId = useMemo(
    () => data?.contentBlock?.publicationScheduleId,
    [data?.contentBlock?.publicationScheduleId],
  );

  // istanbul ignore next
  const handleDateChange = useCallback(
    (date: Date, isPublishingDay: boolean) => {
      setSelectedDate(date);
      setUserSelectedDate(date);
      setIsNonPublishingDay(!!(scheduleId && !isPublishingDay));
    },
    [scheduleId, setSelectedDate],
  );

  // istanbul ignore next
  const getselectedPriceSeries: RowInput[] = useMemo(() => {
    if (!activeGrid) return [];

    return (
      activeGrid.priceSeriesIds?.map((seriesId: string, index: number) => ({
        priceSeriesId: seriesId,
        seriesItemTypeCode: activeGrid.seriesItemTypeCode,
        displayOrder: index,
      })) || []
    );
  }, [activeGrid]);

  // istanbul ignore next
  const getSelectedPriceSeriesInOtherGrids: IPriceEntrySeriesItem[] = useMemo(() => {
    const allGrids = data?.contentBlock?.priceSeriesGrids;

    if (!allGrids) return [];

    /* If the active grid is null, that means a new grid has been selected. In this case, fetch the data from all the other grids, if any.*/
    const notSelectedGrids = activeGrid ? allGrids.filter(g => g.id !== activeGrid.id) : allGrids;

    return notSelectedGrids.flatMap(g => g.priceSeries);
  }, [activeGrid?.id, data?.contentBlock?.priceSeriesGrids]);

  // istanbul ignore next
  const handleGridContentUpdate = async (
    selectedRows: RowInput[],
    onSubmit?: () => void,
  ): Promise<void> => {
    // 1. Build final grids array or a single grid object
    let finalPayload;
    if (insertIndex !== null) {
      // Building a new grid object (no id => brand new)
      const newGrid = {
        priceSeriesIds: selectedRows.map((row) => row.priceSeriesId),
      };
      const currentGrids = data?.contentBlock?.priceSeriesGrids || [];
      // Insert the new grid at the specified position
      finalPayload = [
        ...currentGrids.slice(0, insertIndex),
        newGrid,
        ...currentGrids.slice(insertIndex),
      ];
      setInsertIndex(null);
    } else {
      // Updating an existing grid by id
      finalPayload = {
        gridId: activeGrid?.id,
        priceSeriesIds: selectedRows.map((row) => row.priceSeriesId),
      };
    }

    // 2. Call the mutation
    const response = await handleUpdateContentBlock(finalPayload);
    // If there's no valid response, we can't proceed further
    if (!response?.data?.response) return;

    // 3. Check for errors from the server
    const { isValid, errorCodes } = response.data.response;
    if (!isValid || (errorCodes && errorCodes.length > 0)) {
      setModalErrorCodes(errorCodes);
      return;
    }

    // 4. No errors -> Clear error, close modal, and call onSubmit (if provided)
    setModalErrorCodes([]);
    setShowModal(false);
    onSubmit?.();
  };
  // istanbul ignore next
  const handleStartEditing = useCallback(() => {
    const lockedContentDetails: LockedContentDetails = {
      id:
        getStartOfDayUTC(userSelectedDate || initialSelectedDate)
          ?.getTime()
          .toString() || 'price-entry-authoring',
      data: data?.contentBlock?.contentBlockId,
    };
    onStartEditing([lockedContentDetails]);
  }, [userSelectedDate, initialSelectedDate, data?.contentBlock?.contentBlockId, onStartEditing]);

  // istanbul ignore next
  const handleFinishEditing = () => {
    const lockedContentId = getStartOfDayUTC(userSelectedDate || initialSelectedDate)
      ?.getTime()
      .toString();
    if (data?.contentBlock?.contentBlockId && lockedContentId) {
      onFinishEditing([lockedContentId]);
    }
  };

  // istanbul ignore next
  const handleUpdateUserPreferenceWithGrid = useCallback(
    (params: UpdateUserPreferenceInput, gridId: string) => {
      handleUpdateUserPreference?.({ ...params, gridId });
    },
    [handleUpdateUserPreference],
  );

  // istanbul ignore next
  const handleAddGridAtIndex = (index: number) => {
    // Remember where we want to insert the new grid
    setInsertIndex(index);
    // Indicate we're adding (not editing any existing grid)
    setActiveGrid(null);
    // Open the modal so user can pick series
    setShowModal(true);
  };

  // istanbul ignore next
  const renderGrids = useMemo(() => {
    const grids = data?.contentBlock?.priceSeriesGrids || [];
    return (
      <>
        {!isReviewMode && iconVisibility && (
          <AddGridButtonWrapper>
            <AddGridButton onAddClick={() => handleAddGridAtIndex(0)} />
          </AddGridButtonWrapper>
        )}
        {grids.map((grid: IPriceSeriesGrid, idx: number) => (
          <Fragment key={grid.id || idx}>
            <GridItem
              nmaEnabled={data?.contentBlock?.nmaEnabled || false}
              iconVisibility={!isReviewMode && iconVisibility}
              grid={grid}
              isContentLocked={isContentLocked}
              isContentDisabled={isContentDisabled}
              onSaveTitle={onSaveTitle}
              onEditGrid={() => {
                setActiveGrid(grid);
                setShowModal(true);
              }}
              onAddClick={() => {
                handleAddGridAtIndex(idx + 1);
              }}
              onDeleteGrid={() => handleDeleteGrid(grid)}
              onUpdatePriceSeriesData={handlePriceSeriesDataUpdate}
              onUpdateUserPreference={(params) =>
                handleUpdateUserPreferenceWithGrid(params, grid?.id)
              }
              onStartEditing={handleStartEditing}
              onFinishEditing={handleFinishEditing}
              selectedDate={userSelectedDate || initialSelectedDate}
            />
          </Fragment>
        ))}
      </>
    );
  }, [
    data,
    isContentLocked,
    isContentDisabled,
    handlePriceSeriesDataUpdate,
    handleUpdateUserPreferenceWithGrid,
    onStartEditing,
    onFinishEditing,
  ]);

  // istanbul ignore next
  const handleDeleteGrid = (grid: IPriceSeriesGrid) => {
    const updated = data?.contentBlock.priceSeriesGrids.filter((g) => g.id !== grid.id);
    handleUpdateContentBlock(updated);
  };

  // istanbul ignore next
  const scheduleSelectedDate = isReviewMode ? initialSelectedDate : userSelectedDate;

  return (
    <>
      <HeaderWrapper>
        <HeaderRow>
          <RowLeft>
            <DatePickerWrapper>
              <ScheduleSelectorTool
                show={(data?.contentBlock?.priceSeriesGrids ?? []).length > 0}
                defaultSelectedDate={initialSelectedDate}
                selectedDate={scheduleSelectedDate}
                scheduleId={scheduleId}
                disabled={isReviewMode}
                onDateChange={handleDateChange}
              />
            </DatePickerWrapper>
          </RowLeft>
          <RowRight>
            {data?.contentBlock && data.contentBlock?.priceSeriesGrids?.length >= 0 && (
              <LockedContentCursorOverlay isLocked={isContentLocked}>
                <LockedContentOverlay isLocked={isContentLocked}>
                  <WorkflowButtonWrapper>
                    <WorkflowAuthoring
                      isContentLocked={isContentLocked}
                      nextActions={data?.contentBlock?.nextActions}
                      contentBlockParams={{
                        version: data?.contentBlock?.version,
                        contentBlockId: data?.contentBlock?.contentBlockId,
                        priceSeriesIds: data?.contentBlock?.priceSeriesIds,
                        assessedDateTime: getStartOfDayUTC(userSelectedDate || initialSelectedDate),
                        nmaEnabled: data?.contentBlock?.nmaEnabled,
                      }}
                      getReviewURL={getReviewURL}
                      onRefetchGridData={refetchGridData}
                      isNonPublishingDay={isNonPublishingDay}
                    />
                  </WorkflowButtonWrapper>
                </LockedContentOverlay>
              </LockedContentCursorOverlay>
            )}
          </RowRight>
        </HeaderRow>
      </HeaderWrapper>
      {loading ? (
        /* istanbul ignore next */ <PECapabilitySkeleton contentHeight={400} />
      ) : (
        <>
          {showPriceSeriesSelectionCard && (
            <PriceSeriesSelectionCard
              testId='price-series-selection-card'
              // @ts-ignore
              buttonText={messages.Capabilty.SelectPriceSeries}
              onButtonClick={/* istanbul ignore next */ () => setShowModal(true)}
            />
          )}
          {!showPriceSeriesSelectionCard &&
            data?.contentBlock &&
            data.contentBlock?.priceSeriesGrids?.length >= 0 && (
              <LockedContentCursorOverlay isLocked={isContentLocked}>
                <LockedContentOverlay isLocked={isContentLocked}>
                  <CapabilityContainer>
                    {renderGrids}
                    <PriceEntryCommentary
                      data={data}
                      readOnlyView={isContentDisabled || isContentLocked}
                      contentBlockId={data?.contentBlock?.contentBlockId}
                      selectedDate={userSelectedDate || initialSelectedDate}
                      refetchGridData={refetchGridData}
                      onStartEditing={handleStartEditing}
                      onFinishEditing={handleFinishEditing}
                    />
                  </CapabilityContainer>
                </LockedContentOverlay>
              </LockedContentCursorOverlay>
            )}
          {
            /* istanbul ignore next*/ showModal && (
              <PriceEntrySelectModalContainer
                showModal={showModal}
                errorCodes={modalErrorCodes}
                selectedPriceSeries={getselectedPriceSeries}
                setShowModal={(show) => {
                  setShowModal(show);
                  setModalErrorCodes([]);
                }}
                onUpdateContentBlock={handleGridContentUpdate}
                selectedPriceSeriesInOtherGrids={getSelectedPriceSeriesInOtherGrids}
              />
            )
          }
        </>
      )}
    </>
  );
};

export default PriceEntryContentAuthoring;
