// istanbul ignore file
export type DatePickerProps = {
  dates: DateObject[];
  initialSelectedDate: DateObject;
  onSelectedDateChange: (date: Date, id?: string) => void;
  mode: Mode;
  testId?: string;
  locale?: string;
  showLoader?: boolean;
  /** @function handleSpecialValueFormat - Formats the date for optionValue field, as per the custom provided logic */
  handleOptionValueFormat?: (selected: boolean, date: Date, id?: string) => React.ReactNode;
  /** @function handleSelectionBoxValueFormat - Formats the date for selectionBox value, as per the custom provided logic */
  handleSelectionBoxValueFormat?: (date: Date) => React.ReactNode;
  disabled?: boolean;
};

export type DateObject = {
  id?: string;
  date: Date;
};

export type Locale = 'en' | 'zh';

export type Mode = 'year' | 'month' | 'week';

export enum MODES {
  YEAR = 'year',
  MONTH = 'month',
  WEEK = 'week',
}
