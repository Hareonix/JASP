<template>
  <v-container>
    <v-btn color="primary" class="mb-4" @click="refresh">Refresh</v-btn>
    <v-data-table :headers="headers" :items="clientsStore.clients">
      <template #item.tags="{ item }">
        {{ item.tags.join(', ') }}
      </template>
      <template #item.outbounds="{ item }">
        {{ item.outbounds.length }}
      </template>
      <template #item.actions="{ item }">
        <v-btn variant="text" @click="openDialog(item.id)">Outbounds</v-btn>
      </template>
    </v-data-table>
    <OutboundMappingDialog v-model="dialog" :client-id="currentClientId" />
  </v-container>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useClientsStore } from '~/stores/clients'
import OutboundMappingDialog from '~/components/OutboundMappingDialog.vue'

const clientsStore = useClientsStore()

const headers = [
  { title: 'Name', key: 'name' },
  { title: 'Tags', key: 'tags' },
  { title: 'Outbounds', key: 'outbounds' },
  { title: 'Actions', key: 'actions', sortable: false },
]

const dialog = ref(false)
const currentClientId = ref(0)

function openDialog(id: number) {
  currentClientId.value = id
  dialog.value = true
}

async function refresh() {
  await clientsStore.fetch()
}

onMounted(refresh)
</script>
