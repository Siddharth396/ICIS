// istanbul ignore file
import { theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const OutagesWrapper = styled.div<{isInPreviewMode: boolean}>`
  ${({ isInPreviewMode }) => {
    return `max-height: ${isInPreviewMode ? '340px' : 'initial'};;
    margin-bottom: ${theme.spacing.BASE_2};
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
    width: 100%;
    `;
  }}
`
