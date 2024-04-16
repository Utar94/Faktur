<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppDelete from "@/components/shared/AppDelete.vue";
import CategoryEdit from "./CategoryEdit.vue";
import type { CategorySavedEvent } from "@/types/receipts";
import { useCategoryStore } from "@/stores/categories";
import { useToastStore } from "@/stores/toast";

const categories = useCategoryStore();
const toasts = useToastStore();
const { t } = useI18n();

const emit = defineEmits<{
  (e: "deleted", value: string): void;
  (e: "saved", value: CategorySavedEvent): void;
}>();

function onDeleted(category: string, hideModal: () => void): void {
  if (categories.remove(category)) {
    emit("deleted", category);
    hideModal();
  }
}
function onSaved(newCategory: string, oldCategory?: string): void {
  if (categories.save(newCategory, oldCategory)) {
    emit("saved", { newCategory, oldCategory });
  } else {
    toasts.warning("receipts.categories.alreadyExists");
  }
}

// TODO(fpion): items disappear when renaming/deleting a category
</script>

<template>
  <div>
    <div class="mb-3">
      <CategoryEdit @saved="onSaved" />
    </div>
    <p v-if="categories.categories.length === 0">{{ t("receipts.empty") }}</p>
    <table v-else class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("displayName") }}</th>
          <th scope="col">{{ t("actions.title") }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="category in categories.categories" :key="category">
          <td>{{ category }}</td>
          <td>
            <CategoryEdit class="me-1" :category="category" @saved="onSaved($event, category)" />
            <AppDelete
              class="ms-1"
              confirm="receipts.categories.delete.confirm"
              :display-name="category"
              title="receipts.categories.delete.title"
              @confirmed="onDeleted(category, $event)"
            />
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
