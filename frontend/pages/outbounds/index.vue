<template>
  <v-container>
    <v-btn color="primary" class="mb-4" @click="openCreate">Add Outbound</v-btn>
    <v-data-table :headers="headers" :items="store.outbounds">
      <template #item.actions="{ item }">
        <v-btn variant="text" @click="openEdit(item)">Edit</v-btn>
        <v-btn variant="text" color="error" @click="remove(item.id)">Delete</v-btn>
      </template>
    </v-data-table>
    <v-dialog v-model="dialog" max-width="600">
      <v-card>
        <v-card-title>{{ editing ? 'Edit' : 'Create' }} Outbound</v-card-title>
        <v-card-text>
          <v-text-field v-model="form.name" label="Name" />
          <v-text-field v-model="form.type" label="Type" />
          <v-text-field v-model="form.version" label="Version" />
          <v-switch v-model="form.enabled" label="Enabled" />
          <v-textarea v-model="form.jsonConfig" label="JSON Config" rows="10" />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="dialog = false">Cancel</v-btn>
          <v-btn color="primary" @click="save">Save</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useOutboundsStore, type Outbound } from '~/stores/outbounds'

const store = useOutboundsStore()
const headers = [
  { title: 'Name', key: 'name' },
  { title: 'Type', key: 'type' },
  { title: 'Actions', key: 'actions', sortable: false },
]

const dialog = ref(false)
const editing = ref(false)
const editId = ref(0)
const form = ref<Partial<Outbound>>({ name: '', type: '', version: '', jsonConfig: '{}', enabled: true })

function openCreate() {
  editing.value = false
  editId.value = 0
  form.value = { name: '', type: '', version: '', jsonConfig: '{}', enabled: true }
  dialog.value = true
}

function openEdit(item: Outbound) {
  editing.value = true
  editId.value = item.id
  form.value = { ...item }
  dialog.value = true
}

async function save() {
  if (editing.value) await store.update(editId.value, form.value)
  else await store.create(form.value)
  dialog.value = false
}

async function remove(id: number) {
  await store.remove(id)
}

onMounted(() => store.fetch())
</script>
