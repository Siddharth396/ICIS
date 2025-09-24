import { Modal, ModalVersion } from '@icis/ui-kit';
import React from 'react';

type HeaderProps = {
  visible?: boolean;
  title?: string;
};

export type ModalCalendarProps = {
  testId?: string;
  header?: HeaderProps;
  withModal?: boolean;
  showModal?: boolean;
  onModalDismiss?: (e?: any) => void;
};

type Props = {
  children: React.ReactNode;
  testId?: string;
  header: HeaderProps;
  withModal: boolean;
  showModal: boolean;
  version?: ModalVersion;
  onModalDismiss?: (e?: any) => void;
};

const ModalCalendar = ({
  children,
  testId,
  header,
  withModal,
  showModal,
  version = 'V1',
  onModalDismiss,
}: Props) => {
  return (
    <>
      {withModal && (
        /* istanbul ignore next */ <Modal
          testId={`${testId}-modal`}
          onDismiss={onModalDismiss}
          isOpen={showModal}
          ariaLabel='day--picker--modal'
          fitContent={true}
          variant='Small'
          version={version}
          header={header}
          footer={{ visible: false }}>
          {children}
        </Modal>
      )}
      {!withModal && children}
    </>
  );
};

export default ModalCalendar;
