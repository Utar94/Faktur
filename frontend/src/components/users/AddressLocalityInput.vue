<script setup lang="ts">
import { TarInput, type InputOptions } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = withDefaults(defineProps<InputOptions>(), {
  floating: true,
  id: "address-locality",
  label: "users.address.locality",
  max: 255,
  placeholder: "users.address.locality",
});

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <TarInput v-bind="props" :label="t(label)" :placeholder="t(placeholder)" @update:model-value="$emit('update:model-value', $event)" />
</template>
