import { Button, theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const Wrapper = styled.div`
  width: 100%;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: 9px;
  font-weight: ${theme.fonts.weight.SEMI_BOLD};
`;

export const StyledButton = styled(Button)`
  display: flex;
  padding: 4px 0px;
  gap: 4px;
  align-items: center;
`;

export const PopupContainer = styled.div`
  display: flex;
  justify-content: left;
  width: 100%;
  flex-direction: column;
  overflow: scroll;
  gap: ${theme.spacing.BASE_3};
`;

export const CapabilityContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: left;
  gap: ${theme.spacing.BASE_2};
`;

export const TableHeader = styled.div`
  font-size: 13px;
  font-weight: ${theme.fonts.weight.SEMI_BOLD};
`;