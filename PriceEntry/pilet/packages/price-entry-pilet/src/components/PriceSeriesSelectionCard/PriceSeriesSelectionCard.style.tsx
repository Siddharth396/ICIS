import { theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const Wrapper = styled.div`
  display: flex;
  min-width: calc(580px - 32px);
  justify-content: center;
  flex-direction: column;
  align-items: center;
  height: 120px;
  border-radius: 2px;

  button {
    padding: ${theme.spacing.BASE_1} ${theme.spacing.BASE_2};
    background-color: ${theme.colours.CHARTING_6};
  }
`;
