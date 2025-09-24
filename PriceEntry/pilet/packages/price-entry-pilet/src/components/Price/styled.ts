import { theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const IconStyle = {
  fontSize: '17px',
  width: '16px',
  marginRight: '2px',
  marginBottom: '6px',
};

export const PriceContainer = styled.div`
  display: flex;
  align-items: center;
  gap: 4px;
`;

export const CorrectionContainer = styled.div`
  background-color: ${theme.colours.PRIMARY_1};
  border-radius: 4px;
  word-break: break-word;
  white-space: pre-wrap;
  width: 240px;
  min-height: 61px;
  gap: 10px;
  line-height: 18.2px;
  font-size: 13px;
  font-weight: 400;
  padding-top: 12px;
  padding-right: 16px;
  padding-bottom: 12px;
  font-family: ${theme.fonts.graphikFonts};
  padding-left: 16px;
  color: ${theme.colours.WHITE};
  span {
    font-weight: 700;
    font-size: 13px;
  }
`;
