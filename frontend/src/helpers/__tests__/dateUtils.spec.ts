import { describe, it, expect } from "vitest";

import { toDateTimeLocal } from "../dateUtils";

describe("dateUtils.toDateTimeLocal", () => {
  it.concurrent("should return the correct converted date", () => {
    const date = new Date(2024, 3, 14, 23, 24, 25);
    const local = toDateTimeLocal(date);
    expect(local).toBe("2024-04-14T23:24");
  });
});
