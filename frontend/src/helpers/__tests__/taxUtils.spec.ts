import { describe, it, expect } from "vitest";

import { isTaxable } from "../taxUtils";

describe("taxUtils.isTaxable", () => {
  it.concurrent("should return false when the flags are empty", () => {
    expect(isTaxable("", "")).toBe(false);
    expect(isTaxable(" ", "")).toBe(false);
    expect(isTaxable("", " ")).toBe(false);
  });

  it.concurrent("should return false when the item is not taxable", () => {
    expect(isTaxable("M", "FPRJ")).toBe(false);
    expect(isTaxable("FPRJ", "M")).toBe(false);
  });

  it.concurrent("should return true when the item is taxable", () => {
    expect(isTaxable("F", "F")).toBe(true);
    expect(isTaxable("P", "FPMRJ")).toBe(true);
    expect(isTaxable("FPMRJ", "  F  ")).toBe(true);
  });
});
