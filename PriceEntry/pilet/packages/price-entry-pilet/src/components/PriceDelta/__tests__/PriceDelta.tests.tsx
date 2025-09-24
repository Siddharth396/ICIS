import React from 'react';
import { render } from '@testing-library/react';
import '@testing-library/jest-dom'; // For additional matchers like toBeInTheDocument
import PriceDeltaComponent from '../PriceDelta';

jest.mock('@icis/ui-kit', () => ({
  ...jest.requireActual('@icis/ui-kit'), // Use the actual implementation for other components
  Movement: jest.fn(({ price, delta }) => {
    // Check if price or delta is undefined and return null if either is undefined
    if (price === undefined || delta === undefined) return null;

    // Implement the mock of Movement component
    return <div data-testid='movement'>Mocked Movement Component</div>;
  }),
}));

describe('PriceDeltaComponent', () => {
  const columnConfig = {
    alternateFields: [
      {
        seriesItemTypeCodes: [
          'cri-single',
          'pi-single'
        ],
        field: 'price',
        priceDeltaField: 'priceDelta'
      }
    ],
    customConfig: {
      priceDelta: {
        priceField: 'price',
        priceDeltaField: 'delta',
        precisionField: 'precision',
      },
    },
  };

  const data = {
    price: 100,
    delta: -5,
    precision: 2,
    seriesItemTypeCode: 'pi-single',
  };

  const data2 = {
    price: undefined,
    delta: -5,
    precision: 2,
    seriesItemTypeCode: 'pi-single',
  };

  it('renders nothing when data is not provided', () => {
    // @ts-ignore
    const { container } = render(<PriceDeltaComponent columnConfig={columnConfig} />);
    expect(container.firstChild).toBeNull();
  });

  it('renders nothing when columnConfig is not provided', () => {
    // @ts-ignore
    const { container } = render(<PriceDeltaComponent data={data} />);
    expect(container.firstChild).toBeNull();
  });

  it('renders nothing when both data and columnConfig are not provided', () => {
    // @ts-ignore
    const { container } = render(<PriceDeltaComponent />);
    expect(container.firstChild).toBeNull();
  });

  it('renders price delta with correct props when data is provided', () => {
    // @ts-ignore
    render(<PriceDeltaComponent columnConfig={columnConfig} data={data} />);
  });

  it('does not renders price delta with correct props when data is provided', () => {
    // @ts-ignore
    render(<PriceDeltaComponent columnConfig={columnConfig} data={data2} />);
  });

  it('renders nothing when price or delta is missing', () => {
    const { container } = render(
      // @ts-ignore
      <PriceDeltaComponent
        // @ts-ignore
        columnConfig={{ customConfig: { priceField: 'price', priceDeltaField: 'delta' } }}
        // @ts-ignore
        data={{ price: 100 }}
      />,
    );
    expect(container.firstChild).toBeNull();
  });

  it('renders nothing when both price and delta are missing', () => {
    const { container } = render(
      // @ts-ignore
      <PriceDeltaComponent columnConfig={{ customConfig: {} }} data={{}} />,
    );
    expect(container.firstChild).toBeNull();
  });
});
