import { render } from '@testing-library/react';

import { mockConfig } from 'constants/global';
import getAppConfig from 'constants/getAppConfig';

import Outages from '../Outages';
import { OutagesData } from '../Outages.types';
import IResponse from '../../../Common/Model/Response/IResponse';
import { ApolloError, ApolloQueryResult, NetworkStatus } from '@apollo/client';
import graphQlService from '../../../services/graphQl';

jest.mock('constants/getAppConfig');
const mockAppConfig = getAppConfig as jest.Mock;

jest.mock('../../../services/graphQl');
const mockgraphQlService = graphQlService.getData as jest.Mock;

const mockData: ApolloQueryResult<OutagesData> = {
  data: { outagesByCommoditiesAndRegions: [] }, error: undefined, loading: false, networkStatus: NetworkStatus.refetch
};

const refetchMock = jest.fn().mockResolvedValue({
  data: mockData,
});

jest.mock('../../../constants/capabilityPropsMetadata');

describe('Outages', () => {
  beforeEach(() => {
    mockAppConfig.mockImplementation(() => mockConfig);
    mockgraphQlService.mockImplementation(() => { return { data: { outagesByCommoditiesAndRegions: [] }, error: undefined, refetch: refetchMock } as IResponse<OutagesData, ApolloError> });
  });

  it('renders without crashing', () => {
    render(
      <Outages commodities={''} regions={''} isInPreviewMode contentBlockId={''} />
    );
  });
});
