let isAuthoring = false;

export const setIsAuthoring = (newIsAuthoring: boolean) => {
  isAuthoring = newIsAuthoring;
};

export const getIsAuthoring = () => {
  return isAuthoring;
};
