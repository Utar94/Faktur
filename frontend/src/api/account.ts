import type { CurrentUser, SignInPayload } from "@/types/account";
import { post } from ".";

export async function signIn(payload: SignInPayload): Promise<CurrentUser> {
  return (await post<SignInPayload, CurrentUser>("/account/sign/in", payload)).data;
}

export async function signOut(): Promise<void> {
  await post("/account/sign/out");
}
