// istanbul ignore file
import { theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const AppWrapper = styled.div`
  width: 100%;
  background-color: ${theme.colours.WHITE};
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  padding: ${theme.spacing.BASE_2};
`;

export const SubscriberWrapper = styled.div`
  display: contents;
`;
