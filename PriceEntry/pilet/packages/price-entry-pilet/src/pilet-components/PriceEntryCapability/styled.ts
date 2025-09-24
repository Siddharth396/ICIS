import { Text, theme } from '@icis/ui-kit';
import styled, { css } from 'styled-components';

// can't use the lock icon from ui-kit as we need the svg string to create a blob from it.
const LockCursorSvg = `<svg width="32" height="32" viewBox="0 0 32 32" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M14.3125 12.25V14H18.6875V12.25C18.6875 11.0469 17.7031 10.0625 16.5 10.0625C15.2695 10.0625 14.3125 11.0469 14.3125 12.25ZM13 14V12.25C13 10.3359 14.5586 8.75 16.5 8.75C18.4141 8.75 20 10.3359 20 12.25V14H20.875C21.832 14 22.625 14.793 22.625 15.75V21C22.625 21.9844 21.832 22.75 20.875 22.75H12.125C11.1406 22.75 10.375 21.9844 10.375 21V15.75C10.375 14.793 11.1406 14 12.125 14H13ZM11.6875 15.75V21C11.6875 21.2461 11.8789 21.4375 12.125 21.4375H20.875C21.0938 21.4375 21.3125 21.2461 21.3125 21V15.75C21.3125 15.5312 21.0938 15.3125 20.875 15.3125H12.125C11.8789 15.3125 11.6875 15.5312 11.6875 15.75Z" fill="${theme.colours.NEUTRALS_2}" />
</svg>`;

const blobFromSvg = new Blob([LockCursorSvg], { type: 'image/svg+xml' });
const LockCursorSvgURL = window.URL.createObjectURL(blobFromSvg);

export const HeaderWrapper = styled.div`
  width: 100%;
  padding-bottom: 15px;
  display: flex;
  flex-direction: column;
  gap: 16px;
`;

export const DatePickerWrapper = styled.div``;

export const ValidationMessageWrapper = styled.div`
  width: 100%;
  display: flex;
  gap: 5px;
  align-items: center;
  white-space: nowrap;
`;

export const RowLeft = styled.div`
  flex: 1;
  min-width: 300px;

  @media (min-width: 769px) {
    max-width: 40%;
  }
`;

export const RowRight = styled.div`
  flex: 1;
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: 12px;
  min-width: 300px;

  @media (max-width: 768px) {
    justify-content: flex-start;
    margin-top: 8px;
  }
`;

export const HeaderRow = styled.div`
  display: flex;
  flex-wrap: wrap;
  gap: ${theme.spacing.BASE_2};
  @media (max-width: 768px) {
    flex-direction: column;
  }
`;

export const ShowHideButtonWrapper = styled.div`
  display: flex;
  justify-content: flex-end;
  width: auto;
`;

export const CapabilityContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
`;

export const CommentaryTitle = styled(Text.H4)`
  margin-bottom: ${theme.spacing.BASE_2};
`;

export const CommentaryContainer = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;
  margin-left: 50%;
  margin-top: ${theme.spacing.BASE_4};
`;

export const WorkflowButtonWrapper = styled.div`
  display: flex;
  justify-content: flex-end;
`;

export const LockedContentOverlay = styled.div<{ isLocked?: boolean }>`
  ${({ isLocked }) =>
    isLocked &&
    css`
      pointer-events: none;
    `};
`;

export const LockedContentCursorOverlay = styled.div<{ isLocked?: boolean }>`
  ${({ isLocked }) =>
    isLocked &&
    css`
      cursor:
        url(${LockCursorSvgURL}) 0 0,
        auto;
    `};
`;

export const ErrorSection = styled.div`
  display: flex;
  align-items: center;
  justify-content: start;
  gap: 5px;
  border-radius: 4px;
  white-space: nowrap;
`;

// Add an icon for error, optional
export const ErrorIcon = styled.div`
  margin-right: 8px;
  font-size: 16px;
  color: #cc0000;
  display: flex;
  align-items: center;
`;

export const AddGridButtonWrapper = styled.div`
  display: flex;
  justify-content: center;
  margin: 16px 0; /* add vertical space above and below */
`;
