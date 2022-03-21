<template>
  <b-container>
    <h1 v-t="'stores.title'" />
    <div class="my-2">
      <icon-button class="mx-1" :disabled="loading" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
      <icon-button class="mx-1" icon="plus" text="actions.create" variant="success" v-b-modal.createStore />
      <create-store-modal id="createStore" @created="onCreate" />
    </div>
    <b-form>
      <b-row>
        <search-field class="col" v-model="search" />
        <banner-select class="col" v-model="bannerId" />
      </b-row>
      <b-row>
        <sort-select class="col" :desc="desc" :options="sortOptions" v-model="sort" @desc="desc = $event" />
        <count-select class="col" v-model="count" />
      </b-row>
    </b-form>
    <template v-if="stores.length">
      <table id="results" class="table table-striped">
        <thead>
          <tr>
            <th scope="col" v-t="'name.label'" />
            <th scope="col" v-t="'number.label'" />
            <th scope="col" v-t="'updatedAt'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="store in stores" :key="store.id">
            <td>
              <router-link :to="{ name: 'Store', params: { id: store.id } }" v-text="store.name" />
            </td>
            <td>{{ store.number || $t('none') }}</td>
            <td>{{ $d(new Date(store.updatedAt || store.createdAt), 'medium') }}</td>
            <td>
              <icon-button class="mx-1" icon="trash-alt" text="actions.delete" variant="danger" v-b-modal="`deleteStore_${store.id}`" />
              <delete-modal
                confirm="stores.delete.confirm"
                :disabled="loading"
                :displayName="store.name"
                :id="`deleteStore_${store.id}`"
                :loading="loading"
                title="stores.delete.title"
                @ok="onDelete(store, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <b-pagination v-model="page" :total-rows="total" :per-page="count" aria-controls="results" />
    </template>
    <p v-else v-t="'stores.none'" />
  </b-container>
</template>

<script>
import BannerSelect from './BannerSelect.vue'
import CreateStoreModal from './CreateStoreModal.vue'
import { deleteStore, getStores } from '@/api/stores'

export default {
  components: {
    BannerSelect,
    CreateStoreModal
  },
  data: () => ({
    bannerId: null,
    count: 10,
    desc: false,
    stores: [],
    loading: false,
    page: 1,
    search: null,
    sort: 'Name',
    total: 0
  }),
  computed: {
    params() {
      return {
        bannerId: this.bannerId,
        deleted: false,
        search: this.search,
        sort: this.sort,
        desc: this.desc,
        index: (this.page - 1) * this.count,
        count: this.count
      }
    },
    sortOptions() {
      return Object.entries(this.$i18n.t('stores.sort'))
        .map(([key, value]) => ({
          text: value,
          value: key
        }))
        .sort((a, b) => (a.text < b.text ? -1 : a.text > b.text ? 1 : 0))
    }
  },
  methods: {
    onCreate({ id }) {
      this.$router.push({ name: 'Store', params: { id } })
    },
    async onDelete({ id }, callback) {
      let deleted = false
      if (!this.loading) {
        this.loading = true
        try {
          await deleteStore(id)
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
          const { data } = await getStores(params ?? this.params)
          this.stores = data.items
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
    }
  }
}
</script>
