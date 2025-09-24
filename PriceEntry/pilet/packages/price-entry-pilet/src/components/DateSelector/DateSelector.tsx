import { Icon } from '@icis/ui-kit';
import DayPicker from 'components/DayPicker';
import { memo, useCallback, useEffect, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import { TEST_IDS } from './DateSelector.constants';
import {
  DateContainer,
  DatePickerContainer,
  DateSelectorContainer,
  DateWrapper,
  IconWrapper,
} from './DateSelector.styles';
import { DateSelectorProps, PickerPosition, SelectedDate } from './DateSelector.type';
import {
  compareDates,
  convertToUtcDate,
  formatDateValue,
  handlePickerPosition,
  handlePropagation,
  parseDate,
  useOutsideClick,
} from './DateSelector.utils';

// #region Component Definition
const DateSelector = memo(
  ({
    date,
    disabled,
    highlightedDates,
    onDateChange,
    onOutsideClick,
    onMonthChange,
  }: DateSelectorProps) => {
    // #region Hooks and State
    const wrapperRef = useRef<HTMLDivElement>(null);
    const pickerRef = useRef<HTMLDivElement>(null);
    const [pickerPosition, setPickerPosition] = useState<PickerPosition>();
    const [showPicker, setShowPicker] = useState<boolean>(false);
    const [selectedDate, setSelectedDate] = useState<SelectedDate>(parseDate(date));
    // #endregion

    // #region Callbacks and Handlers
    const updatePickerPosition = useCallback(() => {
      /* istanbul ignore else -- wrapperRef value get populated in first render only.*/
      if (wrapperRef.current) {
        const wrapper = wrapperRef.current.getBoundingClientRect();
        setPickerPosition(handlePickerPosition(wrapper));
      }
    }, []);

    // istanbul ignore next
    const handleDateChange = useCallback(
      (date: SelectedDate) => {
        if (compareDates(date, selectedDate)) return;
        const convertedDate = convertToUtcDate(date);
        setSelectedDate(convertedDate);
        onDateChange?.(convertedDate);
        setShowPicker(false);
      },
      [selectedDate, onDateChange],
    );

    const handleShowPicker = useCallback(
      (event: React.MouseEvent<HTMLDivElement, MouseEvent>) => {
        handlePropagation(event);
        if (disabled || !onDateChange) return; // Prevent showing picker if disabled
        setShowPicker(true);
      },
      [onDateChange, disabled],
    );
    // #endregion

    // #region Effects
    useOutsideClick([wrapperRef, pickerRef], () => {
      setShowPicker(false);
      onOutsideClick?.();
    });

    useEffect(() => {
      updatePickerPosition();
      window.addEventListener('scroll', updatePickerPosition);
      window.addEventListener('resize', updatePickerPosition);
      return () => {
        window.removeEventListener('scroll', updatePickerPosition);
        window.removeEventListener('resize', updatePickerPosition);
      };
    }, [updatePickerPosition]);
    // #endregion

    // #region Render Helpers
    const [fromDate] = formatDateValue(selectedDate);
    // #endregion

    // #region Render
    return (
      <DateSelectorContainer
        tabIndex={0}
        ref={wrapperRef}
        disabled={disabled}
        onClick={handleShowPicker}
        data-testid={TEST_IDS.DATE_SELECTOR}>
        <DateContainer data-testid={TEST_IDS.DATE_CONTAINER}>
          <DateWrapper data-testid={TEST_IDS.DATE_WRAPPER}>{fromDate}</DateWrapper>
        </DateContainer>
        <IconWrapper data-testid={TEST_IDS.ICON_WRAPPER}>
          <Icon testId={TEST_IDS.CALENDAR_ICON} icon='calendar' />
        </IconWrapper>
        {showPicker &&
          pickerPosition &&
          createPortal(
            <DatePickerContainer
              ref={pickerRef}
              position={pickerPosition}
              data-testid={TEST_IDS.DATE_PICKER_CONTAINER}>
              <DayPicker
                disabled={disabled}
                selectedDate={selectedDate}
                highlightedDates={highlightedDates}
                onDateSelect={handleDateChange}
                onMonthChange={onMonthChange}
              />
            </DatePickerContainer>,
            document.body,
          )}
      </DateSelectorContainer>
    );
    // #endregion
  },
);
// #endregion

DateSelector.displayName = 'DateSelector';

export default DateSelector;
