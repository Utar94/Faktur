<script setup lang="ts">
import AppInput from "./AppInput.vue";

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
    id="flag"
    label="flag"
    max="1"
    :model-value="modelValue"
    placeholder="flag"
    validation="server"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
