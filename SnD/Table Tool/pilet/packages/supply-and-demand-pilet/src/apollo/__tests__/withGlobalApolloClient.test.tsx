import { act, render, waitFor } from '@testing-library/react';
import withGlobalApolloClient from '../withGlobalApolloClient';
import getClient from '../getClient';
import { ApolloClient, ApolloConsumer, InMemoryCache } from '@apollo/client';

jest.mock('apollo/getClient');

const mockGetClient = getClient as jest.MockedFunction<typeof getClient>;

const mockClient = new ApolloClient({
  uri: '',
  cache: new InMemoryCache(),
});

// Update ChildComponent to access the Apollo Client
const ChildComponent = () => {
  return (
    <ApolloConsumer>
      {(client) => (
        <>
          Child Component
          {client && <span>Client is available</span>}
        </>
      )}
    </ApolloConsumer>
  );
};

describe('withGlobalApolloClient', () => {
  it('renders a component with the client that was passed to all child components', async () => {

    mockGetClient.mockResolvedValue(mockClient);

    const WrappedTestComponent = withGlobalApolloClient(ChildComponent) as any;

    let getByTxt: any, findByTxt: any;
    
    await act(() => {
      const { getByText, findByText } = render(<WrappedTestComponent />);
      getByTxt = getByText;
      findByTxt = findByText;
    });

    // Wait for the component to appear
    await waitFor(() => {
      expect(getByTxt('Child Component')).toBeInTheDocument();
    });

    // Wait for the client to be resolved and the component to re-render
    expect(await findByTxt('Client is available')).toBeInTheDocument();
  });
});
