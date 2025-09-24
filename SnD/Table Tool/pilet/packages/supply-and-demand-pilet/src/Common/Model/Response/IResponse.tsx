import { ApolloQueryResult } from '@apollo/client';

interface IResponse<T, E> {
  data: T
  error?: E
  loading?: boolean,
  refetch?: () => Promise<ApolloQueryResult<T>>
}


export default IResponse;
