<script setup lang="ts">
import { TarTextarea, type TextareaOptions } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = withDefaults(defineProps<TextareaOptions>(), {
  floating: true,
  id: "receipt-lines",
  label: "receipts.lines",
  placeholder: "receipts.lines",
  rows: 15,
});

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <TarTextarea v-bind="props" :label="t(label)" :placeholder="t(placeholder)" @update:model-value="$emit('update:model-value', $event)" />
</template>
