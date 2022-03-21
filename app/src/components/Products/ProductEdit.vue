<template>
  <b-container>
    <template v-if="product">
      <h1 v-text="title" />
      <audit-info :entity="product" />
      <validation-observer ref="form">
        <b-form @submit.prevent="submit">
          <b-row>
            <name-field class="col" label="product.label.label" placeholder="product.label.placeholder" :required="false" v-model="label" />
            <form-field class="col" id="sku" label="product.sku.label" :maxLength="32" placeholder="product.sku.placeholder" v-model="sku" />
          </b-row>
          <b-row>
            <form-field class="col" id="flags" label="product.flags.label" :maxLength="10" placeholder="product.flags.placeholder" v-model="flags" />
            <department-select class="col" :storeId="product.storeId" v-model="departmentId" />
          </b-row>
          <b-row>
            <form-field
              append="$"
              class="col"
              id="unitPrice"
              label="product.unitPrice.label"
              :minValue="0.01"
              placeholder="product.unitPrice.placeholder"
              :step="0.01"
              type="number"
              v-model.number="unitPrice"
            />
            <form-field class="col" id="unitType" label="product.unitType.label" :maxLength="4" placeholder="product.unitType.placeholder" v-model="unitType" />
          </b-row>
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
import DepartmentSelect from './DepartmentSelect.vue'
import { getProduct, updateProduct } from '@/api/products'

export default {
  components: {
    DepartmentSelect
  },
  data: () => ({
    departmentId: null,
    description: null,
    flags: null,
    label: null,
    loading: false,
    product: null,
    sku: null,
    unitPrice: null,
    unitType: null
  }),
  computed: {
    hasChanges() {
      return (
        (this.label ?? '') !== (this.product.label ?? '') ||
        (this.sku ?? '') !== (this.product.sku ?? '') ||
        (this.flags ?? '') !== (this.product.flags ?? '') ||
        this.departmentId !== this.product.departmentId ||
        (this.unitPrice || null) !== this.product.unitPrice ||
        (this.unitType ?? '') !== (this.product.unitType ?? '') ||
        (this.description ?? '') !== (this.product.description ?? '')
      )
    },
    title() {
      return this.product.label ?? this.product.article.name
    }
  },
  methods: {
    setModel(product) {
      this.product = product
      this.departmentId = product.departmentId
      this.description = product.description
      this.flags = product.flags
      this.label = product.label
      this.sku = product.sku
      this.unitPrice = product.unitPrice
      this.unitType = product.unitType
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await updateProduct(this.product.id, {
              departmentId: this.departmentId,
              description: this.description,
              flags: this.flags,
              label: this.label,
              sku: this.sku,
              unitPrice: this.unitPrice || null,
              unitType: this.unitType
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
