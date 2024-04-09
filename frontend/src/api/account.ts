import type { ChangePasswordInput, CurrentUser, SaveProfilePayload, SignInPayload, UserProfile } from "@/types/account";
import { get, post, put } from ".";

export async function changePassword(payload: ChangePasswordInput): Promise<UserProfile> {
  return (await put<ChangePasswordInput, UserProfile>("/account/password/change", payload)).data;
}

export async function getProfile(): Promise<UserProfile> {
  return (await get<UserProfile>("/account/profile")).data;
}

export async function putProfile(payload: SaveProfilePayload): Promise<UserProfile> {
  return (await put<SaveProfilePayload, UserProfile>("/account/profile", payload)).data;
}

export async function signIn(payload: SignInPayload): Promise<CurrentUser> {
  return (await post<SignInPayload, CurrentUser>("/account/sign/in", payload)).data;
}

export async function signOut(): Promise<void> {
  await post("/account/sign/out");
}
