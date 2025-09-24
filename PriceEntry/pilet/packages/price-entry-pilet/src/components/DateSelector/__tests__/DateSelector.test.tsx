import { fireEvent, render, screen } from '@testing-library/react';
import { act } from 'react';
import DateSelector from '..';
import { TEST_IDS } from '../DateSelector.constants';
import { SelectedDate } from '../DateSelector.type';

describe('DateSelector', () => {
  const mockDate: SelectedDate = new Date('2023-01-31');

  const mockOnDateChange = jest.fn();
  const mockOnOutsideClick = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('renders date selector with single date', () => {
    render(<DateSelector date={new Date('2023-01-01')} />);

    expect(screen.getByTestId(TEST_IDS.DATE_SELECTOR)).toBeInTheDocument();
    expect(screen.getByTestId(TEST_IDS.DATE_CONTAINER)).toBeInTheDocument();
    expect(screen.getByText('01 Jan 2023')).toBeInTheDocument();
  });

  it('shows date picker when clicked and onDateChange is provided', () => {
    render(<DateSelector date={mockDate} onDateChange={mockOnDateChange} />);
    fireEvent.click(screen.getByTestId(TEST_IDS.DATE_SELECTOR));
    expect(screen.getByTestId(TEST_IDS.DATE_PICKER_CONTAINER)).toBeInTheDocument();
  });

  it('does not show date picker when clicked and onDateChange is not provided', () => {
    render(<DateSelector date={mockDate} />);

    fireEvent.click(screen.getByTestId(TEST_IDS.DATE_SELECTOR));
    expect(screen.queryByTestId(TEST_IDS.DATE_PICKER_CONTAINER)).not.toBeInTheDocument();
  });

  it('calls onOutsideClick when clicking outside', () => {
    render(
      <DateSelector
        date={mockDate}
        onOutsideClick={mockOnOutsideClick}
        onDateChange={mockOnDateChange}
      />,
    );

    fireEvent.click(screen.getByTestId(TEST_IDS.DATE_SELECTOR));
    fireEvent.click(document.body);

    expect(mockOnOutsideClick).toHaveBeenCalled();
  });

  it('updates picker position on window scroll and resize', () => {
    render(<DateSelector date={mockDate} onDateChange={mockOnDateChange} />);

    const scrollEvent = new Event('scroll');
    const resizeEvent = new Event('resize');

    act(() => {
      window.dispatchEvent(scrollEvent);
      window.dispatchEvent(resizeEvent);
    });
  });
});
