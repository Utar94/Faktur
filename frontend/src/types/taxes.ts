import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";

export type CreateTaxPayload = {
  code: string;
  rate: number;
  flags?: string;
};

export type ReplaceTaxPayload = {
  code: string;
  rate: number;
  flags?: string;
};

export type SearchTaxesPayload = SearchPayload & {
  flag?: string;
  sort: TaxSortOption[];
};

export type Tax = Aggregate & {
  code: string;
  rate: number;
  flags?: string;
};

export type TaxSort = "Code" | "Rate" | "UpdatedOn";

export type TaxSortOption = SortOption & {
  field: TaxSort;
};
