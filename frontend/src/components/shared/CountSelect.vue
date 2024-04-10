<script setup lang="ts">
import { TarSelect, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    ariaLabel?: string;
    floating?: boolean;
    id?: string;
    label?: string;
    modelValue?: number;
    options?: SelectOption[];
    placeholder?: string;
  }>(),
  {
    ariaLabel: "count.ariaLabel",
    floating: true,
    id: "count",
    label: "count.label",
    options: () => [{ text: "10" }, { text: "25" }, { text: "50" }, { text: "100" }],
  },
);

defineEmits<{
  (e: "update:model-value", value?: number): void;
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
    :placeholder="placeholder"
    @update:model-value="$emit('update:model-value', parsingUtils.parseNumber($event))"
  />
</template>
