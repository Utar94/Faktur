<script setup lang="ts">
import AppInput from "./AppInput.vue";

defineProps<{
  modelValue?: string;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();

// TODO(fpion): always ignore validation
</script>

<template>
  <AppInput floating id="flag" label="flag" max="1" :model-value="modelValue" placeholder="flag" @update:model-value="$emit('update:model-value', $event)" />
</template>
