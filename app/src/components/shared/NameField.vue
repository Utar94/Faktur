<template>
  <form-field :id="id" :label="label" :max-length="maxLength" :placeholder="placeholder" :required="required" :value="value" @input="$emit('input', $event)">
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
    label: {
      type: String,
      default: 'name.label'
    },
    maxLength: {
      type: Number,
      default: 100
    },
    placeholder: {
      type: String,
      default: 'name.placeholder'
    },
    required: {
      type: Boolean,
      default: true
    },
    value: {}
  }
}
</script>
