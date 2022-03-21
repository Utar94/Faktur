<template>
  <create-modal :disabled="loading" :id="id" :loading="loading" title="products.create" @hidden="reset" @ok="submit">
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <article-select required v-model="articleId" />
        <store-select v-if="selectStore" required v-model="storeId" />
      </b-form>
    </validation-observer>
  </create-modal>
</template>

<script>
import ArticleSelect from './ArticleSelect.vue'
import StoreSelect from './StoreSelect.vue'
import { createProduct } from '@/api/products'

export default {
  props: {
    departmentId: {
      type: Number,
      default: null
    },
    id: {
      type: String,
      default: 'createProduct'
    },
    selectStore: {
      type: Boolean,
      default: true
    },
    selectedArticleId: { default: null },
    selectedStoreId: { default: null }
  },
  components: {
    ArticleSelect,
    StoreSelect
  },
  data: () => ({
    articleId: null,
    loading: false,
    storeId: null
  }),
  methods: {
    reset() {
      this.articleId = null
      this.storeId = null
    },
    async submit(callback) {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await createProduct({
              articleId: this.articleId,
              departmentId: this.departmentId,
              storeId: this.storeId ?? this.selectedStoreId
            })
            this.$emit('created', data)
            if (typeof callback === 'function') {
              callback()
            }
          }
        } catch (e) {
          if (e.status === 409 && e.data?.field === 'ArticleId') {
            this.toast('product.conflict.title', 'product.conflict.body', 'warning')
          } else {
            this.handleError(e)
          }
        } finally {
          this.loading = false
        }
      }
    }
  },
  watch: {
    selectedArticleId: {
      immediate: true,
      handler(articleId) {
        this.articleId = articleId ?? null
      }
    },
    selectedStoreId: {
      immediate: true,
      handler(storeId) {
        this.storeId = storeId ?? null
      }
    }
  }
}
</script>
