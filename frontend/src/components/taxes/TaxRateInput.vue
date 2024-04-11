<script setup lang="ts">
import { TarInput, parsingUtils, type InputOptions } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
const { parseNumber } = parsingUtils;

const props = withDefaults(defineProps<InputOptions>(), {
  floating: true,
  id: "rate",
  label: "taxes.rate",
  min: 0.00001,
  placeholder: "taxes.rate",
  step: 0.00001,
  type: "number",
});

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <TarInput v-bind="props" :label="t(label)" :placeholder="t(placeholder)" @update:model-value="$emit('update:model-value', parseNumber($event))" />
</template>
