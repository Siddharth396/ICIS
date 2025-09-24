import { useCallback, useState } from 'react';
import { Modal, Button, Banner } from '@icis/ui-kit';
import { useUser } from '@icis/app-shell-apis';

import getMessages from 'constants/getMessages';

import { BannerBody, ConfigWrapper } from './Config.style';
import { ConfigState, Props } from './Config.types';
import Footer from './Config.Footer';
import Body from './Config.Body';
import graphQlService from '../../services/graphQl';
import { SAVE_CONTENTBLOCK } from '../../apollo/queries';
import Spinner from '../../Common/Spinner';
import ISaveContentBlockResponse from './ISaveContentBlockResponse';
import componentHelper from '../../Common/Helper/Component';
import IContentBlockFilter from '../App/IContentBlockFilter';

const initialConfigState: ConfigState = {
  isOpen: false,
  saveDisabled: true,
  commodity: { value: 'STYRENE', label: 'Styrene' },
  region: { value: 'europe', label: 'Europe' },
  type: undefined,
};

const Config = ({ setSelectedConfigTableFilters, saveToCanvas, contentBlockId }: Props) => {
  const { locale } = useUser();
  const messages = getMessages(locale).config;
  const [config, setConfig] = useState<ConfigState>(initialConfigState);
  const [isSaving, setIsSaving] = useState<boolean>(false);
  const [errorPage, setError] = useState<JSX.Element>();

  const { mutate } = graphQlService.mutateData<ISaveContentBlockResponse>(SAVE_CONTENTBLOCK);

  const resetRegion = useCallback(( key : keyof ConfigState) => {
      // Reset region to default if commodity is changed
      if(key === 'commodity'){
        setConfig(prev => ({ ...prev, region: { value: 'europe', label: 'Europe' } }));
      }
  }, []);

  const handleSelectChange = useCallback((key: keyof ConfigState) => (value: string, label: string | undefined) => {
    saveDisabled(true);
    resetRegion(key);
    setError(undefined);
    setConfig(prev => ({ ...prev, [key]: { value, label: label || '' } }));
  }, []);

  const saveConfig = async () => {
    setIsSaving(true);
    setError(undefined);
    try {
      const selectedFilters: IContentBlockFilter = { region: config.region.value, product: config.commodity.value, tableType: config.type?.value };
      const response = await mutate({
        variables: {
          contentBlockId: contentBlockId,
          filter: JSON.stringify(selectedFilters)
        }
      });

      if (response.errors) {
        return graphQlService.getGraphQLErrorComponent(response.errors);
      }

      saveToCanvas(response.data?.saveContentBlock.version || '');
      setConfig(prev => ({ ...prev, isOpen: false }));
      setSelectedConfigTableFilters(selectedFilters);
    }
    catch (ex) {
      console.error(ex);
      setError(componentHelper.getErrorPageComponent({ variant: 'BoldWhite'}));
    }
    finally {
      setIsSaving(false);
    }
  };

  const toggleModal = useCallback((isOpen: boolean) => {
    setError(undefined);
    setConfig({ ...initialConfigState, isOpen });
  }, []);

  const saveDisabled = (saveDisabled: boolean) => {
    setConfig(prev => ({ ...prev, saveDisabled: saveDisabled }));
  };

  return (
    <ConfigWrapper>
      <Button onClick={() => toggleModal(true)} testId='Add__Table__Button' variant='AuthoringPrimary'>{messages.addButton}</Button>
      <Modal
        isOpen={config.isOpen}
        onDismiss={() => toggleModal(false)}
        variant='Large'
        ariaLabel={messages.title}
        header={{
          title: messages.title,
          visible: true,
        }}
        footer={{
          visible: true,
          FooterContent: () => <Footer saveConfig={saveConfig} saveDisabled={config.saveDisabled} toggleModal={() => toggleModal(false)} />,
        }}
        testId='Config__Modal'
      >
        {errorPage && <BannerBody><Banner level='Error' hideReloadButton={true} forceShowCloseButton={true}> {errorPage} </Banner></BannerBody>}
        {isSaving && <Spinner />}
        <Body
          commodity={config.commodity}
          region={config.region}
          type={config.type}
          saveDisabled={config.saveDisabled}
          setSaveDisabled={saveDisabled}
          handleSelectChange={handleSelectChange} />
      </Modal>
    </ConfigWrapper>
  );
};

export default Config;
