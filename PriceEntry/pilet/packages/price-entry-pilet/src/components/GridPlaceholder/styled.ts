import { Text, theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const PlaceholderWrapper = styled.div`
  max-width: 320px;
  margin: 0 auto;
  padding-top: 30px;
  text-align: center;
`;

export const PlaceholderIcon = styled.div`
  display: block;
  margin: 0 auto 10px;
`;

export const PlaceholderMessage = styled(Text.Body)`
  text-align: center;
  color: ${theme.colours.PRIMARY_3};
  font-weight: ${theme.fonts.weight.REGULAR};
  font-size: ${theme.fonts.size.MEDIUM};
`;
