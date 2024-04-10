<script setup lang="ts">
import { TarCheckbox, TarSelect, type SelectOption } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    ariaLabel?: string;
    descending?: boolean;
    floating?: boolean;
    id?: string;
    label?: string;
    modelValue?: string;
    options?: SelectOption[];
    placeholder?: string;
  }>(),
  {
    ariaLabel: "sort.select.ariaLabel",
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
  <TarSelect
    :aria-label="t(ariaLabel)"
    :floating="floating"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #after>
      <TarCheckbox
        :ariaLabel="t('sort.isDescending.ariaLabel')"
        :id="`${id}_desc`"
        :label="t('sort.isDescending.label')"
        :model-value="descending"
        @update:model-value="$emit('descending', $event)"
      />
    </template>
  </TarSelect>
</template>
