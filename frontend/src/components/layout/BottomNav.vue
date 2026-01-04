<template>
  <nav class="fixed bottom-0 left-0 right-0 bg-white z-[100]" style="height: 70px;">
    <!-- Animated line indicator INNANFÖR -->
    <div class="relative h-1 bg-gray-200">
      <div 
        class="absolute h-1 bg-emerald-600 rounded-full transition-all duration-300 ease-out"
        :style="{ 
          width: indicatorWidth + 'px',
          left: indicatorLeft + 'px'
        }"
      ></div>
    </div>
    
    <!-- Navigation buttons -->
    <div class="flex items-center justify-around h-full px-2">
      <button 
        v-for="(tab, index) in tabs" 
        :key="tab.value"
        :ref="el => { if (el) tabRefs[index] = el }"
        @click="handleTabClick(tab.value, index)"
        class="relative flex flex-col items-center justify-center gap-1.5 px-4 h-full transition-colors duration-200"
      >
        <!-- Icon -->
        <svg 
          class="w-6 h-6 transition-colors duration-200" 
          :class="modelValue === tab.value ? 'text-emerald-600' : 'text-gray-500'"
          viewBox="0 0 24 24" 
          fill="none" 
          stroke="currentColor" 
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        >
          <component :is="'path'" :d="tab.icon" />
        </svg>
        
        <!-- Label -->
        <span 
          class="text-xs font-medium transition-colors duration-200"
          :class="modelValue === tab.value ? 'text-emerald-600' : 'text-gray-500'"
        >
          {{ tab.label }}
        </span>
      </button>
    </div>
  </nav>
</template>

<script setup>
import { ref, watch, onMounted, nextTick } from 'vue'

const props = defineProps({
  modelValue: {
    type: String,
    required: true
  }
})

const emit = defineEmits(['update:modelValue'])

const tabs = [
  {
    value: 'discover',
    label: 'Upptäck',
    icon: 'M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z'
  },
  {
    value: 'browse',
    label: 'Bläddra',
    icon: 'M4 6h16M4 10h16M4 14h16M4 18h16'
  },
  {
    value: 'add',
    label: 'Lägg till',
    icon: 'M12 4v16m8-8H4'
  },
  {
    value: 'saved',
    label: 'Sparade',
    icon: 'M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z'
  },
  {
    value: 'macros',
    label: 'Makron',
    icon: 'M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z'
  },
  {
    value: 'calendar',
    label: 'Kalender',
    icon: 'M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z'
  },
  {
    value: 'shopping',
    label: 'Inköp',
    icon: 'M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z'
  }
]

const tabRefs = ref([])
const indicatorWidth = ref(48)
const indicatorLeft = ref(0)

function handleTabClick(value, index) {
  emit('update:modelValue', value)
  updateIndicator(index)
}

function updateIndicator(index) {
  nextTick(() => {
    const button = tabRefs.value[index]
    if (button) {
      const rect = button.getBoundingClientRect()
      const navRect = button.parentElement.getBoundingClientRect()
      indicatorLeft.value = rect.left - navRect.left + (rect.width / 2) - 24
      indicatorWidth.value = 48
    }
  })
}

onMounted(() => {
  const activeIndex = tabs.findIndex(tab => tab.value === props.modelValue)
  if (activeIndex !== -1) {
    updateIndicator(activeIndex)
  }
})

watch(() => props.modelValue, (newValue) => {
  const activeIndex = tabs.findIndex(tab => tab.value === newValue)
  if (activeIndex !== -1) {
    updateIndicator(activeIndex)
  }
})
</script>