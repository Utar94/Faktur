<script setup lang="ts">
import { TarButton, TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import BannerSelect from "@/components/banners/BannerSelect.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import EmailInput from "@/components/shared/EmailInput.vue";
import NumberInput from "@/components/shared/NumberInput.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { EmailPayload, Store } from "@/types/stores";
import { createStore, readStore, replaceStore } from "@/api/stores";
import { handleErrorKey } from "@/inject/App";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const bannerId = ref<string>("");
const description = ref<string>("");
const displayName = ref<string>("");
const email = ref<EmailPayload>({ address: "", isVerified: false });
const hasLoaded = ref<boolean>(false);
const number = ref<string>("");
const store = ref<Store>();

const hasChanges = computed<boolean>(() => {
  return (
    displayName.value !== (store.value?.displayName ?? "") ||
    bannerId.value !== (store.value?.banner?.id ?? "") ||
    number.value !== (store.value?.number ?? "") ||
    description.value !== (store.value?.description ?? "") ||
    email.value.address !== (store.value?.email?.address ?? "")
  );
});

function setModel(model: Store): void {
  store.value = model;
  bannerId.value = model.banner?.id ?? "";
  description.value = model.description ?? "";
  displayName.value = model.displayName;
  email.value = { address: model.email?.address ?? "", isVerified: model.email?.isVerified ?? false };
  number.value = model.number ?? "";
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    if (store.value) {
      const updatedStore = await replaceStore(
        store.value.id,
        {
          bannerId: bannerId.value,
          number: number.value,
          displayName: displayName.value,
          description: description.value,
          email: email.value.address ? email.value : undefined,
        },
        store.value.version,
      );
      setModel(updatedStore);
      toasts.success("stores.updated");
    } else {
      const createdStore = await createStore({
        bannerId: bannerId.value,
        number: number.value,
        displayName: displayName.value,
        description: description.value,
      });
      setModel(createdStore);
      toasts.success("stores.created");
      router.replace({ name: "StoreEdit", params: { id: createdStore.id } });
    }
  } catch (e: unknown) {
    handleError(e);
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const store = await readStore(id);
      setModel(store);
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
      <h1>{{ store?.displayName ?? t("stores.title.new") }}</h1>
      <StatusDetail v-if="store" :aggregate="store" />
      <form @submit.prevent="onSubmit">
        <div class="mb-3">
          <TarButton
            class="me-1"
            :disabled="isSubmitting || !hasChanges"
            :icon="store ? 'fas fa-save' : 'fas fa-plus'"
            :loading="isSubmitting"
            :status="t('loading')"
            :text="t(store ? 'actions.save' : 'actions.create')"
            type="submit"
            :variant="store ? 'primary' : 'success'"
          />
          <TarButton class="ms-1" icon="fas fa-chevron-left" :text="t('actions.back')" :variant="hasChanges ? 'danger' : 'secondary'" @click="router.back()" />
        </div>
        <TarTabs>
          <TarTab active :title="t('general')">
            <DisplayNameInput required v-model="displayName" />
            <div class="row">
              <BannerSelect class="col-lg-6" v-model="bannerId" />
              <NumberInput class="col-lg-6" v-model="number" />
            </div>
            <DescriptionTextarea v-model="description" />
          </TarTab>
          <TarTab :disabled="!store" :title="t('stores.contact')">
            <EmailInput v-model="email.address" />
          </TarTab>
        </TarTabs>
      </form>
    </template>
  </main>
</template>
