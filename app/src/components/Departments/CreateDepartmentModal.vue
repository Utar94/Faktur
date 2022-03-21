<template>
  <create-modal :disabled="loading" :id="id" :loading="loading" title="departments.create" @hidden="reset" @ok="submit">
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <name-field v-model="name" />
      </b-form>
    </validation-observer>
  </create-modal>
</template>

<script>
import { createDepartment } from '@/api/departments'

export default {
  props: {
    id: {
      type: String,
      default: 'createDepartment'
    },
    storeId: {
      type: Number,
      required: true
    }
  },
  data: () => ({
    loading: false,
    name: null
  }),
  methods: {
    reset() {
      this.name = null
    },
    async submit(callback) {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await createDepartment(this.storeId, { name: this.name })
            this.$emit('created', data)
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
  }
}
</script>
