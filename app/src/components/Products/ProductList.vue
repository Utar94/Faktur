<template>
  <b-container>
    <h1 v-if="displayTitle" v-t="'products.title'" />
    <div class="my-2">
      <icon-button class="mx-1" :disabled="loading" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
      <icon-button class="mx-1" icon="plus" text="actions.create" variant="success" v-b-modal.createProduct />
      <create-product-modal
        id="createProduct"
        :departmentId="departmentId"
        :selectStore="selectStore"
        :selectedArticleId="articleId"
        :selectedStoreId="storeId || selectedStoreId"
        @created="onCreate"
      />
    </div>
    <b-form>
      <b-row>
        <article-select class="col" v-model="articleId" />
        <store-select v-if="!selectedStoreId" class="col" v-model="storeId" />
        <department-select
          v-if="(storeId || selectedStoreId) && !selectedDepartmentId"
          class="col"
          :storeId="storeId || selectedStoreId"
          v-model="departmentId"
        />
      </b-row>
      <b-row>
        <search-field class="col" v-model="search" />
        <sort-select class="col" :desc="desc" :options="sortOptions" v-model="sort" @desc="desc = $event" />
        <count-select class="col" v-model="count" />
      </b-row>
    </b-form>
    <template v-if="products.length">
      <table id="results" class="table table-striped">
        <thead>
          <tr>
            <th scope="col" v-t="'product.label.label'" />
            <th scope="col" v-t="'product.sku.label'" />
            <th scope="col" v-t="'updatedAt'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="product in products" :key="product.id">
            <td>
              <router-link :to="{ name: 'Product', params: { id: product.id } }" v-text="product.label || product.article.name" />
            </td>
            <td>{{ product.sku || $t('none') }}</td>
            <td>{{ $d(new Date(product.updatedAt || product.createdAt), 'medium') }}</td>
            <td>
              <icon-button class="mx-1" icon="trash-alt" text="actions.delete" variant="danger" v-b-modal="`deleteProduct_${product.id}`" />
              <delete-modal
                confirm="products.delete.confirm"
                :disabled="loading"
                :displayName="product.label || product.article.name"
                :id="`deleteProduct_${product.id}`"
                :loading="loading"
                title="products.delete.title"
                @ok="onDelete(product, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <b-pagination v-model="page" :total-rows="total" :per-page="count" aria-controls="results" />
    </template>
    <p v-else v-t="'products.none'" />
  </b-container>
</template>

<script>
import ArticleSelect from './ArticleSelect.vue'
import CreateProductModal from './CreateProductModal.vue'
import DepartmentSelect from './DepartmentSelect.vue'
import StoreSelect from '../shared/StoreSelect.vue'
import { getProducts, setProductDeleted } from '@/api/products'

export default {
  components: {
    ArticleSelect,
    CreateProductModal,
    DepartmentSelect,
    StoreSelect
  },
  props: {
    selectStore: {
      type: Boolean,
      default: true
    },
    selectedDepartmentId: { default: null },
    selectedStoreId: { default: null }
  },
  data: () => ({
    articleId: null,
    count: 10,
    departmentId: null,
    desc: false,
    products: [],
    loading: false,
    page: 1,
    search: null,
    sort: 'Label',
    storeId: null,
    total: 0
  }),
  computed: {
    displayTitle() {
      return !this.selectedStoreId
    },
    params() {
      return {
        articleId: this.articleId,
        deleted: false,
        departmentId: this.storeId || this.selectedStoreId ? this.departmentId ?? this.selectedDepartmentId : null,
        search: this.search,
        storeId: this.storeId || this.selectedStoreId,
        sort: this.sort,
        desc: this.desc,
        index: (this.page - 1) * this.count,
        count: this.count
      }
    },
    sortOptions() {
      return Object.entries(this.$i18n.t('products.sort'))
        .map(([key, value]) => ({
          text: value,
          value: key
        }))
        .sort((a, b) => (a.text < b.text ? -1 : a.text > b.text ? 1 : 0))
    }
  },
  methods: {
    onCreate({ id }) {
      this.$router.push({ name: 'ProductEdit', params: { id } })
    },
    async onDelete({ id }, callback) {
      let deleted = false
      if (!this.loading) {
        this.loading = true
        try {
          await setProductDeleted(id)
          deleted = true
          if (typeof callback === 'function') {
            callback()
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
        if (deleted) {
          await this.refresh()
        }
      }
    },
    async refresh(params = null) {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await getProducts(params ?? this.params)
          this.products = data.items
          this.total = data.total
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  watch: {
    params: {
      deep: true,
      immediate: true,
      async handler(params) {
        await this.refresh(params)
      }
    },
    storeId: {
      immediate: true,
      handler(storeId) {
        if (!storeId) {
          this.departmentId = null
        }
      }
    }
  }
}
</script>
