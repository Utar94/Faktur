import type { Store, CreateStorePayload, ReplaceStorePayload, SearchStoresPayload } from "@/types/stores";
import type { SearchResults } from "@/types/search";
import { _delete, get, post, put } from ".";

export async function createStore(payload: CreateStorePayload): Promise<Store> {
  return (await post<CreateStorePayload, Store>("/stores", payload)).data;
}

export async function deleteStore(id: string): Promise<Store> {
  return (await _delete<Store>(`/stores/${id}`)).data;
}

export async function readStore(id: string): Promise<Store> {
  return (await get<Store>(`/stores/${id}`)).data;
}

export async function replaceStore(id: string, payload: ReplaceStorePayload, version?: number): Promise<Store> {
  const query: string | undefined = version ? `?version=${version}` : undefined; // TODO(fpion): refactor
  return (await put<CreateStorePayload, Store>(`/stores/${id}${query}`, payload)).data;
}

export async function searchStores(payload: SearchStoresPayload): Promise<SearchResults<Store>> {
  const params: string[] = [];
  if (payload.bannerId) {
    params.push(`banner=${payload.bannerId}`);
  }
  payload.ids.forEach((id) => params.push(`ids=${id}`));
  if (payload.search.terms.length) {
    payload.search.terms.forEach((term) => params.push(`search_terms=${term.value}`));
    params.push(`search_operator=${payload.search.operator}`);
  }
  payload.sort.forEach((sort) => params.push(`sort=${sort.isDescending ? `DESC.${sort.field}` : sort.field}`));
  params.push(`skip=${payload.skip}`);
  params.push(`limit=${payload.limit}`);
  const query: string | undefined = params.length ? `?${params.join("&")}` : undefined; // TODO(fpion): refactor
  return (await get<SearchResults<Store>>(`/stores${query}`)).data;
}
