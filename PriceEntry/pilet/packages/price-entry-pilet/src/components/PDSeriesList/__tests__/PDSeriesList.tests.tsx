import { render } from '@testing-library/react';
import PDSeriesList from '../PDSeriesList';

describe('PDSeriesList Component', () => {
  const seriesData = [
    { id: '1', name: 'Series 1' },
    { id: '2', name: 'Series 2' },
    { id: '3', name: 'Series 3' },
  ];

  it('renders with series data', () => {
    const onSeriesSelect = jest.fn();
    const { getByText } = render(
      // @ts-ignore
      <PDSeriesList seriesData={seriesData} onSeriesSelect={onSeriesSelect} />,
    );

    expect(getByText('Series 1')).toBeInTheDocument();
    expect(getByText('Series 2')).toBeInTheDocument();
    expect(getByText('Series 3')).toBeInTheDocument();
  });
});
