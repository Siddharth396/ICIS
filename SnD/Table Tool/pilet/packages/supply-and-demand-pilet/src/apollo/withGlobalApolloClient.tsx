import React, { useEffect, useState } from 'react';
import { ApolloClient, ApolloProvider, NormalizedCacheObject } from '@apollo/client';
import getClient from 'apollo/getClient';

/**
 * Passes the specified apollo client to all the child components and appends it implicitly when the query or mutation is executed.
 *
 * @param ChildComponent Component to render with the provided client.
 * @param apolloClient A promise that resolves to an Apollo client that should be passed to the wrapped component.
 * @returns A component with the specified client as the prop when it has resolved. Returns null until the client is resolved.
 */

function withGlobalApolloClient<P extends object>(ChildComponent: React.ComponentType<P>): React.ComponentType<P> {
  return (props: P) => {
    const [client, setClient] = useState<ApolloClient<NormalizedCacheObject>>();

    useEffect(() => {
      async function awaitClient() {
        const client: any = await getClient();
        setClient(client);
      }

      awaitClient();
    }, []);

    if (!client) {
      return null;
    }

    return (
      <ApolloProvider client={client}>
        <ChildComponent {...props} />
      </ApolloProvider>
    );
  };
}

export default withGlobalApolloClient;
