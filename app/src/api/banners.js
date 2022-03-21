import { _delete, get, post, put } from '.'

export async function createBanner({ name }) {
  return await post('/banners', { name })
}

export async function deleteBanner(id) {
  return await _delete(`/banners/${id}`)
}

export async function getBanner(id) {
  return await get(`/banners/${id}`)
}

export async function getBanners({ deleted, search, sort, desc, index, count }) {
  const query = [
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
  return await get(`/banners?${query}`)
}

export async function updateBanner(id, { description, name }) {
  return await put(`/banners/${id}`, { description, name })
}
