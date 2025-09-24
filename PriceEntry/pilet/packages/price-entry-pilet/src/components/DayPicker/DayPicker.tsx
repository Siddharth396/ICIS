import { Icon, ModalCalendarProps, theme } from '@icis/ui-kit';
import { useEffect, useState } from 'react';
import { CaptionProps, Matcher, DayPicker as ReactDayPicker } from 'react-day-picker';
import { PickerContent } from './Calendars.styled';
import { Spinner } from './styled';
import {
  formatWeekdayName,
  getAllDisabledDays,
  getBaseLocale,
  getFirstDayOfWeek,
  getSelectedDate,
} from './DayPicker.utils';
import ModalCalendar from './ModalCalendar';
import YearMonthForm from './YearMonthForm';
import usePrevious from 'utils/hooks/usePrevious';
import { isSameDay } from 'date-fns';

export type Position = { top?: string; bottom?: string; left?: string; right?: string };
export type YearsOrder = 'ASC' | 'DESC';

export type DayPickerProps = {
  disabled?: boolean;
  locale?: string;
  disabledDays?: Matcher[];
  enabledDays?: Date[];
  dateInputField?: boolean;
  selectedDate?: Date | undefined;
  showLoader?: boolean;
  highlightedDates?: Date[];
  yearsOrder?: YearsOrder;
  onDateSelect: (date?: Date) => void;
  onMonthChange?: (month: Date) => void;
} & ModalCalendarProps;

const DayPicker = ({
  disabled,
  locale,
  testId,
  disabledDays,
  enabledDays,
  highlightedDates = [],
  header = { visible: false },
  withModal = false,
  showModal = false,
  showLoader = false,
  selectedDate,
  yearsOrder = 'ASC',
  onDateSelect,
  onModalDismiss,
  onMonthChange,
}: DayPickerProps) => {
  const currentDate = new Date();
  const currentYear = currentDate.getFullYear();
  const modifiers = {
    // highlight these event dates
    eventDay: highlightedDates,

    // everything else: if a date is NOT in eventDates
    nonEventDay: (date: Date) => {
      // Return true if it's NOT an event day
      // istanbul ignore next
      return !highlightedDates.some((evtDate) => isSameDay(evtDate, date));
    },
  };
  // from 5 years in the past (January)
  const fromMonth = new Date(currentYear - 5, 0);
  // to 5 years in the future (December)
  const toMonth = new Date(currentYear + 5, 11);
  const previousSelectedDate = usePrevious<Date | undefined>(selectedDate);
  const [selectedDay, setSelectedDay] = useState<Date | undefined>(selectedDate);
  // istanbul ignore next
  const [month, setMonth] = useState(getSelectedDate(selectedDate) || toMonth);

  let calculatedDisabledDays: ReturnType<typeof getAllDisabledDays> = disabledDays || [];

  // istanbul ignore next
  if (enabledDays?.length || disabledDays?.length) {
    calculatedDisabledDays = getAllDisabledDays(enabledDays, disabledDays, undefined);
  }

  const modifiersStyles = {
    // uncomment this when holiday calendar is available
    // eventDay: {
    //   color: theme.colours.NEUTRALS_2,
    // },
    // uncomment this when holiday calendar is available
    // nonEventDay: {
    //   color: theme.colours.NEUTRALS_4,
    // },
    selected: {
      backgroundColor: theme.colours.SELECTED,
    },
  };

  // istanbul ignore next
  useEffect(() => {
    if (selectedDate && selectedDate !== previousSelectedDate && selectedDate !== selectedDay) {
      setSelectedDay(selectedDate);
    }
  }, [selectedDate, selectedDay, previousSelectedDate]);

  useEffect(() => {
    onDateSelect(selectedDay);
  }, [onDateSelect, selectedDay]);

  const CustomCaption = (props: CaptionProps) => {
    return (
      <YearMonthForm
        date={props.displayMonth}
        locale={locale}
        fromMonth={fromMonth}
        toMonth={toMonth}
        onChange={handleYearMonthChange}
        yearsOrder={yearsOrder}
      />
    );
  };

  // istanbul ignore next
  const handleYearMonthChange = (monthEvent: Date) => {
    let selectedMonth;
    if (monthEvent < fromMonth) {
      selectedMonth = fromMonth;
    } else if (monthEvent > toMonth) {
      selectedMonth = toMonth;
    } else {
      selectedMonth = monthEvent;
    }
    setMonth(selectedMonth);
    onMonthChange?.(monthEvent);
  };

  return (
    <>
      <ModalCalendar
        testId={testId}
        header={header}
        withModal={withModal}
        showModal={showModal}
        onModalDismiss={onModalDismiss}>
        <PickerContent dayPicker={true} showLoader={showLoader}>
          <ReactDayPicker
            mode='single'
            month={month}
            selected={selectedDay}
            formatters={{ formatWeekdayName }}
            showOutsideDays
            disabled={disabled || calculatedDisabledDays}
            fromMonth={fromMonth}
            toMonth={toMonth}
            modifiers={modifiers}
            modifiersStyles={modifiersStyles}
            locale={getBaseLocale(locale)}
            onDayClick={setSelectedDay}
            onMonthChange={handleYearMonthChange}
            components={{ Caption: CustomCaption }}
            weekStartsOn={getFirstDayOfWeek(locale)}
          />
          {showLoader && (
            /* istanbul ignore next */ <Spinner>
              <Icon size='2x' className='fa-spin' icon='circle-notch' />
            </Spinner>
          )}
        </PickerContent>
      </ModalCalendar>
    </>
  );
};

export default DayPicker;
