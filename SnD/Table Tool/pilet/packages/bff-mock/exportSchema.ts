import { join } from 'path';
import bff from './index';

bff.exportThisSchema(join(__dirname, '../schema.json'));
