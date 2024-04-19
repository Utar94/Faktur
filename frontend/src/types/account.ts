import type { Locale } from "./i18n";

export type ChangePasswordInput = {
  current: string;
  new: string;
};

export type CurrentUser = {
  displayName: string;
  emailAddress?: string;
  pictureUrl?: string;
};

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
  personalInformation?: PersonalInformation;
};

export type SignInPayload = {
  username: string;
  password: string;
};

export type UserProfile = {
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
