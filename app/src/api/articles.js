import { get, patch, post, put } from '.'

export async function createArticle({ name }) {
  return await post('/articles', { name })
}

export async function getArticle(id) {
  return await get(`/articles/${id}`)
}

export async function getArticles({ deleted, search, sort, desc, index, count }) {
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
  return await get(`/articles?${query}`)
}

export async function setArticleDeleted(id) {
  return await patch(`/articles/${id}/delete`)
}

export async function updateArticle(id, { description, gtin, name }) {
  return await put(`/articles/${id}`, { description, gtin, name })
}
