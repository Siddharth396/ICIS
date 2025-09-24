// istanbul ignore file
export const mockVersion = '';
export const mockMethod = (version: string): void => {
  const urlSearchParams = new URLSearchParams(window.location.search);
  const queryStringId = urlSearchParams.get('id');
  updateTestHarnessUrl(queryStringId, version);
};

export const updateTestHarnessUrl = (id: string | null, version: string) => {
  const { origin, pathname } = window.location;
  let queryString;
  if (id?.length) {
    queryString = `id=${id}`;
  }
  if (version.length) {
    queryString += `&version=${Number(version)}`;
  }
  const url = `${origin + pathname}${queryString && `?${queryString}`}`;
  window.history.replaceState({}, '', url);
  // window.location.reload();
};
