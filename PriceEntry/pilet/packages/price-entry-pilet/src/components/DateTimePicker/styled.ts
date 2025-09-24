// istanbul ignore file
import styled, { css } from 'styled-components';
import { Icon, theme } from '@icis/ui-kit';

const { spacing, colours, fonts, borderRadius } = theme;

export const DatePickerContainer = styled.div`
  width: 248px;
  min-width: 248px;
`;

export const MenuWrapper = styled.div`
  border-radius: ${borderRadius.default};
  background-color: ${colours.WHITE};
  color: ${colours.NEUTRALS_2};
  margin-top: 5px;
  position: absolute;
  width: inherit;
  z-index: 1000;
  outline: 1px solid ${colours.SECONDARY_4};

  .select__dropdown-indicator {
    padding: 0 ${spacing.BASE_1};
  }
  .select__option:not(.select__option--is-selected):hover {
    background-color: ${colours.NEUTRALS_7};
  }
`;

export const HeaderWrapper = styled.div`
  display: flex;
  gap: 8px;
  align-items: center;
  padding: 8px;
  background-color: ${colours.NEUTRALS_7};
  & > div {
    width: 100%;
  }
  .select__single-value {
    margin: 0;
  }
  .select__value-container {
    padding-right: 0;
  }
`;

export const ListWrapper = styled.div`
  display: flex;
  align-items: flex-start;
  align-self: stretch;
  flex-direction: column;
  max-height: 296px;
  overflow-y: auto;
  overflow-x: hidden;
`;

export const List = styled.div<{
  selected: boolean;
}>`
  display: flex;
  padding: 0px ${spacing.BASE_2};
  align-self: stretch;
  padding: 12px ${spacing.BASE_2};
  border-top: 1px solid ${colours.NEUTRALS_5};
  ${({ selected }) => css`
    background-color: ${selected ? colours.SECONDARY_4 : colours.WHITE};
    color: ${selected ? colours.WHITE : colours.NEUTRALS_2};
    border-color: ${selected ? colours.SECONDARY_4 : colours.NEUTRALS_5};
    &:hover {
      background-color: ${selected ? colours.SECONDARY_4 : colours.NEUTRALS_7};
    }
  `};

  &:focus,
  &:active {
    background-color: ${colours.NEUTRALS_5};
    border-color: ${colours.NEUTRALS_5};
    color: ${colours.WHITE};
  }
`;

export const SelectionBoxIcon = styled(Icon)`
  position: absolute;
  color: ${colours.NEUTRALS_2};
  top: 50%;
  transform: translateY(-50%);
  right: 5px;
  &:hover,
  &:active,
  &:focus {
    color: ${colours.SECONDARY_4};
  }
`;

export const SelectionBox = styled.div<{ menuIsOpen: boolean; disabled?: boolean }>`
  position: relative;
  display: inline-block;
  width: inherit;
  height: ${spacing.BASE_4};
  padding: 6px 0 8px 10px;
  border-radius: 4px;
  cursor: pointer;
  background: ${colours.WHITE};
  box-sizing: border-box;
  ${({ menuIsOpen }) => css`
    border: 1px solid ${menuIsOpen ? colours.SECONDARY_4 : colours.NEUTRALS_4};
    svg {
      color: ${menuIsOpen ? colours.SECONDARY_4 : colours.NEUTRALS_2};
    }
  `};
  &:hover,
  &:active,
  &:focus {
    border-color: ${colours.SECONDARY_4};
    ${SelectionBoxIcon} {
      color: ${colours.SECONDARY_4};
    }
  }

  ${({ disabled }) =>
    disabled &&
    css`
      pointer-events: none;
      border: 1px solid ${colours.NEUTRALS_4};
      ${SelectionBoxIcon} {
        color: ${colours.NEUTRALS_4};
      }
      ${SelectionBoxValue} {
        color: ${colours.NEUTRALS_4};
      }
    `}
`;

export const SelectionBoxValue = styled.div`
  text-align: left;
  font-size: 13px;
  font-weight: 400;
  line-height: 18px;
  white-space: nowrap;
  user-select: none;
  overflow: hidden;
  text-overflow: ellipsis;
  color: ${colours.NEUTRALS_9};
`;

export const DateContainer = styled.div`
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;
  align-content: center;
  justify-content: flex-start;
  align-items: center;
  gap: 4px;
`;

export const WeekContainer = styled.span`
  width: 27.5px;
  text-align: right;
  color: ${colours.PRIMARY_1};
`;
export const DayContainer = styled.span`
  width: 20px;
  text-align: center;
  color: ${colours.PRIMARY_1};
  font-weight: ${fonts.weight.BOLD};
`;

export const Spinner = styled.div`
  position: absolute;
  top: 50%;
  left: calc(50% - 16px);
`;
