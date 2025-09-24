import { getIsAuthoring, setIsAuthoring } from 'constants/isAuthoring';

describe('isAuthoring', () => {
  it('getIsAuthoring returns false by default', () => {
    expect(getIsAuthoring()).toBe(false);
  });

  it('getIsAuthoring returns true when setIsAuthoring was last called with true', () => {
    setIsAuthoring(true);
    expect(getIsAuthoring()).toBe(true);
  });
});
