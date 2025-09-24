import React from 'react';
import { render, screen } from '@testing-library/react';
import GridPlaceholder from '../GridPlaceholder';
import * as useLocaleHook from 'utils/hooks/useLocaleMessage';

// Mock the custom hook
jest.mock('utils/hooks/useLocaleMessage');

describe('GridPlaceholder', () => {
  const mockMessages = {
    Capabilty: {
      PlaceholderTitle: 'Select from Table configuration on the left',
    },
  };

  beforeEach(() => {
    (useLocaleHook.default as jest.Mock).mockReturnValue(mockMessages);
  });

  it('renders the placeholder icon (SVG)', () => {
    render(<GridPlaceholder />);
    const svgElement = screen.getByTestId('grid-placeholder-icon');
    expect(svgElement.querySelector('svg')).toBeInTheDocument();
  });

  it('renders the placeholder message text correctly', () => {
    render(<GridPlaceholder />);
    expect(screen.getByText(mockMessages.Capabilty.PlaceholderTitle)).toBeInTheDocument();
  });
});
