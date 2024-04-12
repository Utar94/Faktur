import type { Actor } from "./actor";
import type { SearchPayload, SortOption } from "./search";
import type { Store } from "./stores";

export type CreateOrReplaceDepartmentPayload = {
  displayName: string;
  description?: string;
};

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

export type DepartmentSort = "DisplayName" | "Number" | "UpdatedOn";

export type DepartmentSortOption = SortOption & {
  field: DepartmentSort;
};

export type SearchDepartmentsPayload = SearchPayload & {
  storeId: string;
  sort: DepartmentSortOption[];
};
