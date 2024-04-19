<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { Article } from "@/types/articles";
import { searchArticles } from "@/api/articles";

defineProps<{
  disabled?: boolean | string;
  modelValue?: string;
  required?: boolean | string;
}>();

const articles = ref<Article[]>([]);

const options = computed<SelectOption[]>(() => articles.value.map(({ id, displayName }) => ({ value: id, text: displayName })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value?: Article): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id?: string): void {
  emit("update:model-value", id);
  if (id) {
    const article = articles.value.find((article) => article.id === id);
    emit("selected", article);
  } else {
    emit("selected");
  }
}

onMounted(async () => {
  try {
    const results = await searchArticles({
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
    articles.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <AppSelect
    :disabled="disabled"
    floating
    id="article"
    label="articles.select.label"
    :model-value="modelValue"
    :options="options"
    placeholder="articles.select.placeholder"
    :required="required"
    @update:model-value="onModelValueUpdate"
  />
</template>
