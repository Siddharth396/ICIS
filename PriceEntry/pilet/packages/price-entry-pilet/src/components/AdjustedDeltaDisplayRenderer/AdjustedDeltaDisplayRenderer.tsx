import { Tooltip } from '@icis/ui-kit';
import { createPortal } from 'react-dom';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import { PRICE_DELTA_TYPES } from 'utils/constants';
import { CustomCellEditorProps } from 'ag-grid-react';
import { AdjustedDeltaCellWrapper, AdjustedDeltaTooltipContainer } from './styled';

const getAdjustedDeltaTooltip = (id: string, label: string, TooltipMessage: string) => {
  return (
    <>
      {createPortal(
        <Tooltip id={`tooltip${id}`} data-testid={`tooltip${id}`} effect='solid' arrowColor='black'>
          <AdjustedDeltaTooltipContainer>
            <p>
              {label}
              <span>{TooltipMessage}</span>
            </p>
          </AdjustedDeltaTooltipContainer>
        </Tooltip>,
        document.body,
      )}
    </>
  );
};

const AdjustedDeltaDisplayRenderer = ({ data }: CustomCellEditorProps) => {
  // istanbul ignore next
  const { priceDeltaType = null, id } = data;
  const messages = useLocaleMessages();

  if (priceDeltaType === PRICE_DELTA_TYPES.REGULAR || priceDeltaType === null) return <></>;

  return (
    <AdjustedDeltaCellWrapper>
      <p data-for={`tooltip${id}`} data-tip>
        {messages.Workflow.StaticLabels.NonMarketAdjustments.Label}
      </p>
      {getAdjustedDeltaTooltip(
        id,
        messages.Workflow.StaticLabels.NonMarketAdjustments.NmaApplied,
        messages.Workflow.StaticLabels.NonMarketAdjustments.TooltipMessage,
      )}
    </AdjustedDeltaCellWrapper>
  );
};

export default AdjustedDeltaDisplayRenderer;
