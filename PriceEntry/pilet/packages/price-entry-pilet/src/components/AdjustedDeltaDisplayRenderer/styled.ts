import { theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const AdjustedDeltaCellWrapper = styled.div`
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;

  p {
    color: ${theme.colours.NEUTRALS_2};
    width: max-content;
    padding-top: 2px;
    padding-right: 4px;
    letter-spacing: 0.5px;
    text-transform: uppercase;
    font-weight: 600;
    font-size: 9px;
    line-height: 140%;
    padding-bottom: 2px;
    padding-left: 4px;
    border-radius: 4px;
    border: 1px solid ${theme.colours.NEUTRALS_2};
  }
`;
export const AdjustedDeltaTooltipContainer = styled.div`
  width: 295px;
  height: 79px;
  border-radius: 4px;
  gap: 10px;
  padding-top: 12px;
  padding-right: 16px;
  padding-bottom: 12px;
  padding-left: 16px;
  background-color: ${theme.colours.PRIMARY_1};
  color: ${theme.colours.WHITE};

  p {
    font-weight: 600;
    font-size: 13px;
    line-height: 140%;
    letter-spacing: 0px;

    span {
      display: block;
      font-weight: 400;
      font-size: 13px;
      line-height: 140%;
      letter-spacing: 0px;
    }
  }
`;
