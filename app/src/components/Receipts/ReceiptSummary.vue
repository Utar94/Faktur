<template>
  <p>
    <strong>
      <template v-if="number">
        {{ $t('receipt.numberFormat', { number: number }) }}
        <br />
      </template>
      {{ $t('receipt.issuedFormat', { date: $d(issuedAt, 'medium') }) }}
    </strong>
  </p>
</template>

<script>
export default {
  props: {
    issuedAt: {
      type: Date,
      required: true
    },
    number: {
      type: String,
      default: null
    }
  }
}
</script>
