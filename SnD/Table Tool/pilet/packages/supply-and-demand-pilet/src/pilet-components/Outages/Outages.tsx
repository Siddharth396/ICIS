// istanbul ignore file
import { Text } from '@icis/ui-kit';
import { useUser } from '@icis/app-shell-apis';

import { TableProps } from 'types';
import { FETCH_OUTAGE } from 'apollo/queries';
import { outagesTable } from 'constants/global';
import getMessages from 'constants/getMessages';
import Loader from 'pilet-components/Loader';
import Table from 'pilet-components/Table';

import { OutagesData, Props } from './Outages.types';
import { OutagesWrapper } from './Outages.style';
import graphQlService from '../../services/graphQl';
import { useEffect, useState } from 'react';
import componentHelper from '../../Common/Helper/Component';
import { setLastUpdatedDate } from '../../constants/capabilityPropsMetadata';
import { getLatestDateUnix } from '../../Common/Helper/date';

const Outages = ({ regions, commodities, isInPreviewMode, isComponentMounted, contentBlockId }: Props) => {
  const { locale } = useUser();
  const messages = getMessages(locale).outages;
  const [tableProps, setTableProps] = useState<TableProps>();
  const [isRequestCompleted, setIsRequestCompleted] = useState<boolean>(false);
  const [errorPage, setError] = useState<JSX.Element>();

  const { refetch } = graphQlService.getData<OutagesData>(FETCH_OUTAGE,
    {
      commodities: commodities,
      regions: regions,
    });

  useEffect(() => {
    setTableProps(undefined);
    setError(undefined);
    setIsRequestCompleted(false);

    async function getOutages() {
      try {
        const { data, error } = await refetch!();

        if (error && !graphQlService.getApolloErrorByStatusCode(error, 404)) {
          setError(graphQlService.getApolloErrorComponent(error));
        }

        if (data) {
          const lastUpdatedDateUnix = getLatestDateUnix(data.outagesByCommoditiesAndRegions.map(x => x.lastUpdated));
          setLastUpdatedDate(contentBlockId, lastUpdatedDateUnix);
          setTableProps(getTableProps(data));
        }
      }
      catch (ex) {
        console.error(ex);
        setError(componentHelper.getErrorPageComponent());
      }
      finally {
        setIsRequestCompleted(true);
      }
    }

    getOutages();

  }, [commodities, regions, refetch]);

  useEffect(() => {
    if (isComponentMounted) {
      isComponentMounted!((tableProps !== null && tableProps !== undefined) && !errorPage);
    }
  }, [tableProps, errorPage]);
  
  const getTableProps = (data: OutagesData): TableProps => {
    const tableProps: TableProps = {
      schema: outagesTable,
      data: data.outagesByCommoditiesAndRegions,
      noRecords: {
        title: messages.noRecords.title,
        subtitle: messages.noRecords.subtitle,
      },
      testId: 'Outages__Table',
    };

    return tableProps;
  }

  if (!isRequestCompleted) {
    return <Loader testId='Outages__Loader' height="262px" />;
  }

  if (errorPage) {
    return <OutagesWrapper isInPreviewMode={isInPreviewMode} data-testid='Outages__Wrapper'>
      {errorPage}
    </OutagesWrapper>
  }

  return (
    <OutagesWrapper isInPreviewMode={isInPreviewMode} data-testid='Outages__Wrapper'>
      <Text.Body variant='SemiBold'>{messages.title}</Text.Body>
      <Table {...tableProps!} />
    </OutagesWrapper>
  );
};

export default Outages;
