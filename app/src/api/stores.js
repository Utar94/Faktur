import { _delete, get, post, put } from '.'

export async function createStore({ name }) {
  return await post('/stores', { name })
}

export async function deleteStore(id) {
  return await _delete(`/stores/${id}`)
}

export async function getStore(id) {
  return await get(`/stores/${id}`)
}

export async function getStores({ bannerId, deleted, search, sort, desc, index, count }) {
  const query = [
    ['bannerId', bannerId],
    ['deleted', deleted],
    ['search', search],
    ['sort', sort],
    ['desc', desc],
    ['index', index],
    ['count', count]
  ]
    .filter(([, value]) => typeof value !== 'undefined' && value !== null && value !== '')
    .map(pair => pair.join('='))
    .join('&')
  return await get(`/stores?${query}`)
}

export async function updateStore(id, { address, bannerId, city, country, description, name, number, phone, postalCode, state }) {
  return await put(`/stores/${id}`, { address, bannerId, city, country, description, name, number, phone, postalCode, state })
}
