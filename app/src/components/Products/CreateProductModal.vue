<template>
  <create-modal :disabled="loading" :id="id" :loading="loading" title="products.create" @hidden="reset" @ok="submit">
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <article-select required v-model="articleId" />
        <store-select v-if="!selectedStoreId" required v-model="storeId" />
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
    selectedArticleId: {
      type: Number,
      default: null
    },
    selectedStoreId: {
      type: Number,
      default: null
    }
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
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  watch: {
    selectedArticleId(articleId) {
      this.articleId = articleId
    }
  }
}
</script>
