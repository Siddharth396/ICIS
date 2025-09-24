import { Icon, Text, theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const GridWrapper = styled.div`
  position: relative;
  margin-bottom: 16px;
  width: 100%;
`;

export const RightGridActions = styled.div<{ isVisible: boolean }>`
  position: absolute;
  right: -40px;
  display: ${(props) => (props.isVisible ? 'flex' : 'none')};
  z-index: 2;
  transition: opacity 0.2s ease-in-out;
  opacity: ${(props) => (props.isVisible ? 1 : 0)};
`;

export const LeftGridActions = styled.div<{ isVisible: boolean }>`
  position: absolute;
  left: 4px;
  transform: translateX(-40px);
  display: ${(props) => (props.isVisible ? 'flex' : 'none')};
  z-index: 2;
  transition: opacity 0.2s ease-in-out;
  opacity: ${(props) => (props.isVisible ? 1 : 0)};
`;

export const ValidationMessage = styled(Text.Body)`
  color: ${theme.colours.NEGATIVE};
`;

export const ValidationMessageIcon = styled(Icon)`
  font-size: 1.4em;
  color: ${theme.colours.NEGATIVE};
`;

export const Wrapper = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 10px;
  margin-bottom: 10px;
`;

export const ErrorWrapper = styled.div`
  display: flex;
  align-items: center;
`;

export const ButtonWrapper = styled.div`
  display: flex;
  align-items: center;
`;
