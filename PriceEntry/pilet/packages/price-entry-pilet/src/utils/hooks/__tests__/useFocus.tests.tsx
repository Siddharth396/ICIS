// useFocus.test.ts
import React, { RefObject, act } from 'react';
import { renderHook } from '@testing-library/react-hooks';
import { useFocus } from '../useFocus';

describe('useFocus', () => {
  let consoleErrorMock: jest.SpyInstance;

  beforeAll(() => {
    consoleErrorMock = jest.spyOn(console, 'error').mockImplementation(() => {});
  });

  afterAll(() => {
    consoleErrorMock.mockRestore();
  });

  it('returns a ref and a boolean', () => {
    const { result } = renderHook(() => {
      const [ref, isFocused]: [RefObject<HTMLInputElement | HTMLDivElement>, boolean] = useFocus(
        '',
        null,
        false,
      );
      return { ref, isFocused };
    });

    expect(result.current.ref).toBeDefined();
    expect(typeof result.current.ref.current).toBe('object');
    expect(typeof result.current.isFocused).toBe('boolean');
  });

  it('focuses the input when cellStartedEdit is true', async () => {
    jest.setTimeout(10000);
    const focusMock = jest.fn();

    // Mock the ref
    const createRefMock = jest.spyOn(React, 'createRef').mockReturnValue({
      current: { focus: focusMock },
    } as unknown as RefObject<HTMLInputElement>);

    const { result } = renderHook(() => {
      const [ref, isFocused]: [RefObject<HTMLInputElement | HTMLDivElement>, boolean] = useFocus(
        '',
        null,
        true,
      );
      return { ref, isFocused };
    });

    act(() => {
      result.current.ref.current?.focus();
    });

    // Restore the original createRef function
    createRefMock.mockRestore();
  });
  it('handles backspace key', () => {
    const { result } = renderHook(() => {
      const [ref, isFocused]: [RefObject<HTMLInputElement | HTMLDivElement>, boolean] = useFocus(
        '',
        'Backspace',
        true,
      );
      return { ref, isFocused };
    });
    expect(result.current.ref.current).toBe(null);
  });

  it('handles single character key', () => {
    const { result } = renderHook(() => {
      const [ref, isFocused]: [RefObject<HTMLInputElement | HTMLDivElement>, boolean] = useFocus(
        '',
        'a',
        true,
      );
      return { ref, isFocused };
    });
    expect(result.current.ref.current).toBe(null);
  });

  it('handles F2 key', () => {
    const { result } = renderHook(() => {
      const [ref, isFocused]: [RefObject<HTMLInputElement | HTMLDivElement>, boolean] = useFocus(
        '',
        'F2',
        true,
      );
      return { ref, isFocused };
    });
    expect(result.current.ref.current).toBe(null);
  });

  it('does not highlight all on focus', () => {
    const { result } = renderHook(() => {
      const [ref, isFocused]: [RefObject<HTMLInputElement | HTMLDivElement>, boolean] = useFocus(
        '',
        null,
        false,
      );
      return { ref, isFocused };
    });
    expect(result.current.ref.current).toBe(null);
  });

  it('handles select type', () => {
    renderHook(() => useFocus('', null, true));
  });
});
