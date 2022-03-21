<template>
  <b-modal :id="id" :title="$t('receipt.edit')">
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <form-field id="issuedAt" label="receipt.issuedAt" :maxValue="getDatetimeLocal(new Date())" type="datetime-local" v-model="issuedAt" />
        <number-field v-model="number" />
      </b-form>
    </validation-observer>
    <template #modal-footer="{ cancel, ok }">
      <icon-button icon="ban" text="actions.cancel" @click="cancel()" />
      <icon-button :disabled="loading" icon="save" :loading="loading" text="actions.save" variant="primary" @click="submit(ok)" />
    </template>
  </b-modal>
</template>

<script>
import { updateReceipt } from '@/api/receipts'

export default {
  props: {
    id: {
      type: String,
      default: 'editReceipt'
    },
    receipt: {
      type: Object,
      required: true
    }
  },
  data: () => ({
    issuedAt: null,
    loading: false,
    number: null
  }),
  methods: {
    getDatetimeLocal(date) {
      const s = typeof date === 'string' ? date : date.toISOString()
      const index = s.lastIndexOf(':')
      return s.substring(0, index)
    },
    async submit(callback) {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await updateReceipt(this.receipt.id, {
              issuedAt: this.issuedAt ? new Date(this.issuedAt).toISOString() : null,
              number: this.number
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
    receipt: {
      deep: true,
      immediate: true,
      handler(receipt) {
        if (receipt) {
          this.issuedAt = this.getDatetimeLocal(receipt.issuedAt) // TODO(fpion): convert UTC to local
          this.number = receipt.number
        }
      }
    }
  }
}
</script>
