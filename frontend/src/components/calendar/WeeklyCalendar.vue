<template>
  <div style="padding: 24px 16px; max-width: 1400px; margin: 0 auto; background: #fefdf8; min-height: 100vh;">
    <!-- Header -->
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; flex-wrap: wrap; gap: 16px;">
      <h2 style="font-size: 32px; font-weight: 800; color: #1f2937;">Veckokalender</h2>
      
      <div style="display: flex; align-items: center; gap: 16px;">
        <button 
          @click="previousWeek"
          style="width: 40px; height: 40px; border-radius: 50%; background: #f3f4f6; border: none; display: flex; align-items: center; justify-content: center; cursor: pointer; transition: all 0.2s;"
          @mouseenter="$event.target.style.background = '#e5e7eb'"
          @mouseleave="$event.target.style.background = '#f3f4f6'"
        >
          <svg style="width: 20px; height: 20px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="15 18 9 12 15 6"/>
          </svg>
        </button>
        
        <span style="font-weight: 700; min-width: 140px; text-align: center; color: #1f2937; font-size: 16px;">
          {{ weekLabel }}
        </span>
        
        <button 
          @click="nextWeek"
          style="width: 40px; height: 40px; border-radius: 50%; background: #f3f4f6; border: none; display: flex; align-items: center; justify-content: center; cursor: pointer; transition: all 0.2s;"
          @mouseenter="$event.target.style.background = '#e5e7eb'"
          @mouseleave="$event.target.style.background = '#f3f4f6'"
        >
          <svg style="width: 20px; height: 20px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="9 18 15 12 9 6"/>
          </svg>
        </button>
      </div>
    </div>

    <!-- Meal Type Filters -->
    <div style="background: white; border-radius: 16px; padding: 20px; margin-bottom: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.08);">
      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px;">
        <h3 style="font-size: 18px; font-weight: 700; color: #1f2937;">Visa måltider</h3>
        <div style="display: flex; gap: 8px;">
          <button 
            @click="selectAllMeals"
            style="padding: 6px 12px; background: #d1fae5; color: #047857; border: none; border-radius: 8px; font-size: 12px; font-weight: 600; cursor: pointer; transition: all 0.2s;"
            @mouseenter="$event.target.style.background = '#a7f3d0'"
            @mouseleave="$event.target.style.background = '#d1fae5'"
          >
            Välj alla
          </button>
          <button 
            @click="deselectAllMeals"
            style="padding: 6px 12px; background: #fee2e2; color: #991b1b; border: none; border-radius: 8px; font-size: 12px; font-weight: 600; cursor: pointer; transition: all 0.2s;"
            @mouseenter="$event.target.style.background = '#fecaca'"
            @mouseleave="$event.target.style.background = '#fee2e2'"
          >
            Rensa alla
          </button>
        </div>
      </div>

      <div style="display: flex; gap: 10px; flex-wrap: wrap;">
        <button 
          v-for="mealType in allMealTypes" 
          :key="mealType.value"
          @click="toggleMealType(mealType.value)"
          style="padding: 10px 20px; border-radius: 20px; font-weight: 600; border: 2px solid; cursor: pointer; transition: all 0.2s; display: flex; align-items: center; gap: 8px; font-size: 14px;"
          :style="visibleMealTypes.includes(mealType.value)
            ? 'background: #10b981; color: white; border-color: #10b981; box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);'
            : 'background: white; color: #6b7280; border-color: #e5e7eb;'"
          @mouseenter="visibleMealTypes.includes(mealType.value) ? $event.target.style.background = '#059669' : $event.target.style.background = '#f9fafb'"
          @mouseleave="visibleMealTypes.includes(mealType.value) ? $event.target.style.background = '#10b981' : $event.target.style.background = 'white'"
        >
          <span style="font-size: 18px;">{{ mealType.icon }}</span>
          <span>{{ mealType.label }}</span>
        </button>
      </div>

      <p style="margin-top: 12px; font-size: 13px; color: #6b7280;">
        {{ visibleMealTypes.length }} av {{ allMealTypes.length }} måltider valda
      </p>
    </div>
    
    <!-- Calendar Grid -->
    <div style="background: white; border-radius: 16px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.08);">
      <!-- Header Row -->
      <div style="display: grid; border-bottom: 2px solid #e5e7eb; background: #d1fae5;" :style="`grid-template-columns: 120px repeat(7, 1fr);`">
        <div style="border-right: 2px solid #e5e7eb; padding: 16px; text-align: center; font-weight: 700; color: #047857;">
          Måltid
        </div>
        
        <div 
          v-for="day in weekDays" 
          :key="day.date"
          style="padding: 16px; text-align: center; border-right: 1px solid #e5e7eb; transition: all 0.2s;"
          :style="isToday(day.date) ? 'background: #10b981; color: white;' : ''"
        >
          <div style="font-weight: 700; font-size: 14px; margin-bottom: 6px;">{{ day.name }}</div>
          <div style="font-size: 20px; font-weight: 700;">{{ day.dayNum }}</div>
        </div>
      </div>
      
      <!-- No meals selected message -->
      <div v-if="visibleMealTypes.length === 0" style="padding: 60px 20px; text-align: center; color: #6b7280;">
        <div style="font-size: 60px; margin-bottom: 16px;">🍽️</div>
        <h3 style="font-size: 20px; font-weight: 700; color: #1f2937; margin-bottom: 8px;">Inga måltider valda</h3>
        <p style="font-size: 14px;">Välj måltider ovan för att visa dem i kalendern</p>
      </div>

      <!-- Meal Rows -->
      <div 
        v-for="mealType in filteredMealTypes" 
        :key="mealType.value"
        style="display: grid; border-bottom: 1px solid #e5e7eb;"
        :style="`grid-template-columns: 120px repeat(7, 1fr);`"
      >
        <div style="padding: 16px; font-weight: 600; border-right: 2px solid #e5e7eb; background: #fefdf8; display: flex; align-items: center; justify-content: center; text-align: center; font-size: 14px;">
          {{ mealType.icon }} {{ mealType.label }}
        </div>
        
        <div 
          v-for="day in weekDays" 
          :key="`${day.date}-${mealType.value}`"
          @click="handleCellClick(day.date, mealType.value)"
          style="padding: 8px; border-right: 1px solid #e5e7eb; min-height: 100px; cursor: pointer; transition: background 0.2s; position: relative;"
          @mouseenter="$event.target.style.background = '#fefdf8'"
          @mouseleave="$event.target.style.background = 'white'"
        >
          <!-- Meal Card -->
          <div v-if="getMeal(day.date, mealType.value)" style="position: relative; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 4px rgba(0,0,0,0.1); transition: all 0.2s;">
            <div style="width: 100%; height: 60px; overflow: hidden;">
              <img 
                :src="getMeal(day.date, mealType.value).imageUrl" 
                :alt="getMeal(day.date, mealType.value).title"
                style="width: 100%; height: 100%; object-fit: cover;"
              />
            </div>
            <div style="padding: 6px; font-size: 13px; font-weight: 600; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">
              {{ getMeal(day.date, mealType.value).title }}
            </div>
            
            <!-- Remove Button -->
            <button 
              @click.stop="removeMeal(day.date, mealType.value)"
              style="position: absolute; top: 4px; right: 4px; width: 24px; height: 24px; border-radius: 50%; border: none; background: rgba(239, 68, 68, 0.95); color: white; cursor: pointer; font-size: 16px; line-height: 1; opacity: 0; transition: all 0.2s; display: flex; align-items: center; justify-content: center;"
              @mouseenter="$event.target.style.opacity = '1'; $event.target.style.transform = 'scale(1.1)'"
              @mouseleave="$event.target.style.transform = 'scale(1)'"
            >×</button>
          </div>
          
          <!-- Empty Cell -->
          <div v-else style="height: 100%; display: flex; align-items: center; justify-content: center; color: #9ca3af; opacity: 0; transition: opacity 0.2s;">
            <svg style="width: 24px; height: 24px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
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
const visibleMealTypes = ref(['breakfast', 'lunch', 'dinner']) // Default visible meals

const allMealTypes = [
  { value: 'breakfast', label: 'Frukost', icon: '🌅' },
  { value: 'brunch', label: 'Brunch', icon: '🥐' },
  { value: 'lunch', label: 'Lunch', icon: '🍽️' },
  { value: 'snack', label: 'Mellanmål', icon: '🍎' },
  { value: 'dinner', label: 'Middag', icon: '🌙' },
  { value: 'dessert', label: 'Dessert', icon: '🍰' }
]

const filteredMealTypes = computed(() => {
  return allMealTypes.filter(mt => visibleMealTypes.value.includes(mt.value))
})

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

function toggleMealType(mealTypeValue) {
  const index = visibleMealTypes.value.indexOf(mealTypeValue)
  if (index > -1) {
    visibleMealTypes.value.splice(index, 1)
  } else {
    visibleMealTypes.value.push(mealTypeValue)
  }
}

function selectAllMeals() {
  visibleMealTypes.value = allMealTypes.map(mt => mt.value)
}

function deselectAllMeals() {
  visibleMealTypes.value = []
}

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

<style>
/* Show remove button on hover */
div:hover > button[style*="opacity: 0"] {
  opacity: 1 !important;
}

/* Show plus icon on empty cell hover */
div:hover > div[style*="opacity: 0"] {
  opacity: 1 !important;
}
</style>