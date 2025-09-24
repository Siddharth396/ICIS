// istanbul ignore file
import React, { useState, useEffect, useRef } from 'react';
import { IOption, Select } from '@icis/ui-kit';
import { DATE_TIME_PLACEHOLDER } from 'utils/constants';
import {
  DatePickerContainer,
  SelectionBox,
  SelectionBoxValue,
  SelectionBoxIcon,
  MenuWrapper,
  HeaderWrapper,
  ListWrapper,
  List,
} from './styled';
import {
  generateYearOptions,
  generateMonthOptions,
  generateDayOptions,
  generateHalfHourSlots,
} from './DateTimePicker.utils';
import { Locale } from './DatePicker.types';

interface DateTimePickerProps {
  locale?: Locale;
  initial?: { date: Date; time: string };
  dates?: { date: Date }[];
  onChange: (date: Date, time: string) => void;
}

const DateTimePicker: React.FC<DateTimePickerProps> = ({
  locale = 'en',
  initial,
  dates,
  onChange,
}) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const [showMenu, setShowMenu] = useState(false);
  const [hasSelection, setHasSelection] = useState(false); // Always false initially

  // Initialize from initial or first valid date from dates or now
  const base = initial?.date ?? (dates && dates[0]?.date) ?? new Date();
  const [year, setYear] = useState(base.getFullYear().toString());
  const [month, setMonth] = useState(base.getMonth().toString());
  const [day, setDay] = useState(base.getDate().toString());
  const timeSlots = generateHalfHourSlots();
  const [time, setTime] = useState(initial?.time ?? timeSlots[0]);

  // Build dropdown data
  const years = generateYearOptions();
  const months = generateMonthOptions(locale);
  const days = generateDayOptions(year, month);

  // Fallbacks for dropdowns
  const safeDays = days.length > 0 ? days : [{ value: '1', label: '01' }];
  const safeMonths = months.length > 0 ? months : [{ value: '0', label: 'January' }];
  const safeYears =
    years.length > 0
      ? years
      : [
          {
            value: new Date().getFullYear().toString(),
            label: new Date().getFullYear().toString(),
          },
        ];

  const currentDayOption = safeDays.find((o) => o.value === day);
  const validDay = currentDayOption ? day : (safeDays[0]?.value ?? '1');

  useEffect(() => {
    if (!currentDayOption) {
      console.warn('Invalid day state:', { day, safeDays });
      setDay(validDay); // Reset day if invalid
    }
  }, [day, safeDays, validDay]);

  // Close on outside click
  useEffect(() => {
    if (!showMenu) return;
    const onBodyClick = (e: MouseEvent) => {
      const target = e.target as HTMLElement;
      if (containerRef.current?.contains(target)) return;
      if (target.closest('.select__menu')) return;
      setShowMenu(false);
    };
    document.addEventListener('mousedown', onBodyClick);
    return () => document.removeEventListener('mousedown', onBodyClick);
  }, [showMenu]);

  const toggleMenu = () => setShowMenu((v) => !v);

  // Validate and emit change
  const emit = (
    selectedYearString: string,
    selectedMonthString: string,
    selectedDayString: string | undefined,
    selectedTime: string,
  ) => {
    // istanbul ignore if
    if (!selectedDayString) {
      console.error('Day is undefined:', {
        selectedYearString,
        selectedMonthString,
        selectedDayString,
      });
      return;
    }

    const selectedYear = parseInt(selectedYearString, 10);
    const selectedMonth = parseInt(selectedMonthString, 10);
    const selectedDay = parseInt(selectedDayString, 10);

    // istanbul ignore if
    if (
      isNaN(selectedYear) ||
      isNaN(selectedMonth) ||
      isNaN(selectedDay) ||
      selectedMonth < 0 ||
      selectedMonth > 11 ||
      selectedDay < 1
    ) {
      console.error('Invalid date inputs:', {
        selectedYearString,
        selectedMonthString,
        selectedDayString,
      });
      return;
    }

    const selectedDate = new Date(selectedYear, selectedMonth, selectedDay);

    // istanbul ignore if
    if (isNaN(selectedDate.getTime())) {
      console.error('Invalid Date created:', {
        selectedYearString,
        selectedMonthString,
        selectedDayString,
      });
      return;
    }

    if (dates) {
      const isValidDate = dates.some(
        (allowedDate) =>
          allowedDate.date.getFullYear() === selectedYear &&
          allowedDate.date.getMonth() === selectedMonth &&
          allowedDate.date.getDate() === selectedDay,
      );

      if (!isValidDate) {
        console.warn('Selected date not in allowed dates:', selectedDate);
        return;
      }
    }

    onChange(selectedDate, selectedTime);
  };

  // Handlers
  const onYearChange = (option: IOption | string) => {
    const selectedYear = typeof option === 'string' ? option : option?.value;

    // istanbul ignore if
    if (!selectedYear) {
      console.error('Invalid year selection:', option);
      return;
    }

    setYear(selectedYear);

    const updatedDayOptions = generateDayOptions(selectedYear, month);
    const selectedDay =
      updatedDayOptions.find((option) => option.value === day)?.value ??
      updatedDayOptions[0]?.value ??
      '1';

    setDay(selectedDay);
    emit(selectedYear, month, selectedDay, time);
  };

  const onMonthChange = (option: IOption | string) => {
    const selectedMonth = typeof option === 'string' ? option : option?.value;

    // istanbul ignore if
    if (!selectedMonth) {
      console.error('Invalid month selection:', option);
      return;
    }

    setMonth(selectedMonth);

    const updatedDayOptions = generateDayOptions(year, selectedMonth);
    const selectedDay =
      updatedDayOptions.find((option) => option.value === day)?.value ??
      updatedDayOptions[0]?.value ??
      '1';

    setDay(selectedDay);
    emit(year, selectedMonth, selectedDay, time);
  };

  const onDayChange = (option: IOption | string) => {
    const selectedDay = typeof option === 'string' ? option : option?.value;

    // istanbul ignore if
    if (!selectedDay) {
      console.error('Invalid day selection:', option);
      return;
    }

    setDay(selectedDay);
    emit(year, month, selectedDay, time);
  };

  const onTime = (slot: string) => {
    setTime(slot);
    setShowMenu(false);
    setHasSelection(true); // Mark selection as made when time is selected
    emit(year, month, day, slot);
  };

  // Display value
  const display = hasSelection
    ? [
        validDay.padStart(2, '0'),
        safeMonths.find((m) => m.value === month)?.label ?? 'Jan',
        year,
        time,
      ].join(' ')
    : DATE_TIME_PLACEHOLDER;

  const handleKeyDown = (e: React.KeyboardEvent<HTMLDivElement>) => {
    if (e.key === 'Enter' || e.key === ' ') {
      e.preventDefault();
      toggleMenu();
    }
  };
  return (
    <DatePickerContainer ref={containerRef}>
      <SelectionBox
        role='button'
        tabIndex={0}
        menuIsOpen={showMenu}
        onClick={toggleMenu}
        onKeyDown={handleKeyDown}>
        <SelectionBoxValue>{display}</SelectionBoxValue>
        <SelectionBoxIcon icon={showMenu ? 'chevron-up' : 'chevron-down'} />
      </SelectionBox>

      {showMenu && (
        <MenuWrapper>
          <HeaderWrapper>
            <Select
              options={safeDays}
              value={safeDays.find((o) => o.value === validDay) || safeDays[0]}
              onChange={onDayChange}
              variant='clarity'
            />
            <Select
              options={safeMonths}
              value={safeMonths.find((o) => o.value === month) || safeMonths[0]}
              onChange={onMonthChange}
              variant='clarity'
            />
            <Select
              options={safeYears}
              value={safeYears.find((o) => o.value === year) || safeYears[0]}
              onChange={onYearChange}
              variant='clarity'
            />
          </HeaderWrapper>

          <ListWrapper>
            {timeSlots.map((slot) => (
              <List key={slot} selected={slot === time} onClick={() => onTime(slot)}>
                {slot}
              </List>
            ))}
          </ListWrapper>
        </MenuWrapper>
      )}
    </DatePickerContainer>
  );
};

export default DateTimePicker;
