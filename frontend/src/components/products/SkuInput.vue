<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";

defineProps<{
  modelValue?: string;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="sku"
    label="products.sku.label"
    max="32"
    :model-value="modelValue"
    placeholder="products.sku.label"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
