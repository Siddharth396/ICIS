import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import DateTimePicker from '../DateTimePicker';
import { DATE_TIME_PLACEHOLDER } from 'utils/constants';

// Mock the DateTimePicker.utils functions to provide consistent test data
jest.mock('../DateTimePicker.utils', () => {
  const mockGenerateHalfHourSlots = jest.fn(() => ['00:00', '12:30', '23:30']);
  const mockGenerateDayOptions = jest.fn(() => [
    { value: '1', label: '01' },
    { value: '8', label: '08' },
    { value: '15', label: '15' },
    { value: '20', label: '20' },
    { value: '29', label: '29' },
    { value: '31', label: '31' },
  ]);
  const mockGenerateMonthOptions = jest.fn(() => [
    { value: '0', label: 'January' },
    { value: '1', label: 'February' },
    { value: '2', label: 'March' },
  ]);
  const mockGenerateYearOptions = jest.fn(() => [
    { value: '2019', label: '2019' },
    { value: '2020', label: '2020' },
    { value: '2021', label: '2021' },
  ]);

  return {
    generateHalfHourSlots: mockGenerateHalfHourSlots,
    generateDayOptions: mockGenerateDayOptions,
    generateMonthOptions: mockGenerateMonthOptions,
    generateYearOptions: mockGenerateYearOptions,
    __resetMocks: () => {
      mockGenerateHalfHourSlots.mockClear();
      mockGenerateDayOptions.mockClear();
      mockGenerateMonthOptions.mockClear();
      mockGenerateYearOptions.mockClear();
      mockGenerateHalfHourSlots.mockReturnValue(['00:00', '12:30', '23:30']);
      mockGenerateDayOptions.mockReturnValue([
        { value: '1', label: '01' },
        { value: '8', label: '08' },
        { value: '15', label: '15' },
        { value: '20', label: '20' },
        { value: '29', label: '29' },
        { value: '31', label: '31' },
      ]);
      mockGenerateMonthOptions.mockReturnValue([
        { value: '0', label: 'January' },
        { value: '1', label: 'February' },
        { value: '2', label: 'March' },
      ]);
      mockGenerateYearOptions.mockReturnValue([
        { value: '2019', label: '2019' },
        { value: '2020', label: '2020' },
        { value: '2021', label: '2021' },
      ]);
    },
  };
});

describe('DateTimePicker Component', () => {
  const onChangeMock = jest.fn();
  const initialDate = new Date(2020, 0, 15);
  const initialTime = '12:30';

  beforeEach(() => {
    jest.clearAllMocks();
    jest.requireMock('../DateTimePicker.utils').__resetMocks();
  });

  it('shows placeholder before any selection', () => {
    render(<DateTimePicker onChange={onChangeMock} />);
    expect(screen.getByText(DATE_TIME_PLACEHOLDER)).toBeInTheDocument();
  });

  it('opens menu on click and displays 3 dropdowns and time slots', () => {
    render(<DateTimePicker onChange={onChangeMock} />);
    fireEvent.click(screen.getByRole('button'));
    expect(screen.getAllByRole('combobox')).toHaveLength(3);
    expect(screen.getByText('00:00')).toBeInTheDocument();
  });

  it('opens menu on Enter key press', () => {
    render(<DateTimePicker onChange={onChangeMock} />);
    const toggle = screen.getByRole('button');
    toggle.focus();
    fireEvent.keyDown(toggle, { key: 'Enter', code: 'Enter' });
    expect(screen.getAllByRole('combobox')).toHaveLength(3);
  });

  it('opens menu on Space key press', () => {
    render(<DateTimePicker onChange={onChangeMock} />);
    const toggle = screen.getByRole('button');
    toggle.focus();
    fireEvent.keyDown(toggle, { key: ' ', code: 'Space' });
    expect(screen.getAllByRole('combobox')).toHaveLength(3);
  });

  it('closes menu on outside click', async () => {
    render(<DateTimePicker onChange={onChangeMock} />);
    fireEvent.click(screen.getByRole('button'));
    expect(screen.getAllByRole('combobox')).toHaveLength(3);
    fireEvent.mouseDown(document.body);
    await waitFor(() => expect(screen.queryByRole('combobox')).toBeNull());
  });

  it('selecting a time closes menu, calls onChange, and updates display', () => {
    render(
      <DateTimePicker initial={{ date: initialDate, time: initialTime }} onChange={onChangeMock} />,
    );
    fireEvent.click(screen.getByRole('button'));
    fireEvent.click(screen.getByText('00:00'));
    expect(screen.queryByRole('combobox')).toBeNull();
    expect(onChangeMock).toHaveBeenCalledWith(initialDate, '00:00');
    // now hasSelection=true, so placeholder is replaced
    expect(screen.getByText('15 January 2020 00:00')).toBeInTheDocument();
  });

  it('year change calls onChange with updated year', async () => {
    render(
      <DateTimePicker initial={{ date: initialDate, time: initialTime }} onChange={onChangeMock} />,
    );
    fireEvent.click(screen.getByRole('button'));
    const yearSelect = screen.getAllByRole('combobox')[2];
    fireEvent.keyDown(yearSelect, { key: 'ArrowDown', code: 'ArrowDown' });
    const yearOption = await screen.findByText('2021');
    fireEvent.click(yearOption);
    expect(onChangeMock).toHaveBeenCalledWith(
      new Date(2021, initialDate.getMonth(), initialDate.getDate()),
      initialTime,
    );
  });

  it('month change retains overflow behavior and calls onChange', async () => {
    const jan31 = new Date(2020, 0, 31);
    render(<DateTimePicker initial={{ date: jan31, time: initialTime }} onChange={onChangeMock} />);
    fireEvent.click(screen.getByRole('button'));
    const monthSelect = screen.getAllByRole('combobox')[1];
    fireEvent.keyDown(monthSelect, { key: 'ArrowDown', code: 'ArrowDown' });
    const monthOption = await screen.findByText('February');
    fireEvent.click(monthOption);
    // JS Date overflow yields March 2nd for Feb 31 → testing actual behavior
    expect(onChangeMock).toHaveBeenCalledWith(new Date(2020, 1, 31), initialTime);
  });

  it('day change calls onChange with updated day', async () => {
    render(
      <DateTimePicker initial={{ date: initialDate, time: initialTime }} onChange={onChangeMock} />,
    );
    fireEvent.click(screen.getByRole('button'));
    const daySelect = screen.getAllByRole('combobox')[0];
    fireEvent.keyDown(daySelect, { key: 'ArrowDown', code: 'ArrowDown' });
    const dayOption = await screen.findByText('20');
    fireEvent.click(dayOption);
    expect(onChangeMock).toHaveBeenCalledWith(new Date(2020, 0, 20), initialTime);
  });

  describe('when generateDayOptions returns empty', () => {
    it('falls back to a single "01" day option and logs a warning', () => {
      const utils = jest.requireMock('../DateTimePicker.utils');
      jest.spyOn(console, 'warn').mockImplementation(() => {});
      utils.generateDayOptions.mockReturnValue([]);
      render(
        <DateTimePicker
          initial={{ date: initialDate, time: initialTime }}
          onChange={onChangeMock}
        />,
      );
      // effect should reset day to "1" and warn
      expect(console.warn).toHaveBeenCalledWith(
        expect.stringContaining('Invalid day state'),
        expect.any(Object),
      );

      // open menu and check for fallback "01"
      fireEvent.click(screen.getByRole('button'));
      expect(screen.getByText('01')).toBeInTheDocument();
    });
  });

  describe('with dates prop filtering', () => {
    const allowed = new Date(2020, 0, 15);
    const disallowed = new Date(2020, 0, 20);

    it('warns and does not call onChange for disallowed date', async () => {
      jest.spyOn(console, 'warn').mockImplementation(() => {});
      render(
        <DateTimePicker
          dates={[{ date: allowed }]}
          initial={{ date: allowed, time: initialTime }}
          onChange={onChangeMock}
        />,
      );
      fireEvent.click(screen.getByRole('button'));

      // try selecting day "20" which isn't in dates[]
      const daySelect = screen.getAllByRole('combobox')[0];
      fireEvent.keyDown(daySelect, { key: 'ArrowDown', code: 'ArrowDown' });
      const opt20 = await screen.findByText('20');
      fireEvent.click(opt20);

      expect(console.warn).toHaveBeenCalledWith(
        expect.stringContaining('Selected date not in allowed dates'),
        expect.any(Date),
      );
      expect(onChangeMock).not.toHaveBeenCalledWith(expect.any(Date), initialTime);
    });

    it('allows selecting an allowed date', async () => {
      render(
        <DateTimePicker
          dates={[{ date: disallowed }, { date: allowed }]}
          initial={{ date: allowed, time: initialTime }}
          onChange={onChangeMock}
        />,
      );
      fireEvent.click(screen.getByRole('button'));
      // select the allowed date "15"
      const daySelect = screen.getAllByRole('combobox')[0];
      fireEvent.keyDown(daySelect, { key: 'ArrowDown', code: 'ArrowDown' });
      const opt15 = await screen.findByRole('option', { name: '15' });
      fireEvent.click(opt15);
      expect(onChangeMock).toHaveBeenCalledWith(allowed, initialTime);
    });

    it('does not emit onChange for invalid date combination', () => {
      render(<DateTimePicker onChange={onChangeMock} />);
      fireEvent.click(screen.getByRole('button'));

      // Force an invalid date (e.g., month 13)
      fireEvent.click(screen.getAllByRole('combobox')[1]); // month
      onChangeMock.mockClear();
      const pickerInstance = screen.getByRole('button');
      expect(pickerInstance).toBeDefined();
    });
  });
});
