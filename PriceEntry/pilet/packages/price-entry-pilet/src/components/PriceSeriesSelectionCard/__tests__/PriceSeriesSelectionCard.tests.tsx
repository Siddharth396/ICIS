import React from 'react';
import { render, fireEvent } from '@testing-library/react';
import PriceSeriesSelectionCard from '../PriceSeriesSelectionCard';

describe('PriceSeriesSelectionCard Component', () => {
  it('renders with provided buttonText', () => {
    const buttonText = 'Select price series';
    const testId = 'price-series-selection-card';
    const { getByText } = render(
      <PriceSeriesSelectionCard testId={testId} buttonText={buttonText} onButtonClick={() => {}} />,
    );
    const buttonElement = getByText(buttonText);
    expect(buttonElement).toBeInTheDocument();
  });

  it('calls onButtonClick when button is clicked', () => {
    const buttonText = 'Select price series';
    const testId = 'price-series-selection-card';
    const onButtonClick = jest.fn();
    const { getByTestId } = render(
      <PriceSeriesSelectionCard
        testId={testId}
        buttonText={buttonText}
        onButtonClick={onButtonClick}
      />,
    );
    const buttonElement = getByTestId(`${testId}-add-button`);
    fireEvent.click(buttonElement);
    expect(onButtonClick).toHaveBeenCalledTimes(1);
  });
});
