import { defineStore } from 'pinia'

export interface Outbound {
  id: number
  name: string
  type: string
  version: string
  jsonConfig: string
  enabled: boolean
}

export const useOutboundsStore = defineStore('outbounds', {
  state: () => ({
    outbounds: [] as Outbound[],
  }),
  actions: {
    async fetch() {
      this.outbounds = await $fetch<Outbound[]>('/api/outbounds')
    },
    async create(data: Partial<Outbound>) {
      const outbound = await $fetch<Outbound>('/api/outbounds', {
        method: 'POST',
        body: data,
      })
      this.outbounds.push(outbound)
    },
    async update(id: number, data: Partial<Outbound>) {
      await $fetch(`/api/outbounds/${id}`, {
        method: 'PUT',
        body: data,
      })
      const idx = this.outbounds.findIndex((o) => o.id === id)
      if (idx !== -1) this.outbounds[idx] = { ...this.outbounds[idx], ...data }
    },
    async remove(id: number) {
      await $fetch(`/api/outbounds/${id}`, { method: 'DELETE' })
      this.outbounds = this.outbounds.filter((o) => o.id !== id)
    },
  },
})
