// istanbul ignore file
import { format, isValid, Locale as LocaleDateFns } from 'date-fns';
import { enUS, zhCN } from 'date-fns/locale';
import { DateFormatter, Matcher } from 'react-day-picker';

export const getAllDisabledDays = (
  enabledDays?: Date[],
  disabledDays?: Matcher[],
  toDate?: Date | null,
): Matcher[] => {
  let disabledDaysModifier = [] as Matcher[];

  if (enabledDays) {
    const sortedEnabledDays = enabledDays.sort((a, b) => a.getTime() - b.getTime());
    sortedEnabledDays.forEach((enabledDay, index, daysArray) => {
      if (index === 0) {
        const modifier: Matcher = {
          before: enabledDay,
        };
        disabledDaysModifier.push(modifier);
      } else {
        const modifier: Matcher = {
          after: daysArray[index - 1],
          before: enabledDay,
        };
        disabledDaysModifier.push(modifier);
      }
    });
    disabledDaysModifier.push({
      after: sortedEnabledDays[sortedEnabledDays.length - 1],
    });
  }

  if (disabledDays) {
    disabledDaysModifier = [...disabledDays, ...disabledDaysModifier];
  }

  if (toDate) {
    const modifier: Matcher = {
      after: toDate,
    };
    disabledDaysModifier.push(modifier);
  }

  return disabledDaysModifier;
};

export const getValidationError = (value: string) => {
  const validDatePattern = /(\d{4})-(\d{2})-(\d{2})/;
  if (!value) {
    return;
  }
  /* istanbul ignore next */
  if (value.length) {
    if (!validDatePattern.test(value)) {
      //  Test valid date format
      return 'invalidFormat';
    } else if (!isValid(new Date(value))) {
      //  Test valid date
      return 'invalidDate';
    }
  }
  return false;
};

type Locale = 'en' | 'zh';

const LOCALEMAP: Record<Locale, LocaleDateFns> = {
  en: enUS,
  zh: zhCN,
};

const MONTHS: Record<Locale, string[]> = {
  en: [
    'January',
    'February',
    'March',
    'April',
    'May',
    'June',
    'July',
    'August',
    'September',
    'October',
    'November',
    'December',
  ],
  zh: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
};

const MONTHS_SHORT: Record<Locale, string[]> = {
  en: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
  zh: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
};

const FIRST_DAY: Record<Locale, 0 | 1 | 2 | 3 | 4 | 5 | 6> = {
  en: 0,
  zh: 0, // Use 1 if Monday as first day of the week
};

export const isSupportedLocale = (locale: string | undefined): locale is Locale =>
  locale === 'en' || locale === 'zh';

export const getLocale = (locale: string | undefined): Locale => {
  if (locale) {
    locale = locale.includes('-') ? locale.split('-')[0] : locale;
    return isSupportedLocale(locale) ? locale : 'en';
  }
  return 'en';
};

export const getBaseLocale = (locale: string | undefined) => {
  const baseLocale = locale && locale.includes('-') ? locale.split('-')[0] : locale;
  return isSupportedLocale(baseLocale) ? LOCALEMAP[baseLocale] : enUS;
};

export const getFirstDayOfWeek = (locale: string | undefined) => {
  return FIRST_DAY[getLocale(locale)];
};

export const getMonths = (locale: string | undefined) => {
  return MONTHS[getLocale(locale)];
};

export const getShortMonths = (locale: string | undefined) => {
  return MONTHS_SHORT[getLocale(locale)];
};

const formatMonthTitle = (date: Date, locale: string = 'en') => {
  const year = date.getFullYear();
  const month = MONTHS[getLocale(locale)][date.getMonth()];
  return `${month} ${year}`;
};

export const formatWeekdayName: DateFormatter = (date: Date, locale) => {
  return format(date, 'eeeee', locale);
};

export const customLocaleUtils = {
  getMonths,
  formatMonthTitle,
};

export const getSelectedDate = (
  date: Date | { from?: Date; to?: Date } | undefined,
): Date | undefined => {
  if (!date) {
    return undefined;
  }
  return date instanceof Date ? date : date.from;
};
