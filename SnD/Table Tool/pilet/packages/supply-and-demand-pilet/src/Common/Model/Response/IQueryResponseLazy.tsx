import { LazyQueryExecFunction } from '@apollo/client';
import IResponse from './IResponse';

interface IQueryResponseLazy<T, E> {
  execute: LazyQueryExecFunction<T, any>,
  response: IResponse<T, E>
}


export default IQueryResponseLazy;
