<script setup lang="ts">
import { TarAlert } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBackButton from "@/components/shared/AppBackButton.vue";
import AppDelete from "@/components/shared/AppDelete.vue";
import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import GtinInput from "@/components/articles/GtinInput.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError, PropertyError } from "@/types/api";
import type { Article } from "@/types/articles";
import { createArticle, deleteArticle, readArticle, replaceArticle } from "@/api/articles";
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
const gtinAlreadyUsed = ref<boolean>(false);
const hasLoaded = ref<boolean>(false);
const isDeleting = ref<boolean>(false);

const hasChanges = computed<boolean>(
  () =>
    displayName.value !== (article.value?.displayName ?? "") ||
    gtin.value !== (article.value?.gtin ?? "") ||
    description.value !== (article.value?.description ?? ""),
);

async function onDelete(hideModal: () => void): Promise<void> {
  if (article.value && !isDeleting.value) {
    isDeleting.value = true;
    try {
      await deleteArticle(article.value.id);
      hideModal();
      toasts.success("articles.delete.success");
      router.push({ name: "ArticleList" });
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isDeleting.value = false;
    }
  }
}

function setModel(model: Article): void {
  article.value = model;
  description.value = model.description ?? "";
  displayName.value = model.displayName;
  gtin.value = model.gtin ?? "";
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  gtinAlreadyUsed.value = false;
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
    const { data, status } = e as ApiError;
    if (status === 409 && (data as PropertyError)?.code === "GtinAlreadyUsed") {
      gtinAlreadyUsed.value = true;
    } else {
      handleError(e);
    }
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
      <TarAlert :close="t('actions.close')" dismissible variant="warning" v-model="gtinAlreadyUsed">
        <strong>{{ t("articles.gtin.alreadyUsed.lead") }}</strong> {{ t("articles.gtin.alreadyUsed.help") }}
      </TarAlert>
      <StatusDetail v-if="article" :aggregate="article" />
      <form @submit.prevent="onSubmit">
        <div class="mb-3">
          <AppSaveButton class="me-1" :disabled="isSubmitting || !hasChanges" :exists="Boolean(article)" :loading="isSubmitting" />
          <AppBackButton class="mx-1" :has-changes="hasChanges" />
          <AppDelete
            v-if="article"
            class="ms-1"
            confirm="articles.delete.confirm"
            :displayName="article.displayName"
            :loading="isDeleting"
            title="articles.delete.title"
            @confirmed="onDelete"
          />
        </div>
        <div class="row">
          <DisplayNameInput class="col-lg-6" required v-model="displayName" />
          <GtinInput class="col-lg-6" v-model="gtin" />
        </div>
        <DescriptionTextarea v-model="description" />
      </form>
    </template>
  </main>
</template>
