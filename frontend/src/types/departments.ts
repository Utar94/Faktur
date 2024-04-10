import type { Actor } from "./actor";
import type { Store } from "./stores";

export type Department = {
  number: string;
  displayName: string;
  description?: string;
  createdBy: Actor;
  createdOn: string;
  updatedBy: Actor;
  updatedOn: string;
  store: Store;
};
