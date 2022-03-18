import { get, post, put } from '.'

export async function changePassword({ current, password }) {
  return await post('/identity/password/change', { current, password })
}

export async function confirm({ id, token }) {
  return await post('/identity/confirm', { id, token })
}

export async function getProfile() {
  return await get('/identity/profile')
}

export async function recoverPassword({ username }) {
  return await post('/identity/password/recover', { username })
}

export async function resetPassword({ id, password, token }) {
  return await post('/identity/password/reset', { id, password, token })
}

export async function saveProfile({ firstName, lastName, locale, picture }) {
  return await put('/identity/profile', { firstName, lastName, locale, picture })
}

export async function signIn({ password, username }) {
  return await post('/identity/sign/in', { password, username })
}

export async function signOut({ refresh_token }) {
  return await post('/identity/sign/out', { refresh_token })
}

export async function signUp({ email, firstName, lastName, locale, password }) {
  return await post('/identity/sign/up', { email, firstName, lastName, locale, password })
}
