<script setup lang="ts">
import { TarAlert } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBackButton from "@/components/shared/AppBackButton.vue";
import AppDelete from "@/components/shared/AppDelete.vue";
import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import ArticleSelect from "@/components/articles/ArticleSelect.vue";
import DepartmentSelect from "@/components/departments/DepartmentSelect.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import FlagsInput from "@/components/shared/FlagsInput.vue";
import PriceInput from "@/components/shared/PriceInput.vue";
import SkuInput from "@/components/products/SkuInput.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import StoreSelect from "@/components/stores/StoreSelect.vue";
import UnitTypeSelect from "@/components/products/UnitTypeSelect.vue";
import type { ApiError, PropertyError } from "@/types/api";
import type { Article } from "@/types/articles";
import type { Department } from "@/types/departments";
import type { Product, UnitType } from "@/types/products";
import type { Store } from "@/types/stores";
import { createOrReplaceProduct, deleteProduct, readProduct, readProductByArticle } from "@/api/products";
import { handleErrorKey } from "@/inject/App";
import { readStore } from "@/api/stores";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const article = ref<Article>();
const department = ref<Department>();
const description = ref<string>("");
const displayName = ref<string>("");
const flags = ref<string>("");
const hasLoaded = ref<boolean>(false);
const isDeleting = ref<boolean>(false);
const product = ref<Product>();
const productAlreadyExists = ref<boolean>(false);
const sku = ref<string>("");
const skuAlreadyUsed = ref<boolean>(false);
const store = ref<Store>();
const unitPrice = ref<number>(0);
const unitType = ref<UnitType>();

const hasChanges = computed<boolean>(
  () =>
    (store.value?.id ?? "") !== (product.value?.store.id ?? "") ||
    (article.value?.id ?? "") !== (product.value?.article.id ?? "") ||
    (department.value?.number ?? "") !== (product.value?.department?.number ?? "") ||
    sku.value !== (product.value?.sku ?? "") ||
    displayName.value !== (product.value?.displayName ?? "") ||
    flags.value !== (product.value?.flags ?? "") ||
    unitPrice.value !== (product.value?.unitPrice ?? 0) ||
    (unitType.value ?? "") !== (product.value?.unitType ?? "") ||
    description.value !== (product.value?.description ?? ""),
);

async function onDelete(hideModal: () => void): Promise<void> {
  if (product.value && !isDeleting.value) {
    isDeleting.value = true;
    try {
      await deleteProduct(product.value.id);
      hideModal();
      toasts.success("products.delete.success");
      router.push({ name: "ProductList" });
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isDeleting.value = false;
    }
  }
}

async function checkAlreadyExists(): Promise<void> {
  if (store.value && article.value) {
    try {
      await readProductByArticle(store.value.id, article.value.id);
      productAlreadyExists.value = true;
    } catch (e: unknown) {
      const { status } = e as ApiError;
      if (status === 404) {
        productAlreadyExists.value = false;
      } else {
        handleError(e);
      }
    }
  } else {
    productAlreadyExists.value = false;
  }
}
function onArticleSelected(selected?: Article): void {
  article.value = selected;
  checkAlreadyExists();
}
function onStoreSelected(selected?: Store): void {
  store.value = selected;
  department.value = undefined;
  checkAlreadyExists();
}

function setModel(model: Product): void {
  product.value = model;
  article.value = model.article;
  department.value = model.department;
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  flags.value = model.flags ?? "";
  sku.value = model.sku ?? "";
  unitPrice.value = model.unitPrice ?? 0;
  unitType.value = model.unitType;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  skuAlreadyUsed.value = false;
  try {
    if (store.value && article.value) {
      const savedProduct = await createOrReplaceProduct(
        store.value.id,
        article.value.id,
        {
          departmentNumber: department.value?.number,
          sku: sku.value,
          displayName: displayName.value,
          description: description.value,
          flags: flags.value,
          unitPrice: unitPrice.value || undefined,
          unitType: unitType.value || undefined,
        },
        product.value?.version,
      );
      if (product.value) {
        toasts.success("products.updated");
        setModel(savedProduct);
      } else {
        toasts.success("products.created");
        setModel(savedProduct);
        router.replace({ name: "ProductEdit", params: { id: savedProduct.id } });
      }
    }
  } catch (e: unknown) {
    const { data, status } = e as ApiError;
    if (status === 409 && (data as PropertyError)?.code === "SkuAlreadyUsed") {
      skuAlreadyUsed.value = true;
    } else {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    let storeId = route.query.storeId?.toString();
    if (id) {
      const product = await readProduct(id);
      storeId = product.store.id;
      setModel(product);
    } else {
      unitType.value = route.query.unitType?.toString() as UnitType;
    }
    if (storeId) {
      store.value = await readStore(storeId);
      const departmentNumber = route.query.departmentNumber?.toString();
      if (!id) {
        department.value = store.value.departments.find((department) => department.number === departmentNumber);
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
      <h1>{{ product?.displayName ?? product?.article.displayName ?? t("products.title.new") }}</h1>
      <TarAlert :close="t('actions.close')" dismissible variant="warning" v-model="productAlreadyExists">
        <strong>{{ t("products.alreadyExists.lead") }}</strong> {{ t("products.alreadyExists.help") }}
      </TarAlert>
      <TarAlert :close="t('actions.close')" dismissible variant="warning" v-model="skuAlreadyUsed">
        <strong>{{ t("products.sku.alreadyUsed.lead") }}</strong> {{ t("products.sku.alreadyUsed.help") }}
      </TarAlert>
      <StatusDetail v-if="product" :aggregate="product" />
      <form @submit.prevent="onSubmit">
        <div class="mb-3">
          <AppSaveButton class="me-1" :disabled="isSubmitting || !hasChanges || productAlreadyExists" :exists="Boolean(product)" :loading="isSubmitting" />
          <AppBackButton class="mx-1" :has-changes="hasChanges" />
          <AppDelete
            v-if="product"
            class="ms-1"
            confirm="products.delete.confirm"
            :displayName="product.displayName ?? product.article.displayName"
            :loading="isDeleting"
            title="products.delete.title"
            @confirmed="onDelete"
          />
        </div>
        <div class="row">
          <StoreSelect class="col-lg-6" :disabled="Boolean(product)" :model-value="store?.id" required @error="handleError" @selected="onStoreSelected" />
          <ArticleSelect class="col-lg-6" :disabled="Boolean(product)" :model-value="article?.id" required @error="handleError" @selected="onArticleSelected" />
        </div>
        <template v-if="store">
          <div class="row">
            <DepartmentSelect class="col-lg-6" :model-value="department?.number" :store="store" @selected="department = $event" />
            <SkuInput class="col-lg-6" v-model="sku" />
          </div>
          <div class="row">
            <DisplayNameInput class="col-lg-6" v-model="displayName" />
            <FlagsInput class="col-lg-6" v-model="flags" />
          </div>
          <div class="row">
            <PriceInput class="col-lg-6" id="unit-price" label="products.unitPrice" :model-value="unitPrice" @update:model-value="unitPrice = $event ?? 0" />
            <UnitTypeSelect class="col-lg-6" v-model="unitType" />
          </div>
          <DescriptionTextarea v-model="description" />
        </template>
      </form>
    </template>
  </main>
</template>
