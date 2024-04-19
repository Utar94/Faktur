<script setup lang="ts">
import { TarTextarea, parsingUtils, type TextareaOptions, type TextareaStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { nanoid } from "nanoid";
import { useField } from "vee-validate";
import { useI18n } from "vue-i18n";

import type { ValidationListeners, ValidationRules, ValidationType } from "@/types/validation";
import { isEmpty } from "@/helpers/objectUtils";

const { parseBoolean, parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<
    TextareaOptions & {
      rules?: ValidationRules;
      validation?: ValidationType;
    }
  >(),
  {
    id: () => nanoid(),
    validation: "client",
  },
);

const textareaRef = ref<InstanceType<typeof TarTextarea> | null>(null);

const describedBy = computed<string>(() => `${props.id}_invalid-feedback`);
const inputName = computed<string>(() => props.name ?? props.id);

const validationRules = computed<ValidationRules>(() => {
  const rules: ValidationRules = {};
  if (props.validation === "server") {
    return rules;
  }

  const required: boolean | undefined = parseBoolean(props.required);
  if (required) {
    rules.required = true;
  }

  const max: number | undefined = parseNumber(props.max);
  if (max) {
    rules.max_length = max;
  }
  const min: number | undefined = parseNumber(props.min);
  if (min) {
    rules.min_length = min;
  }

  return { ...rules, ...props.rules };
});
const displayLabel = computed<string>(() => (props.label ? t(props.label).toLowerCase() : inputName.value));
const { errorMessage, handleChange, meta, value } = useField<string>(inputName, validationRules, {
  initialValue: props.modelValue,
  label: displayLabel,
});
const status = computed<TextareaStatus | undefined>(() => {
  if (isEmpty(validationRules.value) || (!meta.dirty && !meta.touched)) {
    return undefined;
  }
  return meta.valid ? "valid" : "invalid";
});
const validationListeners = computed<ValidationListeners>(() => ({
  blur: handleChange,
  change: handleChange,
  input: errorMessage.value ? handleChange : (e: unknown) => handleChange(e, false),
}));

function focus(): void {
  textareaRef.value?.focus();
}
defineExpose({ focus });
</script>

<template>
  <TarTextarea
    :cols="cols"
    :described-by="describedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label ? t(label) : undefined"
    :max="validation === 'server' ? max : undefined"
    :min="validation === 'server' ? min : undefined"
    :model-value="value"
    :name="name"
    :placeholder="placeholder ? t(placeholder) : undefined"
    :plaintext="plaintext"
    :readonly="readonly"
    ref="textareaRef"
    :required="parseBoolean(required) ? (validation === 'server' ? true : 'label') : undefined"
    :rows="rows"
    :size="size"
    :status="status"
    v-on="validationListeners"
  >
    <template #after>
      <div v-if="errorMessage" class="invalid-feedback" :id="describedBy">{{ errorMessage }}</div>
      <slot name="after"></slot>
    </template>
  </TarTextarea>
</template>
