import { useState, useEffect } from 'react';
import { useMutation } from '@apollo/client';
import withClient, { ApolloClientProps } from 'components/HOCs/withClient';
import {
  DATA_PACKAGE_TRANSITION_TO_STATE,
  DataPackageTransitionToState,
  dataPackageTransitionToStateResponse,
  INITIATE_CORRECTION,
  NextAction,
  TOGGLE_NON_MARKET_ADJUSTMENT,
} from 'apollo/queries';
import NotificationModal from 'components/NotificationModal';
import { WorkflowButton } from './WorkflowButton';
import { WorkflowButtonWrapper } from './styled';
import { NMA_ACTIONS, WORKFLOW_ACTIONS } from 'utils/constants';

interface IContentBlockParams {
  version: number;
  contentBlockId: string;
  priceSeriesIds: string[];
  assessedDateTime?: Date;
  nmaEnabled?: boolean;
}

interface IWorkflowAuthoring {
  isContentLocked: boolean;
  contentBlockParams: IContentBlockParams;
  nextActions: NextAction[];
  client?: any;
  getReviewURL: () => string;
  onRefetchGridData?: (includeNotStarted?: boolean) => void;
  isNonPublishingDay: boolean;
}

const WorkflowAuthoring = ({
  isContentLocked,
  contentBlockParams,
  client,
  nextActions,
  getReviewURL,
  onRefetchGridData,
  isNonPublishingDay,
}: IWorkflowAuthoring & ApolloClientProps) => {
  const [modalData, setModalData] = useState<DataPackageTransitionToState | null>(null);
  const [modalType, setModalType] = useState<'notification' | 'confirmation'>('notification');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [currentAction, setCurrentAction] = useState<NextAction | null>(null);
  const isCorrection = Boolean(nextActions?.find((action) => action.name === 'CANCEL'));

  const [dataPackageTransitionToState, { loading: initiatingDataPackageTransition }] =
    useMutation<dataPackageTransitionToStateResponse>(DATA_PACKAGE_TRANSITION_TO_STATE, {
      client,
    });

  const [initiateCorrection, { loading: initiatingCorrection }] = useMutation<boolean>(
    INITIATE_CORRECTION,
    {
      client,
      onCompleted: /* istanbul ignore next */ (response: boolean) =>
        handlePostCorrectionInitiated(response),
    },
  );

  const [toggleNonMarketAdjustment, { loading: togglingNma }] = useMutation(
    TOGGLE_NON_MARKET_ADJUSTMENT,
    {
      client,
      onCompleted: /* istanbul ignore next */ (response) => {
        // istanbul ignore next
        if (response?.toggleNonMarketAdjustment) {
          setModalData({
            isSuccess: true,
            errorCode: '',
          });
          setModalType('notification');
          onRefetchGridData?.();
        }
      },
    },
  );

  useEffect(() => {
    if (modalData || modalType === 'confirmation') {
      setIsModalOpen(true);
    } else {
      setIsModalOpen(false);
    }
  }, [modalData, modalType]);

  // istanbul ignore next
  const handlePostCorrectionInitiated = (response: any) => {
    if (response?.initiateCorrectionForDataPackage) {
      onRefetchGridData?.();
    }
    if (nextActions?.find((action) => action.displayValue === 'Re-publish')) {
      setModalData({
        isSuccess: response?.initiateCorrectionForDataPackage,
        errorCode: '',
      });
      setModalType('notification');
    }
  };

  // istanbul ignore next
  const handleConfirmation = (action: NextAction) => {
    // no popup for corrections (only for advance workflow)
    setModalData(null);
    setCurrentAction(action);
    if (
      (action.name === WORKFLOW_ACTIONS.INITIATE_CORRECTION &&
        action.displayValue !== 'Re-publish') ||
      (isCorrection && action.name === WORKFLOW_ACTIONS.SEND_FOR_REVIEW)
    ) {
      handleSaveStatus(action.name);
    } else {
      setModalType('confirmation');
      setIsModalOpen(true);
    }
  };

  // istanbul ignore next
  const handlePostAction = (actionName: string, response: dataPackageTransitionToStateResponse) => {
    const isPublishAction = actionName === WORKFLOW_ACTIONS.PUBLISH;
    const refetchDelay = isPublishAction ? 0 : 1000;
    const transitionResponse = response.dataPackageTransitionToState;

    if (transitionResponse?.isSuccess) {
      setTimeout(() => {
        onRefetchGridData?.(isPublishAction);
      }, refetchDelay);
    } else {
      onRefetchGridData?.();
    }

    setModalData({
      isSuccess: transitionResponse?.isSuccess ?? false,
      errorCode: transitionResponse?.errorCode ?? '',
    });
    setModalType('notification');
  };

  // istanbul ignore next
  const handleSaveStatus = (actionName: string) => {
    const { version, ...restContentBlockParams } = contentBlockParams;

    if (actionName === WORKFLOW_ACTIONS.INITIATE_CORRECTION) {
      initiateCorrection({
        variables: {
          contentBlockId: restContentBlockParams.contentBlockId,
          version,
          assessedDateTime: restContentBlockParams.assessedDateTime,
          reviewPageUrl: getReviewURL(),
        },
      });
    } else if (actionName === NMA_ACTIONS.START_NMA || actionName === NMA_ACTIONS.CANCEL_NMA) {
      toggleNonMarketAdjustment({
        variables: {
          contentBlockId: restContentBlockParams.contentBlockId,
          version,
          assessedDateTime: restContentBlockParams.assessedDateTime,
          enabled: !restContentBlockParams?.nmaEnabled,
        },
      });
    } else {
      const isPublishAction = actionName === WORKFLOW_ACTIONS.PUBLISH;
      const operation = isCorrection && !isPublishAction ? 'correction' : '';

      dataPackageTransitionToState({
        variables: {
          contentBlockId: restContentBlockParams.contentBlockId,
          version,
          assessedDateTime: restContentBlockParams.assessedDateTime,
          nextState: actionName,
          operationType: operation,
          reviewPageUrl: getReviewURL(),
        },
        onCompleted: (response) => handlePostAction(actionName, response),
      });
    }
  };

  const handleCloseModal = () => {
    setModalData(null);
    setModalType('notification');
    setIsModalOpen(false);
  };

  return (
    <WorkflowButtonWrapper>
      {nextActions
        ?.filter((action) => action.enabled)
        .map((action) => {
          return (
            <div key={action.name}>
              <WorkflowButton
                disabled={isContentLocked}
                action={action}
                onConfirmation={() => handleConfirmation(action)}
              />
            </div>
          );
        })}
      {isModalOpen && currentAction && (
        <NotificationModal
          loading={initiatingCorrection || initiatingDataPackageTransition || togglingNma}
          action={currentAction}
          type={modalType}
          data={modalData}
          isCorrection={isCorrection && currentAction.name !== WORKFLOW_ACTIONS.CANCEL}
          onClose={handleCloseModal}
          onConfirm={handleSaveStatus}
          isNonPublishingDay={isNonPublishingDay}
        />
      )}
    </WorkflowButtonWrapper>
  );
};

export default withClient(WorkflowAuthoring);
