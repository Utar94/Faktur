export function isTaxable(taxFlags: string, itemFlags: string): boolean {
  for (let i = 0; i < taxFlags.length; i++) {
    if (itemFlags.includes(taxFlags.charAt(i))) {
      return true;
    }
  }
  return false;
}
