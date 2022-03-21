<template>
  <b-col>
    <table class="table table-borderless" @click="$bvModal.show(editId)">
      <tr>
        <td>
          <span class="font-weight-bold" v-text="item.label" />
          <template v-if="item.flags">
            &nbsp;
            <sub v-text="`(${item.flags})`" />
          </template>
        </td>
        <td class="font-weight-bold text-right" v-text="$n(item.price, 'currency')" />
      </tr>
      <tr>
        <td v-text="item.code" />
        <td class="text-right">{{ $n(item.quantity) }} &times; {{ $n(item.unitPrice, 'currency') }}</td>
      </tr>
    </table>
    <edit-item-modal :id="editId" :item="item" @updated="$emit('updated', $event)" />
  </b-col>
</template>

<script>
import EditItemModal from './EditItemModal.vue'

export default {
  components: {
    EditItemModal
  },
  props: {
    item: {
      type: Object,
      required: true
    }
  },
  computed: {
    editId() {
      return `editItem_${this.item.id}`
    }
  }
}
</script>

<style scoped>
table {
  border: 1px solid lightgray;
}
table:hover {
  cursor: pointer;
  background-color: lightgray;
}
</style>
