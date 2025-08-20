import { defineStore } from 'pinia'

export const useSingBoxStore = defineStore('singbox', {
  state: () => ({
    running: false,
  }),
  actions: {
    async fetchStatus() {
      const res = await $fetch<{ running: boolean }>('/api/singbox/status')
      this.running = res.running
    },
    async start() {
      await $fetch('/api/singbox/start', { method: 'POST' })
      this.running = true
    },
    async stop() {
      await $fetch('/api/singbox/stop', { method: 'POST' })
      this.running = false
    },
    async restart() {
      await $fetch('/api/singbox/restart', { method: 'POST' })
      this.running = true
    },
  },
})
