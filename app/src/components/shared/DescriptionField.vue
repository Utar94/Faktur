<template>
  <form-textarea :id="id" :label="label" :placeholder="placeholder" :required="required" :rows="rows" :value="value" @input="$emit('input', $event)" />
</template>

<script>
export default {
  props: {
    id: {
      type: String,
      default: 'description'
    },
    label: {
      type: String,
      default: 'description.label'
    },
    placeholder: {
      type: String,
      default: 'description.placeholder'
    },
    required: {
      type: Boolean,
      default: false
    },
    rows: {
      type: Number,
      default: 10
    },
    value: {}
  }
}
</script>
