import { Col, Icon, Row, Text, theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const SubscriberWrapper = styled.div`
  width: 100%;
`;

export const Title = styled.div`
  display: flex;
  width: 100%;
  position: relative;
  margin-bottom: 1em;
  font-weight: ${theme.fonts.weight.SEMI_BOLD};
  font-size: ${theme.fonts.size.LARGE};
`;

export const ContentBlockWrapper = styled.div`
  width: 100%;
  flex-direction: column;
  display: flex;
`;

export const ValidationMessage = styled(Text.Body)`
  color: ${theme.colours.NEGATIVE};
`;

export const ValidationMessageIcon = styled(Icon)`
  font-size: 1.4em;
  color: ${theme.colours.NEGATIVE};
`;

export const HeaderRow = styled(Row)`
  display: flex;
  justify-content: space-between;
`;

export const RowLeft = styled(Col)`
  margin-right: auto;
  flex-direction: row;
  display: flex;
  justify-content: space-between;
  gap: 10px;
  align-items: center;
`;

export const RowRight = styled(Col)`
  display: flex;
  justify-content: flex-end;
  gap: ${theme.spacing.BASE_2};

  /* At smaller widths, stack items vertically and align them to the right */
  @media (max-width: 768px) {
    flex-direction: column;
    align-items: flex-end;
  }
`;
