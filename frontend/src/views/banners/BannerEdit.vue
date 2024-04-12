<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { Banner } from "@/types/banners";
import { createBanner, readBanner, replaceBanner } from "@/api/banners";
import { handleErrorKey } from "@/inject/App";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const banner = ref<Banner>();
const description = ref<string>("");
const displayName = ref<string>("");
const hasLoaded = ref<boolean>(false);

const hasChanges = computed<boolean>(() => {
  return displayName.value !== (banner.value?.displayName ?? "") || description.value !== (banner.value?.description ?? "");
});

function setModel(model: Banner): void {
  banner.value = model;
  description.value = model.description ?? "";
  displayName.value = model.displayName;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    if (banner.value) {
      const updatedBanner = await replaceBanner(
        banner.value.id,
        {
          displayName: displayName.value,
          description: description.value,
        },
        banner.value.version,
      );
      setModel(updatedBanner);
      toasts.success("banners.updated");
    } else {
      const createdBanner = await createBanner({
        displayName: displayName.value,
        description: description.value,
      });
      setModel(createdBanner);
      toasts.success("banners.created");
      router.replace({ name: "BannerEdit", params: { id: createdBanner.id } });
    }
  } catch (e: unknown) {
    handleError(e);
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const banner = await readBanner(id);
      setModel(banner);
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
      <h1>{{ banner?.displayName ?? t("banners.title.new") }}</h1>
      <StatusDetail v-if="banner" :aggregate="banner" />
      <form @submit.prevent="onSubmit">
        <div class="mb-3">
          <TarButton
            class="me-1"
            :disabled="isSubmitting || !hasChanges"
            :icon="banner ? 'fas fa-save' : 'fas fa-plus'"
            :loading="isSubmitting"
            :status="t('loading')"
            :text="t(banner ? 'actions.save' : 'actions.create')"
            type="submit"
            :variant="banner ? 'primary' : 'success'"
          />
          <TarButton class="ms-1" icon="fas fa-chevron-left" :text="t('actions.back')" :variant="hasChanges ? 'danger' : 'secondary'" @click="router.back()" />
        </div>
        <DisplayNameInput required v-model="displayName" />
        <DescriptionTextarea v-model="description" />
      </form>
    </template>
  </main>
</template>
