const js = require('@eslint/js');
const globals = require('globals');
const importPlugin = require('eslint-plugin-import');

module.exports = [
  js.configs.recommended,
  {
    ignores: ['node_modules/**'],
    files: ['migrations/**/*.js', 'repeatable_migrations/**/*.js', 'helpers/**/*.js'],
    languageOptions: {
      ecmaVersion: 2021,
      sourceType: 'commonjs',
      globals: {
        ...globals.node
      }
    },
    plugins: {
      import: importPlugin
    },
    rules: {
      'no-unused-vars': 'off',
      'no-console': 'off',
      'no-plusplus': 'off',
      'no-underscore-dangle': 'off',
      'semi': ['error', 'always'],
      'quotes': ['error', 'single'],
      'import/no-unresolved': 'error'
    }
  }
];