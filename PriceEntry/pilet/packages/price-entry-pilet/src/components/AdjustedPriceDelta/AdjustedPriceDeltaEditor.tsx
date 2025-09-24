// istanbul ignore file
import React, { memo, useState, useCallback } from 'react';
import { CustomCellEditorProps } from 'ag-grid-react';
import { AdjustedPriceDeltaEditorWrapper } from './styled';
import PriceDeltaComponent from 'components/PriceDelta';
import NumberCellEditor from 'components/NumberCellEditor';

const AdjustedPriceDeltaEditor = (props: CustomCellEditorProps) => {
  const [currentValue, setCurrentValue] = useState(props.value);
  // @ts-ignore
  const { editableWhen, editable } = props.columnConfig;

  // This callback updates our local state as the user types.
  const handleLocalValueChange = useCallback((newValue: any) => {
    setCurrentValue(newValue);
  }, []);

  const { customConfig: { priceDelta = {} } = {} } = (props as any)?.columnConfig || {};
  const priceDeltaField = priceDelta.priceDeltaField;
  const updatedData = { ...props.data, [priceDeltaField]: currentValue };
  const [isEditable] = useState(
    editableWhen
      ? props?.data?.[editableWhen.field as keyof typeof props.data] === editableWhen.value
      : editable,
  );
  return (
    isEditable && (
      <AdjustedPriceDeltaEditorWrapper>
        <div className='text-box'>
          <NumberCellEditor
            {...props}
            value={currentValue}
            onValueChange={props.onValueChange}
            onLocalValueChange={handleLocalValueChange}
          />
        </div>
        <div className='percentage-delta'>
          <PriceDeltaComponent
            data={updatedData}
            // @ts-ignore
            columnConfig={props.columnConfig}
            isPriceEntry={true}
            adjustmentDelta={true}
          />
        </div>
      </AdjustedPriceDeltaEditorWrapper>
    )
  );
};

export default memo(AdjustedPriceDeltaEditor);
