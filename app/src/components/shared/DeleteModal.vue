<template>
  <b-modal :id="id" :title="$t(title)">
    <p v-if="confirm || displayName">
      <template v-if="confirm">{{ $t(confirm) }}</template>
      <br v-if="confirm && displayName" />
      <span v-if="displayName" class="text-danger">{{ displayName }}</span>
    </p>
    <slot />
    <template #modal-footer="{ cancel, ok }">
      <icon-button icon="ban" text="actions.cancel" @click="cancel()" />
      <icon-button :disabled="disabled" icon="trash-alt" :loading="loading" text="actions.delete" variant="danger" @click="$emit('ok', ok)" />
    </template>
  </b-modal>
</template>

<script>
export default {
  props: {
    confirm: {
      type: String,
      default: ''
    },
    disabled: {
      type: Boolean,
      default: false
    },
    displayName: {
      type: String,
      default: ''
    },
    id: {
      type: String,
      default: 'deleteModal'
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
