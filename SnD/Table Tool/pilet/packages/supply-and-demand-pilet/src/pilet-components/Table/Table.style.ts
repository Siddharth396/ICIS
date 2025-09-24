// istanbul ignore file
import { Icon, theme } from '@icis/ui-kit';
import styled from 'styled-components';

const border = `
  border-bottom: 1px solid ${theme.colours.NEUTRALS_5};
`;

const stickyBorder = `
  &::after {
    content: "";
    position: absolute;
    bottom: -1px;
    right: -1px;
    width: 100%;
    height: 1px;
    background-color: ${theme.colours.NEUTRALS_5};
  }

  &::before {
    content: "";
    position: absolute;
    top: -1px;
    right: -1px;
    width: 1px;
    height: 100%;
    background-color: ${theme.colours.NEUTRALS_5};
  }
`;

const cellCommonStyles = `
  padding-top: ${theme.spacing.BASE_1};
  padding-bottom: ${theme.spacing.BASE_1};
  vertical-align: middle;
  font-size: 13px;
  line-height: 16px;
  height: 40px;
`;

export const TableWrapper = styled.div`
  width: 100%;
  overflow-x: auto;
  border-radius: 4px;
  border: 1px solid ${theme.colours.NEUTRALS_5};
`;

export const StyledTable = styled.table`
  width: 100%;
`;

export const HeaderRow = styled.tr`
  border-bottom: 1px solid ${theme.colours.NEUTRALS_7};
`;

export const HeaderCell = styled.th<{ isFirst?: boolean, align?: string, width?: number }>`
  ${cellCommonStyles}
  padding-left: ${({ align, isFirst }) => (align === 'right' ? theme.spacing.BASE_4 : isFirst ? theme.spacing.BASE_3 : theme.spacing.BASE_1)};
  padding-right: ${({ align }) => (align === 'right' ? theme.spacing.BASE_1 : theme.spacing.BASE_4)};
  text-align: ${({ align }) => (align === 'right' ? 'right' : 'left')};
  color: ${theme.colours.PRIMARY_1};
  font-weight: 600;
  background-color: ${theme.colours.NEUTRALS_6};
  &:hover {
    background-color: ${theme.colours.NEUTRALS_5};
    transition: background-color ${theme.animation.DURATION} ease 0s;
  }
  min-width: ${({ width }) => (width ? width + 'px' : '300px')};
  .sortIcon {
    color: ${theme.colours.PRIMARY_1};
    width: 9px;
    height: 16px;
    margin: 0 8px;
    cursor: pointer;
  }
  ${border}
  // &::after {
  //   content: '';
  //   position: absolute;
  //   top: 0;
  //   right: 0;
  //   bottom: 0;
  //   width: 20px; // Adjust this to change the width of the shadow
  //   background: linear-gradient(to left, #fff 0%, #ddd 100%);
  // }
`;

export const StickyHeaderCell = styled(HeaderCell)<{ size: number }>`
  position: sticky;
  z-index: 1;
  left: ${({ size }) => (size + 'px')};
  ${stickyBorder}
`;

export const Row = styled.tr<{ isFirst?: boolean }>`
  padding: ${theme.spacing.BASE_1};
`;

export const Cell = styled.td<{ isFirst?: boolean, align?: string, width?: number }>`
  ${cellCommonStyles}
  padding-left: ${({ align, isFirst }) => (align === 'right' ? theme.spacing.BASE_4 : isFirst ? theme.spacing.BASE_3 : theme.spacing.BASE_1)};
  padding-right: ${({ align }) => (align === 'right' ? theme.spacing.BASE_1 : theme.spacing.BASE_4)};
  text-align: ${({ align }) => (align === 'right' ? 'right' : 'left')};
  background-color: ${theme.colours.WHITE};
  height: 40px;
  &:hover {
    background-color: ${theme.colours.NEUTRALS_7};
    transition: background-color ${theme.animation.DURATION} ease 0s;
  }
  ${border}
  // &::after {
  //   content: '';
  //   position: absolute;
  //   top: 0;
  //   right: 0;
  //   bottom: 0;
  //   width: 20px; // Adjust this to change the width of the shadow
  //   background: linear-gradient(to left, #fff 0%, #ddd 100%);
  // }
`;

export const StickyCell = styled(Cell)<{ size: number }>`
  position: sticky;
  z-index: 1;
  left: ${({ size }) => (size + 'px')};
  ${stickyBorder}
`;

export const ButtonWrapper = styled.div<{ isLast?: boolean }>`
  display: flex;
  justify-content: end;
  gap: ${theme.spacing.BASE_1};
`;

export const NoRecordsCell = styled(Cell)`
  height: 200px;
`;

export const NoRecordsWrapper = styled.div`
  padding: ${theme.spacing.BASE_4} 0;
  position: absolute;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-direction: column;
  gap: ${theme.spacing.BASE_1};
  left: 50%;
  transform: translate(-50%, -50%);
  & *:first-child {
    padding-bottom: ${theme.spacing.BASE_1};
  }
  & * {
    color: ${theme.colours.NEUTRALS_2};
  }
`;

export const NoRecordsIcon = styled(Icon)`
  height: 48px;
`;