import React from 'react';
import { render } from '@testing-library/react';
import CustomHeader from '..';

describe('CustomHeader', () => {
  it('renders the displayName passed as a prop', () => {
    const { getByText } = render(<CustomHeader displayName='Test Header' />);
    expect(getByText('Test Header')).toBeInTheDocument();
  });

  it('applies the correct styles to the header container', () => {
    const { container } = render(<CustomHeader displayName='Test Header' />);
    const headerContainer = container.firstChild;

    expect(headerContainer).toHaveStyle('display: flex');
    expect(headerContainer).toHaveStyle('align-items: center');
  });
});
