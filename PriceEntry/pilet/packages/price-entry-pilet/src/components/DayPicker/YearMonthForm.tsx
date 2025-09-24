// istanbul ignore file
import { Icon } from '@icis/ui-kit';
import { MouseEventHandler, useMemo, useState } from 'react';
import { useNavigation } from 'react-day-picker';
import { useClickOutsideListenerRef } from 'utils/hooks/useClickOutsideListenerRef';
import { DropdownWrapper, OptionWrapper, SelectBox, SelectBoxWrapper } from './Calendars.styled';
import { YearsOrder } from './DayPicker';
import { getMonths, getShortMonths } from './DayPicker.utils';

interface YearMonth {
  date: Date;
  fromMonth: Date;
  toMonth: Date;
  onChange: (date: any) => void;
  locale: string | undefined;
  yearsOrder?: YearsOrder;
}

const YearMonthForm = ({
  date,
  onChange,
  fromMonth,
  toMonth,
  locale,
  yearsOrder = 'ASC',
}: YearMonth) => {
  const monthShorts = getShortMonths(locale);
  const monthIndex = date.getMonth();
  const { goToMonth, nextMonth, previousMonth } = useNavigation();

  const years: number[] = useMemo(() => {
    const yearsTmp = [];
    for (let i = fromMonth.getFullYear(); i <= toMonth.getFullYear(); i += 1) {
      yearsTmp.push(i);
    }
    if (yearsOrder === 'DESC') {
      yearsTmp.reverse();
    }
    return yearsTmp;
  }, [fromMonth, toMonth, yearsOrder]);

  const handleMonthChange = (value: string | number) => {
    const i = monthShorts.indexOf(value as string);
    onChange(new Date(date.getFullYear(), i));
  };
  /* istanbul ignore next */
  const handleYearChange = (data: number | string) => {
    onChange(new Date(data as number, monthIndex));
  };

  const monthOptions = useMemo(() => {
    return getMonths(locale).map((monthLong: string, i: number) => ({
      key: monthShorts[i],
      value: monthLong,
    }));
  }, [locale, monthShorts]);

  const yearOptions = useMemo(() => {
    return years.map((year) => ({
      key: year,
      value: year,
    }));
  }, [years]);

  const handlePreviousClick: MouseEventHandler = () => {
    if (!previousMonth) return;
    goToMonth(previousMonth);
  };

  const handleNextClick: MouseEventHandler = () => {
    if (!nextMonth) return;
    goToMonth(nextMonth);
  };

  return (
    <form className='DayPicker-Caption'>
      <span
        data-testid={'month-navigation-previous'}
        onClick={handlePreviousClick}
        className={`NavButton NavButton--prev ${!previousMonth && 'disabled'}`}
        aria-label='Previous Month'></span>
      <DropdownSelection
        testid='month-selection'
        options={monthOptions}
        selectedKey={monthShorts[monthIndex]}
        onChange={handleMonthChange}
      />
      <DropdownSelection
        testid='year-selection'
        options={yearOptions}
        selectedKey={date.getFullYear()}
        onChange={handleYearChange}
      />
      <span
        data-testid={'month-navigation-next'}
        onClick={handleNextClick}
        className={`NavButton NavButton--next ${!nextMonth ? 'disabled' : ''}`}
        aria-label='Next Month'></span>
    </form>
  );
};

type DropdownSelectionProps = {
  testid: string;
  selectedKey: string | number;
  options: { key: string | number; value: string | number }[];
  onChange: (value: string | number) => void;
};

const DropdownSelection = ({ testid, selectedKey, options, onChange }: DropdownSelectionProps) => {
  const [showDropdown, setShowDropdown] = useState(false);
  const dropdownRef = useClickOutsideListenerRef(() => {
    setTimeout(() => {
      setShowDropdown(false);
    });
  });
  const selectedValue = options.find((o) => o.key === selectedKey)?.value;

  return (
    <SelectBoxWrapper data-testid={testid}>
      <SelectBox
        isOpen={showDropdown}
        data-testid={`${testid}-dropdown`}
        onClick={() => setShowDropdown(true)}>
        <span data-testid={`${testid}-value`}>{selectedValue}</span>
        <Icon icon={showDropdown ? 'chevron-up' : 'chevron-down'} />
      </SelectBox>
      {showDropdown && (
        <DropdownWrapper data-testid='options-wrapper' ref={dropdownRef}>
          {options.map(({ key }, i) => (
            <OptionWrapper
              key={i}
              selected={key === selectedKey}
              isLast={i === options.length - 1}
              onClick={() => onChange(key)}
              data-testid={`option-${key}`}>
              {key}
            </OptionWrapper>
          ))}
        </DropdownWrapper>
      )}
    </SelectBoxWrapper>
  );
};

export default YearMonthForm;
