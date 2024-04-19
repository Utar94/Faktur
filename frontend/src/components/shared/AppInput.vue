<script setup lang="ts">
import { TarInput, inputUtils, parsingUtils, type InputOptions, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { nanoid } from "nanoid";
import { useField } from "vee-validate";
import { useI18n } from "vue-i18n";

import type { ValidationListeners, ValidationRules, ValidationType } from "@/types/validation";
import { isEmpty } from "@/helpers/objectUtils";

const { isDateTimeInput, isNumericInput, isTextualInput } = inputUtils;
const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<
    InputOptions & {
      rules?: ValidationRules;
      validation?: ValidationType;
    }
  >(),
  {
    id: () => nanoid(),
    validation: "client",
  },
);

const inputRef = ref<InstanceType<typeof TarInput> | null>(null);

const describedBy = computed<string>(() => `${props.id}_invalid-feedback`);
const inputMax = computed<number | string | undefined>(() => (props.validation === "server" || isDateTimeInput(props.type) ? props.max : undefined));
const inputMin = computed<number | string | undefined>(() => (props.validation === "server" || isDateTimeInput(props.type) ? props.min : undefined));
const inputName = computed<string>(() => props.name ?? props.id);

const validationRules = computed<ValidationRules>(() => {
  const rules: ValidationRules = {};
  if (props.validation === "server") {
    return rules;
  }

  if (props.required) {
    rules.required = true;
  }

  if (isNumericInput(props.type)) {
    if (props.max) {
      rules.max_value = parseNumber(props.max);
    }
    if (props.min) {
      rules.min_value = parseNumber(props.min);
    }
  } else if (isTextualInput(props.type)) {
    if (props.max) {
      rules.max_length = parseNumber(props.max);
    }
    if (props.min) {
      rules.min_length = parseNumber(props.min);
    }
  }

  if (props.pattern) {
    rules.regex = props.pattern;
  }

  if (props.type === "email" || props.type === "url") {
    rules[props.type] = true;
  }

  return { ...rules, ...props.rules };
});
const displayLabel = computed<string>(() => (props.label ? t(props.label).toLowerCase() : inputName.value));
const { errorMessage, handleChange, meta, value } = useField<string>(inputName, validationRules, {
  initialValue: props.modelValue,
  label: displayLabel,
});
const status = computed<InputStatus | undefined>(() => {
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
  inputRef.value?.focus();
}
defineExpose({ focus });
</script>

<template>
  <TarInput
    :described-by="describedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label ? t(label) : undefined"
    :max="inputMax"
    :min="inputMin"
    :model-value="value"
    :name="name"
    :pattern="validation === 'server' ? pattern : undefined"
    :placeholder="placeholder ? t(placeholder) : undefined"
    :plaintext="plaintext"
    :readonly="readonly"
    ref="inputRef"
    :required="required ? (validation === 'server' ? required : 'label') : undefined"
    :size="size"
    :status="status"
    :step="step"
    :type="type"
    v-on="validationListeners"
  >
    <template #after>
      <div v-if="errorMessage" class="invalid-feedback" :id="describedBy">{{ errorMessage }}</div>
      <slot name="after"></slot>
    </template>
  </TarInput>
</template>
