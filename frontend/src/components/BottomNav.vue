<template>
  <nav class="bottom-nav">
    <button 
      v-for="item in navItems" 
      :key="item.id"
      :class="['nav-item', { active: modelValue === item.id }]"
      @click="emit('update:modelValue', item.id)"
    >
      <component :is="item.icon" class="nav-icon" />
      <span class="nav-label">{{ item.label }}</span>
    </button>
  </nav>
</template>

<script setup>
import { h } from 'vue'

defineProps({
  modelValue: {
    type: String,
    required: true
  }
})

const emit = defineEmits(['update:modelValue'])

const navItems = [
  {
    id: 'discover',
    label: 'Discover',
    icon: () => h('svg', {
      viewBox: '0 0 24 24',
      fill: 'none',
      stroke: 'currentColor',
      'stroke-width': '2'
    }, [
      h('path', { d: 'M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z' }),
      h('polyline', { points: '9 22 9 12 15 12 15 22' })
    ])
  },
  {
    id: 'saved',
    label: 'Saved',
    icon: () => h('svg', {
      viewBox: '0 0 24 24',
      fill: 'none',
      stroke: 'currentColor',
      'stroke-width': '2'
    }, [
      h('path', { d: 'M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z' })
    ])
  },
  {
    id: 'calendar',
    label: 'Calendar',
    icon: () => h('svg', {
      viewBox: '0 0 24 24',
      fill: 'none',
      stroke: 'currentColor',
      'stroke-width': '2'
    }, [
      h('rect', { x: '3', y: '4', width: '18', height: '18', rx: '2', ry: '2' }),
      h('line', { x1: '16', y1: '2', x2: '16', y2: '6' }),
      h('line', { x1: '8', y1: '2', x2: '8', y2: '6' }),
      h('line', { x1: '3', y1: '10', x2: '21', y2: '10' })
    ])
  },
  {
    id: 'shopping',
    label: 'Shopping',
    icon: () => h('svg', {
      viewBox: '0 0 24 24',
      fill: 'none',
      stroke: 'currentColor',
      'stroke-width': '2'
    }, [
      h('circle', { cx: '9', cy: '21', r: '1' }),
      h('circle', { cx: '20', cy: '21', r: '1' }),
      h('path', { d: 'M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6' })
    ])
  }
]
</script>

<style scoped>
.bottom-nav {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  background: #FEFDF8;
  display: flex;
  justify-content: space-around;
  padding: 0.75rem 0 calc(0.75rem + env(safe-area-inset-bottom));
  border-top: 1px solid #D4E7C5;
  box-shadow: 0 -2px 10px rgba(0, 0, 0, 0.05);
  z-index: 100;
}

.nav-item {
  background: none;
  border: none;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.25rem;
  color: #888;
  cursor: pointer;
  padding: 0.5rem 1rem;
  transition: all 0.2s ease;
  min-width: 60px;
}

.nav-item:hover {
  color: #8CB369;
}

.nav-item.active {
  color: #8CB369;
}

.nav-icon {
  width: 24px;
  height: 24px;
}

.nav-label {
  font-size: 0.75rem;
  font-weight: 500;
}
</style>