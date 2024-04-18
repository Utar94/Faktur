<script setup lang="ts">
import { TarSelect, parsingUtils, type SelectOption, type SelectSize } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    ariaLabel?: string;
    describedBy?: string;
    disabled?: boolean | string;
    floating?: boolean | string;
    id?: string;
    label?: string;
    modelValue?: number;
    multiple?: boolean | string;
    name?: string;
    options?: SelectOption[];
    placeholder?: string;
    required?: boolean | string;
    size?: SelectSize;
  }>(),
  {
    floating: true,
    id: "count",
    label: "count",
    options: () => [{ text: "10" }, { text: "25" }, { text: "50" }, { text: "100" }],
  },
);

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <TarSelect
    v-bind="props"
    :aria-label="ariaLabel ? t(ariaLabel) : undefined"
    :label="t(label)"
    :placeholder="placeholder ? t(placeholder) : undefined"
    @update:model-value="$emit('update:model-value', parseNumber($event))"
  />
</template>
