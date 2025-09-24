import React from 'react';
import { Language } from '@icis/ui-kit';
import { DeltaContainer } from 'components/PriceDelta/styled';
// istanbul ignore next
const formatNumber = (value: number | null, locale: Language, precision?: number): string => {
  if (isNaN(value as number)) return 'n/a';
  if (value === null || value === undefined) return '';

  let parts = value.toString().split('.');
  if (parts[1]) {
    const num = precision ? value.toFixed(precision) : value.toString();
    parts = num.split('.');
  }
  if (locale !== 'zh') {
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
  }
  const allParts = parts.join('.');
  return allParts.replace('Infinity', '--');
};

type Props = {
  /** Current price. If 0, renders fallback. */
  price?: number;
  /** The adjusted change (delta). */
  delta: number;
  /** Numeric precision to use (default 2). */
  precision?: number;
  /** Locale code (e.g. 'en', 'zh'). */
  locale: Language;
  /** Optional previous price used as denominator. */
  previousPrice?: number;
};

const AdjustedMovement: React.FC<Props> = ({
  price,
  delta,
  previousPrice,
  locale,
  precision = 2,
}) => {
  // If current price is invalid or 0, show fallback.
  if (!price) {
    return <DeltaContainer>(--)</DeltaContainer>;
  }

  // 1) Calculate the magnitude of the percentage
  let percentageDelta: number;
  if (previousPrice !== undefined && previousPrice !== null && previousPrice !== 0) {
    percentageDelta = (Math.abs(delta) / Math.abs(previousPrice)) * 100;
  } else {
    // Fallback if no previous price: use |delta| * 100
    percentageDelta = Math.abs(delta) * 100;
  }
  // Round away floating artifacts
  percentageDelta = parseFloat(percentageDelta.toFixed(precision));

  // 2) Determine sign. Only do ratio-based sign if previousPrice is valid and nonzero
  let sign = '';
  if (previousPrice !== undefined && previousPrice !== null && previousPrice !== 0) {
    const ratio = delta * previousPrice;
    // istanbul ignore else
    if (ratio > 0) sign = '+';
    else if (ratio < 0) sign = '-';
  } else {
    // If there's no valid previousPrice, default to delta-based sign
    if (delta > 0) sign = '+';
    else if (delta < 0) sign = '-';
  }

  // If percentage is 0, drop the sign
  if (percentageDelta === 0) {
    sign = '';
  }

  // 3) Format final number
  const formattedPercentage = formatNumber(percentageDelta, locale, precision);

  return (
    <DeltaContainer>
      ({sign}
      {formattedPercentage}%)
    </DeltaContainer>
  );
};

export default AdjustedMovement;
