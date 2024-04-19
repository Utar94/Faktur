<script setup lang="ts">
import { parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { computed } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { Department } from "@/types/departments";
import type { Store } from "@/types/stores";
import { orderBy } from "@/helpers/arrayUtils";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  modelValue?: string;
  noStatus?: boolean | string;
  store: Store;
}>();
const options = computed<SelectOption[]>(() =>
  orderBy(
    props.store.departments.map(({ displayName, number }) => ({ text: displayName, value: number })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "selected", value?: Department): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(number?: string): void {
  emit("update:model-value", number);
  if (number) {
    const department = props.store.departments.find((department) => department.number === number);
    emit("selected", department);
  } else {
    emit("selected");
  }
}
</script>

<template>
  <AppSelect
    floating
    id="department"
    label="departments.select.label"
    :model-value="modelValue"
    :options="options"
    placeholder="departments.select.placeholder"
    :show-status="parseBoolean(noStatus) ? 'never' : undefined"
    @update:model-value="onModelValueUpdate"
  />
</template>
