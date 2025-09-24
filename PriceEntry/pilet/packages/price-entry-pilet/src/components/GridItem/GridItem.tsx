// istanbul ignore file
import React, { useMemo, useRef, useState } from 'react';
import { CanvasIconButton, ContentBlockWrapper, IconKeys } from '@icis/ui-kit';
import { GridApi } from 'ag-grid-community';
import PriceEntryGrid from 'components/PriceEntryGrid/PriceEntryGrid';
import { IPriceSeriesGrid, SeriesItemInputType, UpdateUserPreferenceInput } from 'apollo/queries';
import { AddGridButtonWrapper, ErrorSection } from 'pilet-components/PriceEntryCapability/styled';
import EditColumns from 'pilet-components/PriceEntryCapability/EditColumns';
import AddGridButton from 'components/AddGridButton';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import {
  ButtonWrapper,
  GridWrapper,
  ValidationMessage,
  ValidationMessageIcon,
  Wrapper,
  ErrorWrapper,
  LeftGridActions,
  RightGridActions,
} from './styled';
import { TITLE_MAX_CHAR_LENGTH } from 'utils/constants';
import DeleteConfirmationModal from 'components/DeleteConfirmationModal';

interface GridItemProps {
  grid: IPriceSeriesGrid;
  nmaEnabled: boolean;
  iconVisibility?: boolean;
  isContentLocked: boolean;
  isContentDisabled: boolean;
  onSaveTitle: (title: string, gridId?: string) => Promise<void>;
  onEditGrid: () => void;
  onDeleteGrid: () => void;
  onAddClick: () => void;
  onUpdateUserPreference: (params: UpdateUserPreferenceInput) => void;
  onUpdatePriceSeriesData: (
    payload: SeriesItemInputType,
    seriesItemTypeCode: string,
  ) => Promise<void>;
  onStartEditing: () => void;
  onFinishEditing: () => void;
  selectedDate: Date;
}

const GridItem: React.FC<GridItemProps> = ({
  grid,
  nmaEnabled,
  iconVisibility,
  isContentLocked,
  isContentDisabled,
  onSaveTitle,
  onEditGrid,
  onDeleteGrid,
  onAddClick,
  onUpdateUserPreference,
  onUpdatePriceSeriesData,
  onStartEditing,
  onFinishEditing,
  selectedDate,
}) => {
  const [gridApi, setGridApi] = useState<GridApi | null>(null);
  const [showConfirmationModal, setShowConfirmationModal] = useState(false);
  const titleRef = useRef<string | undefined>('');
  const messages = useLocaleMessages();
  const [isActionsVisible, setIsActionsVisible] = useState(false);
  const isEditNotAllowed = !iconVisibility || isContentLocked || isContentDisabled;
  const timeoutRef = useRef<NodeJS.Timeout | null>(null);

  const showValidationMessage = useMemo(() => {
    if (!grid?.priceSeries) return false;
    return grid.priceSeries.some(
      (item) => item.validationErrors && Object.keys(item.validationErrors).length > 0,
    );
  }, [grid]);

  const handleMouseEnter = () => {
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current);
    }
    setIsActionsVisible(true);
  };

  const handleMouseLeave = () => {
    timeoutRef.current = setTimeout(() => {
      setIsActionsVisible(false);
    }, 200); // Small delay before hiding
  };

  const renderValidationMessage = () => (
    <>
      <ValidationMessageIcon icon={IconKeys.warning} />
      <ValidationMessage>{messages.Workflow.GenericError}</ValidationMessage>
    </>
  );

  const handleTitleChange = (updatedTitle: string) => {
    if (grid?.title === updatedTitle.trim()) return;
    titleRef.current = updatedTitle;
    onSaveTitle?.(updatedTitle, grid?.id);
  };

  const handleDeleteClick = () => {
    setShowConfirmationModal(true);
  };

  const handleDeleteConfirm = () => {
    setShowConfirmationModal(false);
    onDeleteGrid?.();
  };

  const handleDeleteModalClose = () => {
    setShowConfirmationModal(false);
  };

  return (
    <>
      <ContentBlockWrapper
        header={{
          characterLimit: TITLE_MAX_CHAR_LENGTH,
          title: grid?.title || '',
          placeholder: /* istanbul ignore next */ !isEditNotAllowed
            ? messages.General.GridTitlePlaceholder
            : '',
          disabled: isEditNotAllowed,
          onTitleUpdated: handleTitleChange,
        }}
        width={'100%'}
        editIconVisibility='hide'
        onEditClick={() => {}}
        testId='price-entry-content-block-grid-wrapper'
        Content={
          <GridWrapper onMouseEnter={handleMouseEnter} onMouseLeave={handleMouseLeave}>
            <Wrapper>
              <ErrorWrapper>
                {showValidationMessage && (
                  <ErrorSection data-testid='validation-message'>
                    {renderValidationMessage()}
                  </ErrorSection>
                )}
              </ErrorWrapper>
              <ButtonWrapper>
                <EditColumns
                  setGridApi={setGridApi}
                  disabled={isContentDisabled || isContentLocked}
                  columns={grid?.gridConfiguration?.columns}
                  // @ts-ignore
                  onUpdateUserPreference={onUpdateUserPreference}
                />
              </ButtonWrapper>
            </Wrapper>
            {iconVisibility && isActionsVisible && (
              <LeftGridActions isVisible={isActionsVisible}>
                <CanvasIconButton icon='delete' onClick={handleDeleteClick} />
              </LeftGridActions>
            )}
            {iconVisibility && isActionsVisible && (
              <RightGridActions isVisible={isActionsVisible}>
                <CanvasIconButton icon='edit' onClick={onEditGrid} />
              </RightGridActions>
            )}
            <PriceEntryGrid
              nmaEnabled={nmaEnabled}
              isContentLocked={isContentLocked}
              data={grid}
              gridApi={gridApi}
              setGridApi={setGridApi}
              onStartEditing={onStartEditing}
              onFinishEditing={onFinishEditing}
              onUpdatePriceSeriesData={onUpdatePriceSeriesData}
              onUpdateUserPreference={onUpdateUserPreference}
              selectedDate={selectedDate}
            />
            <DeleteConfirmationModal
              show={showConfirmationModal}
              title={messages.DeleteGridModal.Title}
              deleteConfirmationText={[
                messages.DeleteGridModal.AreYouSure,
                messages.DeleteGridModal.CantBeUndone,
              ]}
              testId='delete-content-block-modal'
              onClose={handleDeleteModalClose}
              onDelete={handleDeleteConfirm}
            />
          </GridWrapper>
        }
      />
      {iconVisibility && (
        <AddGridButtonWrapper>
          <AddGridButton onAddClick={onAddClick} />
        </AddGridButtonWrapper>
      )}
    </>
  );
};

export default GridItem;
