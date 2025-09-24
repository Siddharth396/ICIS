// istanbul ignore file
import React, { memo } from 'react';
import { CustomCellEditorProps } from 'ag-grid-react';
import { AdjustedPriceDeltaRendererWrapper } from './styled';
import PriceDeltaComponent from 'components/PriceDelta';
import NumberCellRenderer from 'components/NumberCellEditor/NumberCellRenderer';
import { Column } from 'apollo/queries';

const AdjustedPriceDeltaRenderer = memo((props: CustomCellEditorProps & Column) => {
  return (
    <AdjustedPriceDeltaRendererWrapper>
      <div className='dummy-textbox'>
        <NumberCellRenderer {...props} />
      </div>
      <div className='percentage-delta'>
        <PriceDeltaComponent
          data={props.data}
          // @ts-ignore
          columnConfig={props.columnConfig}
          isPriceEntry={true}
          adjustmentDelta={true}
        />
      </div>
    </AdjustedPriceDeltaRendererWrapper>
  );
});

export default AdjustedPriceDeltaRenderer;
