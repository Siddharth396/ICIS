import getAppConfig from '../getAppConfig';

jest.mock('@icis/app-shell-apis', () => ({
  settings: {
    environment: 'some-non-dev-env',
  },
}));

describe('getAppConfig', () => {
  it('gets the non-dev config when the environment is not dev', () => {
    expect(getAppConfig().bffUrl).toBe(`http://localhost/api/${getAppConfig().appName}/v1/graphql`);
  });
});
