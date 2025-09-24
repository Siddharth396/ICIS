import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { useLazyQuery } from '@apollo/client';
import userEvent from '@testing-library/user-event';
import ScheduleSelectorTool from '../ScheduleSelectorTool';

// 1) Mock Apollo
jest.mock('@apollo/client', () => {
  const actual = jest.requireActual('@apollo/client');
  return {
    ...actual,
    useLazyQuery: jest.fn(),
  };
});

// 2) Mock withClient HOC
jest.mock('components/HOCs/withClient', () => (component: React.ComponentType) => component);

// 3) Mock the DateSelector properly
const DateSelectorMock = ({ date = new Date(), onDateChange, onMonthChange, disabled }: any) => (
  <div>
    <div>DateSelector Mock</div>
    <div data-testid='selected-date'>{date.toString()}</div>
    <button
      onClick={() => {
        if (!disabled) {
          onDateChange(new Date(date.getTime() + 86400000)); // Add one day
        }
      }}>
      Next Day
    </button>
    <button
      data-testid='month-navigation-next'
      onClick={() => {
        if (onMonthChange) {
          onMonthChange(new Date('2025-01-01T00:00:00.000Z'));
        }
      }}>
      Next Month
    </button>
  </div>
);

jest.mock('components/DateSelector', () => DateSelectorMock);

// 4) Mock PublicationDateMessage
jest.mock(
  'components/PublicationDateMessage',
  () =>
    ({ isPublishingDay, formattedNextPublicationDate }: any) => (
      <div>
        <div>PublicationDateMessage Mock</div>
        <div data-testid='publishing-day'>{String(isPublishingDay)}</div>
        <div data-testid='formatted-date'>{formattedNextPublicationDate}</div>
      </div>
    ),
);

describe('ScheduleSelectorTool', () => {
  const mockUseLazyQuery = useLazyQuery as jest.Mock;
  const defaultScheduleId = 'bb7c3ca8-a69c-44b7-8bb1-ed95ca9ba2f0';
  const defaultInitialDate = new Date('2024-12-02T06:00:00.000Z');

  interface SetupProps {
    scheduleId?: string;
    initialSelectedDate?: Date;
    userSelectedDate?: Date;
    limit?: number;
    disabled?: boolean;
    onDateChange: (date: Date, isPublishingDay: boolean) => void;
    show?: boolean;
  }

  beforeAll(() => {
    jest.useFakeTimers({ legacyFakeTimers: false });
    // Freeze "today" as Dec 29, 2024
    jest.setSystemTime(new Date('2024-12-29T06:00:00.000Z'));
  });

  afterAll(() => {
    jest.useRealTimers();
  });

  // Helper to render with defaults
  const setup = (props: SetupProps) => {
    return render(
      <ScheduleSelectorTool
        scheduleId={props.scheduleId ?? defaultScheduleId}
        defaultSelectedDate={props.initialSelectedDate ?? defaultInitialDate}
        selectedDate={props.userSelectedDate}
        limit={props.limit}
        disabled={props.disabled || false}
        show={props.show ?? true}
        onDateChange={props.onDateChange}
      />,
    );
  };

  const setupWithoutDefaults = (props: SetupProps) => {
    return render(
      <ScheduleSelectorTool
        scheduleId={props.scheduleId}
        defaultSelectedDate={props.initialSelectedDate}
        selectedDate={props.userSelectedDate}
        limit={props.limit}
        disabled={props.disabled || false}
        show={props.show ?? true}
        onDateChange={props.onDateChange}
      />,
    );
  };

  beforeEach(() => {
    jest.clearAllMocks();
    // Default: loading state
    mockUseLazyQuery.mockReturnValue([
      jest.fn(),
      { loading: true, data: undefined, error: undefined },
    ]);
  });

  it('shows error state if query fails', async () => {
    mockUseLazyQuery.mockReturnValue([
      jest.fn(),
      { loading: false, error: new Error('Network Error'), data: undefined },
    ]);
    setup({ onDateChange: jest.fn() });
    await waitFor(() => {
      expect(screen.getByText(/Error loading events/)).toBeInTheDocument();
    });
  });

  it('renders DateSelector and PublicationDateMessage with no events', async () => {
    mockUseLazyQuery.mockReturnValue([
      jest.fn(),
      { loading: false, error: undefined, data: { events: { events: [] } } },
    ]);
    setup({ onDateChange: jest.fn() });

    await waitFor(() => {
      expect(screen.queryByText('Loading events...')).not.toBeInTheDocument();
      expect(screen.getByText('DateSelector Mock')).toBeInTheDocument();
      expect(screen.getByText('PublicationDateMessage Mock')).toBeInTheDocument();
    });

    expect(screen.getByTestId('publishing-day').textContent).toBe('false');
    expect(screen.getByTestId('formatted-date').textContent).toBe('');
  });

  it('handles events and sets next publication date correctly', async () => {
    const onDateChange = jest.fn();
    const testSelected = new Date('2024-12-29T06:00:00.000Z');

    mockUseLazyQuery.mockReturnValue([
      jest.fn(),
      { loading: false, error: undefined, data: { events: { events: [] } } },
    ]);

    setup({ onDateChange, userSelectedDate: testSelected });

    const nextDayButton = await screen.findByRole('button', { name: /Next Day/i });
    userEvent.click(nextDayButton);

    await waitFor(() => {
      expect(onDateChange).toHaveBeenCalledWith(
        new Date('2024-12-30T06:00:00.000Z'),
        false
      );
    });
  });

  it('shows correct publishing day message if selectedDate is nextPubDate', async () => {
    const onDateChange = jest.fn();
    const mockEvents = [{ eventTime: '2024-12-30T06:00:00.000Z' }];

    mockUseLazyQuery.mockReturnValue([
      jest.fn(),
      { loading: false, error: undefined, data: { events: { events: mockEvents } } },
    ]);

    setup({ onDateChange, userSelectedDate: new Date('2024-12-30T06:00:00.000Z') });

    await waitFor(() => {
      expect(screen.getByText('PublicationDateMessage Mock')).toBeInTheDocument();
    });

    expect(screen.getByTestId('publishing-day').textContent).toBe('true');
  });

  it('handles when limit is provided instead of default range', async () => {
    const onDateChange = jest.fn();
    const mockEvents = [
      { eventTime: '2024-12-15T06:00:00.000Z' },
      { eventTime: '2024-12-20T06:00:00.000Z' },
    ];

    mockUseLazyQuery.mockReturnValue([
      jest.fn(),
      { loading: false, error: undefined, data: { events: { events: mockEvents } } },
    ]);

    setup({ onDateChange, limit: 2 });

    await waitFor(() => {
      expect(screen.getByText('DateSelector Mock')).toBeInTheDocument();
    });
  });

  it('does not refetch on month navigation', async () => {
    const mockFetch = jest.fn();
    mockUseLazyQuery.mockReturnValue([
      mockFetch,
      { loading: false, error: undefined, data: { events: { events: [] } } },
    ]);

    setup({ onDateChange: jest.fn() });

    // initial fetch
    await waitFor(() => expect(mockFetch).toHaveBeenCalledTimes(1));

    // click month nav
    const nextMonthBtn = screen.getByTestId('month-navigation-next');
    userEvent.click(nextMonthBtn);

    // ensure no additional fetch call
    await waitFor(() => {
      expect(mockFetch).toHaveBeenCalledTimes(1);
    });
  });

  it('does not call getEvents if scheduleId is missing', async () => {
    const mockFetch = jest.fn();
    mockUseLazyQuery.mockReturnValue([mockFetch, { loading: false, data: undefined }]);

    setupWithoutDefaults({ scheduleId: '', onDateChange: jest.fn() });

    await waitFor(() => {
      expect(mockFetch).not.toHaveBeenCalled();
    });
  });
});
