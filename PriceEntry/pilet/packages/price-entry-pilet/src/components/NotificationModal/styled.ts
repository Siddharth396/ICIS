import { Text, theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const Container = styled.div`
  display: flex;
  width: 100%;
  flex-direction: column;
  padding: 62px 28px 36px 25px;
  text-align: center;
`;

export const MessagesWrapper = styled.div`
  display: flex;
  flex-direction: column;
  gap: ${theme.spacing.BASE_4};
`;

export const IconContainer = styled.div`
  margin-bottom: 23px;
  svg {
    width: 48px;
    height: 50px;
  }
`;

export const CustomIconContainer = styled.div`
  margin-bottom: 23px;
`;

export const MessageBody = styled(Text.Body)`
  font-size: 16px;
  line-height: 22.4px;
`;

export const Footer = styled.div`
  display: flex;
  width: 100%;
  justify-content: end;

  button {
    margin-left: ${theme.spacing.BASE_2};

    &:first-child {
      margin-left: 0;
    }
  }
`;
