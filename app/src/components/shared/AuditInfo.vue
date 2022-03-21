<template>
  <p>
    {{ $t('status.created', { date: $d(new Date(entity.createdAt), 'medium') }) }}
    <template v-if="entity.updatedAt">
      <br />
      {{ $t('status.updated', { date: $d(new Date(entity.updatedAt), 'medium') }) }}
      ({{ $t('status.version', { version: entity.version }) }})
    </template>
    <slot />
  </p>
</template>

<script>
export default {
  props: {
    entity: {
      type: Object,
      required: true
    }
  }
}
</script>
