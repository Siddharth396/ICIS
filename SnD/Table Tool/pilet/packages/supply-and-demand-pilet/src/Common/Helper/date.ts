export const getLatestDateUnix = (dateList: string[]) => {
  return dateList.length > 0 ? Math.max(...dateList.map(date => new Date(date + 'Z').getTime())) : undefined;
};

export const getSnapshotDate = (unixTimestamp: number) => {
  try {
    const date = new Date(unixTimestamp);

    const options: Intl.DateTimeFormatOptions = {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
    };

    return new Intl.DateTimeFormat('en-GB', options).format(date);
  }
  catch (e) {
    console.log((e as Error).message);
    return '';
  }
};
