<script setup lang="ts">
import { TarButton, TarInput, TarPagination, TarSelect, type SelectOption } from "logitar-vue3-ui";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Banner, BannerSort, SearchBannersPayload } from "@/types/banners";
import { deleteBanner, searchBanners } from "@/api/banners";
import { handleErrorKey } from "@/inject/App";
import { isEmpty } from "@/helpers/objectUtils";
import { orderBy } from "@/helpers/arrayUtils";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { rt, t, tm } = useI18n();

const banners = ref<Banner[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => Number(route.query.count) || 10);
const isDescending = computed<boolean>(() => route.query.isDescending === "true");
const page = computed<number>(() => Number(route.query.page) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("banners.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const parameters: SearchBannersPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as BannerSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const data = await searchBanners(parameters);
    if (now === timestamp.value) {
      banners.value = data.items;
      total.value = data.total;
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    if (now === timestamp.value) {
      isLoading.value = false;
    }
  }
}

async function onDelete(banner: Banner, hideModal: () => void): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      await deleteBanner(banner.id);
      hideModal();
      // toasts.success("banners.delete.success"); // TODO(fpion): toasts
    } catch (e: unknown) {
      handleError(e);
      return;
    } finally {
      isLoading.value = false;
    }
    await refresh();
  }
}

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "BannerList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                search: "",
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
    <h1>{{ t("banners.title.list") }}</h1>
    <div class="my-2">
      <TarButton
        class="me-1"
        :disabled="isLoading"
        icon="fas fa-rotate"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('actions.refresh')"
        @click="refresh()"
      />
      <RouterLink :to="{ name: 'CreateBanner' }" class="btn btn-success ms-1"><font-awesome-icon icon="fas fa-plus" /> {{ t("actions.create") }}</RouterLink>
    </div>
    <div class="row">
      <TarInput
        class="col-lg-4"
        floating
        id="search"
        :label="t('search')"
        :model-value="search"
        :placeholder="t('search')"
        type="search"
        @update:model-value="setQuery('search', $event)"
      />
      <TarSelect
        class="col-lg-4"
        floating
        id="sort"
        :label="t('sort')"
        :model-value="sort"
        :options="sortOptions"
        @update:model-value="setQuery('sort', $event)"
      />
      <!-- :descending="isDescending" :placeholder="t('sort')" @descending="setQuery('isDescending', $event)" -->
      <TarSelect
        class="col-lg-4"
        floating
        id="count"
        :label="t('count')"
        :model-value="count"
        :options="[{ text: '10' }, { text: '25' }, { text: '50' }, { text: '100' }]"
        @update:model-value="setQuery('count', $event)"
      />
    </div>
    <template v-if="banners.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("banners.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("banners.sort.options.UpdatedOn") }}</th>
            <th scope="col"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="banner in banners" :key="banner.id">
            <td>
              <RouterLink :to="{ name: 'BannerEdit', params: { id: banner.id } }">
                <font-awesome-icon icon="fas fa-edit" /> {{ banner.displayName }}
              </RouterLink>
            </td>
            <td><StatusBlock :actor="banner.updatedBy" :date="banner.updatedOn" /></td>
            <td>
              <TarButton
                :disabled="isLoading"
                icon="fas fa-trash"
                :text="t('actions.delete')"
                variant="danger"
                data-bs-toggle="modal"
                :data-bs-target="`#deleteModal_${banner.id}`"
              />
              <!-- <delete-modal
                confirm="banners.delete.confirm"
                :display-name="banner.displayName"
                :id="`deleteModal_${banner.id}`"
                :loading="isLoading"
                title="banners.delete.title"
                @ok="onDelete(banner, $event)"
              /> -->
            </td>
          </tr>
        </tbody>
      </table>
      <TarPagination
        aria-first="<<"
        aria-label="pagination"
        aria-last=">>"
        aria-next=">"
        aria-previous="<"
        :count="count"
        first="<<"
        last=">>"
        :model-value="page"
        next=">"
        pages="5"
        position="center"
        previous="<"
        :total="total"
        @update:model-value="setQuery('page', $event)"
      />
    </template>
    <p v-else>{{ t("banners.empty") }}</p>
  </main>
</template>
