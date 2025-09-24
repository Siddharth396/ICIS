import { act } from 'react';
import { render, fireEvent, waitFor } from '@testing-library/react';
import NotificationModal from '../NotificationModal';
import { WORKFLOW_ACTIONS } from 'utils/constants';
import { DataPackageTransitionToState } from 'apollo/queries';

describe('NotificationModal', () => {
  jest.useFakeTimers();
  const onCloseMock = jest.fn();
  const onConfirmMock = jest.fn();

  const data: DataPackageTransitionToState = {
    isSuccess: false,
    errorCode: 'NOT_ALL_PRICE_SERIES_ITEMS_ARE_VALID',
  };

  afterEach(() => {
    jest.clearAllMocks();
    jest.clearAllTimers();
  });

  it('renders the modal when data is provided and isSuccess is false', () => {
    const { getByTestId } = render(
      <NotificationModal
        action={{ name: 'PUBLISH', enabled: true, displayValue: 'Publish' }}
        data={data}
        onClose={onCloseMock}
        type='notification'
        onConfirm={onConfirmMock}
        isNonPublishingDay={false}
      />,
    );

    expect(getByTestId('workflow-validation-modal')).toBeInTheDocument();
  });

  it('does not render the modal when data is not provided and type is notification', () => {
    const { queryByTestId } = render(
      <NotificationModal
        action={{ name: 'PUBLISH', enabled: true, displayValue: 'Publish' }}
        onClose={onCloseMock}
        type='notification'
        onConfirm={onConfirmMock}
        isNonPublishingDay={false}
      />,
    );

    expect(queryByTestId('workflow-validation-modal')).not.toBeInTheDocument();
  });

  it('renders the modal when isSuccess is true', () => {
    const { getByTestId } = render(
      <NotificationModal
        action={{ name: 'PUBLISH', enabled: true, displayValue: 'Publish' }}
        data={{ ...data, isSuccess: true }}
        onClose={onCloseMock}
        type='notification'
        onConfirm={onConfirmMock}
        isNonPublishingDay={false}
      />,
    );

    expect(getByTestId('workflow-validation-modal')).toBeInTheDocument();
  });

  it('renders the error message when contentBlocksInError is not empty and isSuccess is false', () => {
    const { getByText } = render(
      <NotificationModal
        action={{ name: 'PUBLISH', enabled: true, displayValue: 'Publish' }}
        data={data}
        onClose={onCloseMock}
        type='notification'
        onConfirm={onConfirmMock}
        isNonPublishingDay={false}
      />,
    );

    expect(
      getByText('Please complete all prices before they can be sent for review'),
    ).toBeInTheDocument();
  });

  it('renders the success message when isSuccess is true', () => {
    const { getByText } = render(
      <NotificationModal
        action={{ name: 'PUBLISH', enabled: true, displayValue: 'Publish' }}
        data={{ ...data, isSuccess: true }}
        onClose={onCloseMock}
        onConfirm={onConfirmMock}
        isNonPublishingDay={false}
      />,
    );

    expect(getByText('Content submitted successfully for Publish')).toBeInTheDocument();
  });

  it('calls onConfirm when the Publish button is clicked in confirmation modal', () => {
    const { getAllByText } = render(
      <NotificationModal
        action={{ name: 'PUBLISH', enabled: true, displayValue: 'Publish' }}
        onClose={onCloseMock}
        onConfirm={onConfirmMock}
        type='confirmation'
        isNonPublishingDay={false}
      />,
    );

    const publishButtons = getAllByText('Publish');
    const publishButton = publishButtons.find(
      (button) => button.tagName === 'BUTTON',
    ) as HTMLButtonElement;

    act(() => {
      fireEvent.click(publishButton);
    });
    expect(onConfirmMock).toHaveBeenCalledTimes(1);
  });

  it('calls onClose when the Cancel button is clicked in confirmation modal', () => {
    const { getByText } = render(
      <NotificationModal
        loading={false}
        action={{ name: 'PUBLISH', enabled: true, displayValue: 'Publish' }}
        onClose={onCloseMock}
        onConfirm={onConfirmMock}
        type='confirmation'
        isNonPublishingDay={false}
      />,
    );
    act(() => {
      fireEvent.click(getByText('Cancel'));
    });
    expect(onCloseMock).toHaveBeenCalledTimes(1);
  });

  it('renders the default success message when action does not exist', () => {
    const { getByText } = render(
      <NotificationModal
        action={{ name: 'NON_EXISTENT_ACTION', enabled: true, displayValue: 'Non-existent' }}
        data={{ isSuccess: true, errorCode: '' }}
        onClose={onCloseMock}
        type='notification'
        onConfirm={onConfirmMock}
        isNonPublishingDay={false}
      />,
    );

    expect(getByText('Content submitted successfully!')).toBeInTheDocument();
  });

  it('renders the confirmation information message', () => {
    const { getByText } = render(
      <NotificationModal
        action={{ name: 'PUBLISH', enabled: true, displayValue: 'Publish' }}
        onClose={onCloseMock}
        onConfirm={onConfirmMock}
        type='confirmation'
        isNonPublishingDay={false}
      />,
    );

    expect(getByText('You are about to publish the prices.')).toBeInTheDocument();
  });

  it('renders the cancel confirmation information message', () => {
    const { getByText } = render(
      <NotificationModal
        action={{ name: 'CANCEL', enabled: true, displayValue: 'Cancel' }}
        onClose={onCloseMock}
        onConfirm={onConfirmMock}
        type='confirmation'
        isNonPublishingDay={false}
      />,
    );

    expect(getByText('Are you sure you want to cancel this correction?')).toBeInTheDocument();
    expect(getByText('Keep editing')).toBeInTheDocument();
  });

  it('renders the send back confirmation information message', () => {
    const { getByText } = render(
      <NotificationModal
        action={{ name: 'SEND_BACK', enabled: true, displayValue: 'Send Back' }}
        onClose={onCloseMock}
        onConfirm={onConfirmMock}
        type='confirmation'
        isCorrection={true}
        isNonPublishingDay={false}
      />,
    );

    expect(getByText('Would you like to proceed?')).toBeInTheDocument();
  });

  it('auto closes notification popup on send back correction', async () => {
    const { getByText } = render(
      <NotificationModal
        action={{ name: WORKFLOW_ACTIONS.SEND_BACK, enabled: true, displayValue: 'Send Back' }}
        data={{ isSuccess: true, errorCode: '' }}
        onClose={onCloseMock}
        type='notification'
        isCorrection={true}
        onConfirm={onConfirmMock}
        isNonPublishingDay={false}
      />,
    );

    expect(getByText('The correction request was returned to the author.')).toBeInTheDocument();
    act(() => {
      jest.advanceTimersByTime(3000);
    });
    await waitFor(() => {
      expect(onCloseMock).toHaveBeenCalled();
    });
  });

  it('auto closes notification popup on approve correction', async () => {
    const { getByText } = render(
      <NotificationModal
        action={{ name: WORKFLOW_ACTIONS.APPROVE, enabled: true, displayValue: 'Approve' }}
        data={{ isSuccess: true, errorCode: '' }}
        onClose={onCloseMock}
        type='notification'
        isCorrection={true}
        onConfirm={onConfirmMock}
        isNonPublishingDay={false}
      />,
    );

    expect(getByText('Correction request approved')).toBeInTheDocument();
    act(() => {
      jest.advanceTimersByTime(3000);
    });
    await waitFor(() => {
      expect(onCloseMock).toHaveBeenCalled();
    });
  });

  it('auto closes notification popup on cancel correction', async () => {
    const { getByText } = render(
      <NotificationModal
        action={{ name: WORKFLOW_ACTIONS.CANCEL, enabled: true, displayValue: 'Cancel' }}
        data={{ isSuccess: true, errorCode: '' }}
        onClose={onCloseMock}
        type='notification'
        onConfirm={onConfirmMock}
        isNonPublishingDay={false}
      />,
    );

    expect(getByText('Correction cancelled')).toBeInTheDocument();
    act(() => {
      jest.advanceTimersByTime(3000);
    });
    await waitFor(() => {
      expect(onCloseMock).toHaveBeenCalled();
    });
  });

  it('renders the default confirmation information when action does not exist', () => {
    const { getByText } = render(
      <NotificationModal
        action={{ name: 'NON_EXISTENT_ACTION', enabled: true, displayValue: 'Non-existent' }}
        onClose={onCloseMock}
        onConfirm={onConfirmMock}
        type='confirmation'
        isNonPublishingDay={false}
      />,
    );

    expect(getByText('You are about to submit.')).toBeInTheDocument();
  });

  it('renders the confirmation question message', () => {
    const { getByText } = render(
      <NotificationModal
        action={{ name: 'PUBLISH', enabled: true, displayValue: 'Publish' }}
        onClose={onCloseMock}
        onConfirm={onConfirmMock}
        type='confirmation'
        isNonPublishingDay={false}
      />,
    );

    expect(
      getByText('Have the prices been reviewed? Are you sure you would like to proceed?'),
    ).toBeInTheDocument();
  });

  it('renders the error message when user changes workflow state during a correction with no changes', () => {
    const data: DataPackageTransitionToState = {
      isSuccess: false,
      errorCode: 'CORRECTION_CONTAINS_NO_CHANGES',
    };

    const { getByText } = render(
      <NotificationModal
        action={{ name: 'SEND_FOR_REVIEW', enabled: true, displayValue: 'Send for review' }}
        data={data}
        onClose={onCloseMock}
        type='notification'
        onConfirm={onConfirmMock}
        isNonPublishingDay={false}
      />,
    );

    expect(
      getByText('No changes have been made to the content since it was last published.'),
    ).toBeInTheDocument();

    expect(getByText('Please make changes before it can be sent for review.')).toBeInTheDocument();
  });

  it('renders the error message when not all reference price series are published', () => {
    const data: DataPackageTransitionToState = {
      isSuccess: false,
      errorCode: 'NOT_ALL_REFERENCE_PRICE_SERIES_ARE_PUBLISHED',
    };

    const { getByText } = render(
      <NotificationModal
        action={{ name: 'SEND_FOR_REVIEW', enabled: true, displayValue: 'Send for review' }}
        data={data}
        onClose={onCloseMock}
        type='notification'
        onConfirm={onConfirmMock}
        isNonPublishingDay={false}
      />,
    );

    expect(
      getByText('Please publish the referenced price series before publishing these assessments'),
    ).toBeInTheDocument();
  });

  it('renders the error message when not all inputs are published', () => {
    const data: DataPackageTransitionToState = {
      isSuccess: false,
      errorCode: 'NOT_ALL_INPUTS_ARE_PUBLISHED',
    };

    const { getByText } = render(
      <NotificationModal
        action={{ name: 'SEND_FOR_REVIEW', enabled: true, displayValue: 'Send for review' }}
        data={data}
        onClose={onCloseMock}
        type='notification'
        onConfirm={onConfirmMock}
        isNonPublishingDay={false}
      />,
    );

    expect(
      getByText(
        'Please ensure all input price series are published before these can be sent for review',
      ),
    ).toBeInTheDocument();
  });
});
