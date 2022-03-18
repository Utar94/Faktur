<template>
  <b-container>
    <template v-if="product">
      <h1 v-text="product.name" />
      <audit-info :entity="product" />
      <validation-observer ref="form">
        <b-form @submit.prevent="submit">
          <name-field v-model="name" />
          <description-field v-model="description" />
          <div>
            <icon-submit class="mx-1" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
            <icon-button class="mx-1" icon="ban" text="actions.cancel" :to="{ name: 'Products' }" />
          </div>
        </b-form>
      </validation-observer>
    </template>
  </b-container>
</template>

<script>
import { getProduct, updateProduct } from '@/api/products'

export default {
  data: () => ({
    product: null,
    description: null,
    loading: false,
    name: null
  }),
  computed: {
    hasChanges() {
      return this.name !== this.product.name || (this.description ?? '') !== (this.product.description ?? '')
    }
  },
  methods: {
    setModel(product) {
      this.product = product
      this.description = product.description
      this.name = product.name
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await updateProduct(this.product.id, {
              description: this.description,
              name: this.name
            })
            this.setModel(data)
            this.$refs.form.reset()
            this.toast('success', 'product.saved')
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  async created() {
    try {
      const { data } = await getProduct(this.$route.params.id)
      this.setModel(data)
    } catch (e) {
      const { status } = e
      if (status === 404) {
        return this.$router.push({ name: 'NotFound' })
      }
      this.handleError(e)
    }
  }
}
</script>
