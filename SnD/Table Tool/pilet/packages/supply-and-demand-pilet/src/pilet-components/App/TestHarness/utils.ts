// istanbul ignore file
export const mockVersion = '';
export const mockMethod = (): void => {};

export const updateTestHarnessUrl = (id: string, version: string) => {
  const { origin, pathname } = window.location;
  let queryString;
  if (id.length) {
    queryString = `id=${id}`;
  }
  if (version.length) {
    queryString += `&version=${Number(version)}`;
  }
  const url = `${origin + pathname}${queryString && `?${queryString}`}`;
  window.history.replaceState({}, '', url);
  // window.location.reload();
};
