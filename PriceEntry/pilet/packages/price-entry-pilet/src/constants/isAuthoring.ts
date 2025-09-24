let isAuthoring = false;

export const setIsAuthoring = (newIsAuthoring: boolean) => {
  isAuthoring = newIsAuthoring;
};

export const getIsAuthoring = () => {
  const url = window.location.href;
  if (url.includes('localhost')) {
    return url.includes('authoring')
  }
  return isAuthoring;
};
