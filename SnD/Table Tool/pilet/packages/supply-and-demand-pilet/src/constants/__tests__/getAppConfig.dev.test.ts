import getAppConfig from '../getAppConfig';

jest.mock('@icis/app-shell-apis', () => ({
  settings: {
    environment: 'dev',
  },
}));

describe('getAppConfig', () => {
  it('gets the dev config when the environment is dev', () => {
    expect(getAppConfig().bffUrl).toBe('https://localhost:4400/v1/graphql');
  });
});
