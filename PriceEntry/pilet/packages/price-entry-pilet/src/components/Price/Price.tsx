import { Icon, IconKeys, theme, Tooltip } from '@icis/ui-kit';
import { Column, IBaseSeriesItem, IPriceDisplaySeriesItem, PriceSeriesItemVersion } from 'apollo/queries';
import { getDeltaColor, getDeltaIcon, getIconStyle, roundDecimalPlaces } from './Price.utils';
import { CorrectionContainer, PriceContainer } from './styled';
import { createPortal } from 'react-dom';
import { WORKFLOW_ACTIONS } from 'utils/constants';

interface IPriceComponent {
  rowData: IPriceDisplaySeriesItem;
  priceDeltaColumn: Column;
}

const getWarningTooltip = (previousPrice: number, correctedDate: string, cellId: string) => {
  const icon = IconKeys.triangleExclamation
  const formattedPrice = new Intl.NumberFormat().format(roundDecimalPlaces(previousPrice));
  
  return (
    <>
      <Icon data-for={`tooltip${cellId}`} data-testid={`icon${cellId}`} style={getIconStyle(icon).icon} data-tip icon={icon} color={theme.colours.CHARTING_4} />
      {createPortal(
        <Tooltip id={`tooltip${cellId}`} data-testid={`tooltip${cellId}`} effect='solid' arrowColor='black' place='top' >
          <CorrectionContainer>
            <p>Previously published value <span>&apos;{formattedPrice}&apos;</span> corrected on {correctedDate}</p>
          </CorrectionContainer>
        </Tooltip>
        , document.body,
      )}
    </>
  );
}

const getDelta = (price: number, deltaValue: number, warningTooltip: JSX.Element): JSX.Element | null => {
  const icon = getDeltaIcon(deltaValue);

  return (
    <>
      <PriceContainer>
        {warningTooltip}
        {roundDecimalPlaces(price)}
        {icon && (
          <Icon style={getIconStyle(icon).icon} icon={icon} color={getDeltaColor(deltaValue)} />
        )}
      </PriceContainer>
    </>
  );
};

const PriceComponent = ({rowData, priceDeltaColumn}: IPriceComponent) => {
  const { customConfig: { priceDelta = {} } = {} } = priceDeltaColumn || {};
  const { priceField: defaultPriceField, priceDeltaField: defaultPriceDeltaField } = priceDelta;

  const alternateField = priceDeltaColumn?.alternateFields?.find(alternateField => alternateField.seriesItemTypeCodes.includes(rowData?.seriesItemTypeCode));

  const priceField = alternateField?.field ?? defaultPriceField;
  const priceDeltaField = alternateField?.priceDeltaField ?? defaultPriceDeltaField;
  const previousVersion: PriceSeriesItemVersion = rowData?.previousVersion

  if (!(rowData && priceField)) {
    return null;
  }

  const price = rowData[priceField as keyof IBaseSeriesItem] as number;
  const deltaValue = rowData[priceDeltaField as keyof IBaseSeriesItem] as number ?? 0;

  const previousPrice = previousVersion?.[priceField as keyof PriceSeriesItemVersion] ?? price;
  const hasCorrection = rowData?.status === WORKFLOW_ACTIONS.CORRECTION_PUBLISHED && (Number(price) !== Number(previousPrice));
  const correctedDate = rowData?.lastModifiedDate ?? null;
  const cellId = rowData?.id + priceField;

  const warningTooltip = hasCorrection ? getWarningTooltip(previousPrice, correctedDate, cellId) : <></>;

  return (price !== null && price !== undefined) ? getDelta(price, deltaValue, warningTooltip) : <>--</>;
};

export default PriceComponent;