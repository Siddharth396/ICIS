import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import PriceComponent from '../Price';
import { PriceSeriesItemVersion } from 'apollo/queries';

jest.mock('@icis/ui-kit', () => ({
  ...jest.requireActual('@icis/ui-kit'),
  Movement: jest.fn(({ price, delta }) => {
    if (price === undefined || delta === undefined) return null;
    return <div data-testid='movement'>Mocked Movement Component</div>;
  }),
}));

describe('PriceDeltaComponent', () => {
  const rowData = (
    priceHigh: number,
    priceHighDelta: number,
    previousVersion: PriceSeriesItemVersion,
    status: string,
    cellId: number,
  ) => {
    return {
      priceHigh: priceHigh,
      priceHighDelta: priceHighDelta,
      previousVersion: previousVersion,
      status: status,
      id: cellId,
    };
  };

  const priceHighDeltaColumn = {
    alternateFields: [
      {
        seriesItemTypeCodes: ['cri-single', 'pi-single'],
        field: 'price',
        priceDeltaField: 'priceDelta',
      },
    ],
    customConfig: {
      priceDelta: {
        priceDeltaField: 'priceHighDelta',
        priceField: 'priceHigh',
      },
    },
  };

  it('renders nothing when rowData is not provided', () => {
    // @ts-ignore
    const { container } = render(<PriceComponent />);
    expect(container.firstChild).toBeNull();
  });

  it('renders "--" when both price and delta are missing', () => {
    const { container } = render(
      <PriceComponent
        // @ts-ignore
        rowData={rowData(undefined, undefined)}
        // @ts-ignore
        priceDeltaColumn={priceHighDeltaColumn}
      />,
    );
    expect(container.firstChild?.textContent).toBe('--');
  });

  it('renders price when delta is null', () => {
    const { container } = render(
      // @ts-ignore
      <PriceComponent rowData={rowData(7, null)} priceDeltaColumn={priceHighDeltaColumn} />,
    );
    expect(container.firstChild?.textContent).toBe('7');
  });

  it('renders warning icon when correction exists', () => {
    render(
      <PriceComponent
        // @ts-ignore
        rowData={rowData(7, -18, { priceHigh: 22 }, 'CORRECTION_PUBLISHED', 1)}
        // @ts-ignore
        priceDeltaColumn={priceHighDeltaColumn}
      />,
    );
    expect(screen.getByRole('tooltip', { hidden: true })).toBeInTheDocument();
  });

  it('does not render warning icon when correction does not exist', () => {
    render(
      <PriceComponent
        // @ts-ignore
        rowData={rowData(7, -18, null, 'CORRECTION_PUBLISHED', 1)}
        // @ts-ignore
        priceDeltaColumn={priceHighDeltaColumn}
      />,
    );
    expect(screen.queryByTestId('icon1priceHigh')).not.toBeInTheDocument();
  });

  it('renders price value up to 5 precision', () => {
    render(
      <PriceComponent
        // @ts-ignore
        rowData={rowData(700.123451, -18, null, 'CORRECTION_PUBLISHED', 1)}
        // @ts-ignore
        priceDeltaColumn={priceHighDeltaColumn}
      />,
    );

    expect(screen.getByText('700.12345')).toBeInTheDocument();
  });
});
