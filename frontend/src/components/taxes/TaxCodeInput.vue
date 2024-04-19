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
    id="code"
    label="taxes.code.label"
    max="4"
    :model-value="modelValue"
    placeholder="taxes.code.label"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
