import { theme, Text } from '@icis/ui-kit';
import styled from 'styled-components';

export const Wrapper = styled.div`
  display: flex;
  position: relative;
`;

export const ButtonWrapper = styled.div<{ disabled?: boolean; isActive: boolean }>`
  display: flex;
  justify-content: center;
  align-items: center;
  font-weight: ${theme.fonts.weight.SEMI_BOLD};
  font-size: 12px;
  border: 1px solid ${theme.colours.NEUTRALS_4};
  border-radius: ${theme.borderRadius.default};
  height: 32px;
  padding: 0 ${theme.spacing.BASE_2};
  background: ${theme.colours.WHITE};
  cursor: pointer;
  white-space: nowrap;
  svg {
    margin-right: 10px;
  }

  ${({ isActive }) =>
    isActive &&
    `
    border-color: ${theme.colours.SECONDARY_4};
    svg {
      color: ${theme.colours.SECONDARY_4};
    }
  `};

  ${({ disabled }) =>
    disabled &&
    `
    pointer-events: none;
    background: ${theme.colours.NEUTRALS_5};
  `};
`;

export const EditColumnsWrapper = styled.div`
  display: flex;
  flex-direction: column;
  position: absolute;
  right: 0;
  top: 34px;
  z-index: 4;
  width: 216px;
  max-height: 300px;
  overflow-y: auto;
  background: ${theme.colours.WHITE};
  border: 1px solid ${theme.colours.SECONDARY_4};
  border-radius: ${theme.borderRadius.default};
`;

export const CheckboxHolder = styled.label<{ disabled: boolean }>`
  opacity: 1;
  display: flex;
  align-items: center;
  min-height: 40px;
  padding: 0 ${theme.spacing.BASE_2};
  cursor: pointer;

  :hover {
    background: ${theme.colours.NEUTRALS_7};
    input:checked ~ .checkmark {
      background: ${theme.colours.NEUTRALS_1};
    }
  }

  ${({ disabled }) =>
    disabled &&
    `
    opacity: 0.5;
    pointer-events: none;

    input:checked ~ .checkmark {
      background: ${theme.colours.BUTTON_DISABLED};
      border: none;
    }
    input:checked ~ .checkmark svg {
      color: ${theme.colours.NEUTRALS_4};
    }
  `};
`;

export const CheckboxItem = styled.div`
  svg {
    color: ${theme.colours.WHITE};
    font-size: 11px;
  }
`;

export const CheckboxLabel = styled(Text.Label)`
  width: 100%;
  line-height: 18px;
  font-size: 13px;
  cursor: pointer;
`;
