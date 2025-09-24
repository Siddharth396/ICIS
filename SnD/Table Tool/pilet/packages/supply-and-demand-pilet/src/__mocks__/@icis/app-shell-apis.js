export const useUser = () => ({ userId: '123', locale: 'en', isInternal: true });
export const finishSceneLoad = jest.fn();
export const logger = { log: jest.fn(), warn: jest.fn(), debug: jest.fn(), error: jest.fn() };
export const trackPageView = jest.fn();
export const trackAction = jest.fn();
export const getApolloClient = jest.fn(() => Promise.resolve({}));
export const settings = {
    environment: 'dev'
};
export const showErrorPage = jest.fn();
export const handleUnauthorised = jest.fn();
