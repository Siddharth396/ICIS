// istanbul ignore file
import { theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const CapacitiesWrapper = styled.div<{isInPreviewMode: boolean}>`
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

export const Capacity = styled.div`
`;

export const Negative = styled.div`
  color: ${theme.colours.CHARTING_2};
`;

export const Positive = styled.div`
  color: ${theme.colours.CHARTING_1};
`;
