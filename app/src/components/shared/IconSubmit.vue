<template>
  <icon-button :disabled="disabled" :icon="icon" :loading="loading" :size="size" :text="text" :to="to" type="submit" :variant="variant" />
</template>

<script>
export default {
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    icon: {
      type: String,
      required: true
    },
    loading: {
      type: Boolean,
      default: false
    },
    size: {
      type: String,
      default: ''
    },
    text: {
      type: String,
      default: ''
    },
    to: {},
    variant: {
      type: String,
      default: ''
    }
  }
}
</script>
