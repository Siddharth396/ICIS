const componentsLastUpdatedDateUnix: Map<string, number> = new Map<string, number>([]);

export const setLastUpdatedDate = (contentBlockId: string, lastUpdatedDateUnix?: number) => {
  lastUpdatedDateUnix && componentsLastUpdatedDateUnix.set(contentBlockId, lastUpdatedDateUnix);
};

export const getLastUpdatedDateUnix = (contentBlockId: string) => {
  return componentsLastUpdatedDateUnix.get(contentBlockId);
};
