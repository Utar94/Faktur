<script setup lang="ts">
import { TarInput } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    floating?: boolean;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    floating: true,
    id: "search",
    label: "search",
    placeholder: "search",
  },
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <TarInput
    :floating="floating"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :placeholder="t(placeholder)"
    type="search"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
