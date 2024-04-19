<script setup lang="ts">
import { TarButton, TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AddressCountrySelect from "@/components/users/AddressCountrySelect.vue";
import AddressLocalityInput from "@/components/users/AddressLocalityInput.vue";
import AddressPostalCodeInput from "@/components/users/AddressPostalCodeInput.vue";
import AddressRegionSelect from "@/components/users/AddressRegionSelect.vue";
import AddressStreetTextarea from "@/components/users/AddressStreetTextarea.vue";
import AppBackButton from "@/components/shared/AppBackButton.vue";
import AppDelete from "@/components/shared/AppDelete.vue";
import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import BannerSelect from "@/components/banners/BannerSelect.vue";
import DepartmentList from "@/components/departments/DepartmentList.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import EmailAddressInput from "@/components/users/EmailAddressInput.vue";
import NumberInput from "@/components/shared/NumberInput.vue";
import PhoneExtensionInput from "@/components/users/PhoneExtensionInput.vue";
import PhoneNumberInput from "@/components/users/PhoneNumberInput.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import countries from "@/resources/countries.json";
import type { AddressPayload, CountrySettings, EmailPayload, PhonePayload } from "@/types/users";
import type { ApiError } from "@/types/api";
import type { Store } from "@/types/stores";
import { createStore, deleteStore, readStore, replaceStore } from "@/api/stores";
import { handleErrorKey } from "@/inject/App";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const address = ref<AddressPayload>({ street: "", locality: "", country: "", isVerified: false });
const bannerId = ref<string>("");
const country = ref<CountrySettings>();
const description = ref<string>("");
const displayName = ref<string>("");
const email = ref<EmailPayload>({ address: "", isVerified: false });
const hasLoaded = ref<boolean>(false);
const isDeleting = ref<boolean>(false);
const number = ref<string>("");
const phone = ref<PhonePayload>({ number: "", isVerified: false });
const store = ref<Store>();

const hasChanges = computed<boolean>(
  () =>
    displayName.value !== (store.value?.displayName ?? "") ||
    bannerId.value !== (store.value?.banner?.id ?? "") ||
    number.value !== (store.value?.number ?? "") ||
    description.value !== (store.value?.description ?? "") ||
    address.value.street !== (store.value?.address?.street ?? "") ||
    address.value.locality !== (store.value?.address?.locality ?? "") ||
    (address.value.postalCode ?? "") !== (store.value?.address?.postalCode ?? "") ||
    address.value.country !== (store.value?.address?.country ?? "") ||
    (address.value.region ?? "") !== (store.value?.address?.region ?? "") ||
    email.value.address !== (store.value?.email?.address ?? "") ||
    (phone.value.countryCode ?? "") !== (store.value?.phone?.countryCode ?? "") ||
    phone.value.number !== (store.value?.phone?.number ?? "") ||
    (phone.value.extension ?? "") !== (store.value?.phone?.extension ?? ""),
);
const isAddressRequired = computed<boolean>(() =>
  Boolean(address.value.street || address.value.locality || address.value.postalCode || address.value.region || address.value.country),
);

async function onDelete(hideModal: () => void): Promise<void> {
  if (store.value && !isDeleting.value) {
    isDeleting.value = true;
    try {
      await deleteStore(store.value.id);
      hideModal();
      toasts.success("stores.delete.success");
      router.push({ name: "StoreList" });
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isDeleting.value = false;
    }
  }
}

function setModel(model: Store): void {
  store.value = model;
  address.value = {
    street: model.address?.street ?? "",
    locality: model.address?.locality ?? "",
    postalCode: model.address?.postalCode,
    region: model.address?.region,
    country: model.address?.country ?? "",
    isVerified: model.address?.isVerified ?? false,
  };
  bannerId.value = model.banner?.id ?? "";
  description.value = model.description ?? "";
  displayName.value = model.displayName;
  email.value = { address: model.email?.address ?? "", isVerified: model.email?.isVerified ?? false };
  number.value = model.number ?? "";
  phone.value = {
    countryCode: model.phone?.countryCode,
    number: model.phone?.number ?? "",
    extension: model.phone?.extension,
    isVerified: model.phone?.isVerified ?? false,
  };
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    if (store.value) {
      const updatedStore = await replaceStore(
        store.value.id,
        {
          bannerId: bannerId.value || undefined,
          number: number.value,
          displayName: displayName.value,
          description: description.value,
          address: address.value.street ? address.value : undefined,
          email: email.value.address ? email.value : undefined,
          phone: phone.value.number ? { ...phone.value, extension: phone.value.extension?.trim() ? phone.value.extension : undefined } : undefined,
        },
        store.value.version,
      );
      setModel(updatedStore);
      toasts.success("stores.updated");
    } else {
      const createdStore = await createStore({
        bannerId: bannerId.value || undefined,
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

function clearAddress(): void {
  address.value = {
    street: "",
    locality: "",
    postalCode: undefined,
    country: "",
    region: undefined,
    isVerified: false,
  };
  country.value = undefined;
}

onMounted(async () => {
  try {
    bannerId.value = route.query.bannerId?.toString() ?? "";
    const id = route.params.id?.toString();
    if (id) {
      const store = await readStore(id);
      setModel(store);
      if (store.address) {
        country.value = countries.find(({ code }) => code === store.address?.country);
      }
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
          <AppSaveButton class="me-1" :disabled="isSubmitting || !hasChanges" :exists="Boolean(store)" :loading="isSubmitting" />
          <AppBackButton class="mx-1" :has-changes="hasChanges" />
          <AppDelete
            v-if="store"
            class="ms-1"
            confirm="stores.delete.confirm"
            :displayName="store.displayName"
            :loading="isDeleting"
            title="stores.delete.title"
            @confirmed="onDelete"
          />
        </div>
        <TarTabs>
          <TarTab active :title="t('general')">
            <DisplayNameInput required v-model="displayName" />
            <div class="row">
              <BannerSelect class="col-lg-6" v-model="bannerId" @error="handleError" />
              <NumberInput class="col-lg-6" v-model="number" />
            </div>
            <DescriptionTextarea v-model="description" />
          </TarTab>
          <TarTab :disabled="!store" :title="t('stores.contact')">
            <EmailAddressInput v-model="email.address" />
            <div class="row">
              <PhoneNumberInput class="col-lg-6" :required="Boolean(phone.extension)" v-model="phone.number" />
              <PhoneExtensionInput class="col-lg-6" v-model="phone.extension" />
            </div>
            <h5>{{ t("users.address.title") }}</h5>
            <AddressStreetTextarea :required="isAddressRequired" v-model="address.street" />
            <div class="row">
              <AddressLocalityInput class="col-lg-6" :required="isAddressRequired" v-model="address.locality" />
              <AddressPostalCodeInput class="col-lg-6" :country="country" :required="Boolean(country?.postalCode)" v-model="address.postalCode" />
            </div>
            <div class="row">
              <AddressCountrySelect class="col-lg-6" :required="isAddressRequired" v-model="address.country" @selected="country = $event" />
              <AddressRegionSelect class="col-lg-6" :country="country" :required="Boolean(country?.regions?.length)" v-model="address.region" />
            </div>
            <TarButton :disabled="!isAddressRequired" icon="fas fa-times" :text="t('actions.clear')" variant="warning" @click="clearAddress" />
          </TarTab>
          <TarTab :disabled="!store" :title="t('departments.title.list')">
            <DepartmentList v-if="store" :store="store" @error="handleError" />
          </TarTab>
        </TarTabs>
      </form>
    </template>
  </main>
</template>
