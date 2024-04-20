<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "sku",
    label: "products.sku.label",
  },
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppInput
    floating
    :id="id"
    :label="label"
    max="32"
    :model-value="modelValue"
    :placeholder="label"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
