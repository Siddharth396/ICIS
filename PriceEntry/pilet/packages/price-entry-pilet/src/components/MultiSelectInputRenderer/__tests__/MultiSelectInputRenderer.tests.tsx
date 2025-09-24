import { render, fireEvent, screen } from '@testing-library/react';
import MultiSelectInputRenderer from '..';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import { IMultiOption } from '../MultiSelectInputRenderer';

describe('MultiSelectInputRenderer', () => {
  const options: IMultiOption[] = [
    { id: 'id1', label: 'Option 1', value: 'option1' },
    { id: 'id2', label: 'Option 2', value: 'option2' },
    { id: 'id3', label: 'Option 3', value: 'option3' },
  ];

  const selected: IMultiOption[] = [
    { id: 'id1', label: 'Option 1', value: 'option1' },
  ];

  const dummyProps = {
    placeholder: 'Select Commodity',
  }

  const mockOnChange = jest.fn();

  it('renders correctly with the given options', () => {
    const { getByTestId } = render(
      <MultiSelectInputRenderer
        options={options}
        value={[]}
        onChange={mockOnChange}
        testId='test-select'
        placeholder={dummyProps.placeholder ? dummyProps.placeholder : 'Please select'}
      />,
    );

    expect(getByTestId('test-select')).toBeInTheDocument();
  });

  it('displays the placeholder text when no options are selected', () => {
    const messages = useLocaleMessages();
    render(<MultiSelectInputRenderer options={options} value={[]} onChange={mockOnChange} testId='multi-select' placeholder={dummyProps.placeholder ? dummyProps.placeholder : messages.General.SelectPlaceholder} />);
    expect(screen.getByTestId('multi-select')).toBeInTheDocument();
  });


  it('displays the default placeholder text when no options are selected', () => {
    render(<MultiSelectInputRenderer options={options} value={[]} onChange={mockOnChange} testId='multi-select' placeholder={undefined} />);
    expect(screen.getByTestId('multi-select')).toBeInTheDocument();
  });

  it('calls onChange when an option is selected', () => {
    render(<MultiSelectInputRenderer options={options} value={selected} onChange={mockOnChange} testId='multi-select' placeholder={dummyProps.placeholder ? dummyProps.placeholder : 'Please select'} />);
    fireEvent.mouseDown(screen.getByTestId('multi-select'));
    fireEvent.click(screen.getByText('Option 1'));
    fireEvent.click(screen.getByText('Option 2'));
    expect(mockOnChange).toHaveBeenCalledWith([
      { id: 'id1', label: 'Option 1', value: 'option1' },
      { id: 'id2', label: 'Option 2', value: 'option2' },
    ]);

  });

  it('renders with the correct selected value', () => {
    const selectedOption: IMultiOption = { id: 'id', label: 'Option 2', value: 'option2' };

    render(
      <MultiSelectInputRenderer
        options={options}
        value={[selectedOption]}
        onChange={mockOnChange}
        testId='test-select'
        placeholder={dummyProps.placeholder ? dummyProps.placeholder : 'Please select'}
      />,
    );

    // The selected value should be displayed in the input field
    expect(screen.getByText('Option 2')).toBeInTheDocument();
  });

  it('renders with an empty value when value is null', () => {
    render(
      <MultiSelectInputRenderer
        options={options}
        value={undefined}
        onChange={mockOnChange}
        testId="multi-select"
        placeholder={dummyProps.placeholder ? dummyProps.placeholder : 'Please select'}
      />
    );

    expect(screen.getByTestId('multi-select')).toBeInTheDocument();
    expect(screen.queryByText('Option 1')).not.toBeInTheDocument();
  });
});
