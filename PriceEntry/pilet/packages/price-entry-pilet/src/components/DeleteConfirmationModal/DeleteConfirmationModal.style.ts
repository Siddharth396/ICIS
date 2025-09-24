import { theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const MergeWrapper = styled.div`
  display: flex;
  flex-direction: column;
  gap: ${theme.spacing.BASE_2};
  align-items: center;
`;

export const MergeSelectWrapper = styled.div`
  width: 216px;
`;

export const MessagesWrapper = styled.div`
  margin-top: ${theme.spacing.BASE_2};
  text-align: center;

  > div:not(:first-child) {
    margin-top: ${theme.spacing.BASE_3};
  }
`;

export const Wrapper = styled.div`
  display: flex;
  justify-content: center;
  width: 100%;
`;

export const Container = styled.div`
  display: flex;
  justify-content: center;
  flex-direction: column;
  width: 335px;
  height: 263px;
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

export const IconWrapper = styled.div`
  position: relative;
  margin: 0 auto;
`;
