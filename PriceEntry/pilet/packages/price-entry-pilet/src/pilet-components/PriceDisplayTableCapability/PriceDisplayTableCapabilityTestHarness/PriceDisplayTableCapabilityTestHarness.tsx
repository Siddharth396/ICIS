// istanbul ignore file
import React, { useEffect, useState } from 'react';
import {
  ButtonsContainer,
  HarnessWrapper,
  Header,
} from '../../PriceEntryCapability/PriceEntryCapabilityTestHarness/styled';
import { Button } from '@icis/ui-kit';
import {
  mockVersion,
  updateTestHarnessUrl,
} from '../../PriceEntryCapability/PriceEntryCapabilityTestHarness/utils';
import { ContentDisplayMode } from '@icis/app-shell';
import { getDayEndDateInUTC } from '../../../utils/date';
import { getIsAuthoring } from '../../../constants/isAuthoring';

const PriceDisplayTableCapabilityAuthoring = React.lazy(
  () =>
    import(
      /* webpackChunkName: "price-table-PriceDisplayTableCapabilityAuthoring"*/ 'pilet-components/PriceDisplayTableCapability/PriceDisplayTableContainerAuthoring'
    ),
);

const PriceDisplayTableCapabilitySubscriber = React.lazy(
  () =>
    import(
      /* webpackChunkName: "price-table-PriceDisplayTableCapabilitySubscriber"*/ 'pilet-components/PriceDisplayTableCapability/PriceDisplayTableContainerSubscriber'
    ),
);

interface PriceDisplayTableCapabilityTestHarnessProps {
  piralApi: any;
  isAuthoring: boolean;
}

const PriceDisplayTableCapabilityTestHarness: React.FC<
  PriceDisplayTableCapabilityTestHarnessProps
> = ({ piralApi, isAuthoring1 }: any) => {
  const urlSearchParams = new URLSearchParams(window.location.search);
  const [version, setVersion] = useState<string>(() => {
    const queryStringVersion = urlSearchParams.get('version');
    return queryStringVersion || mockVersion;
  });
  const [id, setId] = useState<string>(() => {
    const queryStringId = urlSearchParams.get('id');
    return queryStringId || crypto.randomUUID();
  });

  const isAuthoring = getIsAuthoring();
  const [isLocked, setIsLocked] = useState<boolean>(false);

  useEffect(() => {
    updateTestHarnessUrl(id, version);
  }, [id, version]);

  const handleResetView = () => {
    const newId = crypto.randomUUID();
    setId(newId);
    setVersion('');
    setTimeout(() => window.location.reload(), 100);
  };

  const handleOnStartEditing = () => {
    setIsLocked(true);
  };

  const handleOnFinishEditing = () => {
    setIsLocked(false);
  };

  const handleToggleView = () => {
    const url = window.location.href;
    const subPath = '/mfe/price-display-table';
    const authoringUrl = `${subPath}/authoring`;
    const subscriberUrl = subPath;
    const absoluteUrl = isAuthoring ? url.replace(authoringUrl, subscriberUrl) : url.replace(subscriberUrl, authoringUrl);
    window.location.replace(absoluteUrl);
  };

  const params = {
    id,
    version,
    isAuthoring: isAuthoring,
    onSave: (version: string) => {
      setVersion(version);
    },
    onConfigChanged: () => {},
    isLocked: isLocked,
    lockedToOtherClient: false,
    onStartEditing: handleOnStartEditing,
    onFinishEditing: handleOnFinishEditing,
    metadata: {
      displayMode: 'default',
      config: [
        {
          key: 'type',
          value: 'price-table',
        },
      ],
    },
    displayMode: 'fill' as ContentDisplayMode,
    piralApi,
    notifyLatestContent: () => {},
    filter: { snapshotDate: getDayEndDateInUTC() },
  };

  return (
    <HarnessWrapper>
      <Header>
        <ButtonsContainer>
          {version && (
            <Button variant='Tertiary' onClick={handleResetView}>
              Reset Initial View
            </Button>
          )}
          {(version && id) && (
            <Button variant="Secondary" onClick={handleToggleView}>
              Load {isAuthoring ? 'Subscriber' : 'Authoring'} View
            </Button>
          )}
        </ButtonsContainer>
      </Header>
      {isAuthoring ? (
        /* @ts-ignore */
        <PriceDisplayTableCapabilityAuthoring params={params} />
      ) : (
        /* @ts-ignore */
        <PriceDisplayTableCapabilitySubscriber params={params} />
      )}
    </HarnessWrapper>
  );
};

export default PriceDisplayTableCapabilityTestHarness;
