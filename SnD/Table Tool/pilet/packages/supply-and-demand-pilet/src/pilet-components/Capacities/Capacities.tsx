import { Icon, Text } from '@icis/ui-kit';
import { useUser } from '@icis/app-shell-apis';
import { TableProps } from 'types';
import { FETCH_CAPACITY } from 'apollo/queries';
import { capacityTable } from 'constants/global';
import getMessages from 'constants/getMessages';
import Loader from 'pilet-components/Loader';
import Table from 'pilet-components/Table';

import { CapacitiesWrapper, Capacity, Negative, Positive } from './Capacities.style';
import { Props, CapacitiesData } from './Capacities.types';
import graphQlService from '../../services/graphQl';
import { setLastUpdatedDate } from '../../constants/capabilityPropsMetadata';
import componentHelper from '../../Common/Helper/Component';
import { useEffect, useState } from 'react';
import { getLatestDateUnix } from '../../Common/Helper/date';

const Capacities = ({ regions, commodities, isInPreviewMode, isComponentMounted, contentBlockId }: Props) => {
  const { locale } = useUser();
  const messages = getMessages(locale).capacities;
  const [tableProps, setTableProps] = useState<TableProps>();
  const [isRequestCompleted, setIsRequestCompleted] = useState<boolean>(false);
  const [errorPage, setError] = useState<JSX.Element>();

  const { refetch } = graphQlService.getData<CapacitiesData>(FETCH_CAPACITY,
    {
      commodities: commodities,
      regions: regions,
    });

  useEffect(() => {
    setTableProps(undefined);
    setError(undefined);
    setIsRequestCompleted(false);

    async function getCapacity() {
      try {
        const { data, error } = await refetch!();

        if (error && !graphQlService.getApolloErrorByStatusCode(error, 404)) {
          setError(graphQlService.getApolloErrorComponent(error));
        }

        if (data) {
          const lastUpdatedDateUnix = getLatestDateUnix(data.capacityDevelopmentsByCommoditiesAndRegions.map(x => x.lastUpdated));
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

    getCapacity();

  }, [commodities, regions, refetch]);

  useEffect(() => {
    if (isComponentMounted) {
      isComponentMounted!((tableProps !== null && tableProps !== undefined) && !errorPage);
    }
  }, [tableProps, errorPage]);


  const getTableProps = (data: CapacitiesData): TableProps => {
    const tableData = data?.capacityDevelopmentsByCommoditiesAndRegions.map((item) => {
      return item.capacityChange < 0 ? {
        ...item,
        newAnnualCapacity: <Capacity>{item.newAnnualCapacity} <Icon icon='movement-negative' /></Capacity>,
        capacityChange: <Negative>{item.capacityChange}</Negative>,
        percentChange: <Negative>{item.percentChange}{item.percentChange !== 'n/a' && '%'}</Negative>,
      } : {
        ...item,
        newAnnualCapacity: <Capacity>{item.newAnnualCapacity} <Icon icon='movement-positive' /></Capacity>,
        capacityChange: <Positive>{item.capacityChange}</Positive>,
        percentChange: <Positive>{item.percentChange}{item.percentChange !== 'n/a' && '%'}</Positive>,
      };
    });

    const tableProps: TableProps = {
      schema: capacityTable,
      data: tableData,
      noRecords: {
        title: messages.noRecords.title,
        subtitle: messages.noRecords.subtitle,
      },
      testId: 'Capacity__Table',
    };

    return tableProps;
  }

  if (!isRequestCompleted) {
    return <Loader title={messages.title} testId='Capacities__Loader' height="262px" />;
  }

  if (errorPage) {
    return <CapacitiesWrapper isInPreviewMode={isInPreviewMode} data-testid='Capacities__Wrapper'>
      {errorPage}
    </CapacitiesWrapper>
  }

  return (
    <CapacitiesWrapper isInPreviewMode={isInPreviewMode} data-testid='Capacities__Wrapper'>
      <Text.Body variant='SemiBold'>{messages.title}</Text.Body>
      <Table {...tableProps!} />
    </CapacitiesWrapper>
  );
};

export default Capacities;
