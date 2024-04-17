import { describe, it, expect } from "vitest";

import {
  cleanTrim,
  combineURL,
  isAbsoluteURL,
  isDigit,
  isLetter,
  isLetterOrDigit,
  isNullOrEmpty,
  isNullOrWhiteSpace,
  shortify,
  slugify,
  trim,
  trimEnd,
  trimStart,
  unaccent,
} from "../stringUtils";

describe("stringUtils.cleanTrim", () => {
  it.concurrent("should return undefined when the string is null, empty or white space", () => {
    expect(cleanTrim(undefined)).toBeUndefined();
    expect(cleanTrim("")).toBeUndefined();
    expect(cleanTrim("    ")).toBeUndefined();
  });

  it.concurrent("should return the trimmed string when it is not null, empty nor white space", () => {
    expect(cleanTrim("  test")).toBe("test");
    expect(cleanTrim("test  ")).toBe("test");
    expect(cleanTrim(" test ")).toBe("test");
  });
});

describe("stringUtils.combineURL", () => {
  it.concurrent("should combine the segments with a slash (/)", () => {
    expect(combineURL()).toBe("/");
    expect(combineURL("Hello World!")).toBe("/Hello World!");
    expect(combineURL("hello", "world")).toBe("/hello/world");
    expect(combineURL("http://api.google.com", "users/123")).toBe("http://api.google.com/users/123");
    expect(combineURL("https://api.google.com", "users", "123")).toBe("https://api.google.com/users/123");
  });

  it.concurrent("should ignore empty values", () => {
    expect(combineURL("")).toBe("/");
    expect(combineURL("   ")).toBe("/");
    expect(combineURL("", "api", "user")).toBe("/api/user");
    expect(combineURL("   ", "api", "user")).toBe("/api/user");
    expect(combineURL("api", "", "user")).toBe("/api/user");
    expect(combineURL("api", "   ", "user")).toBe("/api/user");
    expect(combineURL("api", "user", "")).toBe("/api/user");
    expect(combineURL("api", "user", "   ")).toBe("/api/user");
  });

  it.concurrent("should ignore undefined values", () => {
    const u: any = undefined;
    expect(combineURL(u)).toBe("/");
    expect(combineURL(u, "api", "user")).toBe("/api/user");
    expect(combineURL("api", u, "user")).toBe("/api/user");
    expect(combineURL("api", "user", u)).toBe("/api/user");
  });

  it.concurrent("should remove starting & ending slashes (/) from segments", () => {
    expect(combineURL("/api", "users/123")).toBe("/api/users/123");
    expect(combineURL("api", "users/123/")).toBe("/api/users/123");
    expect(combineURL("/api", "users/123/")).toBe("/api/users/123");
  });
});

describe("stringUtils.isAbsoluteURL", () => {
  it.concurrent("should return false when input string is not an absolute URL", () => {
    expect(isAbsoluteURL("")).toBe(false);
    expect(isAbsoluteURL("  ")).toBe(false);
    expect(isAbsoluteURL("/api/users/123")).toBe(false);
  });

  it.concurrent("should return true when input string is an absolute URL", () => {
    expect(isAbsoluteURL("http://api.test.com/users/123")).toBe(true);
    expect(isAbsoluteURL("https://api.test.com/users/123")).toBe(true);
    expect(isAbsoluteURL("ftp://api.test.com/users/123")).toBe(true);
  });
});

describe("stringUtils.isDigit", () => {
  it.concurrent("should return false when input character is not a digit", () => {
    expect(isDigit("")).toBe(false);
    expect(isDigit("   ")).toBe(false);
    expect(isDigit("A")).toBe(false);
  });

  it.concurrent("should return true when input character is a digit", () => {
    expect(isDigit("0")).toBe(true);
    expect(isDigit("4")).toBe(true);
  });
});

describe("stringUtils.isLetter", () => {
  it.concurrent("should return false when input character is not a letter", () => {
    expect(isLetter("")).toBe(false);
    expect(isLetter("   ")).toBe(false);
    expect(isLetter("0")).toBe(false);
    expect(isLetter("|")).toBe(false);
  });

  it.concurrent("should return true when input character is a letter", () => {
    expect(isLetter("A")).toBe(true);
    expect(isLetter("Z")).toBe(true);
  });
});

describe("stringUtils.isLetterOrDigit", () => {
  it.concurrent("should return false when input character is not a letter, nor a digit", () => {
    expect(isLetterOrDigit("")).toBe(false);
    expect(isLetterOrDigit("   ")).toBe(false);
    expect(isLetterOrDigit("|")).toBe(false);
  });

  it.concurrent("should return true when input character is a letter or a digit", () => {
    expect(isLetterOrDigit("B")).toBe(true);
    expect(isLetterOrDigit("D")).toBe(true);
    expect(isLetterOrDigit("1")).toBe(true);
    expect(isLetterOrDigit("3")).toBe(true);
  });
});

describe("stringUtils.isNullOrEmpty", () => {
  it.concurrent("should return false when the string is not null nor empty", () => {
    expect(isNullOrEmpty("    ")).toBe(false);
    expect(isNullOrEmpty("test")).toBe(false);
  });

  it.concurrent("should return true when the string is null or empty", () => {
    expect(isNullOrEmpty(undefined)).toBe(true);
    expect(isNullOrEmpty("")).toBe(true);
  });
});

describe("stringUtils.isNullOrWhiteSpace", () => {
  it.concurrent("should return false when the string is not null nor white space", () => {
    expect(isNullOrWhiteSpace("  test  ")).toBe(false);
  });

  it.concurrent("should return true when the string is null or white space", () => {
    expect(isNullOrWhiteSpace(undefined)).toBe(true);
    expect(isNullOrWhiteSpace("")).toBe(true);
    expect(isNullOrWhiteSpace("    ")).toBe(true);
  });
});

describe("stringUtils.shortify", () => {
  it.concurrent("should return the same string when it is not too long", () => {
    expect(shortify("", 20)).toBe("");
    expect(shortify("   ", 20)).toBe("   ");
    expect(shortify("Hello World!", 20)).toBe("Hello World!");
  });

  it.concurrent("should return the shortified string when it is too long", () => {
    expect(shortify("", -1)).toBe("…");
    expect(shortify("   ", 2)).toBe(" …");
    expect(shortify("Hello World!", 10)).toBe("Hello Wor…");
  });
});

describe("stringUtils.slugify", () => {
  it.concurrent("should return an empty string when it is empty or undefined", () => {
    expect(slugify(undefined)).toBe("");
    expect(slugify("")).toBe("");
    expect(slugify("   ")).toBe("");
  });

  it.concurrent(
    "should separate the input string by non-alphanumeric characters, join them using hyphens (-), remove accents and return a lowercased slug",
    () => {
      expect(slugify("arc-en-ciel")).toBe("arc-en-ciel");
      expect(slugify("Héllo Wôrld!")).toBe("hello-world");
    },
  );
});

describe("stringUtils.trim", () => {
  it.concurrent("should return the original string when it does not start nor end with the specified character", () => {
    expect(trim("", "")).toBe("");
    expect(trim("f|oo", "|")).toBe("f|oo");
  });

  it.concurrent("should trim the start and end of the specified string, removing the specified character", () => {
    expect(trim("||f|oo+", "|")).toBe("f|oo+");
    expect(trim("+f|oo||", "|")).toBe("+f|oo");
    expect(trim("|f|oo||", "|")).toBe("f|oo");
    expect(trim("]f|oo]", "]")).toBe("f|oo");
    expect(trim("^f|oo^", "^")).toBe("f|oo");
    expect(trim("\\f|oo\\", "\\")).toBe("f|oo");
  });
});

describe("stringUtils.trimEnd", () => {
  it.concurrent("should return the original string when it does not end with the specified character", () => {
    expect(trimEnd("", "")).toBe("");
    expect(trimEnd("||f|oo", "|")).toBe("||f|oo");
  });

  it.concurrent("should trim the end of the specified string, removing the specified character", () => {
    expect(trimEnd("||f|oo+", "|")).toBe("||f|oo+");
    expect(trimEnd("+f|oo||", "|")).toBe("+f|oo");
    expect(trimEnd("|f|oo||", "|")).toBe("|f|oo");
    expect(trimEnd("]f|oo]", "]")).toBe("]f|oo");
    expect(trimEnd("^f|oo^", "^")).toBe("^f|oo");
    expect(trimEnd("\\f|oo\\", "\\")).toBe("\\f|oo");
  });
});

describe("stringUtils.trimStart", () => {
  it.concurrent("should return the original string when it does not start with the specified character", () => {
    expect(trimStart("", "")).toBe("");
    expect(trimStart("f|oo||", "|")).toBe("f|oo||");
  });

  it.concurrent("should trim the start of the specified string, removing the specified character", () => {
    expect(trimStart("||f|oo+", "|")).toBe("f|oo+");
    expect(trimStart("+f|oo||", "|")).toBe("+f|oo||");
    expect(trimStart("|f|oo||", "|")).toBe("f|oo||");
    expect(trimStart("]f|oo]", "]")).toBe("f|oo]");
    expect(trimStart("^f|oo^", "^")).toBe("f|oo^");
    expect(trimStart("\\f|oo\\", "\\")).toBe("f|oo\\");
  });
});

describe("stringUtils.unaccent", () => {
  it.concurrent("should return the same string when it contains no supported accents", () => {
    expect(unaccent("")).toBe("");
    expect(unaccent("   ")).toBe("   ");
    expect(unaccent("Hello World!")).toBe("Hello World!");
  });

  it.concurrent("should return the string without accents when it contains supported accents", () => {
    expect(unaccent("français")).toBe("francais");
    expect(unaccent("  Noël  ")).toBe("  Noel  ");
    expect(unaccent("Héllo Wôrld!")).toBe("Hello World!");
  });
});
