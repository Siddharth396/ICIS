import { render } from '@testing-library/react';
import Snapshot from '../Snapshot';

describe('Snapshot', () => {
  it('renders without crashing', async () => {
    const { findByTestId } = render(<Snapshot LastUpdatedDateUnix={new Date().getTime()} />);
    const snapchotContainer = await findByTestId('snapshot-container');
    expect(snapchotContainer).toBeInTheDocument();
  });

  it('should show last updated date message', async () => {
    const { findByTestId } = render(<Snapshot LastUpdatedDateUnix={new Date(2024, 0, 1).getTime()} />);
    const snapshotContainer = await findByTestId('snapshot-container');

    const expectedMessage = 'This table shows latest data from 01 Jan 2024';
    expect(snapshotContainer).toHaveTextContent(expectedMessage);
  });
});
