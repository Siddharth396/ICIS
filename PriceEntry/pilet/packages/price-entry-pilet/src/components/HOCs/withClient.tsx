import React, { useEffect, useState } from 'react';
import { ApolloClient, NormalizedCacheObject } from '@apollo/client';
import getClient from 'apollo/getClient';
import getScheduleSelectorClient from 'apollo/getScheduleSelectorClient';

export type ApolloClientProps = {
  client: ApolloClient<NormalizedCacheObject>;
};

/**
 * Passes the specified apollo client to the wrapped component when the promise resolves.
 *
 * @param InnerComponent Component to render with the provided client.
 * @param apolloClient A promise that resolves to an Apollo client that should be passed to the wrapped component.
 * @returns A component with the specified client as the prop when it has resolved. Returns null until the client is resolved.
 */

function withClient<T>(
  InnerComponent: React.ComponentType<T & ApolloClientProps>,
  isScheduleSelectorTool = false,
): React.ComponentType<T> {
  return (props: T) => {
    const [client, setClient] = useState(undefined);

    useEffect(() => {
      async function awaitClient() {
        const client: any = isScheduleSelectorTool
          ? await getScheduleSelectorClient()
          : await getClient();
        setClient(client);
      }

      awaitClient();
    }, []);

    if (client === undefined) return null;
    return (
      <InnerComponent
        {...props}
        client={client as unknown as ApolloClient<NormalizedCacheObject>}
      />
    );
  };
}

export default withClient;
