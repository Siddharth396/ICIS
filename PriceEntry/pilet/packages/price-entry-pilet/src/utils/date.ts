// istanbul ignore file
import { parseISO } from 'date-fns';
import { toZonedTime } from 'date-fns-tz';

export const getDayStartDateInUTC = (timestamp: number | undefined) => {
  return timestamp ? new Date(new Date(timestamp).setHours(0, 0, 0, 0)) : undefined;
};

export const getDayEndDateInUTC = () => {
  const currentDate = new Date();
  const utcDate = new Date(Date.UTC(currentDate.getUTCFullYear(), currentDate.getUTCMonth(), currentDate.getUTCDate(), 23, 59, 59));
  return utcDate.getTime();
};

export const getStartOfDayUTC = (date: Date | undefined) => {
  if (!date) return undefined;

  // Create a date at midnight UTC
  const dateUTC = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));

  return dateUTC;
};

export const getDateYearsFromNow = (date: Date, years: number) => {
  const newDate = new Date(date);
  newDate.setFullYear(newDate.getFullYear() + years);
  return newDate;
};

export const formatDateTimeString = (inputDate: Date) => {
  // Create a new date at midnight UTC for the given local date components
  const dateUTC = new Date(
    Date.UTC(inputDate.getFullYear(), inputDate.getMonth(), inputDate.getDate()),
  );

  // Format the date as YYYY-MM-DDT00:00:00Z
  return dateUTC.toISOString().split('.')[0] + 'Z';
};

export const convertToLocalTimezone = (utcDateString?: string) => {
  if (!utcDateString) return null;
  const utcDate = parseISO(utcDateString);
  const systemTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
  return toZonedTime(utcDate, systemTimeZone);
};
