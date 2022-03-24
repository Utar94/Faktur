<template>
  <form-select :id="id" :label="label" :options="options" :placeholder="placeholder" :value="value" @input="$emit('input', $event)" />
</template>

<script>
export default {
  props: {
    id: {
      type: String,
      default: 'status'
    },
    label: {
      type: String,
      default: 'receipt.status.label'
    },
    placeholder: {
      type: String,
      default: 'receipt.status.placeholder'
    },
    value: {}
  },
  computed: {
    options() {
      return [
        {
          text: this.$i18n.t('receipt.status.processed'),
          value: 'true'
        },
        {
          text: this.$i18n.t('receipt.status.new'),
          value: 'false'
        }
      ].sort((a, b) => (a.text < b.text ? -1 : a.text > b.text ? 1 : 0))
    }
  }
}
</script>
