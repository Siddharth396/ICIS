import { render } from '@testing-library/react';
import '@testing-library/jest-dom'; // For additional matchers like toBeInTheDocument
import AdjustedDeltaComponent from '../AdjustedDeltaDisplayRenderer';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import { PRICE_DELTA_TYPES } from 'utils/constants';

describe('AdjustedDeltaCellRenderer', () => {
  const data = (priceDeltaType: string, id: string) => ({
    priceDeltaType,
    id,
  });
  const messages = useLocaleMessages();

  it('renders NMA label when NMA is applicable', () => {
    // @ts-ignore
    const { container } = render(<AdjustedDeltaComponent data={data(PRICE_DELTA_TYPES.NON_MARKET_ADJUSTMENT, '1')} />);
    expect(container.firstChild?.textContent).toBe(messages.Workflow.StaticLabels.NonMarketAdjustments.Label);
  });

  it('renders nothing when NMA is not applicable', () => {
    // @ts-ignore
    const { container } = render(<AdjustedDeltaComponent data={data(PRICE_DELTA_TYPES.REGULAR, '2')} />);
    expect(container.firstChild).toBeNull();
  });
  
  it('renders nothing when pricedelta type is null', () => {
    // @ts-ignore
    const { container } = render(<AdjustedDeltaComponent data={data(null, '2')} />);
    expect(container.firstChild).toBeNull();
  });
});
