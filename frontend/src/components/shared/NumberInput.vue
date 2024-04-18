<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";

defineProps<{
  modelValue?: string;
  required?: boolean | string;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="number"
    label="number"
    max="16"
    :model-value="modelValue"
    placeholder="number"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
