<script setup lang="ts">
import { TarSelect, type SelectOption, type SelectOptions } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Article } from "@/types/articles";
import { handleErrorKey } from "@/inject/App";
import { searchArticles } from "@/api/articles";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

const articles = ref<Article[]>([]);

const options = computed<SelectOption[]>(() => articles.value.map(({ id, displayName }) => ({ value: id, text: displayName })));

const props = withDefaults(defineProps<SelectOptions>(), {
  ariaLabel: "articles.select.ariaLabel",
  floating: true,
  id: "article",
  label: "articles.select.label",
  placeholder: "articles.select.placeholder",
});

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
    handleError(e);
  }
});

const emit = defineEmits<{
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
