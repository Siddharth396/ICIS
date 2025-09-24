import { Text, theme } from '@icis/ui-kit';
import Skeleton from 'react-loading-skeleton';
import styled from 'styled-components';

const { colours, spacing } = theme;

export const LEFT_SIDEBAR_WIDTH = 380;

export const MIN_GRID_WIDTH = 680;

interface EmptyStateWrapperProps {
  isEmpty?: boolean;
}

export const Wrapper = styled.div`
  height: 100%;
  width: 100%;
  justify-content: space-between;
`;

export const ContentWrapper = styled.div`
  display: flex;
  height: 100%;
`;

export const HeadingBorder = styled.div`
  height: 1px;
  background: ${theme.colours.PRIMARY_5};
  width: calc(100% + 40px);
  margin-left: -20px;
  margin-top: 4px;
`;

export const ListContainer = styled.div<EmptyStateWrapperProps>`
  height: 100%;
  width: 70%;
  .ag-row-disabled {
    color: ${colours.NEUTRALS_4};
    pointer-events: none;
    opacity: 0.6;
  }
  ${({ isEmpty }) =>
    isEmpty &&
    `
      display: flex;
      align-items: center;
      justify-content: center;
    `}
`;

export const TableArea = styled.div`
  height: 100%;
`;

export const FooterButtonsWrapper = styled.div`
  display: flex;

  button {
    margin-left: ${theme.spacing.BASE_2};
    width: 120px;
  }
`;

export const FilterContainer = styled.div`
  padding-top: ${theme.spacing.BASE_2};
  padding-bottom: ${theme.spacing.BASE_2};
  width: 40%;
  height: 100%;
`;

export const SkeletonWrapper = styled.div<{ editing?: boolean }>`
  width: ${MIN_GRID_WIDTH}px;
  height: 80%;
  margin: auto;
  margin-top: 20px;
`;

export const GridPreviewSkeleton = styled(Skeleton as any)`
  height: 100% !important;
`;

export const MainContainer = styled.div`
  display: flex;
  width: ${LEFT_SIDEBAR_WIDTH}px;
  flex-direction: column;
  position: relative;
  height: 100%;
  background: ${colours.NEUTRALS_7};
  border-right: 1px solid ${colours.NEUTRALS_5};
`;

export const HeaderContainer = styled(Text.H4)`
  display: flex;
  width: 100%;
  padding: ${spacing.BASE_2} ${spacing.BASE_1} ${spacing.BASE_2} ${spacing.BASE_2};
  justify-content: space-between;
  align-items: center;
  color: ${colours.BLACK};
`;

export const Row = styled.div<{
  hoverEffect?: boolean;
  gap?: number;
  multiCol?: boolean;
  disable?: boolean;
}>`
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  width: 100%;
  height: 40px;
  padding-right: 20px;
  align-items: center;
  opacity: ${({ disable }) => (disable ? 0.5 : 1)};

  .dropdown-heading-dropdown-arrow {
    position: absolute;
    right: 0;
    margin-right: 5px;
  }

  ${({ multiCol }) =>
    multiCol &&
    `
    height: auto;
    align-items: start;
  `};

  ${({ gap }) =>
    gap &&
    `
    gap: ${gap}px
  `};
`;

export const Column = styled.div<{ padding?: boolean; gap?: number; highlightBorder?: boolean }>`
  display: flex;
  flex-direction: column;
  width: 100%;
  ${({ padding }) =>
    padding &&
    `
    padding: 0px ${spacing.BASE_3};
  `};
  ${({ gap }) =>
    gap &&
    `
    gap: ${gap}px;
  `};
  ${({ highlightBorder }) =>
    highlightBorder &&
    `
    input {
      border-color: ${theme.colours.WARNING};
    }
  `};
`;

export const TextWrapper = styled(Text.Body)<{
  maxWidth?: boolean;
  color?: string;
  marginBottom?: number;
}>`
  color: ${theme.colours.PRIMARY_1};
  line-height: 16px;
  ${({ maxWidth }) =>
    maxWidth &&
    `
    max-width: 118px;
  `};
  ${({ color }) =>
    color &&
    `
    color: ${color};
  `};
  ${({ marginBottom }) =>
    marginBottom &&
    `
    margin-bottom: ${marginBottom}px;
  `};
  svg {
    padding-left: 8px;
  }
  .place-bottom::after {
    left: 30px !important;
    top: -5px !important;
  }
`;

export const SelectWrapper = styled.div`
  display: flex;
  width: 160px;
  gap: 8px;
  justify-content: flex-end;
  flex-shrink: 0;
  align-items: center;
  div:first-child {
    width: 100%;
  }
  .dropdown-heading-value span {
    color: ${colours.PRIMARY_10};
  }
  .dropdown-container {
    display: flex;
    align-items: center;
    height: 32px;
  }
  .select__option {
    height: 40px;
    padding: ${theme.spacing.BASE_2} ${theme.spacing.BASE_2};
  }
`;

export const HeaderWrapper = styled.div`
  display: flex;
  align-items: center;
  height: 24px;
  font-size: 16px;
  font-weight: ${theme.fonts.weight.SEMI_BOLD};
`;

export const CheckboxContainer = styled.label<{ disabled: boolean }>`
  display: flex;
  flex-direction: row;
  gap: 1px;
  width: max-content;
  padding: 0 ${theme.spacing.BASE_2};
  cursor: pointer;
  position: absolute;
  bottom: 0;

  :hover {
    background: ${theme.colours.NEUTRALS_7};
    input:checked ~ .checkmark {
      background: ${theme.colours.NEUTRALS_1};
    }
  }

  ${({ disabled }) =>
    disabled &&
    `
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

export const FooterWrapper = styled.div`
  display: flex;
  justify-content: space-between;
  alignitems: center;
  width: 100%;
`;

export const ErrorText = styled(Text.Body)`
  color: ${theme.colours.ERROR};
  display: flex;
  white-space: pre-line;
`;

export const TableConfigHeader = styled.div`
  margin-top: 20px;
  margin-bottom: 20px;
`;
