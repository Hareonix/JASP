import { defineStore } from 'pinia'

export interface Inbound {
  id: number
  name: string
  type: string
  version: string
  jsonConfig: string
  enabled: boolean
}

export const useInboundsStore = defineStore('inbounds', {
  state: () => ({
    inbounds: [] as Inbound[],
  }),
  actions: {
    async fetch() {
      this.inbounds = await $fetch<Inbound[]>('/api/inbounds')
    },
    async create(data: Partial<Inbound>) {
      const inbound = await $fetch<Inbound>('/api/inbounds', {
        method: 'POST',
        body: data,
      })
      this.inbounds.push(inbound)
    },
    async update(id: number, data: Partial<Inbound>) {
      await $fetch(`/api/inbounds/${id}`, {
        method: 'PUT',
        body: data,
      })
      const idx = this.inbounds.findIndex((i) => i.id === id)
      if (idx !== -1) this.inbounds[idx] = { ...this.inbounds[idx], ...data }
    },
    async remove(id: number) {
      await $fetch(`/api/inbounds/${id}`, { method: 'DELETE' })
      this.inbounds = this.inbounds.filter((i) => i.id !== id)
    },
  },
})
