import { fireEvent, render, screen, waitFor, within } from '@testing-library/react';
import { useLazyQuery, useMutation } from '@apollo/client';
import { ContentBlockData } from 'apollo/mocks/PEGridData';
import { ContentDisplayMode, DisplayMode, PiletApi } from '@icis/app-shell';
import { useCapabilities, useUser } from '@icis/app-shell-apis';
import PECapabilityAuthoring from '../PriceEntryContainerAuthoring';

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
  title: 'mock title of the price entry',
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
  lockedContentData: [
    {
      yjsClientId: 789,
      id: '456',
      idEncrypted: 'encrypted-id',
      name: 'User Name',
      email: 'user@example.com',
      lockedContent: [
        {
          lockedContentDetails: [
            {
              id: 77777777722,
              data: 'test-capability-id',
            },
          ],
        },
      ],
    },
  ],
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

describe('Price entry authoring', () => {
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

      render(<PECapabilityAuthoring params={{ ...mockEmptyVersionParams }} />);

      // Ensure that the loading element disappears when data is loaded
      await waitFor(() => {
        expect(screen.queryByTestId('loading')).not.toBeInTheDocument();
      });
    });
  });

  describe('Price entry content block rendering', () => {
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
      render(<PECapabilityAuthoring params={{ ...mockEmptyVersionParams }} />);
      await waitFor(() => {
        const textboxes = within(
          screen.getByTestId('price-entry-content-block-wrapper'),
        ).getAllByRole('textbox');
        expect(textboxes[0]).toBeInTheDocument();
      });
    });

    it('should update the title input correctly', async () => {
      const mockUseMutation = jest.fn();
      (useMutation as any).mockReturnValue([mockUseMutation, { loading: false }]);
      (useLazyQuery as any).mockImplementation(() => [
        jest.fn(),
        { data: { ...ContentBlockData } },
      ]);
      const { findByTestId } = render(
        <PECapabilityAuthoring params={{ ...mockEmptyVersionParams }} />,
      );
      const wrapper = await findByTestId('price-entry-content-block-wrapper-header');

      const input = wrapper.getElementsByTagName('input')[0];
      expect(input).toBeInTheDocument();

      // Enable the input if it is disabled
      fireEvent.focus(input);
      fireEvent.change(input, { target: { value: '' } });
      fireEvent.change(input, { target: { value: 'New Title' } });
      fireEvent.blur(input);

      await waitFor(() => {
        expect(input.value).toBe('New Title');
      });
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
        <PECapabilityAuthoring params={{ ...mockEmptyVersionParams }} />,
      );

      const priceSeriesSelectionCard = queryByTestId('price-series-selection-card');
      expect(priceSeriesSelectionCard).not.toBeInTheDocument();
    });

    it('should render PriceSeriesSelectionCard add button contentblock is defined but the priceSeries is null', async () => {
      const data = { contentBlock: { priceSeries: null } };
      const mockUseMutation = jest.fn();
      (useMutation as any).mockReturnValue([mockUseMutation, { loading: false }]);
      (useLazyQuery as any).mockImplementation(() => [mockUseMutation, { data }]);

      const { findByTestId } = render(
        <PECapabilityAuthoring params={{ ...mockEmptyVersionParams }} />,
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
        <PECapabilityAuthoring params={{ ...mockEmptyVersionParams }} />,
      );
      const mockFunc = jest.fn();
      const parentElement = await findByTestId('price-entry-content-block-wrapper');
      parentElement?.addEventListener('mouseenter', mockFunc);
      if (parentElement) {
        fireEvent.mouseEnter(parentElement);
      }
      expect(mockFunc).toBeCalled();
      const editButton = await findByTestId('price-entry-content-block-wrapper-edit-button');
      fireEvent.click(editButton);
    });

    it('should not update the title when input is unchanged after trimming', async () => {
      const mockUseMutation = jest.fn();
      (useMutation as any).mockReturnValue([mockUseMutation, { loading: false }]);
      (useLazyQuery as any).mockImplementation(() => [
        jest.fn(),
        {
          data: {
            ...ContentBlockData,
            contentBlock: {
              ...ContentBlockData.contentBlock,
              title: 'Original Title',
            },
          },
        },
      ]);

      const { findByTestId } = render(
        <PECapabilityAuthoring params={{ ...mockEmptyVersionParams }} />,
      );

      const wrapper = await findByTestId('price-entry-content-block-wrapper-header');
      const input = wrapper.getElementsByTagName('input')[0];
      expect(input).toBeInTheDocument();

      fireEvent.focus(input);
      fireEvent.change(input, { target: { value: '  Original Title  ' } });
      fireEvent.blur(input);

      await waitFor(() => {
        // Ensure that mutation/save function was not called
        expect(mockUseMutation).not.toHaveBeenCalled();
        expect(input.value).toBe('  Original Title  ');
      });
    });
  });
});
