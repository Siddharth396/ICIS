import { useCallback, useEffect, useState } from 'react';
import { getIsAuthoring } from 'constants/isAuthoring';
import Config from 'pilet-components/Config';
import { AppWrapper, SubscriberWrapper } from './App.style';
import componentHelper from '../../Common/Helper/Component';
import IComponent from '../../Common/Model/IComponent';
import withGlobalApolloClient from '../../apollo/withGlobalApolloClient';
import { Props } from '../Config/Config.types';
import { CapabilityProps } from '@icis/app-shell';
import graphQlService from '../../services/graphQl';
import { FETCH_CONTENTBLOCK } from '../../apollo/queries';
import IContentBlockFilter from './IContentBlockFilter';
import { Loader } from '@icis/ui-kit';
import IContentBlock from '../../Common/Model/IContentBlock';
import { getLastUpdatedDateUnix } from '../../constants/capabilityPropsMetadata';
import Snapshot from '../Snapshot/Snapshot';

const App = (capabilityProps: CapabilityProps) => {
  const { params } = capabilityProps;
  const [savedTableMetadata, setSavedTableMetadata] = useState<IContentBlockFilter>();
  const [isLoaded, setIsLoaded] = useState<boolean>(false);
  const [errorPage, setError] = useState<JSX.Element>();
  const [isSavedComponentMounted, setIsSavedComponentMounted] = useState<boolean>(false);

  const handleSelectedConfigTableFilters = useCallback((tableFilters: IContentBlockFilter) => {
    setSavedTableMetadata(tableFilters);
  }, []);

  const { execute } = graphQlService.getLazyOperation<IContentBlock>(FETCH_CONTENTBLOCK);

  useEffect(() => {
    async function getSavedComponent() {
      try {
        const lazyResponse = await execute({
          variables: {
            contentBlockId: params.id,
            version: params.version
          }
        });

        if (lazyResponse.error && !graphQlService.getApolloErrorByStatusCode(lazyResponse.error, 404)) {
          setError(graphQlService.getApolloErrorComponent(lazyResponse.error));
        }
        else {
          const savedTableFilter = lazyResponse.data?.contentBlock?.filter;

          if (savedTableFilter) {
            const filters: IContentBlockFilter = JSON.parse(savedTableFilter);
            setSavedTableMetadata(filters);
          }
        }
      }
      catch (ex) {
        console.error(ex);
        setError(componentHelper.getErrorPageComponent());
      }
      finally {
        setIsLoaded(true);
      }
    }

    if (params?.id && params?.version) {
      getSavedComponent();
    }
    else {
      setIsLoaded(true);
    }

  }, [params?.id, params?.version, execute]);

  useEffect(() => {
    if (isSavedComponentMounted) {
      const lastUpdatedDateUnix = getLastUpdatedDateUnix(params.id);
      lastUpdatedDateUnix && params.notifyLatestContent(lastUpdatedDateUnix);
    }
  }, [isSavedComponentMounted]);

  if (!isLoaded) {
    return <Loader />;
  }

  const getSavedComponent = (): IComponent<any> | undefined => {
    return savedTableMetadata && componentHelper.getComponentWithProps(savedTableMetadata.tableType, { commodities: savedTableMetadata.product, regions: savedTableMetadata.region, contentBlockId: params.id });
  }

  const getNewComponent = (): IComponent<any> => {
    return {
      Component: Config,
      Props: {
        setSelectedConfigTableFilters: handleSelectedConfigTableFilters,
        saveToCanvas: params.onSave,
        contentBlockId: params.id
      } as Props
    };
  }

  const renderAuthoringComponent = () => {
    const { Component, Props } = getSavedComponent() || getNewComponent();
    return <Component
      {...Props}
    />
  };

  const renderSubscriberComponent = () => {
    const savedComponent = getSavedComponent()!;
    const lastUpdatedDateUnix = getLastUpdatedDateUnix(params.id);

    return savedComponent ?
      <SubscriberWrapper>
        <savedComponent.Component
          {...savedComponent.Props}
          isComponentMounted={setIsSavedComponentMounted}
        />
        {params?.filter?.snapshotDate && isSavedComponentMounted && lastUpdatedDateUnix && <Snapshot LastUpdatedDateUnix={lastUpdatedDateUnix!} />}
      </SubscriberWrapper> : componentHelper.getErrorPageComponent()
  };

  if (errorPage) {
    return <AppWrapper>
      {errorPage}
    </ AppWrapper>
  }

  return (
    <AppWrapper>
      {/* update based on config */}
      {getIsAuthoring() ? renderAuthoringComponent() : renderSubscriberComponent()}
    </AppWrapper>
  );
};

export default withGlobalApolloClient<CapabilityProps>(App);
