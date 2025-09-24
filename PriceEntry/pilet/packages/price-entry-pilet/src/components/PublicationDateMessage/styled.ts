import { Icon, Text, theme, Tooltip } from '@icis/ui-kit';
import styled from 'styled-components';

export const DateMessage = styled.div<{ isPublishingDay: boolean }>`
  display: flex;
  width: max-content;
  height: 24px;
  gap: 4px;
  align-items: center;
  background-color: ${({ isPublishingDay }) =>
    isPublishingDay ? theme.colours.NEUTRALS_5 : theme.colours.CHARTING_4};
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: bold;
  text-transform: uppercase;
`;

export const MessageInfoIcon = styled(Icon)`
  color: ${theme.colours.NEUTRALS_9};
  font-size: 1.5em;
`;

export const NonPublishingDayMessage = styled(Text.Caption)`
  color: ${theme.colours.NEUTRALS_9};
  font-weight: bold;
  width: max-content;
`;

export const PublishingDayMessage = styled(Text.Caption)`
  color: ${theme.colours.NEUTRALS_2};
  font-weight: bold;
  width: max-content;
`;

export const DateMessageToolTip = styled(Tooltip)`
  border-radius: 4px;
  max-width: 240px;
  gap: 10px;
  align-items: center;
  display: flex;
  font-size: 13px;
  font-weight: 400;
  line-height: 16px;
  text-transform: none;
`;
