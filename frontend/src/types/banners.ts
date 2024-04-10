import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";
import type { Store } from "./stores";

export type Banner = Aggregate & {
  displayName: string;
  description?: string;
  stores: Store[];
};

export type BannerSort = "DisplayName" | "UpdatedOn";

export type BannerSortOption = SortOption & {
  field: BannerSort;
};

export type CreateBannerPayload = {
  displayName: string;
  description?: string;
};

export type ReplaceBannerPayload = {
  displayName: string;
  description?: string;
};

export type SearchBannersPayload = SearchPayload & {
  sort: BannerSortOption[];
};
