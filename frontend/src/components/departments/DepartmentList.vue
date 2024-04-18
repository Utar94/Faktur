<script setup lang="ts">
import { TarButton, type SelectOption } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import AppDelete from "@/components/shared/AppDelete.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import DepartmentEdit from "./DepartmentEdit.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Department, DepartmentSort, SearchDepartmentsPayload } from "@/types/departments";
import type { Store } from "@/types/stores";
import { deleteDepartment, searchDepartments } from "@/api/departments";
import { formatDepartment } from "@/helpers/displayUtils";
import { orderBy } from "@/helpers/arrayUtils";
import { useToastStore } from "@/stores/toast";

const toasts = useToastStore();
const { rt, t, tm } = useI18n();

const props = defineProps<{
  store: Store;
}>();

const count = ref<number>(10);
const departments = ref<Department[]>([]);
const isDeleting = ref<string>();
const isDescending = ref<boolean>(true);
const isLoading = ref<boolean>(false);
const page = ref<number>(1);
const search = ref<string>("");
const sort = ref<DepartmentSort>("UpdatedOn");
const timestamp = ref<number>(0);
const total = ref<number>(0);

const payload = computed<SearchDepartmentsPayload>(() => ({
  ids: [],
  search: {
    terms: search.value
      .split(" ")
      .filter((term) => Boolean(term))
      .map((term) => ({ value: `%${term}%` })),
    operator: "And",
  },
  storeId: props.store.id,
  sort: sort.value ? [{ field: sort.value as DepartmentSort, isDescending: isDescending.value }] : [],
  skip: (page.value - 1) * count.value,
  limit: count.value,
}));
const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("departments.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

async function refresh(search?: SearchDepartmentsPayload): Promise<void> {
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchDepartments(search ?? payload.value);
    if (now === timestamp.value) {
      departments.value = results.items;
      total.value = results.total;
    }
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    if (now === timestamp.value) {
      isLoading.value = false;
    }
  }
}

async function onDelete(department: Department, hideModal: () => void): Promise<void> {
  if (department && !isDeleting.value) {
    isDeleting.value = department.number;
    try {
      await deleteDepartment(props.store.id, department.number);
      hideModal();
      toasts.success("departments.delete.success");
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isDeleting.value = undefined;
    }
    await refresh();
  }
}

function onSaved(department?: Department): void {
  toasts.success(department ? "departments.updated" : "departments.created");
  refresh();
}

watch(payload, (payload) => refresh(payload), { deep: true, immediate: true });
</script>

<template>
  <div>
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
      <DepartmentEdit :store="store" @error="$emit('error', $event)" @saved="onSaved()" />
    </div>
    <div class="row">
      <SearchInput class="col-lg-4" v-model="search" />
      <SortSelect class="col-lg-4" :descending="isDescending" :options="sortOptions" v-model="sort" @descending="isDescending = $event" />
      <CountSelect class="col-lg-4" :model-value="count" @update:model-value="count = $event ?? 0" />
    </div>
    <template v-if="departments.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("departments.sort.options.Number") }}</th>
            <th scope="col">{{ t("departments.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("departments.sort.options.UpdatedOn") }}</th>
            <th scope="col">{{ t("actions.title") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="department in departments" :key="department.number">
            <td>{{ department.number }}</td>
            <td>{{ department.displayName }}</td>
            <td><StatusBlock :actor="department.updatedBy" :date="department.updatedOn" /></td>
            <td>
              <DepartmentEdit class="me-1" :department="department" :store="store" @error="$emit('error', $event)" @saved="onSaved" />
              <AppDelete
                class="ms-1"
                confirm="departments.delete.confirm"
                :displayName="formatDepartment(department)"
                :id="department.number"
                :loading="isDeleting === department.number"
                title="departments.delete.title"
                @confirmed="onDelete(department, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :total="total" v-model="page" />
    </template>
    <p v-else>{{ t("departments.empty") }}</p>
  </div>
</template>
