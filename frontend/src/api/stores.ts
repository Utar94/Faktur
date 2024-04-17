import type { Store, CreateStorePayload, ReplaceStorePayload, SearchStoresPayload } from "@/types/stores";
import type { SearchResults } from "@/types/search";
import { UrlBuilder, type IUrlBuilder } from "@/helpers/urlUtils";
import { _delete, get, post, put } from ".";

function createUrlBuilder(id?: string): IUrlBuilder {
  if (id) {
    return new UrlBuilder({ path: "/stores/{id}" }).setParameter("id", id);
  }
  return new UrlBuilder({ path: "/stores" });
}

export async function createStore(payload: CreateStorePayload): Promise<Store> {
  return (await post<CreateStorePayload, Store>(createUrlBuilder().buildRelative(), payload)).data;
}

export async function deleteStore(id: string): Promise<Store> {
  return (await _delete<Store>(createUrlBuilder(id).buildRelative())).data;
}

export async function readStore(id: string): Promise<Store> {
  return (await get<Store>(createUrlBuilder(id).buildRelative())).data;
}

export async function replaceStore(id: string, payload: ReplaceStorePayload, version?: number): Promise<Store> {
  const url: string = createUrlBuilder(id).setQueryString(`?version=${version}`).buildRelative();
  return (await put<CreateStorePayload, Store>(url, payload)).data;
}

export async function searchStores(payload: SearchStoresPayload): Promise<SearchResults<Store>> {
  const url: string = createUrlBuilder()
    .setQuery("banner", payload.bannerId ?? "")
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
  return (await get<SearchResults<Store>>(url)).data;
}
