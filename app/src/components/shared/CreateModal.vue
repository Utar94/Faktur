<template>
  <b-modal :id="id" :title="$t(title)" @hidden="$emit('hidden', $event)">
    <slot />
    <template #modal-footer="{ cancel, ok }">
      <icon-button icon="ban" text="actions.cancel" @click="cancel()" />
      <icon-button :disabled="disabled" icon="plus" :loading="loading" text="actions.create" variant="success" @click="$emit('ok', ok)" />
    </template>
  </b-modal>
</template>

<script>
export default {
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: 'createModal'
    },
    loading: {
      type: Boolean,
      default: false
    },
    title: {
      type: String,
      required: true
    }
  }
}
</script>
