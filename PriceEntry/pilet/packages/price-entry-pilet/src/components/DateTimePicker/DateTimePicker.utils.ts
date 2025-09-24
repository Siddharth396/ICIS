import { getDaysInMonth } from 'date-fns';
import { IOption } from '@icis/ui-kit';
import { Locale } from './DatePicker.types';

export const MONTHS_SHORT: Record<Locale, string[]> = {
  en: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
  zh: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
};

export const generateYearOptions = (): IOption[] => {
  const current = new Date().getFullYear();
  const start = current - 3;
  const end = current + 3;
  const opts: IOption[] = [];
  for (let y = start; y <= end; y++) {
    opts.push({ label: y.toString(), value: y.toString() });
  }
  return opts;
};

export const generateMonthOptions = (locale: Locale): IOption[] =>
  MONTHS_SHORT[locale].map((name, idx) => ({
    label: name,
    value: idx.toString(),
  }));

export const generateDayOptions = (yearString: string, monthString: string): IOption[] => {
  const parsedYear = parseInt(yearString, 10);
  const parsedMonth = parseInt(monthString, 10);

  // Fallback to current year/month if invalid
  const safeYear = isNaN(parsedYear) ? new Date().getFullYear() : parsedYear;
  const safeMonth = isNaN(parsedMonth) || parsedMonth < 0 || parsedMonth > 11 ? 0 : parsedMonth;

  try {
    const numberOfDays = getDaysInMonth(new Date(safeYear, safeMonth));
    const dayOptions: IOption[] = [];

    for (let day = 1; day <= numberOfDays; day++) {
      dayOptions.push({
        label: day.toString().padStart(2, '0'),
        value: day.toString(),
      });
    }

    return dayOptions;
  } catch (error) {
    // istanbul ignore next
    console.error('Error generating day options:', { year: yearString, month: monthString, error });

    // istanbul ignore next
    return [{ label: '01', value: '1' }];
  }
};

export const generateHalfHourSlots = (): string[] => {
  const slots: string[] = [];
  for (let h = 0; h < 24; h++) {
    ['00', '30'].forEach((mm) => slots.push(`${h.toString().padStart(2, '0')}:${mm}`));
  }
  return slots;
};
