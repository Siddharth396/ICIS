// istanbul ignore file
import { Button, Text, theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const ConfigWrapper = styled.div`
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  padding: ${theme.spacing.BASE_5};
`;

export const SaveButton = styled(Button)`
  background-color: ${theme.colours.BUTTON_PRIMARY};
`;

export const ModalBody = styled.div`
  width: 100%;
  height: 100%;
  display: grid;
  grid-template-columns: 1fr 2fr;
  gap: ${theme.spacing.BASE_1};
  position: relative;
  &::after {
    content: "";
    position: absolute;
    margin: 0 -20px;
    left: 0; /* Align to the right edge of LeftColumn */
    right: 0; /* Align to the right edge of LeftColumn */
    top: 0;
    height: 1px; /* Width of the line */
    background-color: ${theme.colours.NEUTRALS_5}; /* Color of the line */
  }
`;

export const LeftColumn = styled.div`
  display: flex;
  flex-direction: column;
  padding-right: ${theme.spacing.BASE_1};
  justify-content: start;
  gap: ${theme.spacing.BASE_1};
  position: relative;
  & > div {
    max-height: 40px;
  }
  &::after {
    content: "";
    position: absolute;
    margin-bottom: -10px;
    right: 0; /* Align to the right edge of LeftColumn */
    top: 0;
    bottom: 0;
    width: 1px; /* Width of the line */
    background-color: ${theme.colours.PRIMARY_5}; /* Color of the line */
  }
`;

export const Header = styled(Text.H4)`
  padding: 10px;
  padding-left: ${theme.spacing.BASE_2};
  position: relative;
  margin-left: -20px;
  margin-right: -${theme.spacing.BASE_1};
  &::after {
    content: "";
    position: absolute;
    right: 0; /* Align to the right edge of LeftColumn */
    left: 0;
    bottom: 0;
    height: 1px; /* Width of the line */
    background-color: ${theme.colours.PRIMARY_5}; /* Color of the line */
  }
`;

export const RightColumn = styled.div`
  display: flex;
  justify-content: center;
  overflow: auto;
`;

export const ModalFooter = styled.div`
  display: flex;
  justify-content: right;
  gap: ${theme.spacing.BASE_1};
`;

export const BannerBody = styled.div`
  margin-bottom: 1em;
  width: 100%;
`;
