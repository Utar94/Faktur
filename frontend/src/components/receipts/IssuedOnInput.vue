<script setup lang="ts">
import { TarInput, type InputOptions } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = withDefaults(defineProps<InputOptions>(), {
  floating: true,
  id: "issued-on",
  label: "receipts.issuedOn",
  max: () => {
    const date = new Date().toISOString();
    const index = date.lastIndexOf(":");
    return date.substring(0, index);
  },
  placeholder: "receipts.issuedOn",
  type: "datetime-local",
});

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <TarInput v-bind="props" :label="t(label)" :placeholder="t(placeholder)" @update:model-value="$emit('update:model-value', $event)" />
</template>
