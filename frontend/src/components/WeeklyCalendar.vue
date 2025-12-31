<template>
  <div class="weekly-calendar">
    <header class="page-header">
      <h1>Meal Calendar</h1>
      <div class="week-navigation">
        <button @click="previousWeek" class="week-btn">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="15 18 9 12 15 6"/>
          </svg>
        </button>
        <span class="week-label">{{ weekLabel }}</span>
        <button @click="nextWeek" class="week-btn">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="9 18 15 12 9 6"/>
          </svg>
        </button>
      </div>
    </header>
    
    <div class="calendar-grid">
      <div v-for="day in weekDays" :key="day.date" class="day-column">
        <div class="day-header">
          <div class="day-name">{{ day.dayName }}</div>
          <div class="day-date">{{ day.dateLabel }}</div>
        </div>
        
        <div class="meals-container">
          <div 
            v-for="mealType in mealTypes" 
            :key="`${day.date}-${mealType}`"
            class="meal-slot"
            @click="openMealSelector(day.date, mealType)"
          >
            <div class="meal-header">
              <span class="meal-icon">{{ getMealIcon(mealType) }}</span>
              <span class="meal-label">{{ mealType }}</span>
            </div>
            
            <div v-if="getMeal(day.date, mealType)" class="meal-content">
              <img 
                :src="getMeal(day.date, mealType).image" 
                :alt="getMeal(day.date, mealType).title"
                class="meal-thumb"
              />
              <div class="meal-info">
                <div class="meal-title">{{ getMeal(day.date, mealType).title }}</div>
                <div class="meal-time">⏱️ {{ getMeal(day.date, mealType).cookTime }}m</div>
              </div>
              <button 
                class="btn-remove-meal"
                @click.stop="removeMeal(day.date, mealType)"
              >
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <line x1="18" y1="6" x2="6" y2="18"/>
                  <line x1="6" y1="6" x2="18" y2="18"/>
                </svg>
              </button>
            </div>
            
            <div v-else class="meal-empty">
              <svg class="plus-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                <line x1="12" y1="5" x2="12" y2="19"/>
                <line x1="5" y1="12" x2="19" y2="12"/>
              </svg>
              <span>Add meal</span>
            </div>
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
    default: () => ({})
  }
})

const emit = defineEmits(['add-meal', 'remove-meal'])

const currentWeekStart = ref(getWeekStart(new Date()))
const mealTypes = ['Breakfast', 'Lunch', 'Dinner']

const weekDays = computed(() => {
  const days = []
  const start = new Date(currentWeekStart.value)
  
  for (let i = 0; i < 7; i++) {
    const date = new Date(start)
    date.setDate(start.getDate() + i)
    
    days.push({
      date: formatDate(date),
      dayName: date.toLocaleDateString('en-US', { weekday: 'short' }),
      dateLabel: date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
    })
  }
  
  return days
})

const weekLabel = computed(() => {
  const start = new Date(currentWeekStart.value)
  const end = new Date(start)
  end.setDate(start.getDate() + 6)
  
  return `${start.toLocaleDateString('en-US', { month: 'short', day: 'numeric' })} - ${end.toLocaleDateString('en-US', { month: 'short', day: 'numeric' })}`
})

function getWeekStart(date) {
  const d = new Date(date)
  const day = d.getDay()
  const diff = d.getDate() - day + (day === 0 ? -6 : 1)
  return new Date(d.setDate(diff))
}

function formatDate(date) {
  return date.toISOString().split('T')[0]
}

function getMeal(date, mealType) {
  return props.meals[`${date}-${mealType}`]
}

function getMealIcon(mealType) {
  const icons = {
    Breakfast: '🌅',
    Lunch: '☀️',
    Dinner: '🌙'
  }
  return icons[mealType]
}

function openMealSelector(date, mealType) {
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
  padding: 1.5rem;
  padding-bottom: 100px;
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2rem;
  font-weight: 700;
  color: #2c3e50;
  margin: 0 0 1rem 0;
}

.week-navigation {
  display: flex;
  align-items: center;
  gap: 1rem;
  justify-content: center;
}

.week-btn {
  background: white;
  border: 1px solid #D4E7C5;
  border-radius: 8px;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s;
}

.week-btn:hover {
  background: #D4E7C5;
}

.week-btn svg {
  width: 20px;
  height: 20px;
}

.week-label {
  font-weight: 600;
  color: #2c3e50;
  min-width: 200px;
  text-align: center;
}

.calendar-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: 1rem;
  overflow-x: auto;
}

.day-column {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
  min-width: 180px;
}

.day-header {
  background: #8CB369;
  color: white;
  padding: 0.75rem;
  text-align: center;
}

.day-name {
  font-weight: 600;
  font-size: 0.9rem;
}

.day-date {
  font-size: 0.85rem;
  opacity: 0.9;
}

.meals-container {
  padding: 0.5rem;
}

.meal-slot {
  margin-bottom: 0.5rem;
  border: 2px dashed #D4E7C5;
  border-radius: 8px;
  padding: 0.75rem;
  cursor: pointer;
  transition: all 0.2s;
}

.meal-slot:hover {
  border-color: #8CB369;
  background: #f9fdf7;
}

.meal-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
  font-size: 0.85rem;
  font-weight: 600;
  color: #555;
}

.meal-icon {
  font-size: 1.2rem;
}

.meal-content {
  position: relative;
}

.meal-thumb {
  width: 100%;
  height: 80px;
  object-fit: cover;
  border-radius: 6px;
  margin-bottom: 0.5rem;
}

.meal-info {
  font-size: 0.85rem;
}

.meal-title {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.25rem;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.meal-time {
  color: #888;
  font-size: 0.8rem;
}

.btn-remove-meal {
  position: absolute;
  top: 0.25rem;
  right: 0.25rem;
  width: 24px;
  height: 24px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.95);
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: all 0.2s;
}

.meal-slot:hover .btn-remove-meal {
  opacity: 1;
}

.btn-remove-meal:hover {
  background: #ff6b6b;
  color: white;
}

.btn-remove-meal svg {
  width: 14px;
  height: 14px;
}

.meal-empty {
  text-align: center;
  color: #8CB369;
  font-size: 0.85rem;
  padding: 1rem 0;
}

.plus-icon {
  width: 24px;
  height: 24px;
  margin-bottom: 0.25rem;
  stroke-width: 2;
}
</style>