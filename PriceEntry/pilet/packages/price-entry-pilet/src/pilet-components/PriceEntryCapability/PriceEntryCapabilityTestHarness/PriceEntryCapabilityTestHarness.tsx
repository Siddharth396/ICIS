// istanbul ignore file
import React, { useEffect, useState, useRef, useCallback } from 'react';
import { ButtonsContainer, HarnessWrapper, Header } from './styled';
import { Button } from '@icis/ui-kit';
import { mockVersion, updateTestHarnessUrl } from './utils';
import { ContentDisplayMode } from '@icis/app-shell';

const PriceEntryCapabilityAuthoring = React.lazy(
  () =>
    import(
      /* webpackChunkName: "price-entry-PriceEntryCapabilityAuthoring"*/ 'pilet-components/PriceEntryCapability'
    ),
);

const PriceEntryCapabilityTestHarness = ({ piralApi }: any) => {
  const urlSearchParams = new URLSearchParams(window.location.search);
  const [version, setVersion] = useState<string>(() => {
    const queryStringVersion = urlSearchParams.get('version');
    return queryStringVersion || mockVersion;
  });
  const [id, setId] = useState<string>(() => {
    const queryStringId = urlSearchParams.get('id');
    return queryStringId || crypto.randomUUID();
  });

  const [isLocked, setIsLocked] = useState<boolean>(false);
  const [lockInformation, setLockInformation] = useState<any>({
    lockedContentData: [],
  });

  useEffect(() => {
    updateTestHarnessUrl(id, version);
  }, [id, version]);

  // Handle resetting the view
  const handleResetView = useCallback(() => {
    const newId = crypto.randomUUID();
    setId(newId);
    setVersion('');
    setLockInformation({ lockedContentData: [] });
    setTimeout(() => window.location.reload(), 100);
  }, []);

  // Handle start editing (lock the content)
  const handleOnStartEditing = () => {
    setIsLocked(true);
    const lockedContentDetails = {
      id: id, // Assume id is used as the content identifier
      lockedContent: [
        {
          lockedContentDetails: [
            {
              id: 'price-entry-authoring', // This would be specific to your case
              data: id,
            },
          ],
        },
      ],
    };

    setLockInformation((prev: any) => ({
      ...prev,
      lockedContentData: [...prev.lockedContentData, lockedContentDetails],
    }));
  };

  // Handle finish editing (unlock the content)
  const handleOnFinishEditing = () => {
    setIsLocked(false);
    setLockInformation((prev: any) => ({
      ...prev,
      lockedContentData: prev.lockedContentData.filter((lock: any) => lock.id !== id),
    }));
  };

  const handleOnSave = (newVersion: string) => {
    setVersion(newVersion);
  };

  // Using useRef for params to keep it stable
  const paramsRef = useRef({
    id,
    version,
    isAuthoring: true,
    onSave: handleOnSave,
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
          value: 'price-entry',
        },
      ],
    },
    displayMode: 'fill' as ContentDisplayMode,
    piralApi,
    lockInformation,
    notifyLatestContent: () => {},
  });

  // Update paramsRef whenever id, version, isLocked, or lockInformation changes
  useEffect(() => {
    paramsRef.current = {
      ...paramsRef.current,
      id,
      version,
      isLocked,
      lockInformation,
    };
  }, [id, version, isLocked, lockInformation]);

  return (
    <HarnessWrapper>
      <Header>
        <ButtonsContainer>
          {version && (
            <Button variant='Tertiary' onClick={handleResetView}>
              Reset Initial View
            </Button>
          )}
        </ButtonsContainer>
      </Header>
      {/* @ts-ignore */}
      <PriceEntryCapabilityAuthoring params={paramsRef.current} />
    </HarnessWrapper>
  );
};

export default PriceEntryCapabilityTestHarness;
