import { memo, useState } from 'react';
import { CustomCellEditorProps } from 'ag-grid-react';
import { ErrorIcon, NumberCellRendererWrapper, PlaceHolder } from './styled';
import { placeHolderValue } from '../PriceEntryGrid/priceEntryGrid.utils';

const NumberCellRenderer = memo(
  ({ value, colDef: { cellEditorParams, field }, data }: CustomCellEditorProps) => {
    const hasError = data?.validationErrors?.[field as keyof typeof data.validationErrors];

    const [isEditable] = useState(
      cellEditorParams?.editableWhen
        ? data?.[cellEditorParams?.editableWhen.field as keyof typeof data] ===
            cellEditorParams?.editableWhen.value
        : cellEditorParams?.editable,
    );

    return isEditable ? (
      <NumberCellRendererWrapper
        data-testid='text-box'
        type={cellEditorParams?.type}
        hasError={hasError}>
        {hasError && <ErrorIcon icon='warning' />}
        {value === null || value === undefined ? (
          <PlaceHolder>{placeHolderValue(data, field)}</PlaceHolder>
        ) : (
          value
        )}
      </NumberCellRendererWrapper>
    ) : (
      value
    );
  },
);

export default NumberCellRenderer;
