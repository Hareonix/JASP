<template>
  <v-container>
    <p>Sing-box running: {{ store.running }}</p>
    <v-btn class="mr-2" @click="store.start">Start</v-btn>
    <v-btn class="mr-2" @click="store.stop">Stop</v-btn>
    <v-btn class="mr-2" @click="store.restart">Restart</v-btn>
  </v-container>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useSingBoxStore } from '~/stores/singbox'

const store = useSingBoxStore()

onMounted(() => store.fetchStatus())
</script>
