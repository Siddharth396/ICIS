import { render } from '@testing-library/react';

import { mockConfig } from 'constants/global';
import getAppConfig from 'constants/getAppConfig';

import Body from '../Config.Body';
import IResponse from '../../../Common/Model/Response/IResponse';
import { fetchData } from 'Common/Helper/fetchData';
import { IOption } from '@icis/ui-kit';

jest.mock('constants/getAppConfig');
const mockAppConfig = getAppConfig as jest.Mock;

jest.mock('Common/Helper/fetchData');
const mockFetchData = fetchData as jest.Mock;


describe('Body', () => {
  beforeEach(() => {
    mockAppConfig.mockImplementation(() => mockConfig);
    mockFetchData.mockImplementation(() => {
      return {
        data: [{
          label: 'Europe',
          value: 'Europe',
          isDisabled: false
        }], error: undefined
      } as IResponse<IOption[], any>
    });
  });

  it('renders without crashing', () => {
    render(
      <Body
        commodity={{ label: '', value: '', isDisabled: false }}
        region={{ label: '', value: '', isDisabled: false }}
        type={{ label: '', value: '', isDisabled: false }}
        saveDisabled={false}
        setSaveDisabled={() => () => { }}
        handleSelectChange={() => () => { }}
      />
    );
  });
});
