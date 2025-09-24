import { FC, useCallback, useEffect } from 'react';
import { Modal, Icon, theme, Button, IconKeys, Text, Loader, SvgIcon } from '@icis/ui-kit';
import { DataPackageTransitionToState, NextAction } from 'apollo/queries';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import { Container, Footer, IconContainer, MessageBody, MessagesWrapper } from './styled';
import PublishIcon from './PublishIcon';
import { ERROR_CODES, REVIEW_ACTIONS, WORKFLOW_ACTIONS } from 'utils/constants';

type Props = {
  type?: 'notification' | 'confirmation';
  loading?: boolean;
  action: NextAction;
  data?: DataPackageTransitionToState | null;
  isCorrection?: boolean;
  onClose: () => void;
  onConfirm: (actionName: string) => void;
  isNonPublishingDay: boolean;
};

const NotificationModal: FC<Props> = ({
  action,
  type = 'notification',
  data,
  loading,
  isNonPublishingDay,
  isCorrection = false,
  onClose,
  onConfirm,
}) => {
  const messages = useLocaleMessages();
  // istanbul ignore next
  const actionName =
    action.name in messages.Workflow.Actions
      ? ((isCorrection
          ? action.name + '_CORRECTION'
          : action.name) as keyof typeof messages.Workflow.Actions)
      : ('DEFAULT_ACTION' as keyof typeof messages.Workflow.Actions);
  const autoClose =
    type === 'notification' &&
    (isCorrection || action.name === WORKFLOW_ACTIONS.CANCEL) &&
    (action.name === WORKFLOW_ACTIONS.APPROVE ||
      action.name === WORKFLOW_ACTIONS.SEND_BACK ||
      action.name === WORKFLOW_ACTIONS.CANCEL);

  const renderIcon = () => {
    // istanbul ignore else
    if (type === 'notification') {
      const iconName: IconKeys = data?.isSuccess ? IconKeys.circleCheck : IconKeys.warningTriangle;
      const iconColor = data?.isSuccess ? theme.colours.SECONDARY_2 : theme.colours.WARNING;
      return (
        <IconContainer>
          <Icon icon={iconName} testId={iconName} color={iconColor} />
        </IconContainer>
      );
    } else if (type === 'confirmation') {
      return action.name === WORKFLOW_ACTIONS.CANCEL ? (
        <IconContainer>
          <SvgIcon icon='rejectPage' size={{ height: '64px', width: '64px' }} />
        </IconContainer>
      ) : (
        <IconContainer>
          <PublishIcon />
        </IconContainer>
      );
    }
    // istanbul ignore next
    return null;
  };

  const renderNotificationMessage = () => {
    const { isSuccess, errorCode } = data || {};

    // istanbul ignore next
    if (isSuccess) {
      const successMessage =
        messages?.Workflow[actionName]?.SuccessMessage?.Submitted ||
        messages.Workflow.DEFAULT_MESSAGE.SuccessMessage.Submitted;

      return <MessageBody>{successMessage}</MessageBody>;
    }

    const CorrectionMessages = messages.Workflow.EmptyPageValidationMessage.Correction;
    const PublishMessages = messages.Workflow.EmptyPageValidationMessage.Publish;
    const DefaultMessages = messages.Workflow.EmptyPageValidationMessage.DEFAULT;

    switch (errorCode) {
      case ERROR_CODES.CORRECTION_CONTAINS_NO_CHANGES:
        return (
          <>
            <MessageBody>
              {CorrectionMessages.CORRECTION_CONTAINS_NO_CHANGES_PARAGRAPH_1}
            </MessageBody>
            <MessageBody>
              {CorrectionMessages.CORRECTION_CONTAINS_NO_CHANGES_PARAGRAPH_2}
            </MessageBody>
          </>
        );

      case ERROR_CODES.NOT_ALL_REFERENCE_PRICE_SERIES_ARE_PUBLISHED:
        return (
          <MessageBody>{PublishMessages.NOT_ALL_REFERENCE_PRICE_SERIES_ARE_PUBLISHED}</MessageBody>
        );

      case ERROR_CODES.NOT_ALL_INPUTS_ARE_PUBLISHED:
        return <MessageBody>{PublishMessages.NOT_ALL_INPUTS_ARE_PUBLISHED}</MessageBody>;

      default:
        return <MessageBody>{DefaultMessages.NOT_ALL_PRICE_SERIES_ITEMS_ARE_VALID}</MessageBody>;
    }
  };

  // istanbul ignore next
  const renderConfirmationMessage = () => (
    <>
      <Text.Body>
        {messages?.Workflow[actionName]?.ConfirmationMessage?.ConfirmationInformation ||
          messages.Workflow.DEFAULT_MESSAGE.ConfirmationMessage.ConfirmationInformation}
      </Text.Body>
      {isNonPublishingDay && REVIEW_ACTIONS.includes(actionName) && (
        <Text.Body>
          <Icon
            icon={IconKeys.information}
            testId={IconKeys.information}
            style={{ marginRight: '0.4em' }}
          />
          {messages?.Workflow.NonPublishingDayWarning}
        </Text.Body>
      )}
      <Text.Body>
        {messages?.Workflow[actionName]?.ConfirmationMessage?.ConfirmationQuestion ||
          messages.Workflow.DEFAULT_MESSAGE.ConfirmationMessage.ConfirmationInformation}
      </Text.Body>
    </>
  );

  const renderMessage = useCallback(() => {
    // istanbul ignore else
    if (type === 'notification') {
      return <MessagesWrapper>{renderNotificationMessage()}</MessagesWrapper>;
    } else if (type === 'confirmation') {
      return <MessagesWrapper>{renderConfirmationMessage()}</MessagesWrapper>;
    }
  }, [data, type]);

  const modalTitle =
    type === 'notification'
      ? data?.isSuccess
        ? messages.Workflow[actionName].SuccessMessage.Title
        : messages.Workflow.EmptyPageValidationMessage.Title
      : messages.Workflow[actionName].ConfirmationMessage.Title;

  const renderFooterContent = () => (
    <Footer>
      <Button testId='modal-footer-cancel-btn' variant='Tertiary' onClick={onClose}>
        {actionName === WORKFLOW_ACTIONS.CANCEL
          ? messages.General.KeepEditing
          : messages.General.Cancel}
      </Button>
      <Button
        disabled={loading}
        testId='modal-footer-publish-btn'
        variant='AuthoringPrimary'
        onClick={() => onConfirm(action.name)}>
        {
          // istanbul ignore next
          loading ? (
            <Loader size='lg' />
          ) : (
            messages.Workflow?.Actions[actionName] || messages.Workflow.Actions.DEFAULT_ACTION
          )
        }
      </Button>
    </Footer>
  );

  useEffect(() => {
    if (autoClose) {
      const timer = setTimeout(() => {
        onClose();
      }, 2000);

      // Cleanup the timer on component unmount or when isOpen changes
      return () => clearTimeout(timer);
    }
  }, [onClose]);

  return (
    <Modal
      testId='workflow-validation-modal'
      ariaLabel='Workflow Validation Modal'
      isOpen={!!data || type === 'confirmation'}
      variant='Small'
      version='V2'
      header={
        autoClose
          ? {
              visible: true,
              title: modalTitle,
              DismissButton: () => <></>,
            }
          : {
              visible: true,
              title: modalTitle,
            }
      }
      footer={{
        visible: type === 'confirmation',
        FooterContent: type === 'confirmation' ? renderFooterContent : undefined,
      }}
      onDismiss={onClose}>
      <Container>
        {!(isCorrection && action.name === WORKFLOW_ACTIONS.SEND_BACK && type === 'confirmation') &&
          renderIcon()}
        {renderMessage()}
      </Container>
    </Modal>
  );
};

export default NotificationModal;
