import dotenv from 'dotenv';

dotenv.config();

const defaultPassword = process.env.CHAPASS || '';
const subscriberPassword = process.env.SUBSCRIBERPASS || '';

const credentials = {
  subscriberUser: {
    username: 'chubtestuser@icis.com',
    password: subscriberPassword,
  },
  chaUser: {
    username: process.env.CHAUSER || '',
    password: defaultPassword
  },
};
export default credentials;
