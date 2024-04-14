<script setup lang="ts">
import { TarCheckbox, TarSelect, type SelectOptions } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = withDefaults(
  defineProps<
    SelectOptions & {
      descending?: boolean;
    }
  >(),
  {
    descending: false,
    floating: true,
    id: "sort",
    label: "sort.select.label",
    placeholder: "sort.select.placeholder",
  },
);

defineEmits<{
  (e: "descending", value: boolean): void;
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <TarSelect v-bind="props" :label="t(label)" :placeholder="t(placeholder)" @update:model-value="$emit('update:model-value', $event)">
    <template #after>
      <TarCheckbox :id="`${id}_desc`" :label="t('sort.isDescending')" :model-value="descending" @update:model-value="$emit('descending', $event)" />
    </template>
  </TarSelect>
</template>
