import { get, patch, post, put } from '.'

export async function getReceipt(id) {
  return await get(`/receipts/${id}`)
}

export async function getReceipts({ deleted, search, storeId, sort, desc, index, count }) {
  const query = [
    ['deleted', deleted],
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
  return await get(`/receipts?${query}`)
}

export async function importReceipt({ culture, issuedAt, lines, number, storeId }) {
  return await post('/receipts/import', { culture, issuedAt, lines, number, storeId })
}

export async function processReceipt(id, { items }) {
  return await put(`/receipts/${id}/process`, { items })
}

export async function setReceiptDeleted(id) {
  return await patch(`/receipts/${id}/delete`)
}

export async function updateItem(id, { price, quantity, unitPrice }) {
  return await put(`/receipts/items/${id}`, { price, quantity, unitPrice })
}

export async function updateReceipt(id, { issuedAt, number }) {
  return await put(`/receipts/${id}`, { issuedAt, number })
}
