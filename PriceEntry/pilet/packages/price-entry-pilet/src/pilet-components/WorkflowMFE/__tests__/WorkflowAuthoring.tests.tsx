import React, { act } from 'react';
import { render, fireEvent, waitFor } from '@testing-library/react';
import { MockedProvider } from '@apollo/client/testing';
import { DATA_PACKAGE_TRANSITION_TO_STATE, INITIATE_CORRECTION } from 'apollo/queries';
import { useLazyQuery, useMutation } from '@apollo/client';
import { useUser } from '@icis/app-shell-apis';
import WorkflowAuthoring from '..';
import { WORKFLOW_ACTIONS } from 'utils/constants';

// Mock the Apollo Client
jest
  .mock('@apollo/client', () => {
    return {
      ...jest.requireActual('@apollo/client'),
      useMutation: jest.fn(() => [jest.fn(), { loading: false }]),
      useLazyQuery: jest.fn(),
    };
  })
  .mock('@icis/app-shell-apis', () => ({
    getApolloClient: () => ({}),
    useUser: jest.fn(),
    settings: { environment: 'dev' },
  }));

const mockContentBlockParams = {
  version: 1,
  contentBlockId: '1',
  priceSeriesIds: ['1', '2'],
  assessedDateTime: new Date('2011-08-30T13:22:53.108+00:00'),
};

const mockOnRefetchGridData = jest.fn();

const mocks = [
  {
    request: {
      query: DATA_PACKAGE_TRANSITION_TO_STATE,
      variables: {
        input: [mockContentBlockParams],
      },
    },
    result: {
      data: {
        dataPackageTransitionToState: { isSuccess: false },
      },
    },
  },
  {
    request: {
      query: INITIATE_CORRECTION,
      variables: {
        contentBlockId: '1',
        version: 1,
        assessedDateTime: '2011-08-30T13:22:53.108+00:00',
      },
    },
    result: {
      data: { initiateCorrectionForDataPackage: true },
    },
  },
];

describe('WorkflowAuthoring', () => {
  beforeEach(() => {
    jest.resetModules();
    (useUser as jest.Mock).mockImplementation(() => ({
      locale: 'en',
      userId: '123',
      entitlements: ['ContentEdit'],
    }));
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  it('renders without crashing', () => {
    render(
      <MockedProvider mocks={mocks} addTypename={false}>
        {/* @ts-ignore */}
        <WorkflowAuthoring
          getReviewURL={jest.fn()}
          isContentLocked={false}
          nextActions={[{ name: 'PUBLISH', enabled: true, displayValue: 'Publish' }]}
          // @ts-ignore
          contentBlockParams={mockContentBlockParams}
          onRefetchGridData={mockOnRefetchGridData}
        />
      </MockedProvider>,
    );
  });

  it('calls the mutation and verifies if there is a button', async () => {
    // Mock the GraphQL query response with loaded data
    const mockDataPackageTransitionToState = jest.fn();
    (useMutation as any).mockReturnValue([mockDataPackageTransitionToState, { loading: false }]);
    (useLazyQuery as any).mockImplementation(() => [
      jest.fn(),
      { input: { ...mockContentBlockParams } },
    ]);

    const { findByText } = render(
      <MockedProvider mocks={mocks} addTypename={false}>
        {/* @ts-ignore */}
        <WorkflowAuthoring
          getReviewURL={jest.fn()}
          isContentLocked={false}
          nextActions={[{ name: 'PUBLISH', enabled: true, displayValue: 'Publish' }]}
          // @ts-ignore
          contentBlockParams={mockContentBlockParams}
          onRefetchGridData={mockOnRefetchGridData}
        />
      </MockedProvider>,
    );

    const button = await findByText('Publish');
    expect(button).toBeInTheDocument();
    act(() => {
      // Simulate clicking the button to open confirmation modal
      fireEvent.click(button);
    });

    // Check that the confirmation modal is rendered
    const confirmationModal = await findByText('You are about to publish the prices.');
    expect(confirmationModal).toBeInTheDocument();
  });

  it('does not call the mockDataPackageTransitionToState mutation if the Cancel button is clicked', async () => {
    // Mock the GraphQL query response with loaded data
    const mockDataPackageTransitionToState = jest.fn();
    (useMutation as any).mockReturnValue([mockDataPackageTransitionToState, { loading: false }]);
    (useLazyQuery as any).mockImplementation(() => [
      jest.fn(),
      { input: { ...mockContentBlockParams } },
    ]);

    const { findByText } = render(
      <MockedProvider mocks={mocks} addTypename={false}>
        {/* @ts-ignore */}
        <WorkflowAuthoring
          getReviewURL={jest.fn()}
          isContentLocked={false}
          nextActions={[
            {
              name: WORKFLOW_ACTIONS.PUBLISH,
              displayValue: 'Publish',
              enabled: true,
            },
          ]}
          // @ts-ignore
          contentBlockParams={mockContentBlockParams}
          onRefetchGridData={mockOnRefetchGridData}
        />
      </MockedProvider>,
    );

    const button = await findByText('Publish');
    expect(button).toBeInTheDocument();

    // Simulate clicking the button to open confirmation modal
    act(() => {
      fireEvent.click(button);
    });

    // Check that the confirmation modal is rendered
    const confirmationModal = await findByText('You are about to publish the prices.');
    expect(confirmationModal).toBeInTheDocument();

    // Simulate clicking the Cancel button on the confirmation modal
    const cancelButton = await findByText('Cancel');
    act(() => {
      fireEvent.click(cancelButton);
    });
    // Verify the mutation is not called after cancelling
    await waitFor(() => {
      expect(mockDataPackageTransitionToState).not.toHaveBeenCalled();
    });
  });

  it('disables WorkflowButton when isContentLocked is true', async () => {
    // Mock the GraphQL query response with loaded data
    const mockDataPackageTransitionToState = jest.fn();
    (useMutation as any).mockReturnValue([mockDataPackageTransitionToState, { loading: false }]);
    (useLazyQuery as any).mockImplementation(() => [
      jest.fn(),
      { input: { ...mockContentBlockParams } },
    ]);

    const { findByText } = render(
      <MockedProvider mocks={mocks} addTypename={false}>
        {/* @ts-ignore */}
        <WorkflowAuthoring
          getReviewURL={jest.fn()}
          isContentLocked={true}
          nextActions={[
            {
              name: WORKFLOW_ACTIONS.PUBLISH,
              displayValue: 'Publish',
              enabled: true,
            },
            {
              name: WORKFLOW_ACTIONS.INITIATE_CORRECTION,
              displayValue: 'Initiate Correction',
              enabled: true,
            },
          ]}
          // @ts-ignore
          contentBlockParams={mockContentBlockParams}
          onRefetchGridData={mockOnRefetchGridData}
        />
      </MockedProvider>,
    );

    const publishButton = await findByText('Publish');
    expect(publishButton).toBeInTheDocument();

    const correctionButton = await findByText('Initiate Correction');
    expect(correctionButton).toBeInTheDocument();

    expect(publishButton).toBeDisabled();
    expect(correctionButton).toBeDisabled();
    await waitFor(() => {
      expect(mockDataPackageTransitionToState).not.toHaveBeenCalled();
    });
  });

  it('calls onConfirmation when WorkflowButton is clicked', async () => {
    // Mock the GraphQL query response with loaded data
    const mockCorrection = jest.fn();
    (useMutation as any).mockReturnValue([mockCorrection, { loading: false }]);
    (useLazyQuery as any).mockImplementation(() => [
      jest.fn(),
      { input: { ...mockContentBlockParams } },
    ]);

    const { findByText } = render(
      <MockedProvider mocks={mocks} addTypename={false}>
        {/* @ts-ignore */}
        <WorkflowAuthoring
          getReviewURL={jest.fn()}
          isContentLocked={false}
          nextActions={[
            {
              name: WORKFLOW_ACTIONS.INITIATE_CORRECTION,
              displayValue: 'Initiate Correction',
              enabled: true,
            },
          ]}
          // @ts-ignore
          contentBlockParams={mockContentBlockParams}
          onRefetchGridData={mockOnRefetchGridData}
        />
      </MockedProvider>,
    );

    const correctionButton = await findByText('Initiate Correction');
    expect(correctionButton).toBeInTheDocument();

    expect(correctionButton).not.toBeDisabled();
    act(() => {
      fireEvent.click(correctionButton);
    });
    await waitFor(() => {
      expect(mockCorrection).toHaveBeenCalled();
    });
  });

  it('calls the transition mutation if the Send Back button is clicked', async () => {
    // Mock the GraphQL query response with loaded data
    const mockTransition = jest.fn();
    (useMutation as any).mockReturnValue([mockTransition, { loading: false }]);
    (useLazyQuery as any).mockImplementation(() => [
      jest.fn(),
      { input: { ...mockContentBlockParams } },
    ]);

    const { findByText } = render(
      <MockedProvider mocks={mocks} addTypename={false}>
        {/* @ts-ignore */}
        <WorkflowAuthoring
          getReviewURL={jest.fn()}
          isContentLocked={false}
          nextActions={[
            {
              name: WORKFLOW_ACTIONS.PUBLISH,
              displayValue: 'Publish',
              enabled: true,
            },
            {
              name: WORKFLOW_ACTIONS.SEND_BACK,
              displayValue: 'Send Back',
              enabled: true,
            },
            {
              name: WORKFLOW_ACTIONS.CANCEL,
              displayValue: 'Cancel',
              enabled: true,
            },
          ]}
          contentBlockParams={mockContentBlockParams}
          onRefetchGridData={mockOnRefetchGridData}
        />
      </MockedProvider>,
    );

    const button = await findByText('Send Back');
    expect(button).toBeInTheDocument();

    act(() => {
      // Simulate clicking the button to open confirmation modal
      fireEvent.click(button);
    });
    // Check that the confirmation modal is rendered
    const confirmationModal = await findByText('Send back to author');
    expect(confirmationModal).toBeInTheDocument();

    // Simulate clicking the submit on the confirmation modal
    const sendBackButton = await findByText('Send back');
    act(() => {
      fireEvent.click(sendBackButton);
    });

    // Verify the mutation is called after confirming
    await waitFor(() => {
      expect(mockTransition).toHaveBeenCalled();
    });
  });
});
