import { render } from '@testing-library/react';

import { WithProviders } from 'test-utils/Providers';
import Loader from 'pilet-components/Loader';

describe('pilet-components/Loader', () => {
  describe('data loading', () => {
    it('show loader', async () => {
      const { findByTestId } = render(
        <WithProviders>
          <Loader testId="Mock__Loader" title="Mock Loader" height="300px" />
        </WithProviders>,
      );
      await findByTestId('Mock__Loader');
    });
  });
});
