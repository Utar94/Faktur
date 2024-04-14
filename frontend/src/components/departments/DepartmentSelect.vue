<script setup lang="ts">
import { TarSelect, type SelectOption, type SelectOptions } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { Department } from "@/types/departments";
import type { Store } from "@/types/stores";
import { orderBy } from "@/helpers/arrayUtils";

const { t } = useI18n();

const props = withDefaults(
  defineProps<
    SelectOptions & {
      store: Store;
    }
  >(),
  {
    ariaLabel: "departments.select.ariaLabel",
    floating: true,
    id: "department",
    label: "departments.select.label",
    placeholder: "departments.select.placeholder",
  },
);

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
  <TarSelect
    v-bind="props"
    :aria-label="t(ariaLabel)"
    :label="t(label)"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="onModelValueUpdate"
  />
</template>
