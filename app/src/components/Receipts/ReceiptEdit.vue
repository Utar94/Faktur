<template>
  <b-container>
    <template v-if="receipt">
      <h1 v-if="receipt.number">{{ $t('receipt.numberFormat', { number: receipt.number }) }}</h1>
      <h1 v-else>{{ $t('receipt.issuedTitle', { date: $d(new Date(receipt.issuedAt), 'medium') }) }}</h1>
      <div class="my-2">
        <icon-button class="mx-1" icon="edit" text="actions.edit" variant="primary" v-b-modal.editReceipt />
        <edit-receipt-modal id="editReceipt" :receipt="receipt" @updated="setModel" />
      </div>
      <audit-info :entity="receipt">
        <p v-if="receipt.number">{{ $t('receipt.issuedFormat', { date: $d(new Date(receipt.issuedAt), 'medium') }) }}</p>
      </audit-info>
      <h2 v-t="'receipt.items'" />
      <headers v-model="headers" />
      <div v-for="(items, department) in groups" :key="department">
        <h4 v-if="department" v-text="department" />
        <b-row v-for="item in items" :key="item.id">
          <!-- Left -->
          <receipt-item v-if="contains(item, 'left')" :item="item" @updated="setModel" />
          <b-col v-else class="clickable" @click="move(item, 'left')" />
          <!-- Center -->
          <receipt-item v-if="!contains(item)" :item="item" @updated="setModel" />
          <b-col v-else class="clickable" @click="move(item)" />
          <!-- Right -->
          <receipt-item v-if="contains(item, 'right')" :item="item" @updated="setModel" />
          <b-col v-else class="clickable" @click="move(item, 'right')" />
        </b-row>
      </div>
      <h2 v-t="'receipt.totals'" />
      <totals :sub-total="receipt.subTotal" :taxes="receipt.taxes" :total="receipt.total" />
      <!-- TODO(fpion): les splits ne sont pas mis à jour quand on modifie un article -->
      <split :columns="columns" :headers="headers" :items="receipt.items" :tax-rates="taxRates" />
      <receipt-summary :number="receipt.number" :issuedAt="new Date(receipt.issuedAt)" />
      <div>
        <icon-button class="mx-1" :disabled="loading" icon="dollar-sign" :loading="loading" text="actions.process" variant="primary" @click="process" />
        <icon-button class="mx-1" icon="ban" text="actions.cancel" :to="{ name: 'Receipts' }" />
        <icon-button class="mx-1 float-right" icon="arrow-up" text="actions.back" variant="info" @click="scrollToTop" />
      </div>
      <div class="mb-3" />
    </template>
  </b-container>
</template>

<script>
import Vue from 'vue'
import { mapActions } from 'vuex'
import EditReceiptModal from './EditReceiptModal.vue'
import Headers from './Headers.vue'
import ReceiptItem from './ReceiptItem.vue'
import Split from './Split.vue'
import ReceiptSummary from './ReceiptSummary.vue'
import Totals from './Totals.vue'
import { getReceipt, processReceipt } from '@/api/receipts'

export default {
  components: {
    EditReceiptModal,
    Headers,
    ReceiptItem,
    ReceiptSummary,
    Split,
    Totals
  },
  data: () => ({
    columns: {
      left: {},
      right: {}
    },
    groups: [],
    headers: {
      left: '',
      right: ''
    },
    loading: false,
    receipt: null
  }),
  computed: {
    taxRates() {
      return Object.fromEntries(this.receipt.taxes.map(({ code, rate }) => [code, rate]))
    }
  },
  methods: {
    ...mapActions(['saveHeaders']),
    contains(item, name = null) {
      if (name) {
        return Boolean(this.columns[name][item.id])
      }
      for (const value of Object.values(this.columns)) {
        if (value[item.id]) {
          return true
        }
      }
      return false
    },
    move(item, name = null) {
      Vue.delete(this.columns.left, item.id)
      Vue.delete(this.columns.right, item.id)
      if (name) {
        Vue.set(this.columns[name], item.id, item)
      }
    },
    async process() {
      if (Boolean(this.columns.left) && Boolean(this.columns.right) && this.columns.left !== this.columns.right && !this.loading) {
        this.loading = true
        const items = {}
        for (const [key, header] of Object.entries(this.headers)) {
          items[header] = Object.keys(this.columns[key])
        }
        items['Shared'] = this.receipt.items.filter(item => Object.values(this.columns).every(column => !column[item.id])).map(item => item.id)
        try {
          await processReceipt(this.receipt.id, { items })
          this.toast('success', 'receipt.processed')
        } catch (e) {
          console.error(e)
        }
        this.loading = false
      }
    },
    scrollToTop() {
      window.scrollTo(0, 0)
    },
    setModel(model) {
      this.receipt = model

      const groups = { '': [] }
      for (const item of model.items) {
        if (item.department === '') {
          groups[''].push(item)
        } else {
          if (!groups[item.department]) {
            groups[item.department] = []
          }
          groups[item.department].push(item)
        }
      }
      this.groups = Object.fromEntries(
        Object.entries(groups)
          .filter(([, items]) => items.length > 0)
          .map(([department, items]) => [department, items.sort((a, b) => (a.code >= b.code ? 1 : -1))])
          .sort(([a], [b]) => (a >= b ? 1 : -1))
      )
    }
  },
  async created() {
    try {
      const { data } = await getReceipt(this.$route.params.id)
      this.setModel(data)
      this.headers = this.$store.state.headers ?? this.headers
    } catch (e) {
      console.error(e)
    }
  },
  watch: {
    headers: {
      deep: true,
      handler(headers) {
        this.saveHeaders(headers)
      }
    }
  }
}
</script>

<style scoped>
.clickable:hover {
  cursor: pointer;
  background-color: lightgray;
}
</style>
