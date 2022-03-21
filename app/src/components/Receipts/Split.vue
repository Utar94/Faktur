<template>
  <b-row>
    <b-col>
      <strong v-text="headers.left || $t('none')" />
      <div class="mb-3">
        {{ $t('receipt.subTotalFormat', { amount: $n(subtotals.left, 'currency') }) }}
      </div>
      <div class="mb-3">
        <div v-for="tax in taxes.left" :key="tax.code">
          {{
            $t('receipt.taxFormat', {
              amount: $n(tax.amount, 'currency'),
              code: tax.code,
              rate: $n(tax.rate, 'percent'),
              taxableAmount: $n(tax.taxableAmount, 'currency')
            })
          }}
        </div>
      </div>
      <div class="mb-3">
        {{ $t('receipt.totalFormat', { amount: $n(totals.left, 'currency') }) }}
      </div>
    </b-col>
    <b-col>
      <strong v-t="'receipt.shared'" />
      <div class="mb-3">
        {{ $t('receipt.subTotalFormat', { amount: $n(subtotals.shared, 'currency') }) }}
      </div>
      <div class="mb-3">
        <div v-for="tax in taxes.shared" :key="tax.code">
          {{
            $t('receipt.taxFormat', {
              amount: $n(tax.amount, 'currency'),
              code: tax.code,
              rate: $n(tax.rate, 'percent'),
              taxableAmount: $n(tax.taxableAmount, 'currency')
            })
          }}
        </div>
      </div>
      <div class="mb-3">
        {{ $t('receipt.totalFormat', { amount: $n(totals.shared, 'currency') }) }}
      </div>
    </b-col>
    <b-col>
      <strong v-text="headers.right || $t('none')" />
      <div class="mb-3">
        {{ $t('receipt.subTotalFormat', { amount: $n(subtotals.right, 'currency') }) }}
      </div>
      <div class="mb-3">
        <div v-for="tax in taxes.right" :key="tax.code" class="tax">
          {{
            $t('receipt.taxFormat', {
              amount: $n(tax.amount, 'currency'),
              code: tax.code,
              rate: $n(tax.rate, 'percent'),
              taxableAmount: $n(tax.taxableAmount, 'currency')
            })
          }}
        </div>
      </div>
      <div class="mb-3">
        {{ $t('receipt.totalFormat', { amount: $n(totals.right, 'currency') }) }}
      </div>
    </b-col>
  </b-row>
</template>

<script>
export default {
  props: {
    columns: {
      type: Object,
      required: true
    },
    headers: {
      type: Object,
      required: true
    },
    items: {
      type: Array,
      required: true
    },
    taxRates: {
      type: Object,
      required: true
    }
  },
  data: () => ({
    subtotals: {
      left: 0,
      right: 0,
      shared: 0
    },
    taxes: {
      left: [],
      right: [],
      shared: []
    },
    totals: {
      left: 0,
      right: 0,
      shared: 0
    }
  }),
  methods: {
    calculate(columns, items) {
      this.subtotals.left = 0
      this.subtotals.right = 0
      this.subtotals.shared = 0

      this.taxes.left = Object.entries(this.taxRates).map(([code, rate]) => ({
        amount: 0,
        code,
        rate,
        taxableAmount: 0
      }))
      this.taxes.right = Object.entries(this.taxRates).map(([code, rate]) => ({
        amount: 0,
        code,
        rate,
        taxableAmount: 0
      }))
      this.taxes.shared = Object.entries(this.taxRates).map(([code, rate]) => ({
        amount: 0,
        code,
        rate,
        taxableAmount: 0
      }))

      for (const item of items) {
        let key = 'shared'
        if (columns.left[item.id]) {
          key = 'left'
        } else if (columns.right[item.id]) {
          key = 'right'
        }

        this.subtotals[key] += item.price

        if (item.flags?.includes('F')) {
          const gst = this.taxes[key].find(tax => tax.code === 'GST')
          if (gst) {
            gst.taxableAmount += item.price
          }
        }

        if (item.flags?.includes('P')) {
          const qst = this.taxes[key].find(tax => tax.code === 'QST')
          if (qst) {
            qst.taxableAmount += item.price
          }
        }
      }

      for (const tax of this.taxes.left) {
        tax.amount = tax.taxableAmount * tax.rate
      }
      for (const tax of this.taxes.right) {
        tax.amount = tax.taxableAmount * tax.rate
      }
      for (const tax of this.taxes.shared) {
        tax.amount = tax.taxableAmount * tax.rate
      }

      this.totals.left = this.subtotals.left + this.taxes.left.reduce((amount, tax) => amount + tax.amount, 0)
      this.totals.right = this.subtotals.right + this.taxes.right.reduce((amount, tax) => amount + tax.amount, 0)
      this.totals.shared = this.subtotals.shared + this.taxes.shared.reduce((amount, tax) => amount + tax.amount, 0)
    }
  },
  watch: {
    columns: {
      deep: true,
      immediate: true,
      handler(columns) {
        this.calculate(columns, this.items)
      }
    },
    items: {
      deep: true,
      immediate: true,
      handler(items) {
        this.calculate(this.columns, items)
      }
    }
  }
}
</script>
