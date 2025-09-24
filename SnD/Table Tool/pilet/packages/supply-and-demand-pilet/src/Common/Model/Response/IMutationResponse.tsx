import { FetchResult, MutationFunctionOptions } from '@apollo/client';
import IResponse from './IResponse';

interface IMutationResponse<T, E> {
  mutate: (options?: MutationFunctionOptions<T, any, any, any>) => Promise<FetchResult<T>>,
  response: IResponse<T, E>
}


export default IMutationResponse;
