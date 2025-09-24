// istanbul ignore file
import { IconKeys, theme } from '@icis/ui-kit';
import { IconStyle } from './styled';
import { PRICE_DECIMAL_PRECISION } from '../PriceEntryGrid/priceEntryGrid.utils';

export const getIconStyle = (icon: string) => {
  const styles = {
    icon: { ...IconStyle },
  };

  if (icon === IconKeys.movementNul) {
    (styles.icon as any).verticalAlign = 'middle';
    (styles.icon as any).marginBottom = '1px';
  }

  if (icon === IconKeys.triangleExclamation) {
    (styles.icon as any).outline = 'none';
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
    return theme.colours.CHARTING_2;
  } else if (delta > 0) {
    return theme.colours.CHARTING_1;
  } else {
    return theme.colours.PRIMARY_5;
  }
};

export const roundDecimalPlaces = (price: number): number => parseFloat(price.toFixed(PRICE_DECIMAL_PRECISION));
