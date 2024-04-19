<script setup lang="ts">
import AppInput from "./AppInput.vue";

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
    id="display-name"
    label="displayName"
    max="255"
    :model-value="modelValue"
    placeholder="displayName"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
