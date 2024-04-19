import type { CurrentUser, SaveProfilePayload, SignInPayload, UserProfile } from "@/types/account";
import { UrlBuilder, type IUrlBuilder } from "@/helpers/urlUtils";
import { get, post, put } from ".";

function createUrlBuilder(path: string): IUrlBuilder {
  return new UrlBuilder({ path: `/account/${path}` });
}

export async function getProfile(): Promise<UserProfile> {
  return (await get<UserProfile>(createUrlBuilder("profile").buildRelative())).data;
}

export async function saveProfile(payload: SaveProfilePayload): Promise<UserProfile> {
  return (await put<SaveProfilePayload, UserProfile>(createUrlBuilder("profile").buildRelative(), payload)).data;
}

export async function signIn(payload: SignInPayload): Promise<CurrentUser> {
  return (await post<SignInPayload, CurrentUser>(createUrlBuilder("sign/in").buildRelative(), payload)).data;
}

export async function signOut(): Promise<void> {
  await post(createUrlBuilder("sign/out").buildRelative());
}
