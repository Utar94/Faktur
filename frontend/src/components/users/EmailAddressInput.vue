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
    id="email-address"
    label="users.email.address"
    max="255"
    :model-value="modelValue"
    placeholder="users.email.address"
    type="email"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
