<script setup lang="ts">
import { TarSelect, type SelectOption, type SelectOptions } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Store } from "@/types/stores";
import { handleErrorKey } from "@/inject/App";
import { searchStores } from "@/api/stores";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

const stores = ref<Store[]>([]);

const options = computed<SelectOption[]>(() => stores.value.map(({ id, displayName }) => ({ value: id, text: displayName })));

const props = withDefaults(defineProps<SelectOptions>(), {
  ariaLabel: "stores.select.ariaLabel",
  floating: true,
  id: "store",
  label: "stores.select.label",
  placeholder: "stores.select.placeholder",
});

onMounted(async () => {
  try {
    const results = await searchStores({
      ids: [],
      search: {
        terms: [],
        operator: "And",
      },
      sort: [
        {
          field: "DisplayName",
          isDescending: false,
        },
      ],
      skip: 0,
      limit: 0,
    });
    stores.value = results.items;
  } catch (e: unknown) {
    handleError(e);
  }
});

const emit = defineEmits<{
  (e: "selected", value?: Store): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id?: string): void {
  emit("update:model-value", id);
  if (id) {
    const store = stores.value.find((store) => store.id === id);
    emit("selected", store);
  } else {
    emit("selected");
  }
}
</script>

<template>
  <TarSelect
    v-bind="props"
    :aria-label="t(ariaLabel)"
    :label="t(label)"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="onModelValueUpdate"
  />
</template>
