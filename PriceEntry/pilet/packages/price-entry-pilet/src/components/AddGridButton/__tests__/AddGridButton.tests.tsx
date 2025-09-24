import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import AddGridButton from '..';

describe('AddGridButton', () => {
  it('renders the Add Grid button', () => {
    render(<AddGridButton onAddClick={jest.fn()} />);
    expect(screen.getByText('Add Grid')).toBeInTheDocument();
  });

  it('calls onAddClick when the button is clicked', () => {
    const onAddClick = jest.fn();
    render(<AddGridButton onAddClick={onAddClick} />);
    fireEvent.click(screen.getByText('Add Grid'));
    expect(onAddClick).toHaveBeenCalledTimes(1);
  });
});
