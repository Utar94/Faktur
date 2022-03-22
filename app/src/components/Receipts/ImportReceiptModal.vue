<template>
  <b-modal :id="id" :title="$t('receipts.import')" @hidden="reset">
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <form-field id="issuedAt" label="receipt.issuedAt" :maxValue="maxIssuedAt" type="datetime-local" v-model="issuedAt" />
        <number-field v-model="number" />
        <description-field id="lines" label="receipt.lines.label" placeholder="receipt.lines.placeholder" required :rows="10" v-model="lines" />
        <div v-if="errors.length > 0">
          <div v-for="(error, index) in errors" :key="index" class="text-danger">
            {{ error }}
          </div>
        </div>
      </b-form>
    </validation-observer>
    <template #modal-footer="{ cancel, ok }">
      <icon-button icon="ban" text="actions.cancel" @click="cancel()" />
      <icon-button :disabled="loading" icon="file-invoice-dollar" :loading="loading" text="actions.import" variant="success" @click="submit(ok)" />
    </template>
  </b-modal>
</template>

<script>
import { importReceipt } from '@/api/receipts'

export default {
  props: {
    id: {
      type: String,
      default: 'importReceipt'
    },
    storeId: {
      type: Number,
      required: true
    }
  },
  data: () => ({
    errors: [],
    issuedAt: null,
    lines: null,
    loading: false,
    number: null
  }),
  computed: {
    maxIssuedAt() {
      const date = new Date().toISOString()
      const index = date.lastIndexOf(':')
      return date.substring(0, index)
    }
  },
  methods: {
    reset() {
      this.issuedAt = null
      this.lines = null
      this.number = null
      this.errors = []
    },
    async submit(callback) {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await importReceipt({
              issuedAt: this.issuedAt ? new Date(this.issuedAt).toISOString() : null,
              lines: this.lines,
              number: this.number,
              storeId: this.storeId
            })
            this.$emit('created', data)
            if (typeof callback === 'function') {
              callback()
            }
          }
        } catch (e) {
          if (e.status === 400) {
            this.errors = []
            for (const [key, values] of Object.entries(e.data)) {
              const start = key.indexOf('[')
              const end = key.indexOf(']', start)
              const line = start >= 0 && end >= 0 ? parseInt(key.substring(start + 1, end)) + 1 : key
              for (const error of values) {
                this.errors.push(this.$i18n.t('receipt.error', { line, error }))
              }
            }
          } else {
            this.handleError(e)
          }
        } finally {
          this.loading = false
        }
      }
    }
  }
}
</script>
