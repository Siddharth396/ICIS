export function capitalizeFirstLetter(string: string): string {
    if (!string) return '';
    const convertToLowerCase = string.toLowerCase();
    return convertToLowerCase.charAt(0).toUpperCase() + convertToLowerCase.slice(1);
  }