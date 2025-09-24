import React, { FC } from 'react';
import { render, fireEvent } from '@testing-library/react';
import { useClickOutsideListenerRef } from '../useClickOutsideListenerRef';

interface IDialogProps {
  onClose: () => void;
}

const Dialog: FC<IDialogProps> = (props) => {
  const { onClose } = props;
  const ref = useClickOutsideListenerRef(onClose);

  return (
    <div
      style={{
        position: 'fixed',
        width: '100%',
        height: '100%',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
      }}
      data-testid='wrapper'
    >
      <div ref={ref} style={{ width: '30%', height: '30%' }}>
        <span data-testid='child'>test dialog</span>
      </div>
    </div>
  );
};

describe('click outside', () => {
  it('call onClose function - on click outside', () => {
    const toggle = jest.fn();
    const { getByTestId } = render(<Dialog onClose={toggle} />);
    fireEvent.click(getByTestId('wrapper'));

    expect(toggle).toBeCalled();
  });

  it('call onClose function - on KeyboardEvent - Esc', () => {
    const toggle = jest.fn();
    const { getByTestId } = render(<Dialog onClose={toggle} />);
    fireEvent.keyUp(getByTestId('wrapper'), { key: 'Escape', code: 'Escape' });

    expect(toggle).toBeCalled();
  });

  it('call onClose function - on KeyboardEvent - Enter', () => {
    const toggle = jest.fn();
    const { getByTestId } = render(<Dialog onClose={toggle} />);
    fireEvent.keyUp(getByTestId('wrapper'), { key: 'Enter', code: 13 });

    expect(toggle).toBeCalled();
  });

  it('onClose not called - on KeyboardEvent - Tab', () => {
    const toggle = jest.fn();
    const { getByTestId } = render(<Dialog onClose={toggle} />);
    fireEvent.keyUp(getByTestId('wrapper'), { key: 'Tab', code: 'Tab' });

    expect(toggle).not.toBeCalled();
  });
});
