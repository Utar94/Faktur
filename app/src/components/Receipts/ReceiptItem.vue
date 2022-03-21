<template>
  <b-col>
    <table class="table table-borderless">
      <tr>
        <td>
          <span class="font-weight-bold" v-text="item.label" />
          &nbsp;
          <sub v-if="item.flags" v-text="`(${item.flags})`" />
        </td>
        <td class="font-weight-bold text-right" v-text="$n(item.price, 'currency')" />
      </tr>
      <tr>
        <td v-text="item.code" />
        <td class="text-right">{{ $n(item.quantity) }} &times; {{ $n(item.unitPrice, 'currency') }}</td>
      </tr>
    </table>
  </b-col>
</template>

<script>
export default {
  props: {
    item: {
      type: Object,
      required: true
    }
  }
}
</script>

<style scoped>
table {
  border: 1px solid lightgray;
}
</style>
