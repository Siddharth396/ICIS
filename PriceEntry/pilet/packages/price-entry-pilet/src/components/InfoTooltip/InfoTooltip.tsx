import React from 'react';
import { PriceChangeTooltip as Tooltip, theme } from '@icis/ui-kit';

interface InfoTooltipProps {
  id: string;
  message: string;
  place?: 'top' | 'right' | 'bottom' | 'left';
  maxWidth?: number;
  followPosition?: { x: number; y: number } | null;
}

// A unified tooltip wrapper to keep consistent styling across disabled inputs & grid rows
const InfoTooltip: React.FC<InfoTooltipProps> = ({
  id,
  message,
  place = 'bottom',
  maxWidth = 240,
  followPosition,
}) => {
  // istanbul ignore if
  if (followPosition) {
    const bg = theme.colours.PRIMARY_1;
    const textColor = theme.colours.NEUTRALS_6;
    const left = Math.min(followPosition.x, window.innerWidth - (maxWidth + 16));
    const top = followPosition.y + 14; // a little lower than cursor
    return (
      <div
        data-follow-tooltip-id={id}
        style={{
          position: 'fixed',
          top,
          left,
          zIndex: 9999,
          pointerEvents: 'none',
        }}>
        <div
          style={{
            position: 'relative',
            background: bg,
            color: textColor,
            padding: '8px 12px',
            borderRadius: 4,
            fontSize: theme.fonts.size.BASE,
            lineHeight: '18px',
            maxWidth,
            boxShadow: '0 2px 6px rgba(0,0,0,0.25)',
            whiteSpace: 'normal',
          }}>
          {message}
          <span
            style={{
              position: 'absolute',
              top: -6,
              left: 14,
              width: 0,
              height: 0,
              borderLeft: '6px solid transparent',
              borderRight: '6px solid transparent',
              borderBottom: `6px solid ${bg}`,
            }}
          />
        </div>
      </div>
    );
  }
  return (
    <Tooltip
      id={id}
      place={place}
      messageContent={message}
      style={{
        maxWidth: `${maxWidth}px`,
        fontSize: theme.fonts.size.BASE,
        lineHeight: '18px',
        whiteSpace: 'normal',
        wordBreak: 'break-word',
      }}
    />
  );
};

export default InfoTooltip;
