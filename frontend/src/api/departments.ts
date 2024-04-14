import type { CreateOrReplaceDepartmentPayload, Department, SearchDepartmentsPayload } from "@/types/departments";
import type { SearchResults } from "@/types/search";
import { _delete, get, put } from ".";

export async function createOrReplaceDepartment(
  storeId: string,
  number: string,
  payload: CreateOrReplaceDepartmentPayload,
  version?: number,
): Promise<Department> {
  const query: string | undefined = version ? `?version=${version}` : "";
  return (await put<CreateOrReplaceDepartmentPayload, Department>(`/stores/${storeId}/departments/${number}${query}`, payload)).data;
}

export async function deleteDepartment(storeId: string, number: string): Promise<Department> {
  return (await _delete<Department>(`/stores/${storeId}/departments/${number}`)).data;
}

export async function searchDepartments(payload: SearchDepartmentsPayload): Promise<SearchResults<Department>> {
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
  return (await get<SearchResults<Department>>(`/stores/${payload.storeId}/departments${query}`)).data;
}
