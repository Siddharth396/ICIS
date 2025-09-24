import { Icon, Text, theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const ReloadButtonWrapper = styled.div`
  margin-top: ${theme.spacing.BASE_1};
`;

export const ErrorFallbackWrapper = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
`;

export const ErrorFallbackTitle = styled(Text.H5)`
  margin-top: ${theme.spacing.BASE_2};
`;

export const WarningIcon = styled(Icon)`
  color: ${theme.colours.PRIMARY_2};
  font-size: 3.5em;
`;
