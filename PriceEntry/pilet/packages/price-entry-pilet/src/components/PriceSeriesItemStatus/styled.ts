import { theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const Wrapper = styled.div`
  width: 100%;
  overflow: hidden;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: 9px;
  font-weight: ${theme.fonts.weight.SEMI_BOLD};
`;

export const StatusWrapper = styled.div`
  display: flex;
  padding: 4px 0px;
  gap: 4px;
  align-items: center;
`;

export const StatusLabel = styled.div`
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;
