import React from 'react';
import { MockedProvider, MockedResponse } from '@apollo/client/testing';
import { InMemoryCache } from 'apollo-cache-inmemory';
import merge from 'lodash/merge';
import { BrowserRouter } from 'react-router-dom';

type Props = {
  apolloCacheToMerge?: unknown;
  apolloMocks?: readonly MockedResponse<Record<string, any>>[];
  children: React.ReactElement;
};

export const getProviders = function ({
  apolloCacheToMerge,
  apolloMocks = [],
  children,
}: Props): JSX.Element {
  const defaultCache = { data: {} };
  const cache: any = new InMemoryCache();
  cache.writeData(merge(defaultCache, apolloCacheToMerge));

  return (
    <MockedProvider mocks={apolloMocks} cache={cache}>
      <BrowserRouter>{children}</BrowserRouter>
    </MockedProvider>
  );
};

export const WithProviders = getProviders;
