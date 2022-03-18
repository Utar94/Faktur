<template>
  <b-container>
    <template v-if="department">
      <h1 v-text="department.name" />
      <audit-info :entity="department" />
      <validation-observer ref="form">
        <b-form @submit.prevent="submit">
          <name-field v-model="name" />
          <number-field v-model="number" />
          <description-field v-model="description" />
          <div>
            <icon-submit class="mx-1" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
            <icon-button class="mx-1" icon="ban" text="actions.cancel" :to="{ name: 'Store', params: { id: $route.params.storeId } }" />
          </div>
        </b-form>
      </validation-observer>
    </template>
  </b-container>
</template>

<script>
import { getDepartment, updateDepartment } from '@/api/departments'

export default {
  data: () => ({
    department: null,
    description: null,
    loading: false,
    name: null,
    number: null
  }),
  computed: {
    hasChanges() {
      return (
        this.name !== this.department.name ||
        (this.description ?? '') !== (this.department.description ?? '') ||
        (this.number ?? '') !== (this.department.number ?? '')
      )
    }
  },
  methods: {
    setModel(department) {
      this.department = department
      this.description = department.description
      this.name = department.name
      this.number = department.number
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await updateDepartment(this.department.id, {
              description: this.description,
              name: this.name,
              number: this.number
            })
            this.setModel(data)
            this.$refs.form.reset()
            this.toast('success', 'department.saved')
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
      const { data } = await getDepartment(this.$route.params.id)
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
