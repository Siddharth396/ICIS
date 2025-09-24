import { theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const TooltipWrapper = styled.div`
  width: auto;
  padding: 5px;
  background-color: ${theme.colours.WHITE};
  border: 1px solid ${theme.colours.PRIMARY_5};
`;

export const TooltipText = styled.div`
  margin: 5px;
  white-space: nowrap;
`;
