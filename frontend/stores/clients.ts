import { defineStore } from 'pinia'

export interface ClientOutbound {
  outboundId: number
  priority: number
  enabled: boolean
}

export interface Client {
  id: number
  name: string
  tags: string[]
  isOnline: boolean
  lastSeenAt?: string
  notes?: string
  outbounds: ClientOutbound[]
}

export const useClientsStore = defineStore('clients', {
  state: () => ({
    clients: [] as Client[],
  }),
  actions: {
    async fetch() {
      this.clients = await $fetch<Client[]>('/api/clients')
    },
    async updateOutbounds(clientId: number, outbounds: ClientOutbound[]) {
      await $fetch(`/api/clients/${clientId}/outbounds`, {
        method: 'PUT',
        body: outbounds,
      })
      const client = this.clients.find((c) => c.id === clientId)
      if (client) {
        client.outbounds = outbounds
      }
    },
  },
})
