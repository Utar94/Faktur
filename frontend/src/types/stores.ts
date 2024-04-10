import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { Banner } from "./banners";
import type { Department } from "./departments";

export type Address = Contact & {
  street: string;
  locality: string;
  postalCode?: string;
  region?: string;
  country: string;
  formatted: string;
};

export type Contact = {
  isVerified: boolean;
  verifiedBy?: Actor;
  verifiedOn?: string;
};

export type Email = Contact & {
  address: string;
};

export type Phone = Contact & {
  countryCode?: string;
  number: string;
  extension?: string;
  e164Formatted: string;
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
