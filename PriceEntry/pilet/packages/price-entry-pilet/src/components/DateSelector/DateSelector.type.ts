// istanbul ignore file
import { ReactNode } from 'react';

export type DateProp = null | string | { from?: string; to?: string } | SelectedDate;
export type SelectedDate = Date | undefined;

export type DateSelectorProps = {
  disabled?: boolean;
  date?: DateProp;
  isDateInRange?: boolean;
  highlightedDates?: Date[];
  onDateChange?: (date: Date | undefined) => void;
  onOutsideClick?: () => void;
  onMonthChange?: (month: Date) => void;
};

export type PickerPosition = { top: number; left: number };

export type PickerPlacement = 'left' | 'right' | 'center';

export type DateCellContainerProps = {
  children: ReactNode;
  testId?: string;
};
