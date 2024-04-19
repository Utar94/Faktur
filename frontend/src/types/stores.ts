import type { Address, AddressPayload, Email, EmailPayload, Phone, PhonePayload } from "./users";
import type { Aggregate } from "./aggregate";
import type { Banner } from "./banners";
import type { Department } from "./departments";
import type { SearchPayload, SortOption } from "./search";

export type CreateStorePayload = {
  bannerId?: string;
  number?: string;
  displayName: string;
  description?: string;
  address?: AddressPayload;
  email?: EmailPayload;
  phone?: PhonePayload;
};

export type ReplaceStorePayload = {
  bannerId?: string;
  number?: string;
  displayName: string;
  description?: string;
  address?: AddressPayload;
  email?: EmailPayload;
  phone?: PhonePayload;
};

export type SearchStoresPayload = SearchPayload & {
  bannerId?: string;
  sort: StoreSortOption[];
};

export type Store = Aggregate & {
  number?: string;
  displayName: string;
  description?: string;
  address?: Address;
  email?: Email;
  phone?: Phone;
  departmentCount: number;
  departments: Department[];
  banner?: Banner;
};

export type StoreSort = "DepartmentCount" | "DisplayName" | "Number" | "UpdatedOn";

export type StoreSortOption = SortOption & {
  bannerId?: string;
  field: StoreSort;
};
