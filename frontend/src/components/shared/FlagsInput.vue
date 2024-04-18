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
    id="flags"
    label="flags"
    max="10"
    :model-value="modelValue"
    placeholder="flags"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
