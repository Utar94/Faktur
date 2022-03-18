<template>
  <form-field
    :id="id"
    label="name.label"
    :max-length="maxLength"
    placeholder="name.placeholder"
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
      default: 'name'
    },
    maxLength: {
      type: Number,
      default: 100
    },
    required: {
      type: Boolean,
      default: true
    },
    value: {}
  }
}
</script>
