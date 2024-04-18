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
    id="gtin"
    label="articles.gtin.label"
    max="14"
    :model-value="modelValue"
    placeholder="articles.gtin.label"
    :rules="{ allowed_characters: '0123456789' }"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
