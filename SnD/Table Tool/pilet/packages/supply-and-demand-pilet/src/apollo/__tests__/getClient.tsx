import getClient from '../getClient';
import { getApolloClient } from '@icis/app-shell-apis';

jest.mock('@icis/app-shell-apis', () => {
  const mockFn = jest.fn();
  mockFn.mockReturnValue('test apollo client');

  return {
    getApolloClient: mockFn,
  };
});

jest.mock('constants/getAppConfig', () => () => ({
  bffUrl: 'testBffUrl',
  version: 'testVersion',
  bypassVersionCheck: false,
  appName: 'example',
}));

describe('exampleApolloClient', () => {
  it('getExampleApolloClient gets an apollo client', async () => {
    await getClient();

    expect(getApolloClient).toHaveBeenCalledTimes(1);
    expect(getApolloClient).toHaveBeenCalledWith('example', 'testVersion', 'testBffUrl', false);
  });
});
