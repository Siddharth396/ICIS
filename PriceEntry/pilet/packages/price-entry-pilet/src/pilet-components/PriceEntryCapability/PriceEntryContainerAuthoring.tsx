import { useCallback, useEffect, useRef, useState } from 'react';
import { ContentBlockWrapper } from '@icis/ui-kit';
import { CapabilityParams, CapabilityProps, PiletApi } from '@icis/app-shell';
import withClient, { ApolloClientProps } from 'components/HOCs/withClient';
import { useLazyQuery, useMutation } from '@apollo/client';
import {
  ContentBlockResponse,
  GET_CONTENT_BLOCK,
  IPriceSeriesGrid,
  NotifierProps,
  SeriesItemInputType,
  UPDATE_CONTENT_BLOCK,
  UPDATE_PRICE_SERIES_DATA,
  UPDATE_USER_PREFERENCE,
  UpdateContentBlock,
  UpdateContentBlockResponse,
  UpdateUserPreference,
  UpdateUserPreferenceInput,
  UserPreferenceColumnInput,
} from 'apollo/queries';
import { WORKFLOW_EVENTS, PRICE_AUTHORING_EVENTS, TITLE_MAX_CHAR_LENGTH } from 'utils/constants';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import { formatDateTimeString, getStartOfDayUTC } from 'utils/date';
import PriceEntryContentAuthoring from './PriceEntryContentAuthoring';
import ErrorFallback from 'components/ErrorFallback/ErrorFallback';
import { useUser } from '@icis/app-shell-apis';

interface IPriceEntryCapabilityAuthoring extends CapabilityProps {
  params: CapabilityParams & { piralApi: PiletApi };
}

const PriceEntryCapabilityAuthoring = ({
  params: capabilityParams,
  client,
}: IPriceEntryCapabilityAuthoring & ApolloClientProps) => {
  const [title, setTitle] = useState<string | undefined>('');
  const [showPriceSeriesSelectionCard, setShowPriceSeriesSelectionCard] = useState(false);
  const [selectedDate, setSelectedDate] = useState<Date>(new Date());
  const messages = useLocaleMessages();
  const allowedForContentConfigure = useUser().entitlements.includes('ContentConfigure');
  /** The added reference resolve the issue of old title
    data being sent when there is a change in the table data. **/
  const titleRef = useRef<string | undefined>('');
  const priceEntryParentRef = useRef<HTMLDivElement>(null);

  // Extract lockInformation from capabilityParams
  const { lockInformation } = capabilityParams;
  // query to fetch the content block
  const [getContentBlock, { data, refetch: refetchGridData, loading, error }] =
    useLazyQuery<ContentBlockResponse>(GET_CONTENT_BLOCK, {
      client,
      fetchPolicy: 'no-cache',
    });

  const [updateContentBlock] = useMutation<UpdateContentBlock>(UPDATE_CONTENT_BLOCK, {
    client,
    onCompleted: /* istanbul ignore next*/ ({ response }) => {
      if (response?.isValid) {
        handlePostUpdate(response);
      }
    },
  });

  const [updateUserPreference] = useMutation<UpdateUserPreference>(UPDATE_USER_PREFERENCE, {
    client,
  });

  const [updatePriceSeriesData] = useMutation(UPDATE_PRICE_SERIES_DATA, { client });

  const getConfigValue = (key: string) =>
    capabilityParams?.metadata?.config?.find((item) => item.key === key)?.value;

  /* istanbul ignore next */
  useEffect(() => {
    const isReviewMode = getConfigValue('isReviewMode') === 'true';
    if (!isReviewMode) return;

    const appliesFrom = getConfigValue('appliesFrom');
    const assessedDateTime = getConfigValue('assessedDateTime');

    const dateToUse = assessedDateTime || appliesFrom;

    if (dateToUse) {
      const newDateUTC = new Date(parseFloat(dateToUse));
      const utcDateObj = new Date(newDateUTC.getTime() - newDateUTC.getTimezoneOffset() * -60000);
      const selectedStartOfDayUTC = getStartOfDayUTC(selectedDate);
      const newStartOfDayUTC = getStartOfDayUTC(utcDateObj);

      if (selectedStartOfDayUTC?.getTime() !== newStartOfDayUTC?.getTime()) {
        setSelectedDate(utcDateObj);
      }
    }
  }, [capabilityParams?.metadata?.config]);

  /* istanbul ignore next */
  useEffect(() => {
    /* This effect refetches the grid data to refresh the latest status 
       when the workflow or price authoring status is updated */
    const workflowKey = data?.contentBlock?.workflowBusinessKey;
    const contentBlockId = data?.contentBlock?.contentBlockId;

    if (workflowKey || contentBlockId) {
      const handleStoreData = ({ name, value }: NotifierProps) => {
        if (
          (name === WORKFLOW_EVENTS && value?.Id === workflowKey) ||
          (name === PRICE_AUTHORING_EVENTS &&
            value?.contentBlockIds?.includes(contentBlockId) &&
            value?.assessedDateTime === formatDateTimeString(selectedDate))
        ) {
          refetchGridData();
        }
      };

      capabilityParams?.piralApi?.on('store-data', handleStoreData);

      return () => {
        capabilityParams?.piralApi?.off('store-data', handleStoreData);
      };
    }
  }, [
    capabilityParams,
    data?.contentBlock?.workflowBusinessKey,
    data?.contentBlock?.contentBlockId,
  ]);

  // istanbul ignore next
  const loadContentBlock = useCallback(() => {
    if (selectedDate) {
      getContentBlock({
        variables: {
          contentBlockId: capabilityParams?.id,
          version: Number(capabilityParams?.version),
          assessedDateTime: getStartOfDayUTC(selectedDate),
          includeNotStarted: false,
          isReviewMode: getConfigValue('isReviewMode') === 'true',
        },
      });
    }
  }, [capabilityParams?.id, capabilityParams?.version, selectedDate]);

  // istanbul ignore next
  useEffect(() => {
    if (capabilityParams?.id) {
      loadContentBlock();
    }
  }, [capabilityParams?.id, selectedDate]);

  useEffect(() => {
    /* istanbul ignore else */
    if (data?.contentBlock?.contentBlockId) {
      const contentTitle = data.contentBlock?.title;
      setTitle(contentTitle);
      titleRef.current = contentTitle;
    }
  }, [data?.contentBlock]);

  // Effect to show or hide price series selection card
  // istanbul ignore next
  useEffect(() => {
    setShowPriceSeriesSelectionCard(
      !(
        data?.contentBlock?.priceSeriesGrids &&
        data.contentBlock.priceSeriesGrids.length > 0 &&
        data?.contentBlock?.priceSeriesGrids[0]?.priceSeriesIds &&
        data.contentBlock.priceSeriesGrids[0].priceSeriesIds?.length > 0
      ),
    );
  }, [data]);

  /* istanbul ignore next */
  const handleUpdateContentBlock = useCallback(
    async (
      gridUpdateOrArray:
        | { gridId?: string; priceSeriesIds: string[] }
        | { id?: string; priceSeriesIds: string[] }[],
    ) => {
      const currentGrids = data?.contentBlock?.priceSeriesGrids || [];
      let finalGrids: Array<{ id?: string; priceSeriesIds: string[] }>;

      if (Array.isArray(gridUpdateOrArray)) {
        // If an entire array is passed, use it directly.
        finalGrids = gridUpdateOrArray;
      } else {
        // Otherwise, update the single grid.
        const gridUpdate = gridUpdateOrArray;
        if (gridUpdate.gridId) {
          // Replace the grid that matches gridId.
          finalGrids = currentGrids.map((grid: any) =>
            grid.id === gridUpdate.gridId
              ? { ...grid, priceSeriesIds: gridUpdate.priceSeriesIds }
              : grid,
          );
        } else {
          // Append new grid if no gridId exists.
          finalGrids = [...currentGrids, { priceSeriesIds: gridUpdate.priceSeriesIds }];
        }
      }

      // Send the complete grids list to the backend.
      return updateContentBlock({
        variables: {
          contentBlockInput: {
            title: data?.contentBlock?.title || titleRef.current,
            contentBlockId: capabilityParams?.id,
            priceSeriesGrids: finalGrids.map((grid: any) => ({
              ...(grid.id ? { id: grid.id } : {}),
              title: grid?.title,
              priceSeriesIds: grid.priceSeriesIds,
            })),
          },
        },
      });
    },
    [data, capabilityParams?.id, updateContentBlock],
  );

  // istanbul ignore next
  const handleSaveTitle = useCallback(
    async (updatedTitle: string, gridId?: string) => {
      let updatedGrids;
      if (gridId) {
        const currentGrids = data?.contentBlock?.priceSeriesGrids || [];
        updatedGrids = currentGrids.map((grid: any) =>
          grid.id === gridId ? { ...grid, title: updatedTitle } : grid,
        );
      } else {
        if (data?.contentBlock?.title === updatedTitle.trim()) {
          return;
        }
        updatedGrids = data?.contentBlock?.priceSeriesGrids || [];
      }

      const sanitizedGrids = updatedGrids.map(
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        ({ seriesItemTypeCode, gridConfiguration, priceSeries, __typename, ...rest }) => rest,
      );

      await updateContentBlock({
        variables: {
          contentBlockInput: {
            title: titleRef.current,
            contentBlockId: capabilityParams?.id,
            priceSeriesGrids: sanitizedGrids,
          },
        },
      });

      capabilityParams?.onFinishEditing();
    },
    [data],
  );

  const handleTitleChange = (updatedTitle: string) => {
    if (title === updatedTitle.trim()) return;
    setTitle(updatedTitle);
    titleRef.current = updatedTitle;
    handleSaveTitle(updatedTitle, '');
  };

  /* istanbul ignore next */
  const handlePostUpdate = (updateContentBlockResponse: UpdateContentBlockResponse) => {
    capabilityParams?.onSave(updateContentBlockResponse?.version.toString());
    refetchGridData({
      contentBlockId: capabilityParams?.id,
      version: updateContentBlockResponse?.version,
      assessedDateTime: getStartOfDayUTC(selectedDate),
      includeNotStarted: false,
    });
    capabilityParams?.onFinishEditing();
  };

  /* istanbul ignore next */
  const handlePriceSeriesDataUpdate = useCallback(
    async (payload: SeriesItemInputType, seriesItemTypeCode: string) => {
      const { id, ...rest } = payload;
      const operation = data?.contentBlock?.nextActions?.find((action) => action.name === 'CANCEL')
        ? 'correction'
        : '';
      await updatePriceSeriesData({
        variables: {
          priceItemInput: {
            operationType: operation,
            seriesId: id,
            seriesItemTypeCode,
            assessedDateTime: getStartOfDayUTC(selectedDate),
            seriesItem: {
              ...rest,
            },
          },
        },
      });
    },
    [data],
  );

  // istanbul ignore next
  const handleUpdateUserPreference = useCallback(
    async (params: UpdateUserPreferenceInput) => {
      const { updatedColumnConfigs, selectedSeriesIDs, gridId, refetch } = params;

      const currentGrid = data?.contentBlock?.priceSeriesGrids?.find(
        (g: IPriceSeriesGrid) => g.id === gridId,
      );

      const defaultColumnConfigs: UserPreferenceColumnInput[] =
        updatedColumnConfigs.length === 0
          ? (currentGrid?.gridConfiguration.columns.map((column) => ({
              field: column.field,
              displayOrder: column.displayOrder,
              hidden: column.hidden,
            })) ?? [])
          : [];

      const userPreferenceInput = {
        contentBlockId: capabilityParams?.id,
        columnInput:
          updatedColumnConfigs.length > 0 ? [...updatedColumnConfigs] : defaultColumnConfigs,
        priceSeriesInput:
          selectedSeriesIDs ||
          (currentGrid ? currentGrid.priceSeries.map((series: any) => series.id) : []),
        priceSeriesGridId: gridId,
      };

      await updateUserPreference({
        variables: { userPreferenceInput },
      });

      if (refetch) {
        refetchGridData();
      }
    },
    [data, capabilityParams?.id, updateUserPreference, refetchGridData],
  );

  // istanbul ignore next
  const handleRefetch = (includeNotStarted?: boolean) => {
    if (includeNotStarted) {
      refetchGridData({
        contentBlockId: capabilityParams?.id,
        version: Number(capabilityParams?.version),
        assessedDateTime: getStartOfDayUTC(selectedDate),
        includeNotStarted,
      });
    } else {
      refetchGridData();
    }
  };

  const getEditIconVisibility = () => {
    // istanbul ignore next
    if (allowedForContentConfigure) {
      return true;
    }
    // istanbul ignore next
    if (getConfigValue('isReviewMode') === 'true') {
      return false;
    }
    if (showPriceSeriesSelectionCard) {
      return true;
    }
    return false;
  };

  // istanbul ignore next
  const getReviewURL = () => {
    const getCanvasFormUrl = capabilityParams?.piralApi?.getData('getCanvasFormUrl');
    return '/content/pricing/form/700ff165-c029-450e-b092-4b1c8b5db8d2';
  };
  const isEditNotAllowed = getConfigValue('isReviewMode') === 'true' || !allowedForContentConfigure;

  return (
    <ContentBlockWrapper
      header={{
        characterLimit: TITLE_MAX_CHAR_LENGTH,
        title: title || '',
        placeholder: /* istanbul ignore next */ !isEditNotAllowed
          ? messages.General.ContentBlockTitlePlaceholder
          : '',
        onTitleUpdated: handleTitleChange,
        disabled: isEditNotAllowed,
      }}
      width={'100%'}
      editIconVisibility='hide'
      onEditClick={() => {}}
      ref={priceEntryParentRef}
      testId='price-entry-content-block-wrapper'
      Content={
        /* istanbul ignore next */
        error ? (
          <ErrorFallback onReload={loadContentBlock} />
        ) : (
          <PriceEntryContentAuthoring
            lockInformation={lockInformation}
            isReviewMode={getConfigValue('isReviewMode') === 'true'}
            loading={loading}
            iconVisibility={getEditIconVisibility()}
            showPriceSeriesSelectionCard={showPriceSeriesSelectionCard}
            data={data}
            selectedDate={selectedDate}
            setSelectedDate={setSelectedDate}
            refetchGridData={handleRefetch}
            getReviewURL={getReviewURL}
            onSaveTitle={handleSaveTitle}
            onStartEditing={capabilityParams?.onStartEditing}
            onFinishEditing={capabilityParams?.onFinishEditing}
            handlePriceSeriesDataUpdate={handlePriceSeriesDataUpdate}
            handleUpdateUserPreference={handleUpdateUserPreference}
            handleUpdateContentBlock={handleUpdateContentBlock}
          />
        )
      }
    />
  );
};

export default withClient(PriceEntryCapabilityAuthoring);
