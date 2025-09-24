// istanbul ignore file
import { Text } from '@icis/ui-kit';
import { FC } from 'react';
import DeleteWarningIcon from 'components/icons/DeleteWarningIcon';
import { IconWrapper, MessagesWrapper } from './DeleteConfirmationModal.style';

type Props = {
  textContents: string[];
};

const DeleteConfirmationMessage: FC<Props> = ({ textContents }) => {
  return (
    <>
      <IconWrapper>
        <DeleteWarningIcon />
      </IconWrapper>
      <MessagesWrapper>
        {textContents.map((text, i) => (
          <Text.Body key={i}>{text}</Text.Body>
        ))}
      </MessagesWrapper>
    </>
  );
};

export default DeleteConfirmationMessage;
