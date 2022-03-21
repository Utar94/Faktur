<template>
  <form-field
    :id="id"
    :label="label"
    :min-length="minLength"
    :placeholder="placeholder"
    :ref="id"
    :required="required"
    :rules="rules"
    type="password"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
import { v4 as uuidv4 } from 'uuid'

export default {
  props: {
    id: {
      type: String,
      default: () => uuidv4()
    },
    label: {
      type: String,
      default: ''
    },
    minLength: {
      type: Number,
      required: false
    },
    placeholder: {
      type: String,
      default: ''
    },
    required: {
      type: Boolean,
      default: false
    },
    rules: {
      type: Object,
      default: null
    },
    value: {}
  },
  methods: {
    focus() {
      this.$refs[this.id].focus()
    }
  }
}
</script>
