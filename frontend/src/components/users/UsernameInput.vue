<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";

defineProps<{
  disabled?: boolean | string;
  modelValue?: string;
  required?: boolean | string;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppInput
    :disabled="disabled"
    floating
    id="username"
    label="users.username"
    max="255"
    :model-value="modelValue"
    placeholder="users.username"
    :required="required"
    :rules="{ allowed_characters: 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+' }"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
