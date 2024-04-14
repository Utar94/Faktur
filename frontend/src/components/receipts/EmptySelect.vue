<script setup lang="ts">
import { TarSelect, type SelectOption, type SelectOptions } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import { orderBy } from "@/helpers/arrayUtils";

const { t } = useI18n();

const props = withDefaults(defineProps<SelectOptions>(), {
  floating: true,
  id: "is-empty",
  label: "receipts.isEmpty.label",
  placeholder: "receipts.isEmpty.placeholder",
});

const options = computed<SelectOption[]>(() =>
  orderBy(
    [
      {
        text: t("yes"),
        value: "true",
      },
      {
        text: t("no"),
        value: "false",
      },
    ],
    "text",
  ),
);
</script>

<template>
  <TarSelect v-bind="props" :label="t(label)" :options="options" :placeholder="t(placeholder)" @update:model-value="$emit('update:model-value', $event)" />
</template>
