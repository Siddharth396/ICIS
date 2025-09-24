import { render } from '@testing-library/react';
import withClient from '../withClient';

jest.mock('apollo/getClient', () => async () => <>test-apollo-client</>);
jest.mock('apollo/getScheduleSelectorClient', () => async () => <>test-schedule-selector-client</>);

const TestComponent = ({ client }: any) => <>{client}</>;

describe('withClient', () => {
  it('renders a component with the client that was passed in as a prop', async () => {
    const WrappedTestComponent = withClient(TestComponent) as any;
    const { findByText } = render(<WrappedTestComponent />);
    await findByText('test-apollo-client');
  });

  it('renders a component with schedule selector passed as true', async () => {
    const WrappedTestComponent = withClient(TestComponent, true) as any;
    const { findByText } = render(<WrappedTestComponent />);
    await findByText('test-schedule-selector-client');
  });
});
