// istanbul ignore file
import { IconKeys, theme } from '@icis/ui-kit';
import { IconStyle } from './styled';

export const getIconStyle = (icon: string) => {
  const styles = {
    icon: { ...IconStyle },
  };

  if (icon === IconKeys.movementNul) {
    (styles.icon as any).verticalAlign = 'middle';
    (styles.icon as any).marginTop = '3px';
  }
  return styles;
};

export const getDeltaIcon = (delta: number) => {
  if (delta < 0) {
    return IconKeys.movementNegative;
  } else if (delta > 0) {
    return IconKeys.movementPositive;
  }
  return IconKeys.movementNul;
};

export const getDeltaColor = (delta: number) => {
  if (delta < 0) {
    return theme.colours.NEGATIVE;
  } else if (delta > 0) {
    return theme.colours.POSITIVE;
  } else {
    return theme.colours.PRIMARY_5;
  }
};
