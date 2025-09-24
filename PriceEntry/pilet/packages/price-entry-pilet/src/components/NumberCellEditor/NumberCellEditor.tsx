import React, { memo } from 'react';
import { CustomCellEditorProps } from 'ag-grid-react';
import { TextBox } from '@icis/ui-kit';
import { useFocus } from 'utils/hooks/useFocus';
import { NumberCellEditorWrapper } from './styled';
import { PRICE_DECIMAL_PRECISION } from '../PriceEntryGrid/priceEntryGrid.utils';

interface INumberCellEditorProps extends CustomCellEditorProps {
  onLocalValueChange?: (newValue: string) => void;
}

const NumberCellEditor = memo((props: INumberCellEditorProps) => {
  const {
    value,
    colDef: { cellEditorParams, field },
    data,
    eventKey,
    cellStartedEdit,
    onValueChange, // AG Grid API callback
    onLocalValueChange, // our additional callback for local updates
  } = props;

  const regex = new RegExp(`^-?[0-9]*\\.?[0-9]{0,${PRICE_DECIMAL_PRECISION}}$`);

  const hasError = !!data?.validationErrors?.[field as keyof typeof data.validationErrors];

  const [isEditable] = React.useState(
    cellEditorParams?.editableWhen
      ? data?.[cellEditorParams.editableWhen.field as keyof typeof data] ===
          cellEditorParams.editableWhen.value
      : cellEditorParams?.editable,
  );

  const [refDiv, isFocused] = useFocus(value, eventKey, cellStartedEdit);

  // istanbul ignore next
  const handleChange = (val: string) => {
    if (regex.test(val)) {
      // Call AG Grid's API to update the grid model
      onValueChange?.(val);
      // Also call the additional callback to update local state (if provided)
      onLocalValueChange?.(val);
    }
  };

  return isEditable || isFocused ? (
    <NumberCellEditorWrapper
      type={cellEditorParams?.type}
      isFocused={!!isFocused}
      hasError={hasError}
      ref={refDiv as React.RefObject<HTMLDivElement>}>
      <TextBox
        disabled={data?.readOnly}
        value={value === 0 ? '0' : value || ''}
        pattern={regex}
        error={hasError}
        onChange={handleChange}
      />
    </NumberCellEditorWrapper>
  ) : (
    value
  );
});

export default NumberCellEditor;
