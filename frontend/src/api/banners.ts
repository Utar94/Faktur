import type { Banner, CreateBannerPayload, ReplaceBannerPayload, SearchBannersPayload } from "@/types/banners";
import type { SearchResults } from "@/types/search";
import { _delete, get, post, put } from ".";

export async function createBanner(payload: CreateBannerPayload): Promise<Banner> {
  return (await post<CreateBannerPayload, Banner>("/banners", payload)).data;
}

export async function deleteBanner(id: string): Promise<Banner> {
  return (await _delete<Banner>(`/banners/${id}`)).data;
}

export async function readBanner(id: string): Promise<Banner> {
  return (await get<Banner>(`/banners/${id}`)).data;
}

export async function replaceBanner(id: string, payload: ReplaceBannerPayload, version?: number): Promise<Banner> {
  const query: string | undefined = version ? `?version=${version}` : "";
  return (await put<CreateBannerPayload, Banner>(`/banners/${id}${query}`, payload)).data;
}

export async function searchBanners(payload: SearchBannersPayload): Promise<SearchResults<Banner>> {
  const params: string[] = [];
  payload.ids.forEach((id) => params.push(`ids=${id}`));
  if (payload.search.terms.length) {
    payload.search.terms.forEach((term) => params.push(`search_terms=${term.value}`));
    params.push(`search_operator=${payload.search.operator}`);
  }
  payload.sort.forEach((sort) => params.push(`sort=${sort.isDescending ? `DESC.${sort.field}` : sort.field}`));
  params.push(`skip=${payload.skip}`);
  params.push(`limit=${payload.limit}`);
  const query: string | undefined = params.length ? `?${params.join("&")}` : "";
  return (await get<SearchResults<Banner>>(`/banners${query}`)).data;
}
