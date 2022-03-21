<template>
  <div>
    <div class="mb-3">{{ $t('receipt.subTotalFormat', { amount: $n(subTotal, 'currency') }) }}</div>
    <div class="mb-3">
      <div v-for="tax in taxes" :key="tax.code">
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
    <div class="mb-3">{{ $t('receipt.totalFormat', { amount: $n(total, 'currency') }) }}</div>
  </div>
</template>

<script>
export default {
  props: {
    subTotal: {
      type: Number,
      required: true
    },
    taxes: {
      type: Array,
      required: true
    },
    total: {
      type: Number,
      required: true
    }
  }
}
</script>

<style scoped>
span.tax {
  display: block;
}
</style>
