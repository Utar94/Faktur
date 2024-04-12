<script setup lang="ts">
import { TarTextarea, type TextareaOptions } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = withDefaults(defineProps<TextareaOptions>(), {
  floating: true,
  id: "street-address",
  label: "users.address.street",
  max: 255,
  placeholder: "users.address.street",
  rows: 3,
});

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <TarTextarea v-bind="props" :label="t(label)" :placeholder="t(placeholder)" @update:model-value="$emit('update:model-value', $event)" />
</template>
