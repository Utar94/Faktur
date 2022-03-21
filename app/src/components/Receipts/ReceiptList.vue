<template>
  <b-container>
    <h1 v-t="'receipts.title'" />
    <div class="my-2">
      <icon-button class="mx-1" :disabled="loading" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
      <icon-button class="mx-1" :disabled="!storeId" icon="file-invoice-dollar" text="actions.import" variant="success" v-b-modal.importReceipt />
      <import-receipt-modal v-if="storeId" id="importReceipt" :storeId="storeId" @created="refresh()" />
    </div>
    <b-form>
      <store-select v-model="storeId" />
      <b-row>
        <search-field class="col" v-model="search" />
        <sort-select class="col" :desc="desc" :options="sortOptions" v-model="sort" @desc="desc = $event" />
        <count-select class="col" v-model="count" />
      </b-row>
    </b-form>
    <template v-if="receipts.length">
      <table id="results" class="table table-striped">
        <thead>
          <tr>
            <th scope="col" v-t="'number.label'" />
            <th scope="col" v-t="'receipt.issuedAt'" />
            <th scope="col" v-t="'receipt.total'" />
            <th scope="col" v-t="'updatedAt'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="receipt in receipts" :key="receipt.id">
            <td>
              <router-link :to="{ name: 'Receipt', params: { id: receipt.id } }" v-text="receipt.number || $t('none')" />
            </td>
            <td>{{ $d(new Date(receipt.issuedAt), 'medium') }}</td>
            <td>{{ $n(receipt.total, 'currency') }}</td>
            <td>{{ $d(new Date(receipt.updatedAt || receipt.createdAt), 'medium') }}</td>
            <td>
              <icon-button class="mx-1" icon="trash-alt" text="actions.delete" variant="danger" v-b-modal="`deleteReceipt_${receipt.id}`" />
              <delete-modal
                confirm="receipts.delete.confirm"
                :disabled="loading"
                :displayName="getDisplayName(receipt)"
                :id="`deleteReceipt_${receipt.id}`"
                :loading="loading"
                title="receipts.delete.title"
                @ok="onDelete(receipt, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <b-pagination v-model="page" :total-rows="total" :per-page="count" aria-controls="results" />
    </template>
    <p v-else v-t="'receipts.none'" />
  </b-container>
</template>

<script>
import ImportReceiptModal from './ImportReceiptModal.vue'
import StoreSelect from '../shared/StoreSelect.vue'
import { getReceipts, setReceiptDeleted } from '@/api/receipts'

export default {
  components: {
    ImportReceiptModal,
    StoreSelect
  },
  data: () => ({
    count: 10,
    desc: true,
    loading: false,
    page: 1,
    receipts: [],
    search: null,
    sort: 'IssuedAt',
    storeId: null,
    total: 0
  }),
  computed: {
    params() {
      return {
        deleted: false,
        search: this.search,
        storeId: this.storeId,
        sort: this.sort,
        desc: this.desc,
        index: (this.page - 1) * this.count,
        count: this.count
      }
    },
    sortOptions() {
      return Object.entries(this.$i18n.t('receipts.sort'))
        .map(([key, value]) => ({
          text: value,
          value: key
        }))
        .sort((a, b) => (a.text < b.text ? -1 : a.text > b.text ? 1 : 0))
    }
  },
  methods: {
    getDisplayName({ issuedAt, number }) {
      const issuedFormatted = this.$i18n.t('receipt.issuedFormat', { date: this.$i18n.d(new Date(issuedAt), 'medium') })
      return (
        (number ? this.$i18n.t('receipt.numberFormat', { number }) : this.$i18n.t('receipt.withoutNumber')) +
        ' ' +
        (issuedFormatted[0].toLowerCase() + issuedFormatted.substring(1))
      )
    },
    async onDelete({ id }, callback) {
      let deleted = false
      if (!this.loading) {
        this.loading = true
        try {
          await setReceiptDeleted(id)
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
          const { data } = await getReceipts(params ?? this.params)
          this.receipts = data.items
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
