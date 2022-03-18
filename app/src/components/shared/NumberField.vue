<template>
  <form-field
    :id="id"
    label="number.label"
    :max-length="maxLength"
    placeholder="number.placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  >
    <slot />
  </form-field>
</template>

<script>
export default {
  props: {
    id: {
      type: String,
      default: 'number'
    },
    maxLength: {
      type: Number,
      default: 32
    },
    required: {
      type: Boolean,
      default: false
    },
    value: {}
  }
}
</script>
