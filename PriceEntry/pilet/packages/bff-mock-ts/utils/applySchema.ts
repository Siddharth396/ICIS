export type Resolver = (...args: any) => any;

export interface ISchema {
  definitions?: string;
  query?: string;
  mutation?: string;
  resolvers?: {
    queries?: {
      [key: string]: Resolver;
    };
    mutations?: {
      [key: string]: Resolver;
    };
  };
}

/**
 * Combine one of the string props from an array of schemas. Basically combining the graphql schema
 * together
 */
export const applyString = (schemas: ISchema[]) => (prop: 'definitions' | 'query' | 'mutation') => schemas.reduce((acc, schema) => {
  const val = schema[prop];

  if (!val) return acc;

  return `${acc}${val}`;
}, '');

/**
 * Combine  the resolver props from an array of schemas.
 */
export const applyObject = (schemas: ISchema[]) => (prop : 'queries' | 'mutations') => schemas.reduce((acc, schema) => {
  if (!schema.resolvers) return acc;

  const val = schema.resolvers[prop];

  if (!val) return acc;

  return {
    ...acc,
    ...val,
  };
}, {});
