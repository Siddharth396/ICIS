import styled from 'styled-components';
import { theme } from '@icis/ui-kit';

const selectBoxHeight = '32px';

type SelectProps = {
  error?: boolean;
};

export const ReactSelect = styled.div<SelectProps>`
  .select__control {
    font-size: ${theme.fonts.size.BASE};
    transition: none;
    min-height: ${selectBoxHeight};
    height: ${selectBoxHeight};
    background-color: ${theme.colours.WHITE};
    border-color: ${theme.colours.NEUTRALS_4};
    box-shadow: none;
    color: ${theme.colours.NEUTRALS_9};
    .select__dropdown-indicator {
      color: ${theme.colours.NEUTRALS_2};
      height: auto;
      padding: 5px 10px;
      ${(props) => (props.error ? `color: ${theme.colours.ERROR};` : '')};
    }
    &:hover,
    &:focus {
      border-color: ${theme.colours.SECONDARY_4};
    }
    &:active {
      background-color: ${theme.colours.NEUTRALS_5};
    }
    ${(props) =>
      props.error
        ? `
        border-color: ${theme.colours.ERROR};`
        : ''};
  }
  .select__control--is-focused {
    border-color: ${theme.colours.SECONDARY_4};
    padding-left: 9px;
  }
  .select__control--menu-is-open {
    z-index: 2;

    .select__dropdown-indicator {
      color: rgb(36, 108, 255);
      transform: rotate(180deg);
      margin-left: auto;
    }
  }
  .select__indicator-separator {
    display: none;
  }
  .select__placeholder {
    color: ${theme.colours.NEUTRALS_9};
    font-weight: ${theme.fonts.weight.REGULAR};
    line-height: ${theme.fonts.lineHeight.BASE};
  }
  .select__pi-single {
    color: ${theme.colours.NEUTRALS_9};
    line-height: ${theme.fonts.lineHeight.BASE};
    margin: 0;
  }
  .select__pi-single--is-disabled {
    color: ${theme.colours.NEUTRALS_8};
  }

  .select__menu > * {
    padding-top: 0;
    padding-bottom: 0;
  }
  .select__option {
    transition: background-color ${theme.animation.DURATION};
    height: ${selectBoxHeight};
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }
  .select__option,
  .select__option--is-focused {
    background: none;
    color: ${theme.colours.NEUTRALS_9};
    line-height: ${theme.fonts.lineHeight.BASE};
  }

  .select__option:hover {
    background-color: ${theme.colours.NEUTRALS_5};
    color: ${theme.colours.NEUTRALS_9};
  }

  .select__option--is-selected {
    background-color: ${theme.colours.SECONDARY_4};
    color: ${theme.colours.WHITE};
  }

  .select__option:active {
    background-color: ${theme.colours.PRIMARY_1};
    color: ${theme.colours.WHITE};
    font-weight: ${theme.fonts.weight.SEMI_BOLD};
  }
`;

export const OptionWithIconWrapper = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
`;
