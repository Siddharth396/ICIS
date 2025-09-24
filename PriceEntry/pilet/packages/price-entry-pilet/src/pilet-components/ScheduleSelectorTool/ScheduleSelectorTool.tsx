import React, { useEffect, useState, useMemo, useCallback, useRef } from 'react';
import {
  addMonths,
  endOfMonth,
  format,
  isAfter,
  isSameDay,
  startOfDay,
  startOfMonth,
} from 'date-fns';
import { useLazyQuery } from '@apollo/client';
import { EventTime, GET_EVENTS } from 'apollo/queries';
import withClient, { ApolloClientProps } from 'components/HOCs/withClient';
import DateSelector from 'components/DateSelector';
import PublicationDateMessage from 'components/PublicationDateMessage';
import { Wrapper } from './styled';
import { DATE_TIME_STRING_FORMAT } from 'utils/constants';
import { DATE_YEAR_FORMAT } from 'components/DateSelector/DateSelector.constants';

export interface ScheduleSelectorToolProps {
  /** uncontrolled initial date */
  defaultSelectedDate?: Date;
  /** controlled selected date */
  selectedDate?: Date;
  scheduleId?: string;
  disabled?: boolean;
  limit?: number;
  show?: boolean;
  onDateChange: (date: Date, isPublishingDay: boolean) => void;
}

const ScheduleSelectorTool: React.FC<ScheduleSelectorToolProps & ApolloClientProps> = ({
  client,
  defaultSelectedDate,
  selectedDate: userSelectedDate,
  scheduleId,
  disabled,
  limit,
  show,
  onDateChange,
}) => {
  const [confirmedDate, setConfirmedDate] = useState<Date>(
    userSelectedDate || defaultSelectedDate || new Date(),
  );
  const [startDate, setStartDate] = useState<Date>(startOfMonth(confirmedDate));
  const [displayMonth, setDisplayMonth] = useState<Date>(startOfMonth(confirmedDate));

  const [publicationDetails, setPublicationDetails] = useState<{
    isPublishingDay: boolean;
    formattedNextPublicationDate: string;
  }>({
    isPublishingDay: false,
    formattedNextPublicationDate: '',
  });

  const [isInitializing, setIsInitializing] = useState<boolean>(true);

  const initialSelectionRef = useRef<boolean>(false);
  const lastSelectedDateRef = useRef<Date>(confirmedDate);

  const [getEvents, { data, error, loading, called }] = useLazyQuery(GET_EVENTS, {
    client,
    fetchPolicy: 'no-cache',
  });

  // Fetch events for a date range
  // istanbul ignore next
  const fetchEventsRange = useCallback(
    (start: Date, end: Date) => {
      if (!scheduleId) {
        setIsInitializing(false);
        return;
      }
      const startStr = format(start, DATE_YEAR_FORMAT);
      const endStr = format(end, DATE_YEAR_FORMAT);
      getEvents({
        variables: {
          scheduleId,
          startDate: startStr,
          endDate: endStr,
          ...(limit ? { limit } : {}),
        },
        onError: () => setIsInitializing(false),
      });
    },
    [scheduleId, limit, getEvents],
  );

  // Initial load or show-only
  useEffect(() => {
    if (!initialSelectionRef.current) {
      if (scheduleId) {
        const quarterEnd = endOfMonth(addMonths(displayMonth, 2));
        fetchEventsRange(startDate, endOfMonth(quarterEnd));
      }
    }
  }, [fetchEventsRange, scheduleId, startDate, displayMonth]);

  useEffect(() => {
    if (!scheduleId) setIsInitializing(false);
  }, [scheduleId]);

  const eventsArray: EventTime[] = useMemo(
    () => (error ? [] : (data?.events?.events ?? [])),
    [data, error, called],
  );

  // istanbul ignore next
  const findNextPublicationDate = useCallback(() => {
    const today = startOfDay(new Date());
    const upcomingEvents = eventsArray
      .map((evt) => new Date(evt.eventTime))
      .filter((d) => !isNaN(d.getTime()) && (isSameDay(d, today) || isAfter(d, today)))
      .sort((a, b) => a.getTime() - b.getTime());
    return upcomingEvents[0] || confirmedDate;
  }, [eventsArray, confirmedDate]);

  // istanbul ignore next
  const handleInitialSelection = useCallback(
    (nextPub: Date) => {
      if (initialSelectionRef.current) return;
      initialSelectionRef.current = true;
      const dateToUse = !scheduleId
        ? userSelectedDate || defaultSelectedDate || new Date()
        : (!called || !data) && !userSelectedDate
          ? nextPub
          : userSelectedDate || nextPub;

      setConfirmedDate(dateToUse);
      setDisplayMonth(startOfMonth(dateToUse));
      setStartDate(startOfMonth(dateToUse));
      setIsInitializing(false);
      lastSelectedDateRef.current = dateToUse;

      const sel = startOfDay(dateToUse);
      const upcoming = eventsArray
        .map((evt) => new Date(evt.eventTime))
        .filter((d) => !isNaN(d.getTime()) && (isSameDay(d, sel) || isAfter(d, sel)))
        .sort((a, b) => a.getTime() - b.getTime());
      const nextPublication = upcoming[0];

      const isPublishingDay = !!nextPublication && isSameDay(nextPublication, dateToUse);

      setPublicationDetails({
        isPublishingDay,
        formattedNextPublicationDate: nextPublication
          ? format(nextPublication, DATE_TIME_STRING_FORMAT)
          : '',
      });

      lastSelectedDateRef.current = dateToUse;
      onDateChange(dateToUse, isPublishingDay);
    },
    [scheduleId, userSelectedDate, defaultSelectedDate, called, data, eventsArray, onDateChange],
  );

  useEffect(() => {
    if (!initialSelectionRef.current && !loading) {
      if (data && !error) {
        handleInitialSelection(findNextPublicationDate());
      } else if (error) {
        handleInitialSelection(confirmedDate);
      }
    }
  }, [loading, error, data, findNextPublicationDate, handleInitialSelection, confirmedDate]);

  // istanbul ignore next
  const handleMonthChange = useCallback(
    (month: Date) => {
      setDisplayMonth(month);
      setStartDate(startOfMonth(month));
      fetchEventsRange(startOfMonth(month), endOfMonth(month));
    },
    [fetchEventsRange],
  );

  // istanbul ignore next
  const handleDateChange = useCallback(
    (pickedDate: Date) => {
      setConfirmedDate(pickedDate);
      setDisplayMonth(startOfMonth(pickedDate));
      setStartDate(startOfMonth(pickedDate));

      const startOfPickedDay = startOfDay(pickedDate);
      const upcomingEventDates = eventsArray
        .map((eventRecord) => new Date(eventRecord.eventTime))
        .filter(
          (eventDate) =>
            !isNaN(eventDate.getTime()) &&
            (isSameDay(eventDate, startOfPickedDay) || isAfter(eventDate, startOfPickedDay)),
        )
        .sort(
          (firstEventDate, secondEventDate) => firstEventDate.getTime() - secondEventDate.getTime(),
        );

      let isPublishingDay: boolean;
      if (upcomingEventDates.length > 0) {
        const nextPublicationDate = upcomingEventDates[0];
        isPublishingDay = isSameDay(nextPublicationDate, pickedDate);
        setPublicationDetails({
          isPublishingDay,
          formattedNextPublicationDate: format(nextPublicationDate, DATE_TIME_STRING_FORMAT),
        });
      } else {
        isPublishingDay = false;
        setPublicationDetails({
          isPublishingDay,
          formattedNextPublicationDate: format(pickedDate, DATE_TIME_STRING_FORMAT),
        });
      }

      onDateChange(pickedDate, isPublishingDay);
    },
    [eventsArray, onDateChange],
  );

  const highlightedDates = useMemo(() => {
    const dates = eventsArray
      .map((evt) => new Date(evt.eventTime))
      .filter((d) => !isNaN(d.getTime()));
    if (!dates.some((d) => isSameDay(d, confirmedDate))) dates.push(confirmedDate);
    return dates;
  }, [eventsArray, confirmedDate]);

  if (isInitializing || !show) return null;

  return (
    <Wrapper>
      <DateSelector
        key={confirmedDate.toISOString()}
        disabled={disabled}
        date={confirmedDate}
        highlightedDates={highlightedDates}
        onMonthChange={handleMonthChange}
        // @ts-ignore
        onDateChange={handleDateChange}
      />
      {scheduleId && (
        <PublicationDateMessage
          isPublishingDay={publicationDetails.isPublishingDay}
          formattedNextPublicationDate={publicationDetails.formattedNextPublicationDate}
        />
      )}
      {error && <div style={{ color: 'red' }}>Error loading events: {error.message}</div>}
    </Wrapper>
  );
};

export default withClient(ScheduleSelectorTool, true);
