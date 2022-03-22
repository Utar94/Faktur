<template>
  <form-select :id="id" :label="label" :options="options" :placeholder="placeholder" :value="value" @input="$emit('input', $event)" />
</template>

<script>
import { getDepartments } from '@/api/departments'

export default {
  props: {
    id: {
      type: String,
      default: 'department'
    },
    label: {
      type: String,
      default: 'product.department.label'
    },
    placeholder: {
      type: String,
      default: 'product.department.placeholder'
    },
    storeId: {
      type: Number,
      required: true
    },
    value: {}
  },
  data: () => ({
    departments: []
  }),
  computed: {
    options() {
      return this.departments.map(({ id, name }) => ({
        text: name,
        value: id
      }))
    }
  },
  async created() {
    try {
      const { data } = await getDepartments(this.storeId, { deleted: false, sort: 'Name' })
      this.departments = data.items
    } catch (e) {
      this.handleError(e)
    }
  }
}
</script>
