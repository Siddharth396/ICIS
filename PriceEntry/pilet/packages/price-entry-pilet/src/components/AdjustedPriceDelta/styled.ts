import styled from 'styled-components';
import { theme, Icon } from '@icis/ui-kit';

export const AdjustedPriceDeltaRendererWrapper = styled.div`
  display: flex;
  width: 100%;

  .dummy-textbox,
  .percentage-delta {
    flex: 1;
    display: flex;
  }

  .dummy-textbox {
    flex: 0 0 100px;
    border-radius: 4px;
    min-height: 32px;
    justify-content: flex-end;
    align-items: center;
  }

  .percentage-delta {
    justify-content: flex-end;
  }
`;

export const AdjustedPriceDeltaEditorWrapper = styled.div`
  display: flex;
  width: 100%;
  .text-box,
  .percentage-delta {
    flex: 1;
    display: flex;
  }

  .text-box {
    flex: 0 0 100px;

    border-radius: 4px;
    min-height: 32px;
    justify-content: flex-end;
  }

  .percentage-delta {
    justify-content: flex-end;
  }
`;

export const ErrorIcon = styled(Icon)`
  color: ${theme.colours.NEGATIVE};
  font-size: 1.2em;
  margin-right: 4px;
`;
