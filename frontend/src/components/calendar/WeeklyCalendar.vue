<template>
  <div class="weekly-calendar">
    <div class="calendar-header">
      <h2>Veckokalender</h2>
      <div class="week-nav">
        <button @click="previousWeek" class="btn-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="15 18 9 12 15 6"/>
          </svg>
        </button>
        <span class="week-label">{{ weekLabel }}</span>
        <button @click="nextWeek" class="btn-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="9 18 15 12 9 6"/>
          </svg>
        </button>
      </div>
    </div>
    
    <div class="calendar-grid">
      <!-- Days header -->
      <div class="grid-header">
        <div class="meal-types-col">
          <div class="meal-type-label">Måltid</div>
        </div>
        <div 
          v-for="day in weekDays" 
          :key="day.date"
          class="day-header"
          :class="{ today: isToday(day.date) }"
        >
          <div class="day-name">{{ day.name }}</div>
          <div class="day-date">{{ day.dayNum }}</div>
        </div>
      </div>
      
      <!-- Meal rows -->
      <div 
        v-for="mealType in mealTypes" 
        :key="mealType.value"
        class="meal-row"
      >
        <div class="meal-type-label">
          {{ mealType.icon }} {{ mealType.label }}
        </div>
        
        <div 
          v-for="day in weekDays" 
          :key="`${day.date}-${mealType.value}`"
          class="meal-cell"
          @click="handleCellClick(day.date, mealType.value)"
        >
          <div v-if="getMeal(day.date, mealType.value)" class="meal-card">
            <div class="meal-image">
              <img :src="getMeal(day.date, mealType.value).imageUrl" :alt="getMeal(day.date, mealType.value).title" />
            </div>
            <div class="meal-title">{{ getMeal(day.date, mealType.value).title }}</div>
            <button 
              @click.stop="removeMeal(day.date, mealType.value)" 
              class="remove-meal-btn"
            >
              ×
            </button>
          </div>
          <div v-else class="empty-meal">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="12" y1="5" x2="12" y2="19"/>
              <line x1="5" y1="12" x2="19" y2="12"/>
            </svg>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  meals: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['add-meal', 'remove-meal'])

const currentWeekStart = ref(getMonday(new Date()))

const mealTypes = [
  { value: 'breakfast', label: 'Frukost', icon: '🌅' },
  { value: 'brunch', label: 'Brunch', icon: '🥐' },
  { value: 'lunch', label: 'Lunch', icon: '🍽️' },
  { value: 'snack', label: 'Mellanmål', icon: '🍎' },
  { value: 'dinner', label: 'Middag', icon: '🌙' },
  { value: 'dessert', label: 'Dessert', icon: '🍰' }
]

const weekDays = computed(() => {
  const days = []
  const start = new Date(currentWeekStart.value)
  
  for (let i = 0; i < 7; i++) {
    const date = new Date(start)
    date.setDate(start.getDate() + i)
    
    days.push({
      date: formatDate(date),
      name: getDayName(date),
      dayNum: date.getDate()
    })
  }
  
  return days
})

const weekLabel = computed(() => {
  const start = new Date(currentWeekStart.value)
  const end = new Date(start)
  end.setDate(start.getDate() + 6)
  
  const startStr = `${start.getDate()}/${start.getMonth() + 1}`
  const endStr = `${end.getDate()}/${end.getMonth() + 1}`
  
  return `${startStr} - ${endStr}`
})

function getMonday(date) {
  const d = new Date(date)
  const day = d.getDay()
  const diff = d.getDate() - day + (day === 0 ? -6 : 1)
  return new Date(d.setDate(diff))
}

function formatDate(date) {
  return date.toISOString().split('T')[0]
}

function getDayName(date) {
  const days = ['Sön', 'Mån', 'Tis', 'Ons', 'Tor', 'Fre', 'Lör']
  return days[date.getDay()]
}

function isToday(dateStr) {
  return dateStr === formatDate(new Date())
}

function getMeal(date, mealType) {
  const key = `${date}-${mealType}`
  return props.meals[key]
}

function handleCellClick(date, mealType) {
  emit('add-meal', { date, mealType })
}

function removeMeal(date, mealType) {
  emit('remove-meal', { date, mealType })
}

function previousWeek() {
  const newStart = new Date(currentWeekStart.value)
  newStart.setDate(newStart.getDate() - 7)
  currentWeekStart.value = newStart
}

function nextWeek() {
  const newStart = new Date(currentWeekStart.value)
  newStart.setDate(newStart.getDate() + 7)
  currentWeekStart.value = newStart
}
</script>

<style scoped>
.weekly-calendar {
  padding: var(--space-lg);
  max-width: 1400px;
  margin: 0 auto;
}

.calendar-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--space-lg);
  flex-wrap: wrap;
  gap: var(--space-md);
}

.calendar-header h2 {
  font-size: 2rem;
  font-weight: 700;
}

.week-nav {
  display: flex;
  align-items: center;
  gap: var(--space-md);
}

.week-label {
  font-weight: 600;
  min-width: 120px;
  text-align: center;
}

.calendar-grid {
  background: var(--surface);
  border-radius: var(--radius-lg);
  overflow: hidden;
  box-shadow: var(--shadow-sm);
}

.grid-header {
  display: grid;
  grid-template-columns: 120px repeat(7, 1fr);
  border-bottom: 2px solid var(--border);
  background: var(--primary-light);
}

.meal-types-col {
  border-right: 2px solid var(--border);
}

.day-header {
  padding: var(--space-md);
  text-align: center;
  border-right: 1px solid var(--border);
}

.day-header:last-child {
  border-right: none;
}

.day-header.today {
  background: var(--primary);
  color: white;
}

.day-name {
  font-weight: 700;
  font-size: 0.9rem;
  margin-bottom: var(--space-xs);
}

.day-date {
  font-size: 1.2rem;
  font-weight: 700;
}

.meal-row {
  display: grid;
  grid-template-columns: 120px repeat(7, 1fr);
  border-bottom: 1px solid var(--border);
}

.meal-row:last-child {
  border-bottom: none;
}

.meal-type-label {
  padding: var(--space-md);
  font-weight: 600;
  border-right: 2px solid var(--border);
  background: var(--background);
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
}

.meal-cell {
  padding: var(--space-sm);
  border-right: 1px solid var(--border);
  min-height: 100px;
  cursor: pointer;
  transition: var(--transition);
}

.meal-cell:last-child {
  border-right: none;
}

.meal-cell:hover {
  background: var(--background);
}

.empty-meal {
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-light);
  opacity: 0;
  transition: var(--transition);
}

.meal-cell:hover .empty-meal {
  opacity: 1;
}

.empty-meal svg {
  width: 24px;
  height: 24px;
}

.meal-card {
  position: relative;
  background: var(--surface);
  border-radius: var(--radius-sm);
  overflow: hidden;
  box-shadow: var(--shadow-sm);
  transition: var(--transition);
}

.meal-card:hover {
  box-shadow: var(--shadow-md);
  transform: translateY(-2px);
}

.meal-image {
  width: 100%;
  height: 60px;
  overflow: hidden;
}

.meal-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.meal-title {
  padding: var(--space-xs);
  font-size: 0.85rem;
  font-weight: 600;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.remove-meal-btn {
  position: absolute;
  top: 2px;
  right: 2px;
  width: 24px;
  height: 24px;
  border-radius: var(--radius-full);
  border: none;
  background: rgba(255, 107, 107, 0.9);
  color: white;
  cursor: pointer;
  font-size: 1.2rem;
  line-height: 1;
  opacity: 0;
  transition: var(--transition);
}

.meal-card:hover .remove-meal-btn {
  opacity: 1;
}

.remove-meal-btn:hover {
  background: #ff6b6b;
  transform: scale(1.1);
}

@media (max-width: 768px) {
  .grid-header,
  .meal-row {
    grid-template-columns: 80px repeat(7, 1fr);
  }
  
  .meal-type-label {
    font-size: 0.8rem;
  }
  
  .day-name {
    font-size: 0.75rem;
  }
  
  .day-date {
    font-size: 1rem;
  }
}
</style>