import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { Banner } from "./banners";
import type { Department } from "./departments";
import type { SearchPayload, SortOption } from "./search";

export type Address = Contact & {
  street: string;
  locality: string;
  postalCode?: string;
  region?: string;
  country: string;
  formatted: string;
};

export type AddressPayload = ContactPayload & {
  street: string;
  locality: string;
  postalCode?: string;
  region?: string;
  country: string;
};

export type Contact = {
  isVerified: boolean;
  verifiedBy?: Actor;
  verifiedOn?: string;
};

export type ContactPayload = {
  isVerified: boolean;
};

export type CreateStorePayload = {
  bannerId?: string;
  number?: string;
  displayName: string;
  description?: string;
  address?: AddressPayload;
  email?: EmailPayload;
  phone?: PhonePayload;
};

export type Email = Contact & {
  address: string;
};

export type EmailPayload = ContactPayload & {
  address: string;
};

export type Phone = Contact & {
  countryCode?: string;
  number: string;
  extension?: string;
  e164Formatted: string;
};

export type PhonePayload = ContactPayload & {
  countryCode?: string;
  number: string;
  extension?: string;
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
