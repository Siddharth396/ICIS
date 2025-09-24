// istanbul ignore file
import { useCallback, useState, useEffect } from 'react';
import { useMutation } from '@apollo/client';
import { useCapabilities } from '@icis/app-shell-apis';
import { Capability } from '@icis/app-shell';
import { ContentBlockResponse, UPDATE_COMMENTARY } from 'apollo/queries';
import withClient, { ApolloClientProps } from 'components/HOCs/withClient';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import { getStartOfDayUTC } from 'utils/date';
import { CommentaryContainer, CommentaryTitle } from '../styled';

interface IPriceEntryCommentary {
  readOnlyView: boolean;
  data: ContentBlockResponse | undefined;
  contentBlockId: string;
  selectedDate: Date;
  refetchGridData: () => void;
  onStartEditing: () => void;
  onFinishEditing: () => void;
}

const PriceEntryCommentary: React.FC<IPriceEntryCommentary & ApolloClientProps> = ({
  client,
  data,
  readOnlyView,
  contentBlockId,
  selectedDate,
  refetchGridData,
  onStartEditing,
  onFinishEditing,
}) => {
  const messages = useLocaleMessages();
  const [commentary, setCommentary] = useState({ commentaryId: '', version: '' });
  const richTextCapability: Capability = useCapabilities()['richtext-capability'];
  const [updateCommentary] = useMutation(UPDATE_COMMENTARY, { client });

  useEffect(() => {
    const existingCommentary = data?.contentBlock?.commentary;
    if (existingCommentary && existingCommentary.commentaryId) {
      setCommentary(existingCommentary);
    } else {
      setCommentary({ commentaryId: crypto.randomUUID(), version: '' });
    }
  }, [
    selectedDate,
    data?.contentBlock?.commentary?.commentaryId,
    data?.contentBlock?.commentary?.version,
  ]);

  const handlePostCommentaryUpdate = useCallback(
    async (version: string) => {
      const operation = data?.contentBlock?.nextActions?.find((action) => action.name === 'CANCEL')
        ? 'correction'
        : '';
      await updateCommentary({
        variables: {
          commentaryInput: {
            contentBlockId,
            version,
            assessedDateTime: getStartOfDayUTC(selectedDate),
            commentaryId: commentary.commentaryId,
            operationType: operation,
          },
        },
      });
      refetchGridData();
    },
    [contentBlockId, commentary.commentaryId, selectedDate, refetchGridData],
  );

  const editorKey = commentary.commentaryId
    ? `${commentary.commentaryId}-${commentary.version}`
    : `empty-${selectedDate.toISOString()}`;

  return (
    richTextCapability?.Component && (
      <CommentaryContainer>
        <CommentaryTitle>{messages.Capabilty.PriceCommentary}</CommentaryTitle>
        <richTextCapability.Component
          key={editorKey}
          // @ts-ignore
          params={{
            id: commentary.commentaryId,
            version: commentary.version,
            metadata: {
              locationInfo: {
                sectionTitle: messages.Capabilty.PriceCommentary,
              },
              displayMode: 'default',
              config: [
                {
                  key: 'type',
                  value: 'price-entry-commentary',
                },
                {
                  key: 'showTableControls',
                  value: 'true',
                },
              ],
            },
            isAuthoring: true,
            forceSubscriberView: readOnlyView,
            onSave: handlePostCommentaryUpdate,
            onStartEditing,
            onFinishEditing,
          }}
        />
      </CommentaryContainer>
    )
  );
};

export default withClient(PriceEntryCommentary);
