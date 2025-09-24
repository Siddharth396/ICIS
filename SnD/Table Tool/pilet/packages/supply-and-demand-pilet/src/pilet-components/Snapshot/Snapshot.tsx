import { useUser } from '@icis/app-shell-apis';
import { SnapshotContainer, TextBody } from './Snapshot.styled';
import { Props } from './Snapshot.types';
import { getSnapshotDate } from '../../Common/Helper/date';
import getMessages from '../../constants/getMessages';

const Snapshot = (props: Props) => {
  const { locale } = useUser();
  const messages = getMessages(locale);

  return (
    <SnapshotContainer data-testid="snapshot-container">
      <TextBody>{formatMessage(messages.messages.Snapshot, getSnapshotDate(props.LastUpdatedDateUnix))}</TextBody>
    </SnapshotContainer>
  )
}

function formatMessage(message: string, ...values: string[]): string {
  return message.replace(/{(\d+)}/g, (match, index) => values[index] || match);
}

export default Snapshot;
