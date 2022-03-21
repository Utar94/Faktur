<template>
  <b-modal :id="id" :title="$t('receipt.editItem')">
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <form-field
          class="col"
          id="quantity"
          label="receipt.item.quantity.label"
          :minValue="0.001"
          placeholder="receipt.item.quantity.placeholder"
          :step="1"
          type="number"
          v-model.number="quantity"
        />
        <form-field
          append="$"
          class="col"
          id="unitPrice"
          label="receipt.item.unitPrice.label"
          :minValue="0.01"
          placeholder="receipt.item.unitPrice.placeholder"
          :step="0.01"
          type="number"
          v-model.number="unitPrice"
        />
        <form-field
          append="$"
          class="col"
          id="price"
          label="receipt.item.price.label"
          :minValue="0.01"
          placeholder="receipt.item.price.placeholder"
          :step="0.01"
          type="number"
          v-model.number="price"
        />
      </b-form>
    </validation-observer>
    <template #modal-footer="{ cancel, ok }">
      <icon-button icon="ban" text="actions.cancel" @click="cancel()" />
      <icon-button :disabled="loading" icon="save" :loading="loading" text="actions.save" variant="primary" @click="submit(ok)" />
    </template>
  </b-modal>
</template>

<script>
import { updateItem } from '@/api/receipts'

export default {
  props: {
    id: {
      type: String,
      default: 'editItem'
    },
    item: {
      type: Object,
      required: true
    }
  },
  data: () => ({
    loading: false,
    price: 0,
    quantity: 0,
    unitPrice: 0
  }),
  methods: {
    async submit(callback) {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await updateItem(this.item.id, {
              price: this.price,
              quantity: this.quantity,
              unitPrice: this.unitPrice
            })
            this.$emit('updated', data)
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
    item: {
      deep: true,
      immediate: true,
      handler(item) {
        if (item) {
          this.price = item.price
          this.quantity = item.quantity
          this.unitPrice = item.unitPrice
        }
      }
    }
  }
}
</script>
