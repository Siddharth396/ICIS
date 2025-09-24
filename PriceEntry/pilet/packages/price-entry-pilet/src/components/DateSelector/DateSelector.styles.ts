import { theme } from '@icis/ui-kit';
import styled from 'styled-components';
import { PickerPosition } from './DateSelector.type';

export const DateSelectorContainer = styled.div<{ disabled?: boolean }>`
  display: flex;
  align-items: center;
  height: 32px;
  width: 220px;
  position: relative;
  padding: 0 8px;
  border: 1px solid ${theme.colours.NEUTRALS_4};
  border-radius: ${theme.borderRadius.default};

  ${({ disabled }) =>
    !disabled &&
    `
    &:hover {
      border-color: ${theme.colours.SECONDARY_4};
      svg {
        color: ${theme.colours.SECONDARY_4};
      }
    }
  `}
`;

export const DateContainer = styled.div`
  display: flex;
  width: 100%;
  flex-direction: row;
  flex-wrap: nowrap;
  align-items: center;
  gap: 8px;
  span:nth-child(3) {
    text-align: left;
  }

  &:hover {
    border-color: ${theme.colours.SECONDARY_4};
    svg {
      color: ${theme.colours.SECONDARY_4};
    }
  }
`;

export const DateWrapper = styled.span`
  width: 93px;
  font-family: ${theme.fonts.graphikFonts};
  font-size: 13px;
  font-weight: 400;
  line-height: 16px;
  text-underline-position: from-font;
  text-decoration-skip-ink: none;

  &:hover {
    border-color: ${theme.colours.SECONDARY_4};
    svg {
      color: ${theme.colours.SECONDARY_4};
    }
  }
`;

export const DatePickerContainer = styled.div<{
  position?: PickerPosition;
}>`
  position: absolute;
  z-index: 9999;
  background-color: white;
  border: 1px solid #ccc;
  border-radius: 4px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
  animation: fadeIn 0.3s ease-in-out;

  &:hover {
    border-color: ${theme.colours.SECONDARY_4};
    svg {
      color: ${theme.colours.SECONDARY_4};
    }
  }
  @keyframes fadeIn {
    from {
      opacity: 0;
    }
    to {
      opacity: 1;
    }
  }
  ${({ position }) =>
    position &&
    `
    top: ${position.top}px;
    left: calc(${position.left}px);
  `}
`;

export const IconWrapper = styled.div`
  display: flex;
  height: 100%;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  svg {
    color: ${theme.colours.PRIMARY_3};
    font-size: 16px;
  }
`;
