import styled from 'styled-components';
import { theme } from '@icis/ui-kit';

/**
 * Helper to set a border color around cells in Price Entry specifically.
 * You can decide if you want to keep or refactor this.
 */
export const cellStyle = {
  border: 'solid',
  borderTopWidth: '0px',
  borderRightWidth: '1px',
  borderLeftWidth: '0px',
  borderBottomWidth: '0px',
  borderColor: theme.colours.NEUTRALS_5,
};

/**
 * Container for the Price Entry Grid
 */
export const PriceEntryGridContainer = styled.div<{
  isScrollAtLeftMostPosition: boolean;
}>`
  .ag-column-last {
    border-right: 0 !important;
  }

  .ag-row .ag-drag-handle {
    opacity: 0;
    transition: opacity 0.2s ease;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .ag-row-hover .ag-drag-handle {
    opacity: 1;
  }

  .ag-cell-inline-editing {
    border-top: 0px !important;
    border-left: 0px !important;
    border-bottom: 0px !important;
    border-right: 1px solid rgb(204, 210, 217) !important;
    box-shadow: none;
    padding-left: 6px;
    padding-right: 6px;
  }
  .ag-cell {
    display: flex;
    align-items: center;
    padding-top: 5px;
    padding-bottom: 5px;
    line-height: 140%;
    width: 100%;

    input {
      border-color: ${theme.colours.NEUTRALS_4};
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
  }
  .ag-right-aligned-header .ag-header-cell-comp-wrapper {
    display: flex;
    justify-content: end;
    text-align: end;
  }
  .ag-header-cell.ag-column-first {
    padding-left: 35px;
  }
  .ag-header .ag-pinned-left-header,
  .ag-body-viewport .ag-pinned-left-cols-container {
    z-index: 1;
    box-shadow: ${({ isScrollAtLeftMostPosition }) =>
      isScrollAtLeftMostPosition
        ? 'none'
        : '-4px 10px 12px 0px rgba(0, 0, 0, 0.10), 10px 10px 12px 0px rgba(174, 174, 192, 0.25)'};
  }
  .ag-header .ag-pinned-right-header,
  .ag-body-viewport .ag-pinned-right-cols-container {
    z-index: 1;
    border-left: 1px solid rgb(204, 210, 217) !important;
    box-shadow:
      4px 10px 12px 0px rgba(0, 0, 0, 0.1),
      -10px 10px 12px 0px rgba(174, 174, 192, 0.25);
  }
  .ag-center-cols-viewport {
    min-height: unset !important;
  }
  .ag-row {
    --ag-internal-content-line-height: none;
  }
`;
