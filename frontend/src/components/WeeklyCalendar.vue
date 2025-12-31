<template>
  <div class="weekly-calendar">
    <header class="calendar-header">
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
      
      <div class="meal-type-selector">
        <h3>Visa måltider:</h3>
        <div class="meal-toggles">
          <label 
            v-for="meal in allMealTypes" 
            :key="meal.id"
            :class="['meal-toggle', { active: enabledMealTypes.includes(meal.id) }]"
          >
            <input 
              type="checkbox" 
              :checked="enabledMealTypes.includes(meal.id)"
              @change="toggleMealType(meal.id)"
            />
            <span class="meal-icon">{{ meal.icon }}</span>
            <span class="meal-name">{{ meal.name }}</span>
          </label>
        </div>
      </div>
    </header>
    
    <div class="calendar-container">
      <div class="calendar-grid">
        <!-- Header row with days -->
        <div class="day-headers">
          <div class="meal-label-column"></div>
          <div 
            v-for="day in weekDays" 
            :key="day.date"
            :class="['day-header', { today: isToday(day.date) }]"
          >
            <div class="day-name">{{ day.dayName }}</div>
            <div class="day-date">{{ day.dateLabel }}</div>
          </div>
        </div>
        
        <!-- Rows for each meal type -->
        <div 
          v-for="mealType in activeMealTypes" 
          :key="mealType.id"
          class="meal-row"
        >
          <div class="meal-label">
            <span class="meal-icon">{{ mealType.icon }}</span>
            <span class="meal-name">{{ mealType.name }}</span>
          </div>
          
          <div 
            v-for="day in weekDays" 
            :key="`${day.date}-${mealType.id}`"
            class="meal-cell"
            @click="openMealSelector(day.date, mealType.id)"
          >
            <div v-if="getMeal(day.date, mealType.id)" class="meal-content">
              <img 
                :src="getMeal(day.date, mealType.id).image" 
                :alt="getMeal(day.date, mealType.id).title"
                class="meal-thumb"
              />
              <div class="meal-info">
                <div class="meal-title">{{ getMeal(day.date, mealType.id).title }}</div>
                <div class="meal-meta">
                  <span>⏱️ {{ getMeal(day.date, mealType.id).cookTime }}m</span>
                </div>
              </div>
              <button 
                class="btn-remove-meal"
                @click.stop="removeMeal(day.date, mealType.id)"
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
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <div class="calendar-stats">
      <div class="stat-card">
        <div class="stat-value">{{ totalMealsPlanned }}</div>
        <div class="stat-label">Måltider planerade</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ uniqueRecipes }}</div>
        <div class="stat-label">Unika recept</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ totalCookTime }}h</div>
        <div class="stat-label">Total matlagning</div>
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
const enabledMealTypes = ref(['breakfast', 'lunch', 'dinner'])

const allMealTypes = [
  { id: 'breakfast', name: 'Frukost', icon: '🌅' },
  { id: 'brunch', name: 'Brunch', icon: '🥐' },
  { id: 'lunch', name: 'Lunch', icon: '☀️' },
  { id: 'snack', name: 'Mellanmål', icon: '🍎' },
  { id: 'dinner', name: 'Middag', icon: '🌙' },
  { id: 'dessert', name: 'Efterrätt', icon: '🍰' }
]

const activeMealTypes = computed(() => 
  allMealTypes.filter(mt => enabledMealTypes.value.includes(mt.id))
)

const weekDays = computed(() => {
  const days = []
  const start = new Date(currentWeekStart.value)
  
  for (let i = 0; i < 7; i++) {
    const date = new Date(start)
    date.setDate(start.getDate() + i)
    
    days.push({
      date: formatDate(date),
      dayName: date.toLocaleDateString('sv-SE', { weekday: 'short' }),
      dateLabel: date.toLocaleDateString('sv-SE', { day: 'numeric', month: 'short' }),
      fullDate: date
    })
  }
  
  return days
})

const weekLabel = computed(() => {
  const start = new Date(currentWeekStart.value)
  const end = new Date(start)
  end.setDate(start.getDate() + 6)
  
  return `Vecka ${getWeekNumber(start)} • ${start.toLocaleDateString('sv-SE', { day: 'numeric', month: 'short' })} - ${end.toLocaleDateString('sv-SE', { day: 'numeric', month: 'short' })}`
})

const totalMealsPlanned = computed(() => 
  Object.keys(props.meals).length
)

const uniqueRecipes = computed(() => {
  const recipeIds = new Set(Object.values(props.meals).map(m => m.id))
  return recipeIds.size
})

const totalCookTime = computed(() => {
  const total = Object.values(props.meals).reduce((sum, meal) => sum + (meal.cookTime || 0), 0)
  return Math.round(total / 60 * 10) / 10
})

function getWeekStart(date) {
  const d = new Date(date)
  const day = d.getDay()
  const diff = d.getDate() - day + (day === 0 ? -6 : 1)
  const weekStart = new Date(d.setDate(diff))
  weekStart.setHours(0, 0, 0, 0)
  return weekStart
}

function getWeekNumber(date) {
  const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()))
  const dayNum = d.getUTCDay() || 7
  d.setUTCDate(d.getUTCDate() + 4 - dayNum)
  const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1))
  return Math.ceil((((d - yearStart) / 86400000) + 1) / 7)
}

function formatDate(date) {
  return date.toISOString().split('T')[0]
}

function isToday(dateStr) {
  const today = formatDate(new Date())
  return dateStr === today
}

function getMeal(date, mealType) {
  return props.meals[`${date}-${mealType}`]
}

function toggleMealType(mealTypeId) {
  const index = enabledMealTypes.value.indexOf(mealTypeId)
  if (index > -1) {
    enabledMealTypes.value.splice(index, 1)
  } else {
    enabledMealTypes.value.push(mealTypeId)
  }
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
  max-width: 100%;
  margin: 0 auto;
}

.calendar-header {
  margin-bottom: 2rem;
}

.calendar-header h1 {
  font-size: 2rem;
  font-weight: 700;
  color: #2c3e50;
  margin: 0 0 1.5rem 0;
}

.week-navigation {
  display: flex;
  align-items: center;
  gap: 1rem;
  justify-content: center;
  margin-bottom: 1.5rem;
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
  min-width: 250px;
  text-align: center;
}

.meal-type-selector {
  background: white;
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.meal-type-selector h3 {
  font-size: 1rem;
  font-weight: 600;
  color: #2c3e50;
  margin: 0 0 1rem 0;
}

.meal-toggles {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.meal-toggle {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  background: #f9fdf7;
  border: 2px solid #D4E7C5;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
  user-select: none;
}

.meal-toggle input {
  display: none;
}

.meal-toggle:hover {
  background: #f0f8eb;
  border-color: #8CB369;
}

.meal-toggle.active {
  background: #8CB369;
  color: white;
  border-color: #8CB369;
}

.meal-toggle .meal-icon {
  font-size: 1.25rem;
}

.meal-toggle .meal-name {
  font-weight: 600;
  font-size: 0.95rem;
}

.calendar-container {
  overflow-x: auto;
  margin-bottom: 2rem;
}

.calendar-grid {
  min-width: 900px;
  background: white;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.day-headers {
  display: grid;
  grid-template-columns: 120px repeat(7, 1fr);
  background: #8CB369;
  color: white;
  position: sticky;
  top: 0;
  z-index: 10;
}

.meal-label-column {
  border-right: 1px solid rgba(255, 255, 255, 0.2);
}

.day-header {
  padding: 1rem;
  text-align: center;
  border-right: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.3s;
}

.day-header.today {
  background: rgba(255, 255, 255, 0.2);
  font-weight: 700;
}

.day-header:last-child {
  border-right: none;
}

.day-name {
  font-weight: 600;
  font-size: 0.9rem;
  text-transform: uppercase;
  margin-bottom: 0.25rem;
}

.day-date {
  font-size: 0.85rem;
  opacity: 0.9;
}

.meal-row {
  display: grid;
  grid-template-columns: 120px repeat(7, 1fr);
  border-bottom: 1px solid #f0f0f0;
}

.meal-row:last-child {
  border-bottom: none;
}

.meal-label {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  background: #f9fdf7;
  border-right: 1px solid #f0f0f0;
  font-weight: 600;
  color: #2c3e50;
}

.meal-label .meal-icon {
  font-size: 1.5rem;
}

.meal-label .meal-name {
  font-size: 0.95rem;
}

.meal-cell {
  min-height: 120px;
  border-right: 1px solid #f0f0f0;
  cursor: pointer;
  transition: all 0.2s;
  position: relative;
}

.meal-cell:hover {
  background: #f9fdf7;
}

.meal-cell:last-child {
  border-right: none;
}

.meal-content {
  padding: 0.75rem;
  height: 100%;
  position: relative;
}

.meal-thumb {
  width: 100%;
  height: 70px;
  object-fit: cover;
  border-radius: 8px;
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
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  line-height: 1.3;
}

.meal-meta {
  color: #888;
  font-size: 0.8rem;
}

.btn-remove-meal {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
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
  z-index: 5;
}

.meal-cell:hover .btn-remove-meal {
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
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: #D4E7C5;
}

.plus-icon {
  width: 32px;
  height: 32px;
  stroke-width: 2;
  transition: all 0.3s;
}

.meal-cell:hover .plus-icon {
  color: #8CB369;
  transform: scale(1.2);
}

.calendar-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
}

.stat-card {
  background: white;
  border-radius: 16px;
  padding: 1.5rem;
  text-align: center;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.stat-value {
  font-size: 2rem;
  font-weight: 700;
  color: #8CB369;
  margin-bottom: 0.5rem;
}

.stat-label {
  font-size: 0.9rem;
  color: #888;
}

@media (max-width: 1200px) {
  .calendar-grid {
    min-width: 800px;
  }
  
  .day-headers,
  .meal-row {
    grid-template-columns: 100px repeat(7, 1fr);
  }
}

@media (max-width: 768px) {
  .weekly-calendar {
    padding: 1rem;
  }
  
  .calendar-header h1 {
    font-size: 1.5rem;
  }
  
  .week-label {
    font-size: 0.9rem;
    min-width: 200px;
  }
  
  .meal-toggles {
    gap: 0.5rem;
  }
  
  .meal-toggle {
    padding: 0.5rem 0.75rem;
    font-size: 0.85rem;
  }
}
</style>