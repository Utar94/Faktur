<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import GtinInput from "@/components/articles/GtinInput.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { Article } from "@/types/articles";
import { createArticle, readArticle, replaceArticle } from "@/api/articles";
import { handleErrorKey } from "@/inject/App";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const article = ref<Article>();
const description = ref<string>("");
const displayName = ref<string>("");
const gtin = ref<string>("");
const hasLoaded = ref<boolean>(false);

const hasChanges = computed<boolean>(() => {
  return (
    displayName.value !== (article.value?.displayName ?? "") ||
    gtin.value !== (article.value?.gtin ?? "") ||
    description.value !== (article.value?.description ?? "")
  );
});

function setModel(model: Article): void {
  article.value = model;
  description.value = model.description ?? "";
  displayName.value = model.displayName;
  gtin.value = model.gtin ?? "";
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    if (article.value) {
      const updatedArticle = await replaceArticle(
        article.value.id,
        {
          gtin: gtin.value,
          displayName: displayName.value,
          description: description.value,
        },
        article.value.version,
      );
      setModel(updatedArticle);
      toasts.success("articles.updated");
    } else {
      const createdArticle = await createArticle({
        gtin: gtin.value,
        displayName: displayName.value,
        description: description.value,
      });
      setModel(createdArticle);
      toasts.success("articles.created");
      router.replace({ name: "ArticleEdit", params: { id: createdArticle.id } });
    }
  } catch (e: unknown) {
    handleError(e);
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const article = await readArticle(id);
      setModel(article);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
  hasLoaded.value = true;
});
</script>

<template>
  <main class="container">
    <template v-if="hasLoaded">
      <h1>{{ article?.displayName ?? t("articles.title.new") }}</h1>
      <StatusDetail v-if="article" :aggregate="article" />
      <form @submit.prevent="onSubmit">
        <div class="mb-3">
          <TarButton
            class="me-1"
            :disabled="isSubmitting || !hasChanges"
            :icon="article ? 'fas fa-save' : 'fas fa-plus'"
            :loading="isSubmitting"
            :status="t('loading')"
            :text="t(article ? 'actions.save' : 'actions.create')"
            type="submit"
            :variant="article ? 'primary' : 'success'"
          />
          <TarButton class="ms-1" icon="fas fa-chevron-left" :text="t('actions.back')" :variant="hasChanges ? 'danger' : 'secondary'" @click="router.back()" />
        </div>
        <DisplayNameInput required v-model="displayName" />
        <GtinInput v-model="gtin" />
        <DescriptionTextarea v-model="description" />
      </form>
    </template>
  </main>
</template>
