import React, { memo } from 'react';
import { createPortal } from 'react-dom';
import { IconKeys, Tooltip } from '@icis/ui-kit';
import { CustomCellEditorProps } from 'ag-grid-react';
import {
  CustomTooltip,
  DropDownIndicator,
  InfoIcon,
  SelectRenderer,
  SelectRendererTitle,
} from './styled';

// istanbul ignore next
const TooltipWithPortal = (props: any) => {
  return createPortal(
    <Tooltip {...props} />,
    document.body, // render into body
  );
};

// This component is used to render the value from the select cell editor.
// This is just to give an indication to the user that the cell is editable.
const SelectCellRenderer: React.FC<CustomCellEditorProps> = memo(
  ({ value, colDef: { field }, data }) => {
    const haserror = data?.validationErrors?.[field as keyof typeof data.validationErrors]
      ? true
      : undefined;

    // istanbul ignore next
    const renderTooltip = (
      <CustomTooltip>
        <p>
          <span style={{ fontWeight: 'bold' }}>Price Series</span> -{' '}
          {data?.referencePrice?.seriesName}{' '}
          {data?.referencePrice?.periodLabel ? `- ${data?.referencePrice?.periodLabel}` : ''}
        </p>
        {data?.referencePrice?.price && (
          <p>
            <span style={{ fontWeight: 'bold' }}>Price</span> - {data?.referencePrice?.price}
          </p>
        )}
      </CustomTooltip>
    );

    return (
      <SelectRenderer haserror={haserror}>
        {
          /* istanbul ignore next */ field === 'referencePrice' &&
            !haserror &&
            data?.referencePrice?.price && (
              <>
                <InfoIcon icon='warning' data-for={`tooltip-${data?.id}`} data-tip />
                <TooltipWithPortal id={`tooltip-${data?.id}`} place='right'>
                  {renderTooltip}
                </TooltipWithPortal>
              </>
            )
        }
        <SelectRendererTitle>{value}</SelectRendererTitle>
        <div style={{ marginLeft: 'auto' }}>
          <DropDownIndicator icon={IconKeys.chevronDown} haserror={haserror} />
        </div>
      </SelectRenderer>
    );
  },
);

export default SelectCellRenderer;
