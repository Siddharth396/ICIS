// istanbul ignore file
import { Button } from '@icis/ui-kit';
import { useEffect, useState } from 'react';
import { ButtonsContainer, Container, HarnessWrapper, Header, ContentSection, Title } from './styled';
import { mockMethod, mockVersion, updateTestHarnessUrl } from './utils';
import App from '../App';
import { getIsAuthoring } from 'constants/isAuthoring';
import getRedirectUrl from '../../../constants/getRedirectUrl';

const AppTestHarness = () => {
  const isAuthoring = getIsAuthoring();
  const urlSearchParams = new URLSearchParams(window.location.search);
  // #region States
  const [version, setVersion] = useState<string>(() => {
    const queryStringVersion = urlSearchParams.get('version');
    return queryStringVersion || mockVersion;
  });
  const [id, setId] = useState<string>(() => {
    const queryStringId = urlSearchParams.get('id');
    return queryStringId || crypto.randomUUID();
  });
  const [isLocked, setIsLocked] = useState<boolean>(false);
  // #endregion

  // #region Effects
  useEffect(() => {
    updateTestHarnessUrl(id, version);
  }, [id, version]);
  // #endregion

  // #region Handlers
  const handleResetView = () => {
    const newId = crypto.randomUUID();
    setId(newId);
    setVersion(version);
    setTimeout(() => window.location.reload(), 100);
  };

  const handleToggleView = () => {
    const redirectUrl = getRedirectUrl(isAuthoring);
    window.location.replace(redirectUrl);
  };

  const handleOnStartEditing = () => {
    setIsLocked(true);
  };

  const handleOnFinishEditing = () => {
    setIsLocked(false);
  };
  // #endregion

  return (
    <HarnessWrapper>
      <Header>
        <Title>SND Tool Capability</Title>
        <ButtonsContainer>
          {version && isAuthoring && (
            <Button variant="Tertiary" onClick={handleResetView}>
              Reset View
            </Button>
          )}
          {((version && id) || !isAuthoring) && (
            <Button variant="Secondary" onClick={handleToggleView}>
              Load {isAuthoring ? 'Subscriber' : 'Authoring'} View
            </Button>
          )}
        </ButtonsContainer>
      </Header>
      <App
        params={{
          isAuthoring: true,
          isLocked: isLocked,
          lockedToOtherClient: false,
          onStartEditing: handleOnStartEditing,
          onFinishEditing: handleOnFinishEditing,
          onConfigChanged: mockMethod,
          onSave: mockMethod,
          metadata: {
            displayMode: 'default', config: [], locationInfo: {sectionTitle : ''}
          },
          version,
          id,
          displayMode: 'default' as any,
          notifyLatestContent: (contentDate: number) => { console.log(contentDate); },
          newlyAdded: false,
          filter: { snapshotDate: new Date().getTime() }
        }}
      />
      <Container>
        <ContentSection>Capability ID : {id ? id : 'N/A'}</ContentSection>
        <ContentSection>Page Id : test-page</ContentSection>
        <ContentSection>Version : {version}</ContentSection>
      </Container>
    </HarnessWrapper>
  );
};

export default AppTestHarness;
