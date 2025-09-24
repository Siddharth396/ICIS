import { Button, Icon, theme } from '@icis/ui-kit';
import styled from 'styled-components';

interface StyledProp {
  disabled?: boolean;
}

export const WorkflowButton = styled(Button)<StyledProp>`
  ${({ disabled }) =>
    disabled &&
    `
      pointer-events: none;
      background: ${theme.colours.NEUTRALS_5};
    `};
`;

export const WorkflowButtonWrapper = styled.div`
  display: flex;
  align-items: center;
  gap: 10px;
  button {
    white-space: nowrap !important;
  }
`;

export const InfoIcon = styled(Icon)`
  color: ${theme.colours.PRIMARY_2};
  font-size: 1.2em;
  margin-right: 8px;
`;

export const CustomTooltip = styled.div`
  z-index: 99999;
  padding: 10px;
`;
