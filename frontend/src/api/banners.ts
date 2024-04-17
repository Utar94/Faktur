import type { Banner, CreateBannerPayload, ReplaceBannerPayload, SearchBannersPayload } from "@/types/banners";
import type { SearchResults } from "@/types/search";
import { UrlBuilder, type IUrlBuilder } from "@/helpers/urlUtils";
import { _delete, get, post, put } from ".";

function createUrlBuilder(id?: string): IUrlBuilder {
  if (id) {
    return new UrlBuilder({ path: "/banners/{id}" }).setParameter("id", id);
  }
  return new UrlBuilder({ path: "/banners" });
}

export async function createBanner(payload: CreateBannerPayload): Promise<Banner> {
  return (await post<CreateBannerPayload, Banner>(createUrlBuilder().buildRelative(), payload)).data;
}

export async function deleteBanner(id: string): Promise<Banner> {
  return (await _delete<Banner>(createUrlBuilder(id).buildRelative())).data;
}

export async function readBanner(id: string): Promise<Banner> {
  return (await get<Banner>(createUrlBuilder(id).buildRelative())).data;
}

export async function replaceBanner(id: string, payload: ReplaceBannerPayload, version?: number): Promise<Banner> {
  const url: string = createUrlBuilder(id).setQueryString(`?version=${version}`).buildRelative();
  return (await put<CreateBannerPayload, Banner>(url, payload)).data;
}

export async function searchBanners(payload: SearchBannersPayload): Promise<SearchResults<Banner>> {
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
  return (await get<SearchResults<Banner>>(url)).data;
}
