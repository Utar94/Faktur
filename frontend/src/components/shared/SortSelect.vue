<script setup lang="ts">
import { TarCheckbox, type SelectOption } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import AppSelect from "./AppSelect.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    descending?: boolean | string;
    id?: string;
    modelValue?: string;
    options?: SelectOption[];
  }>(),
  {
    id: "sort",
  },
);

defineEmits<{
  (e: "descending", value: boolean): void;
  (e: "update:model-value", value?: string): void;
}>();

// TODO(fpion): always ignore validation
// TODO(fpion): sort resets when descending changes
</script>

<template>
  <AppSelect
    floating
    :id="id"
    label="sort.select.label"
    :model-value="modelValue"
    :options="options"
    placeholder="sort.select.placeholder"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #after>
      <TarCheckbox :id="`${id}_desc`" :label="t('sort.isDescending')" :model-value="descending" @update:model-value="$emit('descending', $event)" />
    </template>
  </AppSelect>
</template>
