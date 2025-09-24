// Copyright © 2021 LexisNexis Risk Solutions Group
import { useCallback, useEffect, useRef } from 'react';

export const useClickOutsideListenerRef = (onClose: () => void) => {
  const ref = useRef(null);
  const escapeListener = useCallback(
    (e: KeyboardEvent) => {
      if (e.key === 'Escape' || e.key === 'Enter') {
        onClose();
      }
    },
    [onClose],
  );
  const clickListener = useCallback(
    (e: MouseEvent) => {
      if (ref?.current && !(ref.current! as any).contains(e.target)) {
        onClose?.();
      }
    },
    [onClose],
  );

  useEffect(() => {
    document.addEventListener('click', clickListener, { capture: true });
    document.addEventListener('keyup', escapeListener, { capture: true });
    return () => {
      document.removeEventListener('click', clickListener);
      document.removeEventListener('keyup', escapeListener);
    };
  }, [clickListener, escapeListener]);

  return ref;
};
