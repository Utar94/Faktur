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
  const query: string | undefined = version ? `?version=${version}` : undefined;
  return (await put<CreateBannerPayload, Banner>(`/banners/${id}${query}`, payload)).data;
}

export async function searchBanners(payload: SearchBannersPayload): Promise<SearchResults<Banner>> {
  return (await get<SearchResults<Banner>>("/banners")).data; // TODO(fpion): payload
}
