import type { Aggregate } from "./aggregate";
import type { Article } from "./articles";
import type { Department } from "./departments";
import type { SearchPayload, SortOption } from "./search";
import type { Store } from "./stores";

export type CreateOrReplaceProductPayload = {
  departmentNumber?: string;
  sku?: string;
  displayName?: string;
  description?: string;
  flags?: string;
  unitPrice?: number;
  unitType?: UnitType;
};

export type Product = Aggregate & {
  sku?: string;
  displayName?: string;
  description?: string;
  flags?: string;
  unitPrice?: number;
  unitType?: UnitType;
  article: Article;
  store: Store;
  department?: Department;
};

export type ProductSort = "DisplayName" | "Sku" | "UnitPrice" | "UpdatedOn";

export type ProductSortOption = SortOption & {
  field: ProductSort;
};

export type SearchProductsPayload = SearchPayload & {
  storeId: string;
  departmentNumber?: string;
  unitType?: UnitType;
  sort: ProductSortOption[];
};

export type UnitType = "Kg" | "Lbs";
