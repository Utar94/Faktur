import { beforeEach, describe, expect, it } from "vitest";
import { createPinia, setActivePinia } from "pinia";
import { nanoid } from "nanoid";

import type { Actor } from "@/types/actor";
import type { Receipt, ReceiptItem } from "@/types/receipts";
import type { Store } from "@/types/stores";
import { useCategoryStore } from "../categories";

const actor: Actor = {
  id: nanoid(),
  type: "User",
  isDeleted: false,
  displayName: "Francis Pion",
  emailAddress: "no-reply@francispion.ca",
  pictureUrl: "https://www.francispion.ca/assets/img/profile-img.jpg",
};
const now = new Date().toISOString();
const store: Store = {
  id: nanoid(),
  version: 3,
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
  number: "08772",
  displayName: "Maxi Drummondville",
  departmentCount: 1,
  departments: [],
};
store.departments.push({
  number: "36",
  displayName: "PRET-A-MANGER",
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
  store,
});
const receipt: Receipt = {
  id: nanoid(),
  version: 3,
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
  issuedOn: now,
  number: "117011",
  itemCount: 1,
  items: [],
  subTotal: 9.99,
  total: 11.49,
  taxes: [
    {
      code: "GST",
      rate: 0.05,
      taxableAmount: 9.99,
      amount: 0.5,
    },
    {
      code: "QST",
      rate: 0.09975,
      taxableAmount: 9.99,
      amount: 1.0,
    },
  ],
  hasBeenProcessed: false,
  store,
};
receipt.items.push({
  number: 0,
  gtin: "06038385904",
  label: "PC POULET BBQ",
  flags: "FPMRJ",
  quantity: 1,
  unitPrice: 9.99,
  price: 9.99,
  department: {
    number: "36",
    displayName: "PRET-A-MANGER",
  },
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
  receipt,
});

describe("categoriesStore", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it("should keep the existing categories when loading an uncategorized receipt", () => {
    const store = useCategoryStore();
    store.save("Test");
    expect(store.categories.length).toBe(1);
    expect(store.categories.at(0)).toBe("Test");

    store.load(receipt);
    expect(store.categories.length).toBe(1);
    expect(store.categories.at(0)).toBe("Test");
  });

  it("should load the categories of a categorized receipt", () => {
    const store = useCategoryStore();
    store.save("Test");
    expect(store.categories.length).toBe(1);
    expect(store.categories.at(0)).toBe("Test");

    const item: ReceiptItem | undefined = receipt.items.at(0);
    if (!item) {
      return expect(item).toBeDefined();
    }
    item.category = "Category";

    store.load(receipt);
    expect(store.categories.length).toBe(1);
    expect(store.categories.at(0)).toBe("Category");
  });

  it("should return false when removing a category which does not exist", () => {
    const store = useCategoryStore();
    store.save("Test");
    expect(store.categories.length).toBe(1);
    expect(store.categories.at(0)).toBe("Test");

    expect(store.remove("Category")).toBe(false);
    expect(store.categories.length).toBe(1);
    expect(store.categories.at(0)).toBe("Test");
  });

  it("should return false when saving a category causes a conflict", () => {
    const store = useCategoryStore();
    expect(store.save("Test")).toBe(true);
    expect(store.categories.length).toBe(1);
    expect(store.categories.at(0)).toBe("Test");

    expect(store.save("Test")).toBe(false);
    expect(store.categories.length).toBe(1);
    expect(store.categories.at(0)).toBe("Test");
  });

  it("should return false when saving a category without change", () => {
    const store = useCategoryStore();
    expect(store.save("Test", "Test")).toBe(false);
    expect(store.categories.length).toBe(0);
  });

  it("should return true when removing an existing category", () => {
    const store = useCategoryStore();
    store.save("Test");
    expect(store.categories.length).toBe(1);
    expect(store.categories.at(0)).toBe("Test");

    expect(store.remove("Test")).toBe(true);
    expect(store.categories.length).toBe(0);
  });

  it("should return true when saving a new category", () => {
    const store = useCategoryStore();
    expect(store.save("Test")).toBe(true);
    expect(store.categories.length).toBe(1);
    expect(store.categories.at(0)).toBe("Test");

    expect(store.save("Category")).toBe(true);
    expect(store.categories.length).toBe(2);
    expect(store.categories.at(0)).toBe("Test");
    expect(store.categories.at(1)).toBe("Category");
  });

  it("should return true when saving an existing category", () => {
    const store = useCategoryStore();
    expect(store.save("Test")).toBe(true);
    expect(store.categories.length).toBe(1);
    expect(store.categories.at(0)).toBe("Test");

    expect(store.save("Category", "Test")).toBe(true);
    expect(store.categories.length).toBe(1);
    expect(store.categories.at(0)).toBe("Category");
  });
});
