// istanbul ignore file
import { format, isSameDay } from 'date-fns';
import { useEffect } from 'react';
import { DATE_FORMAT, DATE_PICKER_HEIGHT, DATE_PLACEHOLDER } from './DateSelector.constants';
import { DateProp, SelectedDate } from './DateSelector.type';

const OFFSET = 4;

/** Hook to detect clicks outside of a specified element and trigger a callback */
export const useOutsideClick = (refs: React.RefObject<HTMLElement>[], callback: () => void) => {
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      // if click is inside *any* of these refs, do nothing
      for (const ref of refs) {
        if (ref.current && ref.current.contains(event.target as Node)) {
          return;
        }
      }
      // otherwise, it really was outside
      callback();
    };

    document.addEventListener('click', handleClickOutside);
    return () => document.removeEventListener('click', handleClickOutside);
  }, [refs, callback]);
};

/** Prevents event propagation by stopping the click event */
export const handlePropagation = (e: React.MouseEvent) => {
  e.stopPropagation();
};

/** Calculates the optimal position for a date picker based on available screen space */
export const handlePickerPosition = (rect: DOMRect) => {
  const spaceBelow = window.innerHeight - rect.bottom;
  const spaceAbove = rect.top;
  const scrollY = window.scrollY;
  const scrollX = window.scrollX;

  // If there's enough space below, put it below:
  if (spaceBelow >= DATE_PICKER_HEIGHT) {
    return {
      top: rect.bottom + scrollY + OFFSET,
      left: rect.left + scrollX,
    };
  }

  // If there's enough space above, put it above:
  if (spaceAbove >= DATE_PICKER_HEIGHT) {
    return {
      top: rect.top + scrollY - DATE_PICKER_HEIGHT - OFFSET,
      left: rect.left + scrollX,
    };
  }

  // Otherwise, pick some fallback (e.g. still place it below,
  // but it might overflow the window).
  return {
    top: rect.bottom + scrollY + OFFSET,
    left: rect.left + scrollX,
  };
};

/** Parses and validates the selected date value, handling both single dates and date ranges */
export const parseDate = (dateValue?: DateProp): SelectedDate => {
  if (!dateValue) {
    return undefined;
  }

  if (typeof dateValue === 'string') {
    return new Date(dateValue);
  } else if (dateValue instanceof Date) {
    return dateValue;
  }
};

/** Formats date values into string representations using the specified date format */
export const formatDateValue = (date: SelectedDate): [string, string?] => {
  if (!date) return [DATE_PLACEHOLDER];

  if (date instanceof Date) {
    return [format(date, DATE_FORMAT)];
  }

  return [DATE_PLACEHOLDER];
};

/** Compares two date values for equality, handling both single dates and date ranges */
export const compareDates = (date1: SelectedDate, date2: SelectedDate): boolean => {
  if (!date1 || !date2) return date1 === date2;

  if (date1 instanceof Date && date2 instanceof Date) {
    return isSameDay(date1, date2);
  }

  return false;
};

/** Converts a date value to a date range format if specified */
export const convertToUtcDate = (date: SelectedDate): Date => {
  if (!date) {
    return new Date(); // Return current date as fallback
  }

  if (date instanceof Date) {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate());
  }

  // Assert date as the object type with year, month, day
  const { year, month, day } = date as { year: number; month: number; day: number };
  return new Date(year, month, day);
};
