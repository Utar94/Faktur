import type { Article, CreateArticlePayload, ReplaceArticlePayload, SearchArticlesPayload } from "@/types/articles";
import type { SearchResults } from "@/types/search";
import { _delete, get, post, put } from ".";

export async function createArticle(payload: CreateArticlePayload): Promise<Article> {
  return (await post<CreateArticlePayload, Article>("/articles", payload)).data;
}

export async function deleteArticle(id: string): Promise<Article> {
  return (await _delete<Article>(`/articles/${id}`)).data;
}

export async function readArticle(id: string): Promise<Article> {
  return (await get<Article>(`/articles/${id}`)).data;
}

export async function replaceArticle(id: string, payload: ReplaceArticlePayload, version?: number): Promise<Article> {
  const query: string | undefined = version ? `?version=${version}` : undefined;
  return (await put<CreateArticlePayload, Article>(`/articles/${id}${query}`, payload)).data;
}

export async function searchArticles(payload: SearchArticlesPayload): Promise<SearchResults<Article>> {
  const params: string[] = [];
  payload.ids.forEach((id) => params.push(`ids=${id}`));
  if (payload.search.terms.length) {
    payload.search.terms.forEach((term) => params.push(`search_terms=${term.value}`));
    params.push(`search_operator=${payload.search.operator}`);
  }
  payload.sort.forEach((sort) => params.push(`sort=${sort.isDescending ? `DESC.${sort.field}` : sort.field}`));
  params.push(`skip=${payload.skip}`);
  params.push(`limit=${payload.limit}`);
  const query: string | undefined = params.length ? `?${params.join("&")}` : undefined;
  return (await get<SearchResults<Article>>(`/articles${query}`)).data;
}
