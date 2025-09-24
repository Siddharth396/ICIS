// istanbul ignore file
import {
  DateMessage,
  DateMessageToolTip,
  MessageInfoIcon,
  NonPublishingDayMessage,
  PublishingDayMessage,
} from './styled';
import { theme } from '@icis/ui-kit';

const PublicationDateMessage = ({
  isPublishingDay,
  formattedNextPublicationDate,
}: {
  isPublishingDay: boolean;
  formattedNextPublicationDate: string;
}) => (
  <DateMessage isPublishingDay={isPublishingDay}>
    {isPublishingDay ? (
      <PublishingDayMessage>
        Scheduled publish date: {formattedNextPublicationDate}
      </PublishingDayMessage>
    ) : (
      <>
        <MessageInfoIcon icon='warning' data-for='date-message' data-tip />
        <NonPublishingDayMessage>This is a non-publishing day</NonPublishingDayMessage>
        <DateMessageToolTip
          id='date-message'
          effect='float'
          place='top'
          arrowColor='black'
          style={{
            backgroundColor: theme.colours.PRIMARY_1,
            color: theme.colours.NEUTRALS_6,
            padding: '8px',
          }}>
          The selected date is a non-publishing day. Once the content has been reviewed and approved
          by Copy Editors, it will be published immediately.
        </DateMessageToolTip>
      </>
    )}
  </DateMessage>
);

export default PublicationDateMessage;
