import { DocumentNode } from 'graphql';
import { ApolloError, OperationVariables, ServerError, useLazyQuery, useMutation, useQuery } from '@apollo/client';
import IResponse from '../../Common/Model/Response/IResponse';
import IQueryResponseLazy from '../../Common/Model/Response/IQueryResponseLazy';
import IMutationResponse from '../../Common/Model/Response/IMutationResponse';
import { GraphQLErrors } from '@apollo/client/errors';
import componentHelper from '../../Common/Helper/Component';

function getData<TData>(query: DocumentNode, variables: OperationVariables = {}): IResponse<TData, ApolloError> {
  variables.fetchPolicy = 'cache-and-network';
  variables.errorPolicy = 'all';

  const { data, error, refetch } = useQuery<TData>(query, {
    variables: variables
  });

  return { data: data!, error: error!, refetch: refetch }
}

function getLazyOperation<TData>(query: DocumentNode, variables: OperationVariables = {}): IQueryResponseLazy<TData, ApolloError> {
  variables.fetchPolicy = 'cache-and-network';
  variables.errorPolicy = 'all';

  const [lazyOperation, { data, error }] = useLazyQuery<TData>(query, {
    variables: variables
  });

  return { execute: lazyOperation, response: { data: data!, error: error! } }
}

function mutateData<TData>(mutationQuery: DocumentNode, variables?: OperationVariables): IMutationResponse<TData, ApolloError> {

  const [mutate, { data, loading, error }] = useMutation<TData>(mutationQuery, {
    variables: variables
  });

  return { mutate: mutate, response: { data: data!, error: error!, loading: loading } }
}

const getApolloErrorComponent = (error: ApolloError) => {
  return getApolloErrorByStatusCode(error, 401) ? componentHelper.getGlobalErrorPageComponent(401) : componentHelper.getErrorPageComponent();
};

const getGraphQLErrorComponent = (errors: GraphQLErrors) => {
  return getGraphQlErrorByStatusCode(errors, 401) ? componentHelper.getGlobalErrorPageComponent(401) : componentHelper.getErrorPageComponent();
};

const getApolloErrorByStatusCode = (error: ApolloError, statusCode: number) => {
  const serverError = error.networkError as ServerError;

  if (serverError && serverError.statusCode === statusCode) {
    console.error('Apollo Error:- ', error.message);
    return { statusCode: serverError.statusCode, message: serverError.message }
  }

  return getGraphQlErrorByStatusCode(error.graphQLErrors, statusCode);
};

const getGraphQlErrorByStatusCode = (errors: GraphQLErrors, statusCode: number) => {
  const error = errors?.find((item) => Number(item.extensions?.code) === statusCode);

  if (!error) {
    return null;
  }

  console.error('GraphQL Error:- ', error.message);
  return { statusCode: error.extensions.code, message: error.message };
};

const graphQlService = {
  getData,
  getLazyOperation,
  getApolloErrorComponent,
  getGraphQLErrorComponent,
  getApolloErrorByStatusCode,
  mutateData
};

export default graphQlService;
