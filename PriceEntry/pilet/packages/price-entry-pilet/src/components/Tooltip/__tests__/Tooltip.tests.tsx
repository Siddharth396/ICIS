import React from 'react';
import { render, waitFor } from '@testing-library/react';
import TooltipComponent from '..';

const mockData = {
  validationErrors: {
    // Add sample validation errors here
    column1: ['Error 1', 'Error 2'],
    column2: ['Error 3'],
  },
};

// Mock props object
const mockProps = {
  value: mockData.validationErrors.column1.join(', '),
};

describe('TooltipComponent', () => {
  test('renders tooltip component with validation errors', async () => {
    const { getByText } = render(<TooltipComponent {...mockProps} />);
    // Check if each validation error is rendered
    // Use custom text matcher function to find the error text in the tooltip
    await waitFor(() => {
      // Check if each validation error is rendered
      mockData.validationErrors.column1.forEach(() => {
        expect(getByText('Error 1, Error 2')).toBeInTheDocument();
      });
    });
  });

  test('renders tooltip component without validation errors', async () => {
    // Update mockProps to simulate no validation errors
    const propsWithoutErrors = {
      ...mockProps,
      value: '', // Assuming no errors for this column
    };
    const { queryByText } = render(<TooltipComponent {...propsWithoutErrors} />);
    await waitFor(() => {
      // Check if no validation error is rendered
      expect(queryByText('Error 1, Error 2')).toBeNull();
    });
  });
});
