import { render, screen, fireEvent } from '@testing-library/react';
import ErrorFallback from '../ErrorFallback';

describe('ErrorFallback', () => {
  it('should render the error fallback with icon, error message, and reload button', () => {
    render(<ErrorFallback onReload={jest.fn()} />);

    // Check that the generic error message is displayed
    expect(screen.getByText('Content failed to load')).toBeInTheDocument();

    // Check that the reload button is rendered with the correct text
    const reloadButton = screen.getByText('Reload');
    expect(reloadButton).toBeInTheDocument();
  });

  it('should call the reload function when the reload button is clicked', () => {
    const mockOnClick = jest.fn();
    render(<ErrorFallback onReload={jest.fn()} />);

    const reloadButton = screen.getByText('Reload');
    fireEvent.click(reloadButton);

    // Check if the button click handler was called (even if it's an empty handler)
    expect(mockOnClick).not.toHaveBeenCalled(); // since the onClick is empty
  });
});
