import type { Address, AddressPayload, Email, EmailPayload, Phone, PhonePayload } from "./users";
import type { Locale } from "./i18n";

export type ChangePasswordInput = {
  current: string;
  new: string;
};

export type ContactInformation = {
  address?: AddressPayload;
  email: EmailPayload;
  phone?: PhonePayload;
};

export type CurrentUser = {
  displayName: string;
  emailAddress?: string;
  pictureUrl?: string;
};

export type PersonNameType = "first" | "last" | "middle" | "nick";

export type PersonalInformation = {
  firstName: string;
  middleName?: string;
  lastName: string;
  nickname?: string;
  birthdate?: Date;
  gender?: string;
  locale?: string;
  timeZone?: string;
  picture?: string;
  profile?: string;
  website?: string;
};

export type SaveProfilePayload = {
  contactInformation?: ContactInformation;
  personalInformation?: PersonalInformation;
};

export type SignInPayload = {
  username: string;
  password: string;
};

export type UserProfile = {
  username: string;
  passwordChangedOn?: string;
  authenticatedOn?: string;
  address?: Address;
  email?: Email;
  phone?: Phone;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  fullName?: string;
  nickname?: string;
  birthdate?: string;
  gender?: string;
  locale?: Locale;
  timeZone?: string;
  picture?: string;
  profile?: string;
  website?: string;
};
