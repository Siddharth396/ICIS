// istanbul ignore file
import { useEffect, useRef, useState } from 'react';
import { CapabilityParams, CapabilityProps, PiletApi } from '@icis/app-shell';
import withClient, { ApolloClientProps } from 'components/HOCs/withClient';
import { useLazyQuery } from '@apollo/client';
import {
  PriceDisplayContentBlockResponse,
  GET_PRICE_DISPLAY_CONTENT_BLOCK_SUBSCRIBER,
} from 'apollo/queries';
import PriceDisplayTableContentSubscriber from './PriceDisplayTableContentSubscriber';
import { ContentBlockWrapper, SubscriberWrapper, Title } from './styled';
import { getDayEndDateInUTC } from '../../utils/date';

interface IPriceDisplayCapabilitySubscriber extends CapabilityProps {
  params: CapabilityParams & { piralApi: PiletApi };
}

const PriceDisplayContainerSubscriber = ({
  params: capabilityParams,
  client,
}: IPriceDisplayCapabilitySubscriber & ApolloClientProps) => {
  const [title, setTitle] = useState<string | undefined>('');
  const [contentBlock, setContentBlock] = useState<PriceDisplayContentBlockResponse>();
  const snapshotDate = capabilityParams?.filter?.snapshotDate ?? getDayEndDateInUTC();

  /** The added reference resolve the issue of old title
    data being sent when there is a change in the table data. **/
  const titleRef = useRef<string | undefined>('');

  // query to fetch the content block
  const [getContentBlock, { data, loading }] = useLazyQuery<PriceDisplayContentBlockResponse>(
    GET_PRICE_DISPLAY_CONTENT_BLOCK_SUBSCRIBER,
    {
      client,
      fetchPolicy: 'no-cache',
    },
  );

  useEffect(() => {
    /* istanbul ignore else */
    getContentBlock({
      variables: {
        contentBlockId: capabilityParams?.id,
        version: Number(capabilityParams?.version),
        assessedDateTime: snapshotDate,
      },
    });
  }, [capabilityParams?.id, capabilityParams?.filter?.snapshotDate]);

  useEffect(() => {
    /* istanbul ignore next */
    if (data?.contentBlockForDisplay?.contentBlockId) {
      const contentTitle = data.contentBlockForDisplay?.title;
      setTitle(contentTitle);
      titleRef.current = contentTitle;
      setContentBlock({
        ...data,
        contentBlockForDisplay: {
          ...data.contentBlockForDisplay,
          gridConfiguration: data.contentBlockForDisplay.priceDisplayGridConfiguration,
          priceSeries: data.contentBlockForDisplay.priceSeriesItemForDisplay,
        },
      });
    }
  }, [data?.contentBlockForDisplay]);

  return (
    <SubscriberWrapper data-testid='price-display-subscriber-wrapper'>
      <Title data-testid='title'>{title || ''}</Title>
      <ContentBlockWrapper data-testid='price-display-content-block-wrapper'>
        <PriceDisplayTableContentSubscriber loading={loading} data={contentBlock} />
      </ContentBlockWrapper>
    </SubscriberWrapper>
  );
};

export default withClient(PriceDisplayContainerSubscriber);
