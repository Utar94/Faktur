export type ChangePasswordInput = {
  current: string;
  new: string;
};

export type CurrentUser = {
  displayName: string;
  emailAddress?: string;
  pictureUrl?: string;
};

export type SaveProfilePayload = {
  emailAddress?: string;
  firstName?: string;
  lastName?: string;
  pictureUrl?: string;
};

export type SignInPayload = {
  username: string;
  password: string;
};

export type UserProfile = {
  createdOn: string;
  updatedOn: string;
  username: string;
  passwordChangedOn?: string;
  emailAddress?: string;
  firstName?: string;
  lastName?: string;
  fullName?: string;
  pictureUrl?: string;
  authenticatedOn?: string;
};
