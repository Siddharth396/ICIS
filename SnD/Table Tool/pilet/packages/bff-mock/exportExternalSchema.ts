import BFF from '@icis/bff-mock';
import { join } from 'path';

/**
 * Saves the schema for given bff url. Takes the last argv value as the bff url.
 */
BFF.exportSchema(process.argv[process.argv.length - 1], join(__dirname, '../schema.json'));
