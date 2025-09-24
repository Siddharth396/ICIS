// istanbul ignore file
import { FC, useState } from 'react';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import { Modal, Button } from '@icis/ui-kit';
import DeleteConfirmationMessage from './DeleteConfirmationMessage';
import { Container, Footer, Wrapper } from './DeleteConfirmationModal.style';

type Props = {
  show: boolean;
  title: string;
  deleteConfirmationText: string[];
  warningTextMessages?: string[];
  testId: string;
  showCancel?: boolean;
  onClose: () => void;
  onDelete: () => void;
  canPerformDeleteAction?: () => boolean;
};

const DeleteConfirmationModal: FC<Props> = ({
  show,
  showCancel,
  title,
  deleteConfirmationText,
  testId,
  warningTextMessages = [],
  onClose,
  onDelete,
  canPerformDeleteAction = () => true,
}) => {
  const [showWarning, setShowWarning] = useState(() => !canPerformDeleteAction());

  const messages = useLocaleMessages();

  const handleDelete = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.stopPropagation();

    if (!canPerformDeleteAction()) {
      setShowWarning(true);
      return;
    }

    onDelete();
    onClose();
  };

  const handleClose = () => {
    setShowWarning(false);
    onClose();
  };

  const handleCancel = () => {
    handleClose();
  };

  if (!show) return null;

  return (
    <Modal
      testId={testId}
      ariaLabel={`${title} Modal`}
      isOpen={show}
      variant='Small'
      version='V2'
      onDismiss={handleClose}
      allowOverflow
      header={{
        visible: true,
        title: 'Delete Confirmation',
      }}
      footer={{
        visible: true,
        FooterContent: () => (
          <Footer>
            {showWarning && (
              <Button
                testId='modal-footer-cancel-btn'
                onClick={handleCancel}
                variant='AuthoringPrimary'>
                {messages.General.Cancel}
              </Button>
            )}
            {!showWarning && showCancel && (
              <Button testId='modal-footer-cancel-btn' onClick={handleCancel} variant='Tertiary'>
                {messages.General.Cancel}
              </Button>
            )}
            {!showWarning && (
              <Button testId='modal-footer-delete-btn' variant='Warning' onClick={handleDelete}>
                {messages.General.Delete}
              </Button>
            )}
          </Footer>
        ),
      }}>
      <Wrapper>
        <Container>
          {showWarning && <DeleteConfirmationMessage textContents={warningTextMessages} />}
          {!showWarning && <DeleteConfirmationMessage textContents={deleteConfirmationText} />}
        </Container>
      </Wrapper>
    </Modal>
  );
};

export default DeleteConfirmationModal;
