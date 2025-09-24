import React from 'react';
import { Icon, Movement } from '@icis/ui-kit';
import { Column, IBaseSeriesItem, IPriceEntrySeriesItem } from 'apollo/queries';
import { useUser } from '@icis/app-shell-apis';
import { getDeltaColor, getDeltaIcon, getIconStyle } from './PriceDelta.utils';
import { DeltaContainer } from './styled';
import AdjustedMovement from 'components/Movement/Movement';
import { PRICE_DECIMAL_PRECISION } from '../PriceEntryGrid/priceEntryGrid.utils';

interface IPriceDeltaComponent {
  data: IPriceEntrySeriesItem;
  columnConfig: Column;
  isPriceEntry: boolean;
  /** When true, use adjustment delta behavior */
  adjustmentDelta?: boolean;
}

const getDelta = (
  price: number,
  deltaValue: number,
  precision: number,
  isPriceEntry: boolean,
  adjustmentDelta: boolean,
  previousPrice?: number | undefined,
): JSX.Element | null => {
  const { locale } = useUser();

  if (price && deltaValue !== undefined && deltaValue !== null) {
    // istanbul ignore next
    if (adjustmentDelta) {
      const color = getDeltaColor(deltaValue);
      return (
        <DeltaContainer style={{ color }}>
          <AdjustedMovement
            price={price}
            previousPrice={previousPrice}
            delta={deltaValue}
            precision={precision}
            locale={locale}
          />
        </DeltaContainer>
      );
    } else {
      const icon = getDeltaIcon(deltaValue);
      return (
        <DeltaContainer>
          {isPriceEntry && icon && (
            <Icon style={getIconStyle(icon).icon} icon={icon} color={getDeltaColor(deltaValue)} />
          )}
          <Movement
            version='V2'
            price={price}
            delta={deltaValue}
            precision={precision}
            locale={locale}
            showDeltaAsPercent
          />
        </DeltaContainer>
      );
    }
  }

  return isPriceEntry ? (
    <DeltaContainer>
      <span>--</span>
    </DeltaContainer>
  ) : (
    <>-- (--)</>
  );
};

const PriceDeltaComponent = ({
  columnConfig,
  data,
  isPriceEntry,
  adjustmentDelta = false,
}: IPriceDeltaComponent) => {
  // Read the priceDelta custom config.
  const { customConfig: { priceDelta = {} } = {} } = columnConfig || {};

  const {
    priceField: defaultPriceField,
    priceDeltaField: defaultPriceDeltaField,
    precisionField,
  } = priceDelta;

  const alternateField = columnConfig?.alternateFields?.find((alternateField) =>
    alternateField.seriesItemTypeCodes.includes(data?.seriesItemTypeCode),
  );

  const priceField = alternateField?.field ?? defaultPriceField;
  const priceDeltaField = alternateField?.priceDeltaField ?? defaultPriceDeltaField;

  if (data && priceField && priceDeltaField && precisionField) {
    const price = data[priceField as keyof IBaseSeriesItem] as number;
    const deltaValue = data[priceDeltaField as keyof IBaseSeriesItem] as number;

    const precision = (data[precisionField as keyof IBaseSeriesItem] as number) ?? PRICE_DECIMAL_PRECISION;
    // in case of adjustment delta, price is the previous price
    // istanbul ignore next
    const previousPrice = adjustmentDelta
      ? (data[priceField as keyof IBaseSeriesItem] as number)
      : undefined;
    return getDelta(price, deltaValue, precision, isPriceEntry, adjustmentDelta, previousPrice);
  }

  return null;
};

export default PriceDeltaComponent;
