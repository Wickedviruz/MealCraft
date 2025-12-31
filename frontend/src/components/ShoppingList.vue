<template>
  <div class="shopping-list">
    <header class="page-header">
      <h1>Shopping List</h1>
      <p class="subtitle">{{ totalItems }} items • {{ checkedCount }}/{{ totalItems }} checked</p>
    </header>
    
    <div v-if="totalItems === 0" class="empty-state">
      <svg class="empty-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
        <circle cx="9" cy="21" r="1"/>
        <circle cx="20" cy="21" r="1"/>
        <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"/>
      </svg>
      <h2>No items yet</h2>
      <p>Add meals to your calendar to generate a shopping list</p>
    </div>
    
    <div v-else class="list-container">
      <div class="list-actions">
        <button @click="emit('generate-list')" class="btn-generate">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="23 4 23 10 17 10"/>
            <path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/>
          </svg>
          Regenerate from Calendar
        </button>
        
        <button @click="clearChecked" class="btn-clear">
          Clear Checked
        </button>
      </div>
      
      <div v-for="category in categories" :key="category.name" class="category-section">
        <h3 class="category-title">
          {{ category.icon }} {{ category.name }}
          <span class="category-count">{{ category.items.length }}</span>
        </h3>
        
        <div class="items-list">
          <label 
            v-for="item in category.items" 
            :key="item.id"
            :class="['item-row', { checked: item.checked }]"
          >
            <input 
              type="checkbox" 
              :checked="item.checked"
              @change="toggleItem(item.id)"
              class="item-checkbox"
            />
            <span class="item-name">{{ item.name }}</span>
            <span class="item-amount">{{ item.amount }} {{ item.unit }}</span>
          </label>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  items: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['toggle-item', 'clear-checked', 'generate-list'])

const categories = computed(() => {
  const grouped = {}
  
  props.items.forEach(item => {
    if (!grouped[item.category]) {
      grouped[item.category] = {
        name: item.category,
        icon: getCategoryIcon(item.category),
        items: []
      }
    }
    grouped[item.category].items.push(item)
  })
  
  return Object.values(grouped).sort((a, b) => a.name.localeCompare(b.name))
})

const totalItems = computed(() => props.items.length)

const checkedCount = computed(() => 
  props.items.filter(item => item.checked).length
)

function getCategoryIcon(category) {
  const icons = {
    'Produce': '🥬',
    'Meat': '🥩',
    'Dairy': '🥛',
    'Grains': '🌾',
    'Spices': '🧂',
    'Pantry': '🏺',
    'Other': '📦'
  }
  return icons[category] || '📦'
}

function toggleItem(itemId) {
  emit('toggle-item', itemId)
}

function clearChecked() {
  emit('clear-checked')
}
</script>

<style scoped>
.shopping-list {
  padding: 1.5rem;
  padding-bottom: 100px;
  max-width: 800px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2rem;
  font-weight: 700;
  color: #2c3e50;
  margin: 0 0 0.5rem 0;
}

.subtitle {
  color: #888;
  font-size: 0.95rem;
  margin: 0;
}

.empty-state {
  text-align: center;
  padding: 4rem 2rem;
  color: #888;
}

.empty-icon {
  width: 80px;
  height: 80px;
  margin: 0 auto 1.5rem;
  stroke: #D4E7C5;
  stroke-width: 1.5;
}

.empty-state h2 {
  font-size: 1.5rem;
  color: #555;
  margin: 0 0 0.5rem 0;
}

.empty-state p {
  margin: 0;
}

.list-actions {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.btn-generate,
.btn-clear {
  padding: 0.75rem 1.5rem;
  border-radius: 12px;
  border: none;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.btn-generate {
  background: #8CB369;
  color: white;
  flex: 1;
}

.btn-generate:hover {
  background: #7aa359;
}

.btn-generate svg {
  width: 20px;
  height: 20px;
}

.btn-clear {
  background: white;
  color: #ff6b6b;
  border: 1px solid #ff6b6b;
}

.btn-clear:hover {
  background: #ff6b6b;
  color: white;
}

.category-section {
  background: white;
  border-radius: 16px;
  padding: 1.5rem;
  margin-bottom: 1rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.category-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #2c3e50;
  margin: 0 0 1rem 0;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.category-count {
  font-size: 0.85rem;
  color: #888;
  font-weight: 500;
  background: #f5f5f5;
  padding: 0.2rem 0.6rem;
  border-radius: 12px;
}

.items-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.item-row {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.item-row:hover {
  background: #f9fdf7;
}

.item-row.checked {
  opacity: 0.5;
}

.item-row.checked .item-name {
  text-decoration: line-through;
}

.item-checkbox {
  width: 20px;
  height: 20px;
  cursor: pointer;
  accent-color: #8CB369;
}

.item-name {
  flex: 1;
  font-weight: 500;
  color: #2c3e50;
}

.item-amount {
  color: #888;
  font-size: 0.9rem;
  white-space: nowrap;
}
</style>