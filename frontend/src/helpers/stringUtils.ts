export function cleanTrim(s?: string): string | undefined {
  return isNullOrWhiteSpace(s) ? undefined : s?.trim();
}

export function combineURL(...segments: string[]): string {
  const url = segments
    .map((v) => v?.trim().replace(/^\/+|\/+$/g, "") ?? "")
    .filter((v) => v.length)
    .join("/");
  return isAbsoluteURL(url) ? url : `/${url}`;
}

const absoluteUrlRegex = new RegExp("^(?:[a-z+]+:)?//", "i");
export function isAbsoluteURL(url: string): boolean {
  return absoluteUrlRegex.test(url);
}

export function isDigit(c: string): boolean {
  return c.trim() !== "" && !isNaN(Number(c));
}

export function isLetter(c: string): boolean {
  return c.toLowerCase() !== c.toUpperCase();
}

export function isLetterOrDigit(c: string): boolean {
  return isDigit(c) || isLetter(c);
}

export function isNullOrEmpty(s?: string): boolean {
  return typeof s !== "string" || s.length === 0;
}

export function isNullOrWhiteSpace(s?: string): boolean {
  return isNullOrEmpty(s?.trim());
}

export function shortify(s: string, length: number): string {
  return s.length > length ? s.substring(0, length - 1) + "…" : s;
}

export function slugify(s?: string): string {
  if (!s) {
    return "";
  }
  const words = [];
  let word = "";
  for (let i = 0; i < s.length; i++) {
    const c = s[i];
    if (isLetterOrDigit(c)) {
      word += c;
    } else if (word.length) {
      words.push(word);
      word = "";
    }
  }
  if (word.length) {
    words.push(word);
  }
  return unaccent(words.join("-").toLowerCase());
}

const reservedChars = new Set<string>(["]", "^", "\\"]);
export function trim(s: string, c: string): string {
  c = reservedChars.has(c) ? `\\${c}` : c;
  return s.replace(new RegExp(`^[${c}]+|[${c}]+$`, "g"), "");
}
export function trimEnd(s: string, c: string): string {
  c = reservedChars.has(c) ? `\\${c}` : c;
  return s.replace(new RegExp(`[${c}]+$`, "g"), "");
}
export function trimStart(s: string, c: string): string {
  c = reservedChars.has(c) ? `\\${c}` : c;
  return s.replace(new RegExp(`^[${c}]+`, "g"), "");
}

const accents = new Map<string, string>([
  ["à", "a"],
  ["â", "a"],
  ["ç", "c"],
  ["è", "e"],
  ["é", "e"],
  ["ê", "e"],
  ["ë", "e"],
  ["î", "i"],
  ["ï", "i"],
  ["ô", "o"],
  ["ù", "u"],
  ["û", "u"],
  ["ü", "u"],
]);
export function unaccent(s: string): string {
  return [...s].map((c) => (c.toUpperCase() === c ? (accents.get(c) ?? c).toUpperCase() : accents.get(c) ?? c)).join("");
}
