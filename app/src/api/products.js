import { _delete, get, post, put } from '.'

export async function createProduct({ articleId, departmentId, storeId }) {
  return await post('/products', { articleId, departmentId, storeId })
}

export async function deleteProduct(id) {
  return await _delete(`/products/${id}`)
}

export async function getProduct(id) {
  return await get(`/products/${id}`)
}

export async function getProducts({ articleId, deleted, departmentId, search, storeId, sort, desc, index, count }) {
  const query = [
    ['articleId', articleId],
    ['deleted', deleted],
    ['departmentId', departmentId],
    ['search', search],
    ['storeId', storeId],
    ['sort', sort],
    ['desc', desc],
    ['index', index],
    ['count', count]
  ]
    .filter(([, value]) => typeof value !== 'undefined' && value !== null && value !== '')
    .map(pair => pair.join('='))
    .join('&')
  return await get(`/products?${query}`)
}

export async function updateProduct(id, { departmentId, description, flags, label, sku, unitPrice, unitType }) {
  return await put(`/products/${id}`, { departmentId, description, flags, label, sku, unitPrice, unitType })
}
