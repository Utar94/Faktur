import { describe, it, expect } from "vitest";
import { nanoid } from "nanoid";

import i18n from "@/i18n";
import type { Actor } from "@/types/actor";
import type { Department } from "@/types/departments";
import type { Receipt } from "@/types/receipts";
import type { Store } from "@/types/stores";
import { formatDepartment, formatReceipt } from "../displayUtils";

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
  version: 1,
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
  displayName: "Maxi Drummondville",
  departmentCount: 0,
  departments: [],
};

describe("displayUtils.formatDepartment", () => {
  it.concurrent("should return the correct formatted department", () => {
    const department: Department = {
      number: "36",
      displayName: "PRET-A-MANGER",
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      store,
    };
    store.departmentCount = 1;
    store.departments.push(department);
    expect(formatDepartment(department)).toBe("PRET-A-MANGER (#36)");
  });
});

describe("displayUtils.formatDepartment", () => {
  it.concurrent("should return the correct formatted receipt with number", () => {
    const receipt: Receipt = {
      id: nanoid(),
      version: 1,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      issuedOn: now,
      number: "117011",
      itemCount: 0,
      items: [],
      subTotal: 0,
      total: 0,
      taxes: [],
      hasBeenProcessed: false,
      store,
    };
    const { d } = i18n.global;
    expect(formatReceipt(receipt)).toBe(`${d(new Date(receipt.issuedOn), "medium")} (#117011)`);
  });

  it.concurrent("should return the correct formatted receipt without number", () => {
    const receipt: Receipt = {
      id: nanoid(),
      version: 1,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      issuedOn: now,
      number: undefined,
      itemCount: 0,
      items: [],
      subTotal: 0,
      total: 0,
      taxes: [],
      hasBeenProcessed: false,
      store,
    };
    const { d } = i18n.global;
    expect(formatReceipt(receipt)).toBe(d(new Date(receipt.issuedOn), "medium"));
  });
});
