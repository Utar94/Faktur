<script setup lang="ts">
import { TarSelect, parsingUtils, type SelectOptions } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(defineProps<SelectOptions>(), {
  floating: true,
  id: "count",
  label: "count",
  options: () => [{ text: "10" }, { text: "25" }, { text: "50" }, { text: "100" }],
});

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <TarSelect v-bind="props" :label="t(label)" :options="options" @update:model-value="$emit('update:model-value', parseNumber($event))" />
</template>
