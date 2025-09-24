import { Position, theme } from '@icis/ui-kit';
import styled from 'styled-components';

export interface IStyledDateInputProps {
  error?: boolean;
  disabled?: boolean;
}

export const StyledDateInput = styled.input<IStyledDateInputProps>`
  width: 90px;
  font-family: 'Graphik Web', open-sans, sans-serif;
  background: transparent;
  color: inherit;
  border: none;
  &:focus {
    outline: none;
  }

  ${(props) =>
    props.disabled
      ? `
      ::placeholder,
      ::-webkit-input-placeholder {
        color: ${theme.colours.NEUTRALS_4};
      }
      :-ms-input-placeholder {
        color: ${theme.colours.NEUTRALS_4};
      }
      `
      : ` &:active {
        background-color: ${theme.colours.PRIMARY_1};
        color: ${theme.colours.WHITE};
        border-color: ${theme.colours.NEUTRALS_1};`};
`;

export const Wrapper = styled.div<IStyledDateInputProps>`
  background-color: ${theme.colours.WHITE};
  display: flex;
  align-items: center;
  flex-direction: row;
  justify-content: space-around;
  border: 1px solid ${theme.colours.NEUTRALS_3};
  border-radius: 4px;
  color: ${theme.colours.NEUTRALS_1};
  padding: 8px;
  width: 145px;

  ${(props) =>
    props.disabled
      ? `
        color: ${theme.colours.NEUTRALS_4};
        border-color: ${theme.colours.NEUTRALS_4};
        cursor:auto;
      `
      : `
        cursor:pointer;
        &:active {
          background-color: ${theme.colours.PRIMARY_1};
          color: ${theme.colours.WHITE};
          border-color: ${theme.colours.NEUTRALS_1};
        }`}

  ${(props) =>
    props.error
      ? `
        border-color: ${theme.colours.ERROR};
        color: ${theme.colours.ERROR};
    `
      : ''}
`;

export const Spinner = styled.div`
  position: absolute;
  top: 50%;
  left: calc(50% - 16px);
`;

export const DayPickerContentV2 = styled.div<{ position: Position }>`
  position: absolute;
  z-index: 2;
  ${({ position }) => position};
`;

export const FooterStyle = styled.div`
  & > button {
    width: 100%;
  }
`;
