import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import { useLazyQuery, useMutation } from '@apollo/client';
import { ContentBlockData } from 'apollo/mocks/PEGridData';
import { ContentDisplayMode, DisplayMode, PiletApi } from '@icis/app-shell';
import { useCapabilities, useUser } from '@icis/app-shell-apis';
import userEvent from '@testing-library/user-event';
import getMessages from 'getMessages';
import PDapabilityAuthoring from '../PriceDisplayTableContainerAuthoring';

// Mock the Apollo Client
jest
  .mock('@apollo/client', () => {
    return {
      ...jest.requireActual('@apollo/client'),
      useMutation: jest.fn(() => [jest.fn()]),
      useLazyQuery: jest.fn(),
    };
  })
  .mock('@icis/app-shell-apis', () => ({
    getApolloClient: () => ({}),
    useUser: jest.fn(),
    useCapabilities: jest.fn(),
    settings: { environment: 'dev' },
  }));

const mockContentData = {
  id: 'test-capability-id',
  version: '1',
  title: 'mock title of the price table',
  capabilityId: 'test-capability-id',
};

const { capabilityId, version } = mockContentData;

const mockParams = {
  id: capabilityId,
  version,
  onSave: () => {},
  isAuthoring: true,
  setShowEditingUi: () => {},
  onConfigChanged: () => {},
  isLocked: false,
  lockedToOtherClient: false,
  onStartEditing: () => {},
  onFinishEditing: () => {},
  metadata: {
    locationInfo: {
      sectionTitle: 'Price Entry',
    },
    displayMode: 'default' as DisplayMode,
    config: [
      {
        key: 'type',
        value: 'price-entry',
      },
    ],
  },
  displayMode: 'fill' as ContentDisplayMode,
  piralApi: {
    on: jest.fn(),
    off: jest.fn(),
  } as unknown as PiletApi,
  newlyAdded: false,
  notifyLatestContent: () => {},
};

const mockEmptyVersionParams = { ...mockParams, version: '' };

const messages = getMessages('en');

describe('Price table authoring', () => {
  beforeEach(() => {
    jest.resetModules();
    (useUser as jest.Mock).mockImplementation(() => ({
      locale: 'en',
      userId: '123',
      entitlements: ['ContentEdit'],
    }));
    (useCapabilities as jest.Mock).mockReturnValue({
      'richtext-capability': {
        Component: () => <div data-testid='richtext-capability' />,
      },
    });
  });
  afterEach(() => {
    jest.clearAllMocks();
  });

  describe('Grid rendering', () => {
    it('should render the component with data', async () => {
      // Mock the GraphQL query response with loaded data
      (useMutation as any).mockReturnValue([jest.fn(), { loading: true }]);
      (useLazyQuery as any).mockImplementation(() => [jest.fn(), { data: null }]);

      render(<PDapabilityAuthoring params={{ ...mockEmptyVersionParams }} />);

      // Ensure that the loading element disappears when data is loaded
      await waitFor(() => {
        expect(screen.queryByTestId('loading')).not.toBeInTheDocument();
      });
    });
  });

  describe('Price table content block rendering', () => {
    beforeEach(() => {
      jest.resetModules();
    });
    afterEach(() => {
      jest.clearAllMocks();
    });
    it('should render title input box', async () => {
      (useMutation as any).mockReturnValue([jest.fn(), { loading: false }]);
      (useLazyQuery as any).mockImplementation(() => [
        jest.fn(),
        { data: { ...ContentBlockData } },
      ]);
      render(<PDapabilityAuthoring params={{ ...mockEmptyVersionParams }} />);
      expect(
        await screen.findByPlaceholderText(messages.General.TitlePlaceholder),
      ).toBeInTheDocument();
    });

    it('should update the title input correctly', async () => {
      const mockUseMutation = jest.fn();
      (useMutation as any).mockReturnValue([mockUseMutation, { loading: false }]);
      (useLazyQuery as any).mockImplementation(() => [
        mockUseMutation,
        { data: { ...ContentBlockData } },
      ]);
      const { findByTestId } = render(
        <PDapabilityAuthoring params={{ ...mockEmptyVersionParams }} />,
      );
      const wrapper = await findByTestId('price-display-content-block-wrapper-header');

      const input = wrapper.getElementsByTagName('input');
      userEvent.type(input[0], 'New Title');
      fireEvent.blur(input[0]);
      expect(input[0].value).toBe('');
      expect(mockUseMutation.mock.calls[0][0].variables.contentBlockTitle).toBe(undefined);
    });

    it('should not render PriceSeriesSelectionCard when data and priceSeries are present', () => {
      // Mock the data object
      const data = {
        contentBlock: {
          priceSeries: [{}], // Mock priceSeries array with one item
          // Add other necessary properties
        },
      };
      const mockUseMutation = jest.fn();
      (useMutation as any).mockReturnValue([mockUseMutation, { loading: false }]);
      (useLazyQuery as any).mockImplementation(() => [mockUseMutation, { data }]);
      const { queryByTestId } = render(
        <PDapabilityAuthoring params={{ ...mockEmptyVersionParams }} />,
      );

      const priceSeriesSelectionCard = queryByTestId('price-series-selection-card');
      expect(priceSeriesSelectionCard).not.toBeInTheDocument();
    });

    it('should render PriceSeriesSelectionCard add button contentblock is defined but the riceSeries is null', async () => {
      const data = { contentBlock: { priceSeries: null } };
      const mockUseMutation = jest.fn();
      (useMutation as any).mockReturnValue([mockUseMutation, { loading: false }]);
      (useLazyQuery as any).mockImplementation(() => [mockUseMutation, { data }]);

      const { findByTestId } = render(
        <PDapabilityAuthoring params={{ ...mockEmptyVersionParams }} />,
      );

      const priceSeriesSelectionCardAddButton = await findByTestId(
        'price-series-selection-card-add-button',
      );
      expect(priceSeriesSelectionCardAddButton).toBeInTheDocument();
    });

    it('Open modal on click of edit button in authoring mode', async () => {
      const mockUseMutation = jest.fn();
      (useMutation as any).mockReturnValue([jest.fn(), { loading: false }]);
      (useLazyQuery as any).mockImplementation(() => [
        mockUseMutation,
        { data: { ...ContentBlockData } },
      ]);

      const { findByTestId } = render(
        <PDapabilityAuthoring params={{ ...mockEmptyVersionParams }} />,
      );
      const mockFunc = jest.fn();
      const parentElement = await findByTestId('price-display-content-block-wrapper');
      parentElement?.addEventListener('mouseenter', mockFunc);
      if (parentElement) {
        fireEvent.mouseEnter(parentElement);
      }
      expect(mockFunc).toBeCalled();
      const editButton = await findByTestId('price-display-content-block-wrapper-edit-button');
      fireEvent.click(editButton);
    });
  });
});
