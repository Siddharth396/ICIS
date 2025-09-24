import { theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const AddSectionButtonWrapper = styled.div`
  transform: translateY(-50%);
  width: max-content;
  margin: 0 auto;
`;

export const AddSectionButton = styled.div<{ alwaysShow?: boolean }>`
  cursor: pointer;
  position: relative;
  border: none;
  border-radius: ${theme.borderRadius.large};
  padding: 4px ${theme.spacing.BASE_1};
  background-color: ${theme.colours.SECONDARY_2};
  color: ${theme.colours.WHITE};
  text-transform: uppercase;
  font-weight: ${theme.fonts.weight.SEMI_BOLD};
  font-size: 9px;
  line-height: 10px;
  display: flex;
  flex-direction: row;
  align-items: center;
  gap: ${theme.spacing.BASE_1};
  opacity: 0;
  transform: scale(0.92);
  transition-duration: ${theme.animation.DURATION};
  transition-timing-function: ${theme.animation.TIMING_FUNCTION};
  transition-property: opacity, transform;

  transition:
    opacity ${theme.animation.DURATION} ${theme.animation.TIMING_FUNCTION},
    transform ${theme.animation.DURATION} ${theme.animation.TIMING_FUNCTION},
    background-color ${theme.animation.DURATION};

  &:hover {
    background-color: ${theme.colours.BUILDING_BLUE_HOVER_1};
  }

  &:active {
    background-color: ${theme.colours.PRIMARY_1};
  }
`;

export const AddSectionWrapper = styled.div`
  padding: ${theme.spacing.BASE_2} 0;
  position: absolute;
  width: 100%;
  z-index: 1;
  margin-bottom: 40px;
  transform: translateY(-50%);

  &:hover {
    ${AddSectionButton} {
      opacity: 1;
      transform: scale(1);
    }
  }
`;

export const HorizontalLine = styled.div<{ noBorder?: boolean }>`
  border-top: ${({ noBorder }) =>
    noBorder ? '1px solid transparent' : `1px dashed ${theme.colours.SECONDARY_2}`};
  transition: border-top-color ${theme.animation.DURATION} ${theme.animation.TIMING_FUNCTION};
  height: 0;
`;
