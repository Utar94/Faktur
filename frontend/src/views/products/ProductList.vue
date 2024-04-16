<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import DepartmentSelect from "@/components/departments/DepartmentSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import StoreSelect from "@/components/stores/StoreSelect.vue";
import UnitTypeSelect from "@/components/products/UnitTypeSelect.vue";
import type { Product, ProductSort, SearchProductsPayload, UnitType } from "@/types/products";
import type { Store } from "@/types/stores";
import { formatDepartment } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { isEmpty } from "@/helpers/objectUtils";
import { orderBy } from "@/helpers/arrayUtils";
import { readStore } from "@/api/stores";
import { searchProducts } from "@/api/products";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { parseBoolean, parseNumber } = parsingUtils;
const { n, rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const products = ref<Product[]>([]);
const timestamp = ref<number>(0);
const store = ref<Store>();
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const departmentNumber = computed<string>(() => route.query.departmentNumber?.toString() ?? "");
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const storeId = computed<string>(() => route.query.storeId?.toString() ?? "");
const unitType = computed<string>(() => route.query.unitType?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("products.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  if (storeId.value) {
    if (storeId.value !== store.value?.id) {
      store.value = await readStore(storeId.value);
    }
    const payload: SearchProductsPayload = {
      departmentNumber: departmentNumber.value,
      ids: [],
      search: {
        terms: search.value
          .split(" ")
          .filter((term) => Boolean(term))
          .map((term) => ({ value: `%${term}%` })),
        operator: "And",
      },
      storeId: storeId.value,
      unitType: (unitType.value as UnitType) || undefined,
      sort: sort.value ? [{ field: sort.value as ProductSort, isDescending: isDescending.value }] : [],
      skip: (page.value - 1) * count.value,
      limit: count.value,
    };
    isLoading.value = true;
    const now = Date.now();
    timestamp.value = now;
    try {
      const results = await searchProducts(payload);
      if (now === timestamp.value) {
        products.value = results.items;
        total.value = results.total;
      }
    } catch (e: unknown) {
      handleError(e);
    } finally {
      if (now === timestamp.value) {
        isLoading.value = false;
      }
    }
  } else {
    products.value = [];
    store.value = undefined;
    total.value = 0;
  }
}

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "storeId":
      query.departmentNumber = "";
      query.page = "1";
      break;
    case "departmentNumber":
    case "search":
    case "unitType":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "ProductList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                departmentNumber: "",
                search: "",
                storeId: "",
                unitType: "",
                sort: "UpdatedOn",
                isDescending: "true",
                page: 1,
                count: 10,
              }
            : {
                page: 1,
                count: 10,
                ...query,
              },
        });
      } else {
        refresh();
      }
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <main class="container">
    <h1>{{ t("products.title.list") }}</h1>
    <div class="my-3">
      <TarButton
        class="me-1"
        :disabled="isLoading"
        icon="fas fa-rotate"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('actions.refresh')"
        @click="refresh()"
      />
      <RouterLink
        :to="{
          name: 'CreateProduct',
          query: { storeId: storeId || undefined, departmentNumber: departmentNumber || undefined, unitType: unitType || undefined },
        }"
        class="btn btn-success ms-1"
      >
        <font-awesome-icon icon="fas fa-plus" /> {{ t("actions.create") }}
      </RouterLink>
    </div>
    <div class="row">
      <StoreSelect :class="{ 'col-lg-4': Boolean(store) }" :model-value="storeId" @update:model-value="setQuery('storeId', $event ?? '')" />
      <template v-if="store">
        <DepartmentSelect class="col-lg-4" :model-value="departmentNumber" :store="store" @update:model-value="setQuery('departmentNumber', $event ?? '')" />
        <UnitTypeSelect class="col-lg-4" :model-value="unitType" @update:model-value="setQuery('unitType', $event ?? '')" />
      </template>
    </div>
    <div v-if="store" class="row">
      <SearchInput class="col-lg-4" :model-value="search" @update:model-value="setQuery('search', $event ?? '')" />
      <SortSelect
        class="col-lg-4"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('isDescending', $event.toString())"
        @update:model-value="setQuery('sort', $event ?? '')"
      />
      <CountSelect class="col-lg-4" :model-value="count.toString()" @update:model-value="setQuery('count', ($event ?? 10).toString())" />
    </div>
    <template v-if="products.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("products.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("products.sort.options.Sku") }}</th>
            <th scope="col">{{ t("departments.select.label") }}</th>
            <th scope="col">{{ t("flags") }}</th>
            <th scope="col">{{ t("products.sort.options.UnitPrice") }}</th>
            <th scope="col">{{ t("products.unitType.label") }}</th>
            <th scope="col">{{ t("products.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="product in products" :key="product.id">
            <td>
              <RouterLink :to="{ name: 'ProductEdit', params: { id: product.id } }">
                <font-awesome-icon icon="fas fa-edit" />{{ product.displayName ?? product.article.displayName }}
              </RouterLink>
            </td>
            <td>{{ product.sku ?? "—" }}</td>
            <td>{{ product.department ? formatDepartment(product.department) : "—" }}</td>
            <td>{{ product.flags ?? "—" }}</td>
            <td>{{ product.unitPrice ? n(product.unitPrice, "currency") : "—" }}</td>
            <td>{{ product.unitType ? t(`products.unitType.options.${product.unitType}`) : "—" }}</td>
            <td><StatusBlock :actor="product.updatedBy" :date="product.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("products.empty") }}</p>
  </main>
</template>
