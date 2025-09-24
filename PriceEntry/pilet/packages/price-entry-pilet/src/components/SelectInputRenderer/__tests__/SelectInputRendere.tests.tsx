import React from 'react';
import { render, fireEvent, screen } from '@testing-library/react';
import SelectInputRenderer from '..';
import { IOption } from '@icis/ui-kit';

describe('SelectInputRenderer', () => {
  const options: IOption[] = [
    { label: 'Option 1', value: 'option1' },
    { label: 'Option 2', value: 'option2' },
    { label: 'Option 3', value: 'option3' },
  ];

  const mockOnChange = jest.fn();

  it('renders correctly with the given options', () => {
    render(
      <SelectInputRenderer
        options={options}
        value={undefined}
        onChange={mockOnChange}
        testId='test-select'
      />,
    );

    expect(screen.getByRole('combobox')).toBeInTheDocument();
  });

  it('calls onChange with the correct value when an option is selected', () => {
    render(
      <SelectInputRenderer
        options={options}
        value={undefined}
        onChange={mockOnChange}
        testId='test-select'
      />,
    );

    fireEvent.mouseDown(screen.getByRole('combobox'));
    fireEvent.click(screen.getByText('Option 2'));

    expect(mockOnChange).toHaveBeenCalledWith({
      label: 'Option 2',
      value: 'option2',
    });
  });

  it('renders with the correct selected value', () => {
    render(
      <SelectInputRenderer
        options={options}
        value={{ label: 'Option 2', value: 'option2' }}
        onChange={mockOnChange}
        testId='test-select'
      />,
    );

    expect(screen.getByText('Option 2')).toBeInTheDocument();
  });

  it('displays the placeholder text when no options are selected', () => {
    render(
      <SelectInputRenderer
        options={options}
        value={undefined}
        onChange={mockOnChange}
        testId='test-select'
        placeholder='Select Commodity'
      />,
    );

    expect(screen.getByText('Select Commodity')).toBeInTheDocument();
  });

  it('displays the default placeholder if none is provided', () => {
    render(
      <SelectInputRenderer
        options={options}
        value={undefined}
        onChange={mockOnChange}
        testId='test-select'
        placeholder={undefined}
      />,
    );

    expect(screen.getByRole('combobox')).toBeInTheDocument();
  });

  it('does not render tooltip when not disabled', () => {
    render(
      <SelectInputRenderer
        options={options}
        value={undefined}
        onChange={mockOnChange}
        testId='test-select'
        disabled={false}
      />,
    );

    const tooltip = screen.queryByText(/You can only select options matching the criteria/i);
    expect(tooltip).not.toBeInTheDocument();
  });
  it('renders tooltip wrapper and message when disabled', () => {
    render(
      <SelectInputRenderer
        options={options}
        value={undefined}
        onChange={mockOnChange}
        testId='test-select'
        disabled={true}
      />,
    );

    // wrapper is the parent of the test-select container
    const wrapper = screen.getByTestId('test-select').parentElement!;
    expect(wrapper).toHaveAttribute('data-tip', 'true');
    expect(wrapper).toHaveAttribute('data-for', 'disabled-select-tooltip');

    // and the tooltip content itself should appear
    expect(
      screen.getByText(
        /You can only select options matching the criteria shown\. To change this, clear your current selection\./,
      ),
    ).toBeInTheDocument();
  });
});
