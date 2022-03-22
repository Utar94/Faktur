import { _delete, get, post, put } from '.'

export async function createDepartment(storeId, { name }) {
  return await post(`/stores/${storeId}/departments`, { name })
}

export async function deleteDepartment(id) {
  return await _delete(`/departments/${id}`)
}

export async function getDepartment(id) {
  return await get(`/departments/${id}`)
}

export async function getDepartments(storeId, { deleted, search, sort, desc, index, count }) {
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
  return await get(`/stores/${storeId}/departments?${query}`)
}

export async function updateDepartment(id, { description, name, number }) {
  return await put(`/departments/${id}`, { description, name, number })
}
