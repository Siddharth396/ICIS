// useFocus.ts
import { useEffect, useRef, useState } from 'react';

const KEY_BACKSPACE = 'Backspace';
const KEY_F2 = 'F2';

export const useFocus = (
  value: string,
  eventKey: string | null,
  cellStartedEdit: boolean,
): [React.RefObject<HTMLDivElement>, boolean] => {
  const ref = useRef<HTMLDivElement>(null);
  const [isFocused, setIsFocused] = useState(false);

  useEffect(() => {
    let startValue;
    let highlightAllOnFocus = true;

    if (eventKey === KEY_BACKSPACE) {
      startValue = '';
    } else if (eventKey && eventKey.length === 1) {
      startValue = eventKey;
      highlightAllOnFocus = false;
    } else {
      startValue = value;
      if (eventKey === KEY_F2) {
        highlightAllOnFocus = false;
      }
    }
    if (startValue == null) {
      startValue = '';
    }

    /* istanbul ignore else */
    const eInput = (ref?.current as HTMLDivElement)?.querySelector('input');

    if (eInput) {
      eInput.addEventListener('focus', /* istanbul ignore next */ () => setIsFocused(true));
      eInput.addEventListener('blur', /* istanbul ignore next */ () => setIsFocused(false));
    }

    cellStartedEdit && eInput?.focus();
    if (cellStartedEdit && highlightAllOnFocus) {
      eInput?.select();
    }

    return () => {
      if (eInput) {
        eInput.removeEventListener('focus', /* istanbul ignore next */ () => setIsFocused(true));
        eInput.removeEventListener('blur', /* istanbul ignore next */ () => setIsFocused(false));
      }
    };
  }, []);

  return [ref, isFocused];
};
