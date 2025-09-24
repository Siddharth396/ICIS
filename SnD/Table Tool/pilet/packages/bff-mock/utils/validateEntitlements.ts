import { ApolloError } from 'apollo-server';

export default function validateEntitlements(args: any, data: any) {
    if (args.cId === 1 && args.rId === 1 && args.lId === 1) {
        throw new ApolloError('User is not authorized.', 'UNAUTHORIZED');
    }
    return data;
}