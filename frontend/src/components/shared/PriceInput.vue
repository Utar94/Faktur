<script setup lang="ts">
import { TarInput, parsingUtils, type InputOptions } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
const { parseNumber } = parsingUtils;

const props = withDefaults(defineProps<InputOptions>(), {
  floating: true,
  id: "price",
  label: "price",
  min: 0.0,
  placeholder: "price",
  step: 0.01,
  type: "number",
});

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <TarInput v-bind="props" :label="t(label)" :placeholder="t(placeholder)" @update:model-value="$emit('update:model-value', parseNumber($event))">
    <template #append>
      <span class="input-group-text">
        <font-awesome-icon icon="fas fa-dollar-sign" />
      </span>
    </template>
  </TarInput>
</template>
