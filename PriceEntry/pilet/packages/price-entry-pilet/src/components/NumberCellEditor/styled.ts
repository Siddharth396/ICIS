import { Icon, theme } from '@icis/ui-kit';
import styled from 'styled-components';

interface StyledDivProps {
  type: 'rightAligned' | 'leftAligned';
  isFocused?: boolean;
  hasError?: boolean;
}

export const NumberCellEditorWrapper = styled.div<StyledDivProps>`
  input {
    text-align: ${(props) => (props.type === 'rightAligned' ? 'right' : 'left')};
    ${(props) =>
      props.isFocused &&
      `
    border-color: ${props.hasError ? theme.colours.NEGATIVE : theme.colours.SECONDARY_4} !important;
    button {
      color: ${theme.colours.SECONDARY_4} !important;
    }
  `}
  }
`;

export const NumberCellRendererWrapper = styled.div<StyledDivProps>`
  display: flex;
  align-items: center;
  justify-content: ${(props) => {
    if (props.hasError) {
      return 'space-between';
    } else if (props.type === 'rightAligned') {
      return 'flex-end';
    } else {
      return 'flex-start';
    }
  }};
  border: 1px solid
    ${(props) => (props.hasError ? theme.colours.NEGATIVE : theme.colours.NEUTRALS_4)};
  height: 32px;
  min-height: 32px;
  width: 100%;
  padding-left: 10px;
  padding-right: 10px;
  border-radius: 4px;
  overflow: hidden;
  text-overflow: ellipsis;
`;

export const ErrorIcon = styled(Icon)`
  color: ${theme.colours.NEGATIVE};
  font-size: 1.2em;
`;

export const PlaceHolder = styled.div`
  color: ${theme.colours.PLACEHOLDER_TEXT};
  font-size: ${theme.fonts.size.BASE};
`;
