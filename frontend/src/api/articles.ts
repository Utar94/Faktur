import type { Article, CreateArticlePayload, ReplaceArticlePayload, SearchArticlesPayload } from "@/types/articles";
import type { SearchResults } from "@/types/search";
import { UrlBuilder, type IUrlBuilder } from "@/helpers/urlUtils";
import { _delete, get, post, put } from ".";

function createUrlBuilder(id?: string): IUrlBuilder {
  if (id) {
    return new UrlBuilder({ path: "/articles/{id}" }).setParameter("id", id);
  }
  return new UrlBuilder({ path: "/articles" });
}

export async function createArticle(payload: CreateArticlePayload): Promise<Article> {
  return (await post<CreateArticlePayload, Article>(createUrlBuilder().buildRelative(), payload)).data;
}

export async function deleteArticle(id: string): Promise<Article> {
  return (await _delete<Article>(createUrlBuilder(id).buildRelative())).data;
}

export async function readArticle(id: string): Promise<Article> {
  return (await get<Article>(createUrlBuilder(id).buildRelative())).data;
}

export async function replaceArticle(id: string, payload: ReplaceArticlePayload, version?: number): Promise<Article> {
  const url: string = createUrlBuilder(id).setQueryString(`?version=${version}`).buildRelative();
  return (await put<CreateArticlePayload, Article>(url, payload)).data;
}

export async function searchArticles(payload: SearchArticlesPayload): Promise<SearchResults<Article>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search_terms",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Article>>(url)).data;
}
