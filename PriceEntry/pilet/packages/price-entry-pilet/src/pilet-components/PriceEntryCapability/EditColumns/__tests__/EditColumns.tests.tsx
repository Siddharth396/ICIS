import { act } from 'react';
import { render, fireEvent, waitFor } from '@testing-library/react';
import EditColumns from '..';

describe('EditColumns', () => {
  const mockOnUpdateUserPreference = jest.fn();
  const mockSetGridApi = jest.fn();

  const mockColumns = [
    { field: 'column1', displayOrder: 1, hidden: false, hideable: true },
    { field: 'column2', displayOrder: 2, hidden: false },
  ];

  it('renders without crashing', () => {
    render(
      <EditColumns
        // @ts-ignore
        columns={mockColumns}
        setGridApi={mockSetGridApi}
        onUpdateUserPreference={mockOnUpdateUserPreference}
      />,
    );
  });

  it('calls onUpdateUserPreference when a checkbox is clicked', () => {
    const { getByTestId } = render(
      <EditColumns
        // @ts-ignore
        columns={mockColumns}
        setGridApi={mockSetGridApi}
        onUpdateUserPreference={mockOnUpdateUserPreference}
      />,
    );

    // Simulate a click on the edit-column-button to set isEditing to true
    fireEvent.click(getByTestId('edit-column-button'));

    // Now the checkboxes should be rendered, so you can simulate a click on one of them
    fireEvent.click(getByTestId('column1-checkbox'));

    expect(mockOnUpdateUserPreference).toHaveBeenCalled();
  });

  it('sets isEditing to false after a delay when clicked outside', async () => {
    jest.useFakeTimers();

    const { getByTestId, findByTestId } = render(
      <div>
        <div data-testid='outside-container'></div>
        {/* @ts-ignore */}
        <EditColumns
          // @ts-ignore
          columns={mockColumns}
          setGridApi={mockSetGridApi}
          onUpdateUserPreference={mockOnUpdateUserPreference}
        />
        ,
      </div>,
    );

    // Simulate a click on the edit-column-button to set isEditing to true
    fireEvent.click(getByTestId('edit-column-button'));

    // Simulate a click outside the EditColumnsWrapper
    fireEvent.click(await findByTestId('outside-container'));
    // Fast-forward all timers so any pending timeouts are executed
    act(() => {
      jest.runAllTimers();
    });

    await waitFor(() => {
      expect(getByTestId('edit-column-button')).toBeInTheDocument();
    });
  });
});
