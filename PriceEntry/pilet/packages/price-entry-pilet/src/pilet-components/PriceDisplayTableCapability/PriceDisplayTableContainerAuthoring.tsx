import { useCallback, useEffect, useRef, useState } from 'react';
import { ContentBlockWrapper } from '@icis/ui-kit';
import { CapabilityParams, CapabilityProps, PiletApi } from '@icis/app-shell';
import withClient, { ApolloClientProps } from 'components/HOCs/withClient';
import { useLazyQuery, useMutation } from '@apollo/client';
import {
  GET_PRICE_DISPLAY_CONTENT_BLOCK,
  PriceDisplayContentBlockResponse,
  RowInputPriceDisplayTable,
  UPDATE_PRICE_DISPLAY_CONTENT_BLOCK,
  UpdateContentBlock,
  UserPreferenceColumnInput,
  SelectedFilterInput,
} from 'apollo/queries';
import { TITLE_MAX_CHAR_LENGTH } from 'utils/constants';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import PriceDisplayContentAuthoring from './PriceDisplayTableContentAuthoring';

interface IPriceDisplayCapabilityAuthoring extends CapabilityProps {
  params: CapabilityParams & { piralApi: PiletApi };
}

const PriceDisplayContainerAuthoring = ({
  params: capabilityParams,
  client,
}: IPriceDisplayCapabilityAuthoring & ApolloClientProps) => {
  const [title, setTitle] = useState<string | undefined>('');
  const [showPriceSeriesSelectionCard, setshowPriceSeriesSelectionCard] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [contentBlock, setContentBlock] = useState<PriceDisplayContentBlockResponse>();
  const [editMode, setEditMode] = useState<boolean>(false);
  const messages = useLocaleMessages();

  /** The added reference resolve the issue of old title
    data being sent when there is a change in the table data. **/
  const titleRef = useRef<string | undefined>('');
  const priceDisplayParentRef = useRef<HTMLDivElement>(null);

  // query to fetch the content block
  const [getContentBlock, { data, refetch: refetchGridData, loading }] =
    useLazyQuery<PriceDisplayContentBlockResponse>(GET_PRICE_DISPLAY_CONTENT_BLOCK, {
      client,
      fetchPolicy: 'no-cache',
    });

  const [updateContentBlock] = useMutation<UpdateContentBlock>(UPDATE_PRICE_DISPLAY_CONTENT_BLOCK, {
    client,
    onCompleted: /* istanbul ignore next*/ (data) => handlePostUpdate(data),
  });

  useEffect(() => {
    /* istanbul ignore else */
    getContentBlock({
      variables: {
        contentBlockId: capabilityParams?.id,
        version: Number(capabilityParams?.version),
      },
    });
  }, [capabilityParams?.id]);

  useEffect(() => {
    /* istanbul ignore next */
    if (data?.contentBlockForDisplay?.contentBlockId) {
      const contentTitle = data.contentBlockForDisplay?.title;
      setTitle(contentTitle);
      titleRef.current = contentTitle;
      setContentBlock({
        ...data,
        contentBlockForDisplay: {
          ...data.contentBlockForDisplay,
          gridConfiguration: data.contentBlockForDisplay.priceDisplayGridConfiguration,
          priceSeries: data.contentBlockForDisplay.priceSeriesItemForDisplay,
          selectedFilters: data.contentBlockForDisplay.selectedFilters,
        },
      });
    }
  }, [data?.contentBlockForDisplay]);

  useEffect(() => {
    setshowPriceSeriesSelectionCard(!data?.contentBlockForDisplay?.priceSeriesItemForDisplay);
  }, [data]);

  /* istanbul ignore next */
  const handleUpdateContentBlock = useCallback(
    async (
      selectedSeries?: RowInputPriceDisplayTable[],
      displayColumns?: UserPreferenceColumnInput[],
      savedFilter?: SelectedFilterInput,
    ) => {
      const savedFilters = savedFilter ?? contentBlock?.contentBlockForDisplay.selectedFilters
      await updateContentBlock({
        variables: {
          contentBlockInput: {
            title: titleRef.current,
            contentBlockId: capabilityParams?.id,
            rows: (selectedSeries ?? contentBlock?.contentBlockForDisplay?.rows ?? []).map(
              (series) => ({
                displayOrder: series.displayOrder,
                priceSeriesId: series.priceSeriesId,
                seriesItemTypeCode: series.seriesItemTypeCode,
              }),
            ),
            columns: (displayColumns ?? contentBlock?.contentBlockForDisplay?.columns ?? []).map(
              (columns) => ({
                displayOrder: columns.displayOrder,
                field: columns.field,
                hidden: columns.hidden,
              }),
            ),
            selectedFilters: {
              isInactiveIncluded: savedFilters?.isInactiveIncluded,
              selectedAssessedFrequencies: savedFilters?.selectedAssessedFrequencies,
              selectedCommodities: savedFilters?.selectedCommodities,
              selectedPriceCategories: savedFilters?.selectedPriceCategories,
              selectedRegions: savedFilters?.selectedRegions,
              selectedTransactionTypes: savedFilters?.selectedTransactionTypes,
            },
          },
        },
      });
    },
    [contentBlock],
  );

  const handleSaveTitle = useCallback(
    async (updatedTitle: string) => {
      /* istanbul ignore next */
      if (contentBlock?.contentBlockForDisplay?.title === updatedTitle.trim()) {
        return;
      }
      handleUpdateContentBlock();
      capabilityParams?.onFinishEditing();
    },
    [contentBlock],
  );

  const handleTitleChange = (updatedTitle: string) => {
    setTitle(updatedTitle);
    titleRef.current = updatedTitle;
    handleSaveTitle(updatedTitle);
  };

  /* istanbul ignore next */
  const handlePostUpdate = (updateContentBlock: UpdateContentBlock) => {
    capabilityParams?.onSave(updateContentBlock?.response?.version.toString());
    refetchGridData({
      contentBlockId: capabilityParams?.id,
      version: updateContentBlock?.response?.version,
    });
    capabilityParams?.onFinishEditing();
  };

  // istanbul ignore next
  const handleUpdateUserPreference = useCallback(
    async (updatedColumnConfigs: UserPreferenceColumnInput[], selectedSeriesIDs?: string[]) => {
      let defaultColumnConfigs: UserPreferenceColumnInput[] = [];
      if (updatedColumnConfigs?.length === 0) {
        defaultColumnConfigs =
          contentBlock?.contentBlockForDisplay.priceDisplayGridConfiguration.columns.map(
            (column) => ({
              field: column.field,
              displayOrder: column.displayOrder,
              hidden: column.hidden,
            }),
          ) ?? [];
      }

      const rows =
        contentBlock?.contentBlockForDisplay?.rows
          ?.map((series) => ({
            ...series,
            displayOrder: selectedSeriesIDs?.indexOf(series.priceSeriesId) ?? series.displayOrder,
          }))
          .sort((x, y) => x.displayOrder - y.displayOrder) ?? [];

      const columns =
        updatedColumnConfigs?.length > 0 ? [...updatedColumnConfigs] : defaultColumnConfigs;

      handleUpdateContentBlock(rows, columns, contentBlock?.contentBlockForDisplay.selectedFilters);
    },
    [contentBlock],
  );

  return (
    <ContentBlockWrapper
      header={{
        characterLimit: TITLE_MAX_CHAR_LENGTH,
        title: title || '',
        placeholder: messages.General.TitlePlaceholder,
        onTitleUpdated: handleTitleChange,
      }}
      width={'100%'}
      editIconVisibility={showPriceSeriesSelectionCard ? 'hide' : 'auto'}
      onEditClick={() => {
        setShowModal(true);
        setEditMode(true);
      }}
      ref={priceDisplayParentRef}
      testId='price-display-content-block-wrapper'
      Content={
        <PriceDisplayContentAuthoring
          loading={loading}
          showPriceSeriesSelectionCard={showPriceSeriesSelectionCard}
          data={contentBlock}
          showModal={showModal}
          setShowModal={setShowModal}
          onStartEditing={capabilityParams?.onStartEditing}
          onFinishEditing={capabilityParams?.onFinishEditing}
          handleUpdateUserPreference={handleUpdateUserPreference}
          handleUpdateContentBlock={handleUpdateContentBlock}
          selectedFiltersData={data?.contentBlockForDisplay?.selectedFilters}
          editMode={editMode}
        />
      }
    />
  );
};

export default withClient(PriceDisplayContainerAuthoring);
