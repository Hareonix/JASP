<template>
  <v-dialog v-model="dialog" max-width="600">
    <v-card>
      <v-card-title>Assign Outbounds</v-card-title>
      <v-card-text>
        <v-list>
          <v-list-item
            v-for="(item, index) in assigned"
            :key="item.outboundId"
            draggable="true"
            @dragstart="onDragStart(index)"
            @dragover.prevent
            @drop="onDrop(index)"
          >
            <v-list-item-title>{{ outboundName(item.outboundId) }}</v-list-item-title>
            <template #append>
              <v-switch v-model="item.enabled" class="mr-2" />
              <v-btn variant="text" size="small" @click="remove(index)">Remove</v-btn>
            </template>
          </v-list-item>
        </v-list>
        <v-select
          v-model="selected"
          :items="availableOutbounds"
          item-title="name"
          item-value="id"
          label="Add Outbound"
          @update:model-value="add"
        />
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn variant="text" @click="dialog = false">Cancel</v-btn>
        <v-btn color="primary" @click="save">Save</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useClientsStore, type ClientOutbound } from '~/stores/clients'
import { useOutboundsStore } from '~/stores/outbounds'

const props = defineProps<{ modelValue: boolean; clientId: number }>()
const emit = defineEmits(['update:modelValue'])

const dialog = computed({
  get: () => props.modelValue,
  set: (v: boolean) => emit('update:modelValue', v),
})

const clientsStore = useClientsStore()
const outboundsStore = useOutboundsStore()

const assigned = ref<ClientOutbound[]>([])
const selected = ref<number | null>(null)
const dragIndex = ref<number | null>(null)

watch(
  () => props.clientId,
  (id) => {
    const client = clientsStore.clients.find((c) => c.id === id)
    assigned.value = client ? [...client.outbounds].sort((a, b) => a.priority - b.priority) : []
  },
  { immediate: true }
)

const availableOutbounds = computed(() =>
  outboundsStore.outbounds.filter((o) => !assigned.value.some((a) => a.outboundId === o.id))
)

function outboundName(id: number) {
  return outboundsStore.outbounds.find((o) => o.id === id)?.name || `#${id}`
}

function add(id: number | null) {
  if (!id) return
  assigned.value.push({ outboundId: id, priority: assigned.value.length + 1, enabled: true })
  selected.value = null
}

function remove(index: number) {
  assigned.value.splice(index, 1)
}

function onDragStart(index: number) {
  dragIndex.value = index
}

function onDrop(index: number) {
  if (dragIndex.value === null) return
  const item = assigned.value.splice(dragIndex.value, 1)[0]
  assigned.value.splice(index, 0, item)
  dragIndex.value = null
}

async function save() {
  assigned.value.forEach((a, idx) => (a.priority = idx + 1))
  await clientsStore.updateOutbounds(props.clientId, assigned.value)
  dialog.value = false
}

onMounted(async () => {
  if (!outboundsStore.outbounds.length) {
    await outboundsStore.fetch()
  }
})
</script>
