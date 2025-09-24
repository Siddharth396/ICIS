import { render } from '@testing-library/react';

import { mockConfig } from 'constants/global';
import getAppConfig from 'constants/getAppConfig';

import Capacities from '../Capacities';
import graphQlService from '../../../services/graphQl';
import IResponse from '../../../Common/Model/Response/IResponse';
import { CapacitiesData } from '../Capacities.types';
import { ApolloError, ApolloQueryResult, NetworkStatus } from '@apollo/client';

jest.mock('constants/getAppConfig');
const mockAppConfig = getAppConfig as jest.Mock;

jest.mock('../../../services/graphQl');
const mockgraphQlService = graphQlService.getData as jest.Mock;

jest.mock('../../../constants/capabilityPropsMetadata');

const mockData = (): ApolloQueryResult<CapacitiesData> => ({
  data: { capacityDevelopmentsByCommoditiesAndRegions: [] }, error: undefined, loading: false, networkStatus: NetworkStatus.refetch
});
const refetchMock = () => Promise.resolve((mockData()));

describe('Capacities', () => {
  beforeEach(() => {
    mockAppConfig.mockImplementation(() => mockConfig);
    mockgraphQlService.mockImplementation(() => { return { data: { capacityDevelopmentsByCommoditiesAndRegions: [] }, error: undefined, refetch: refetchMock } as IResponse<CapacitiesData, ApolloError> });
  });

  it('renders without crashing', () => {
    render(
      <Capacities commodities={''} regions={''} isInPreviewMode contentBlockId={''} />
    );
  });
});
