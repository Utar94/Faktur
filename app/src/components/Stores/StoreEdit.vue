<template>
  <b-container>
    <template v-if="store">
      <h1 v-text="store.name" />
      <audit-info :entity="store" />
      <validation-observer ref="form">
        <b-form @submit.prevent="submit">
          <b-tabs content-class="mt-3">
            <b-tab :title="$t('properties')">
              <name-field v-model="name" />
              <b-row>
                <number-field class="col" v-model="number" />
                <banner-select class="col" v-model="bannerId" />
              </b-row>
              <description-field v-model="description" />
            </b-tab>
            <b-tab :title="$t('store.contact')">
              <b-row>
                <form-field class="col" id="address" label="store.address.label" :max-length="100" placeholder="store.address.placeholder" v-model="address" />
                <form-field class="col" id="city" label="store.city.label" :max-length="100" placeholder="store.city.placeholder" v-model="city" />
                <form-field class="col" id="phone" label="store.phone.label" :max-length="40" placeholder="store.phone.placeholder" v-model="phone" />
              </b-row>
              <b-row>
                <form-field
                  class="col"
                  id="country"
                  label="store.country.label"
                  :max-length="2"
                  :min-length="2"
                  placeholder="store.country.placeholder"
                  v-model="country"
                />
                <form-field
                  class="col"
                  id="state"
                  label="store.state.label"
                  :max-length="2"
                  :min-length="2"
                  placeholder="store.state.placeholder"
                  v-model="state"
                />
                <form-field class="col" id="postalCode" label="store.postalCode.label" placeholder="store.postalCode.placeholder" v-model="postalCode" />
              </b-row>
            </b-tab>
            <b-tab :title="$t('departments.title')">
              <department-list :storeId="store.id" />
            </b-tab>
            <b-tab :title="$t('products.title')">
              <product-list :selectStore="false" :selectedStoreId="store.id" />
            </b-tab>
          </b-tabs>
          <div>
            <icon-submit class="mx-1" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
            <icon-button class="mx-1" icon="ban" text="actions.cancel" :to="{ name: 'Stores' }" />
          </div>
        </b-form>
      </validation-observer>
    </template>
  </b-container>
</template>

<script>
import BannerSelect from './BannerSelect.vue'
import DepartmentList from '../Departments/DepartmentList.vue'
import ProductList from '../Products/ProductList.vue'
import { getStore, updateStore } from '@/api/stores'

export default {
  components: {
    BannerSelect,
    DepartmentList,
    ProductList
  },
  data: () => ({
    address: null,
    bannerId: null,
    city: null,
    country: null,
    store: null,
    description: null,
    loading: false,
    name: null,
    number: null,
    phone: null,
    postalCode: null,
    state: null
  }),
  computed: {
    hasChanges() {
      return (
        this.name !== this.store.name ||
        (this.description ?? '') !== (this.store.description ?? '') ||
        (this.number ?? '') !== this.store.number ||
        this.bannerId !== this.store.bannerId ||
        (this.address ?? '') !== this.store.address ||
        (this.city ?? '') !== this.store.city ||
        (this.country ?? '') !== this.store.country ||
        (this.phone ?? '') !== this.store.phone ||
        (this.postalCode ?? '') !== this.store.postalCode ||
        (this.state ?? '') !== this.store.state
      )
    }
  },
  methods: {
    setModel(store) {
      this.store = store
      this.address = store.address
      this.bannerId = store.bannerId
      this.city = store.city
      this.country = store.country
      this.description = store.description
      this.name = store.name
      this.number = store.number
      this.phone = store.phone
      this.postalCode = store.postalCode
      this.state = store.state
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await updateStore(this.store.id, {
              address: this.address,
              bannerId: this.bannerId,
              city: this.city,
              country: this.country,
              description: this.description,
              name: this.name,
              number: this.number,
              phone: this.phone,
              postalCode: this.postalCode,
              state: this.state
            })
            this.setModel(data)
            this.$refs.form.reset()
            this.toast('success', 'store.saved')
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
      const { data } = await getStore(this.$route.params.id)
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
