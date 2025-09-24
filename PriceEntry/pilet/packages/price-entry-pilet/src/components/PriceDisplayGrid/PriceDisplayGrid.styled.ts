import styled from 'styled-components';
import { theme } from '@icis/ui-kit';

const HEADER_BG_COLOR = '#f1f1f1';
const DISABLED_COLOR = '#d8d8d8';

export const cellStyle = {
  border: 'solid',
  borderTopWidth: '0px',
  borderRightWidth: '1px',
  borderLeftWidth: '0px',
  borderBottomWidth: '0px',
  borderColor: theme.colours.PRIMARY_6,
};

/**
 * Container for the Price Display Grid
 */
export const PriceDisplayGridContainer = styled.div<{
  isScrollAtLeftMostPosition: boolean;
  scrollWidth: number;
}>`
  .ag-root-wrapper {
    border: 1px solid ${theme.colours.PRIMARY_6};

    .correction-published-row {
      background: rgba(255, 174, 116, 0.15);
    }
    .prepublish-row {
      background: ${theme.colours.SECONDARY_3};
    }
  }
  .ag-center-cols-viewport {
    min-height: 40px !important;
  }
  .ag-column-last {
    border-right: 0 !important;
  }
  .ag-pinned-left-header {
    border-right: 1px solid ${theme.colours.PRIMARY_6};
  }
  .ag-header {
    border-bottom: none;
    background: ${theme.colours.NEUTRALS_6};
    :hover {
      background-color: ${theme.colours.NEUTRALS_6};
    }
  }
  .ag-header-cell {
    border-right: 1px solid ${theme.colours.PRIMARY_6};
    border-bottom: 1px solid ${theme.colours.PRIMARY_6};
    padding-right: ${theme.spacing.BASE_1};
    padding-left: ${theme.spacing.BASE_1};
    text-align: right;
  }
  .ag-cell {
    display: flex;
    align-items: center;
    padding: 0.5em;
    font-size: ${theme.fonts.size.BASE};
    font-weight: ${theme.fonts.weight.REGULAR};
    line-height: ${theme.fonts.size.LARGE};
    font-family: ${theme.fonts.graphikFonts};
    font-style: normal;
    color: ${({ color }) => (color ? color : theme.colours.BLACK)};
    gap: 10px;
    align-self: stretch;

    input {
      border-color: ${theme.colours.NEUTRALS_4};
    }
    &--disabled-background {
      background-color: ${DISABLED_COLOR};
    }
    &.ag-cell-inline-editing {
      padding-left: 8px;
      padding-right: 8px;
    }
    &--right-aligned-cell {
      display: inline-flex;
      justify-content: flex-end;
      align-items: center;
    }
    &--left-aligned-cell {
      display: inline-flex;
      justify-content: flex-start;
      align-items: center;
    }
    &--fixed-error-cell {
      border-left: 0.31em solid ${theme.colours.WARNING} !important;
    }
    &--correction-published-cell {
      background: rgba(255, 174, 116, 0.15);
    }
  }
  .ag-theme-alpine {
    --ag-row-border-color: ${theme.colours.NEUTRALS_4};
    --ag-header-background-color: ${HEADER_BG_COLOR};
    font-family: ${theme.fonts.graphikFonts};
  }

  .ag-header .ag-pinned-left-header,
  .ag-body-viewport .ag-pinned-left-cols-container {
    z-index: 1;
    box-shadow: ${({ isScrollAtLeftMostPosition }) =>
      isScrollAtLeftMostPosition
        ? 'none'
        : '-4px 10px 12px 0px rgba(0, 0, 0, 0.10), 10px 10px 12px 0px rgba(174, 174, 192, 0.25)'};
  }

  .ag-header-right .ag-header-cell-comp-wrapper {
    text-align: right !important;
    display: flex !important;
    justify-content: end !important;
  }

  .ag-horizontal-left-spacer {
    display: none;
  }

  .ag-body-horizontal-scroll-container {
    width: ${({ scrollWidth }) => scrollWidth + 'px !important'};
  }
  .ag-header-cell,
  .ag-cell {
    transition:
      width 0.3s ease,
      opacity 0.3s ease;
  }
`;
