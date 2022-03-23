<template>
  <form-select
    :disabled="disabled"
    :id="id"
    :label="label"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
import { getStores } from '@/api/stores'

export default {
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: 'store'
    },
    label: {
      type: String,
      default: 'product.store.label'
    },
    placeholder: {
      type: String,
      default: 'product.store.placeholder'
    },
    required: {
      type: Boolean,
      default: false
    },
    value: {}
  },
  data: () => ({
    stores: []
  }),
  computed: {
    options() {
      return this.stores.map(({ id, name, number }) => ({
        text: number ? `${name} (#${number})` : name,
        value: id
      }))
    }
  },
  async created() {
    try {
      const { data } = await getStores({ deleted: false, sort: 'Name' })
      this.stores = data.items
    } catch (e) {
      this.handleError(e)
    }
  }
}
</script>
