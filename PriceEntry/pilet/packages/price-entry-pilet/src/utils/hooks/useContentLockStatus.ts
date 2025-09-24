// istanbul ignore file
import { useMemo } from 'react';
import { LockInformationType } from '@icis/app-shell';
import { getStartOfDayUTC } from 'utils/date';

type UseContentLockStatusParams = {
  lockInformation: LockInformationType | undefined;
  contentBlockId: string | undefined;
  selectedDate: Date;
  userId: string | undefined;
};

export const useContentLockStatus = ({
  lockInformation,
  contentBlockId,
  selectedDate,
  userId,
}: UseContentLockStatusParams): boolean => {
  return useMemo(() => {
    // Get the selected date timestamp in UTC
    const selectedDateTimestamp = getStartOfDayUTC(selectedDate)?.getTime().toString();

    if (!lockInformation?.lockedContentData || !contentBlockId || !selectedDateTimestamp) {
      return false; // Return false if required data is missing
    }

    // Filter locked content by other users
    const contentLockedByOtherUsers = lockInformation.lockedContentData.filter(
      (lock: any) => lock.id !== userId,
    );

    if (contentLockedByOtherUsers.length === 0) {
      return false; // No content locked by other users
    }

    // Check if the selected content is locked
    return contentLockedByOtherUsers.some((userLockedContent) =>
      userLockedContent.lockedContent?.some((lockedContent) =>
        lockedContent.lockedContentDetails?.some(
          (lockedContentDetail) =>
            lockedContentDetail.id === selectedDateTimestamp &&
            lockedContentDetail.data === contentBlockId,
        ),
      ),
    );
  }, [lockInformation, contentBlockId, selectedDate, userId]);
};
