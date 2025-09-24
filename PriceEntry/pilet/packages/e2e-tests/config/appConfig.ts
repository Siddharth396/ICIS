import dotenv from 'dotenv';

dotenv.config();

const appConfig = {
    
    authoringUrl: `https://authoring.${process.env.ENV}.cha.rbxd.ds`
}

export default appConfig;
