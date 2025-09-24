/* eslint-disable-next-line @typescript-eslint/no-var-requires */
const replace = require('replace-in-file');

// eslint-disable-next-line no-undef
const appShellType = process.argv[2];

const authoringTrueStatement = 'REACT_APP_IS_AUTHORING="true"';
const authoringFalseStatement = 'REACT_APP_IS_AUTHORING="false"';

const options = {
  files: 'node_modules/@icis/app-shell/index.html',
  from: appShellType === 'authoring' ? authoringFalseStatement : authoringTrueStatement,
  to: appShellType === 'authoring' ? authoringTrueStatement : authoringFalseStatement,
};

try {
  const results = replace.sync(options);
  console.log('Replacement results:', results);
} catch (error) {
  console.error('Error occurred:', error);
}
