<template>
  <nav class="bottom-nav">
    <button 
      v-for="tab in tabs" 
      :key="tab.value"
      @click="$emit('update:modelValue', tab.value)"
      class="nav-item"
      :class="{ active: modelValue === tab.value }"
    >
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <component :is="'path'" :d="tab.icon" />
      </svg>
      <span>{{ tab.label }}</span>
    </button>
  </nav>
</template>

<script setup>
defineProps({
  modelValue: {
    type: String,
    required: true
  }
})

defineEmits(['update:modelValue'])

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
</script>

<style scoped>
.bottom-nav {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  background: var(--surface);
  border-top: 1px solid var(--border);
  display: flex;
  justify-content: space-around;
  padding: var(--space-xs) 0;
  z-index: var(--z-header);
  box-shadow: 0 -2px 10px rgba(0, 0, 0, 0.05);
}

.nav-item {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--space-xs);
  padding: var(--space-sm);
  background: none;
  border: none;
  cursor: pointer;
  color: var(--text-light);
  transition: var(--transition);
  border-radius: var(--radius-sm);
}

.nav-item:hover {
  background: var(--background);
}

.nav-item.active {
  color: var(--primary);
}

.nav-item svg {
  width: 24px;
  height: 24px;
}

.nav-item span {
  font-size: 0.75rem;
  font-weight: 600;
}

@media (max-width: 640px) {
  .nav-item span {
    font-size: 0.7rem;
  }
  
  .nav-item svg {
    width: 20px;
    height: 20px;
  }
}
</style>