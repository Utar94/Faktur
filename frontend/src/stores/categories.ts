import { defineStore } from "pinia";
import { ref } from "vue";

export const useCategoryStore = defineStore(
  "category",
  () => {
    const categories = ref<string[]>([]);

    function remove(category: string): boolean {
      const index = categories.value.findIndex((c) => c === category);
      if (index < 0) {
        return false;
      }
      categories.value.splice(index, 1);
      return true;
    }

    function save(newCategory: string, oldCategory?: string): boolean {
      if (newCategory === oldCategory || categories.value.includes(newCategory)) {
        return false;
      }
      const index = categories.value.findIndex((c) => c === oldCategory);
      if (index < 0) {
        categories.value.push(newCategory);
      } else {
        categories.value.splice(index, 1, newCategory);
      }
      return true;
    }

    return { categories, remove, save };
  },
  { persist: true },
); // TODO(fpion): tests
