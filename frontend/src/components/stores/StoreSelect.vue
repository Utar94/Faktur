<script setup lang="ts">
import { parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { Store } from "@/types/stores";
import { searchStores } from "@/api/stores";

const { parseBoolean } = parsingUtils;

defineProps<{
  disabled?: boolean | string;
  modelValue?: string;
  noStatus?: boolean | string;
  required?: boolean | string;
}>();

const stores = ref<Store[]>([]);

const options = computed<SelectOption[]>(() => stores.value.map(({ id, displayName }) => ({ value: id, text: displayName })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
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
    emit("error", e);
  }
});
</script>

<template>
  <AppSelect
    :disabled="disabled"
    floating
    id="store"
    label="stores.select.label"
    :model-value="modelValue"
    :options="options"
    placeholder="stores.select.placeholder"
    :required="required"
    :show-status="parseBoolean(noStatus) ? 'never' : undefined"
    @update:model-value="onModelValueUpdate"
  />
</template>
