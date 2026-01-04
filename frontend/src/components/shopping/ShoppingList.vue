<template>
  <div style="padding: 24px 16px; max-width: 800px; margin: 0 auto; background: #fefdf8; min-height: 100vh;">
    <!-- Header -->
    <div style="margin-bottom: 24px;">
      <h2 style="font-size: 32px; font-weight: 800; color: #1f2937; margin-bottom: 16px;">Inköpslista</h2>
      
      <div style="display: flex; gap: 12px; flex-wrap: wrap;">
        <button 
          @click="$emit('generate')" 
          class="btn btn-primary"
          style="display: inline-flex; align-items: center; gap: 8px;"
        >
          <svg style="width: 20px; height: 20px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="23 4 23 10 17 10"/>
            <path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/>
          </svg>
          Generera från kalender
        </button>
        
        <button 
          @click="$emit('clear-checked')" 
          :disabled="!hasCheckedItems"
          style="padding: 12px 24px; border: none; border-radius: 12px; font-weight: 600; cursor: pointer; transition: all 0.2s; display: inline-flex; align-items: center; gap: 8px;"
          :style="!hasCheckedItems 
            ? 'background: #f3f4f6; color: #9ca3af; cursor: not-allowed;' 
            : 'background: #d1fae5; color: #047857;'"
        >
          Rensa avbockade
        </button>
      </div>
    </div>
    
    <!-- Items -->
    <div v-if="items.length" style="display: flex; flex-direction: column; gap: 20px;">
      <div 
        v-for="category in categories" 
        :key="category"
        style="background: white; border-radius: 16px; padding: 24px; box-shadow: 0 2px 8px rgba(0,0,0,0.08);"
      >
        <h3 style="font-size: 20px; font-weight: 700; margin-bottom: 16px; padding-bottom: 12px; border-bottom: 2px solid #e5e7eb; color: #1f2937;">
          {{ getCategoryName(category) }}
        </h3>
        
        <div style="display: flex; flex-direction: column; gap: 8px;">
          <div 
            v-for="item in getItemsByCategory(category)" 
            :key="item.id"
            @click="$emit('toggle', item.id)"
            style="display: flex; align-items: center; gap: 16px; padding: 14px; border-radius: 8px; cursor: pointer; transition: all 0.2s; border: 2px solid transparent;"
            :style="item.checked ? 'opacity: 0.6;' : ''"
            @mouseenter="$event.target.style.background = '#fefdf8'; $event.target.style.borderColor = '#d1fae5'"
            @mouseleave="$event.target.style.background = 'transparent'; $event.target.style.borderColor = 'transparent'"
          >
            <!-- Checkbox -->
            <div style="flex-shrink: 0;">
              <div 
                style="width: 28px; height: 28px; border-radius: 6px; display: flex; align-items: center; justify-content: center; transition: all 0.2s;"
                :style="item.checked 
                  ? 'background: #10b981; border: 2px solid #10b981;' 
                  : 'border: 2px solid #e5e7eb;'"
              >
                <svg v-if="item.checked" style="width: 18px; height: 18px; color: white;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                  <polyline points="20 6 9 17 4 12"/>
                </svg>
              </div>
            </div>
            
            <!-- Content -->
            <div style="flex: 1; display: flex; justify-content: space-between; align-items: center; gap: 16px;">
              <span 
                style="font-weight: 600; transition: all 0.2s;"
                :style="item.checked 
                  ? 'text-decoration: line-through; color: #9ca3af;' 
                  : 'color: #1f2937;'"
              >
                {{ item.name }}
              </span>
              <span style="color: #6b7280; font-size: 14px; white-space: nowrap;">
                {{ item.amount }} {{ item.unit }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Empty State -->
    <div v-else style="text-align: center; padding: 80px 20px; background: white; border-radius: 16px; box-shadow: 0 2px 8px rgba(0,0,0,0.08);">
      <div style="font-size: 80px; margin-bottom: 24px;">🛒</div>
      <h3 style="font-size: 28px; font-weight: 800; color: #1f2937; margin-bottom: 12px;">Inköpslistan är tom</h3>
      <p style="color: #6b7280; font-size: 16px; max-width: 400px; margin: 0 auto; line-height: 1.6;">
        Lägg till måltider i kalendern och klicka på "Generera från kalender"
      </p>
    </div>
    
    <!-- Summary -->
    <div v-if="items.length" style="margin-top: 24px; padding: 20px; background: #d1fae5; border-radius: 12px; display: flex; justify-content: space-around; gap: 24px;">
      <div style="display: flex; flex-direction: column; align-items: center; gap: 6px;">
        <span style="color: #047857; font-size: 14px; font-weight: 600;">Totalt</span>
        <strong style="color: #047857; font-size: 24px; font-weight: 800;">{{ items.length }}</strong>
      </div>
      <div style="display: flex; flex-direction: column; align-items: center; gap: 6px;">
        <span style="color: #047857; font-size: 14px; font-weight: 600;">Avbockade</span>
        <strong style="color: #047857; font-size: 24px; font-weight: 800;">{{ checkedCount }} / {{ items.length }}</strong>
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