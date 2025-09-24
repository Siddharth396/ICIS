import { useState, useCallback, useEffect } from 'react';
import { Icon, CheckBox } from '@icis/ui-kit';
import { GridApi } from 'ag-grid-community';
import {
  Wrapper,
  EditColumnsWrapper,
  ButtonWrapper,
  CheckboxItem,
  CheckboxLabel,
  CheckboxHolder,
} from './styled';
import { FIELDS } from 'components/PriceEntryGrid/priceEntryGrid.utils';
import { Column, UpdateUserPreferenceInput, UserPreferenceColumnInput } from 'apollo/queries';
import { useClickOutsideListenerRef } from 'utils/hooks/useClickOutsideListenerRef';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';

type IEditColumns = {
  isPriceDisplay?: boolean;
  disabled?: boolean;
  columns: Column[];
  setGridApi: (gridApi: GridApi) => void;
  onUpdateUserPreference: (params: UpdateUserPreferenceInput | UserPreferenceColumnInput[]) => void;
};

const EditColumns = ({
  isPriceDisplay,
  disabled,
  columns,
  setGridApi,
  onUpdateUserPreference,
}: IEditColumns) => {
  // Initialize local state from incoming columns prop.
  const [localColumns, setLocalColumns] = useState<UserPreferenceColumnInput[]>(
    columns.map((col) => ({
      field: col.field,
      displayOrder: col.displayOrder,
      hidden: col.hidden,
    })),
  );
  const [isEditing, setIsEditing] = useState(false);
  const messages = useLocaleMessages();

  const popupRef = useClickOutsideListenerRef(() => {
    setTimeout(() => {
      setIsEditing(false);
    });
  });

  // Sync localColumns if the prop changes.
  useEffect(() => {
    setLocalColumns(
      columns.map((col) => ({
        field: col.field,
        displayOrder: col.displayOrder,
        hidden: col.hidden,
      })),
    );
  }, [columns]);

  // istanbul ignore next
  const handleColumnVisibilityChange = useCallback(
    (columnId: string, isVisible: boolean) => {
      // @ts-ignore
      setGridApi((prevGridApi: GridApi) => {
        if (prevGridApi) {
          const columnState = localColumns.map((column) => ({
            colId: column.field,
            hide: column.field === columnId ? !isVisible : column.hidden,
          }));
          prevGridApi.applyColumnState({
            state: columnState,
            applyOrder: true,
          });
          prevGridApi.sizeColumnsToFit();
        }
        return prevGridApi;
      });
    },
    [localColumns, setGridApi],
  );

  // istanbul ignore next
  const onItemSelectionChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>, selectedColumnId: string) => {
      const { checked } = e.target;
      const filteredColumns = localColumns.filter(
        (column) => column.field !== FIELDS.LINKED_PRICES,
      );
      // Update local state
      const updatedColumns = filteredColumns.map((column) =>
        column.field === selectedColumnId ? { ...column, hidden: !checked } : column,
      );
      setLocalColumns(updatedColumns);
      handleColumnVisibilityChange(selectedColumnId, checked);

      // Call the parent's callback. Use object shape if not price display; else, pass the array.
      if (isPriceDisplay) {
        onUpdateUserPreference(updatedColumns);
      } else {
        onUpdateUserPreference({ updatedColumnConfigs: updatedColumns });
      }
    },
    [localColumns, handleColumnVisibilityChange, onUpdateUserPreference, isPriceDisplay],
  );

  return (
    <Wrapper>
      <ButtonWrapper
        data-testid='edit-column-button'
        isActive={isEditing}
        disabled={disabled}
        onClick={() => setIsEditing(true)}>
        <Icon icon='table' />
        <span>{messages.Capabilty.ShowHideButtonTitle}</span>
      </ButtonWrapper>
      {isEditing && (
        <EditColumnsWrapper data-testid='edit-columns-wrapper' ref={popupRef}>
          {localColumns
            .filter((column) => column.field !== FIELDS.LINKED_PRICES)
            .sort((a, b) => a.displayOrder - b.displayOrder)
            .map(({ field, hidden }) => {
              // Retrieve headerName and hideable flag from the original columns
              const originalColumn = columns.find((col) => col.field === field);
              return (
                <CheckboxHolder
                  key={field}
                  data-testid={`${field}-holder`}
                  disabled={!originalColumn?.hideable}>
                  <CheckboxItem>
                    <CheckBox
                      data-testid={`${field}-checkbox`}
                      value={field}
                      checked={!hidden}
                      onChange={(e) => {
                        // istanbul ignore next
                        if (!originalColumn?.hideable) {
                          e.preventDefault();
                          return;
                        }
                        onItemSelectionChange(e, field);
                      }}
                    />
                  </CheckboxItem>
                  <CheckboxLabel>{originalColumn?.headerName}</CheckboxLabel>
                </CheckboxHolder>
              );
            })}
        </EditColumnsWrapper>
      )}
    </Wrapper>
  );
};

export default EditColumns;
