import 'jest-styled-components';
import '@testing-library/jest-dom';
(global as any).uncaughtErrorHandler = window.onerror = jest.fn((err) => {
  throw new Error(err as string);
});

window.crypto = {
  ...window.crypto,
  randomUUID: jest.fn(),
};

window.URL.createObjectURL = () => '';
