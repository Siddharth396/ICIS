// istanbul ignore file
import React from 'react';
import { FooterButtonsWrapper } from './PriceSeriesSelectModal.style';
import { Button } from '@icis/ui-kit';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';

type Props = {
  onClose: ((e?: any) => void) | undefined;
  canSave: boolean;
  onSave: () => void;
  saveButtonTitle?: string;
  cancelButtonTestId?: string;
  saveButtonTestId?: string;
};

const ModalFooter = ({
  onClose,
  onSave,
  canSave,
  saveButtonTitle,
  cancelButtonTestId,
  saveButtonTestId,
}: Props) => {
  const messages = useLocaleMessages();

  return (
    <FooterButtonsWrapper data-testid='footer-buttons'>
      <Button variant='Tertiary' size='Large' onClick={onClose as any} testId={cancelButtonTestId}>
        {messages.General.Cancel}
      </Button>
      <Button
        disabled={!canSave}
        variant='AuthoringPrimary'
        size='Large'
        onClick={onSave}
        testId={saveButtonTestId}>
        {saveButtonTitle || messages.General.Save}
      </Button>
    </FooterButtonsWrapper>
  );
};

export default ModalFooter;
