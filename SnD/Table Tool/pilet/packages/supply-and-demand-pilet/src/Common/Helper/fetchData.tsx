import { DocumentNode } from 'graphql';
import { showErrorPage } from '@icis/app-shell-apis';
import graphQlService from 'services/graphQl/graphQl';
import { IOption } from '@icis/ui-kit';
import { ServerError } from '@apollo/client';
import IResponse from 'Common/Model/Response/IResponse';

// Helper function to fetch and process GraphQL data
export function fetchData<T>(
  query: DocumentNode,
  processData: (data: T) => IOption[],
  filterCondition: (option: IOption) => boolean
): IResponse<IOption[], any> {
  const { data, error } =  graphQlService.getData<T>(query);

  if (
    error?.graphQLErrors.find((item) => item.extensions?.statusCode === 401) ||
    (error?.networkError as ServerError)?.statusCode === 401
  ) {
    return { data: [], error: showErrorPage(401) };
  } else if (error) {
    return { data: [], error: showErrorPage(0) };
  }

  return { data: processData(data)?.filter(filterCondition), error };
}
