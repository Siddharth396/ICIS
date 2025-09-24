import { memo } from 'react';
import { CustomCellEditorProps } from 'ag-grid-react';
import { StatusLabel, StatusWrapper, Wrapper } from './styled';
import { PRICE_DISPLAY_WORKFLOW_STATUS, WORKFLOW_STATUS } from 'utils/constants';
import { Icon } from '@icis/ui-kit';

export const getStatus = (status: keyof typeof WORKFLOW_STATUS) => {
  const formatStatus = (statusValue: string) =>
    statusValue.includes('_') ? WORKFLOW_STATUS[status] : status;

  switch (status) {
    case WORKFLOW_STATUS.READY_TO_START:
    case PRICE_DISPLAY_WORKFLOW_STATUS.READY_TO_START:
      return (
        <StatusWrapper>
          <Icon icon='movement-nul' size='2x' color='#246CFF' />
          <StatusLabel>{formatStatus(status)}</StatusLabel>
        </StatusWrapper>
      );
    case WORKFLOW_STATUS.READY_TO_PUBLISH:
    case WORKFLOW_STATUS.CORRECTION_READY_TO_PUBLISH:
    case WORKFLOW_STATUS.PUBLISHED:
    case WORKFLOW_STATUS.CORRECTION_PUBLISHED:
    case PRICE_DISPLAY_WORKFLOW_STATUS.READY_TO_PUBLISH:
    case PRICE_DISPLAY_WORKFLOW_STATUS.CORRECTION_READY_TO_PUBLISH:
    case PRICE_DISPLAY_WORKFLOW_STATUS.PUBLISHED:
    case PRICE_DISPLAY_WORKFLOW_STATUS.CORRECTION_PUBLISHED:
      return (
        <StatusWrapper>
          <Icon icon='movement-nul' size='2x' color='#00A972' />
          <StatusLabel>{formatStatus(status)}</StatusLabel>
        </StatusWrapper>
      );
    case WORKFLOW_STATUS.DRAFT:
    case WORKFLOW_STATUS.CORRECTION_DRAFT:
    case WORKFLOW_STATUS.READY_FOR_REVIEW:
    case WORKFLOW_STATUS.CORRECTION_READY_FOR_REVIEW:
    case WORKFLOW_STATUS.IN_REVIEW:
    case WORKFLOW_STATUS.CORRECTION_IN_REVIEW:
    case WORKFLOW_STATUS.SENT_BACK:
    case PRICE_DISPLAY_WORKFLOW_STATUS.DRAFT:
    case PRICE_DISPLAY_WORKFLOW_STATUS.CORRECTION_DRAFT:
    case PRICE_DISPLAY_WORKFLOW_STATUS.READY_FOR_REVIEW:
    case PRICE_DISPLAY_WORKFLOW_STATUS.CORRECTION_READY_FOR_REVIEW:
    case PRICE_DISPLAY_WORKFLOW_STATUS.IN_REVIEW:
    case PRICE_DISPLAY_WORKFLOW_STATUS.CORRECTION_IN_REVIEW:
    case PRICE_DISPLAY_WORKFLOW_STATUS.SENT_BACK:
      return (
        <StatusWrapper>
          <Icon icon='movement-nul' size='2x' color='#FFA600' />
          <StatusLabel>{formatStatus(status)}</StatusLabel>
        </StatusWrapper>
      );
    default:
      // istanbul ignore next
      return status;
  }
};

const PriceSeriesItemStatusRenderer = memo(({ data }: CustomCellEditorProps) => {
  return data?.status ? (
    <Wrapper>
      <span data-testid='price-series-item-status' data-tip data-for={`tooltip-${data?.id}`}>
        {getStatus(data?.status)}
      </span>
    </Wrapper>
  ) : null;
});

export default PriceSeriesItemStatusRenderer;
