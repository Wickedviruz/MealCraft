<template>
  <div class="shopping-list">
    <div class="list-header">
      <h2>Inköpslista</h2>
      <div class="header-actions">
        <button @click="$emit('generate')" class="btn btn-primary">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="23 4 23 10 17 10"/>
            <path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/>
          </svg>
          Generera från kalender
        </button>
        <button 
          @click="$emit('clear-checked')" 
          class="btn btn-secondary"
          :disabled="!hasCheckedItems"
        >
          Rensa avbockade
        </button>
      </div>
    </div>
    
    <div v-if="items.length" class="items-container">
      <!-- Group by category -->
      <div 
        v-for="category in categories" 
        :key="category"
        class="category-section"
      >
        <h3 class="category-title">{{ getCategoryName(category) }}</h3>
        
        <div class="items-list">
          <div 
            v-for="item in getItemsByCategory(category)" 
            :key="item.id"
            class="list-item"
            :class="{ checked: item.checked }"
            @click="$emit('toggle', item.id)"
          >
            <div class="item-checkbox">
              <div class="checkbox" :class="{ checked: item.checked }">
                <svg v-if="item.checked" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                  <polyline points="20 6 9 17 4 12"/>
                </svg>
              </div>
            </div>
            
            <div class="item-content">
              <span class="item-name">{{ item.name }}</span>
              <span class="item-amount">{{ item.amount }} {{ item.unit }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <div v-else class="empty-state">
      <div class="empty-icon">🛒</div>
      <h3>Inköpslistan är tom</h3>
      <p>Lägg till måltider i kalendern och klicka på "Generera från kalender"</p>
    </div>
    
    <div v-if="items.length" class="list-summary">
      <div class="summary-item">
        <span>Totalt:</span>
        <strong>{{ items.length }} varor</strong>
      </div>
      <div class="summary-item">
        <span>Avbockade:</span>
        <strong>{{ checkedCount }} / {{ items.length }}</strong>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  items: {
    type: Array,
    required: true
  }
})

defineEmits(['toggle', 'clear-checked', 'generate'])

const categories = computed(() => {
  const cats = new Set(props.items.map(item => item.category || 'Other'))
  return Array.from(cats).sort()
})

const checkedCount = computed(() => {
  return props.items.filter(item => item.checked).length
})

const hasCheckedItems = computed(() => {
  return checkedCount.value > 0
})

function getItemsByCategory(category) {
  return props.items.filter(item => (item.category || 'Other') === category)
}

function getCategoryName(category) {
  const names = {
    'Produce': '🥬 Grönsaker & Frukt',
    'Meat': '🥩 Kött & Fisk',
    'Dairy': '🥛 Mejeri',
    'Grains': '🌾 Spannmål',
    'Spices': '🌶️ Kryddor',
    'Pantry': '🏺 Skafferi',
    'Other': '📦 Övrigt'
  }
  return names[category] || category
}
</script>

<style scoped>
.shopping-list {
  padding: var(--space-lg);
  max-width: 800px;
  margin: 0 auto;
}

.list-header {
  margin-bottom: var(--space-lg);
}

.list-header h2 {
  font-size: 2rem;
  font-weight: 700;
  margin-bottom: var(--space-md);
}

.header-actions {
  display: flex;
  gap: var(--space-md);
  flex-wrap: wrap;
}

.btn svg {
  width: 20px;
  height: 20px;
  margin-right: var(--space-xs);
}

.items-container {
  display: flex;
  flex-direction: column;
  gap: var(--space-lg);
}

.category-section {
  background: var(--surface);
  border-radius: var(--radius-lg);
  padding: var(--space-lg);
  box-shadow: var(--shadow-sm);
}

.category-title {
  font-size: 1.2rem;
  font-weight: 700;
  margin-bottom: var(--space-md);
  padding-bottom: var(--space-sm);
  border-bottom: 2px solid var(--border);
}

.items-list {
  display: flex;
  flex-direction: column;
  gap: var(--space-sm);
}

.list-item {
  display: flex;
  align-items: center;
  gap: var(--space-md);
  padding: var(--space-md);
  border-radius: var(--radius-sm);
  cursor: pointer;
  transition: var(--transition);
  border: 2px solid transparent;
}

.list-item:hover {
  background: var(--background);
  border-color: var(--primary-light);
}

.list-item.checked {
  opacity: 0.6;
}

.item-checkbox {
  flex-shrink: 0;
}

.checkbox {
  width: 28px;
  height: 28px;
  border: 2px solid var(--border);
  border-radius: var(--radius-sm);
  display: flex;
  align-items: center;
  justify-content: center;
  transition: var(--transition);
}

.checkbox.checked {
  background: var(--primary);
  border-color: var(--primary);
}

.checkbox svg {
  width: 18px;
  height: 18px;
  color: white;
}

.item-content {
  flex: 1;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: var(--space-md);
}

.item-name {
  font-weight: 600;
  color: var(--text);
}

.list-item.checked .item-name {
  text-decoration: line-through;
  color: var(--text-light);
}

.item-amount {
  color: var(--text-light);
  font-size: 0.9rem;
  white-space: nowrap;
}

.empty-state {
  text-align: center;
  padding: var(--space-xl);
  background: var(--surface);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
}

.empty-icon {
  font-size: 5rem;
  margin-bottom: var(--space-md);
}

.empty-state h3 {
  font-size: 1.5rem;
  margin-bottom: var(--space-sm);
}

.empty-state p {
  color: var(--text-light);
}

.list-summary {
  margin-top: var(--space-lg);
  padding: var(--space-md);
  background: var(--primary-light);
  border-radius: var(--radius-md);
  display: flex;
  justify-content: space-around;
  gap: var(--space-lg);
}

.summary-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--space-xs);
}

.summary-item span {
  color: var(--text-light);
  font-size: 0.9rem;
}

.summary-item strong {
  color: var(--primary-dark);
  font-size: 1.2rem;
}
</style>