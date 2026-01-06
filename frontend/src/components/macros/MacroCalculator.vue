<template>
  <div style="min-height: 100vh; overflow: auto; padding: 16px; padding-bottom: 100px; background: #fefdf8;">
    <div style="max-width: 600px; margin: 0 auto;">
      <!-- Header -->
      <div style="text-align: center; margin-bottom: 32px;">
        <div style="display: inline-flex; align-items: center; justify-content: center; width: 56px; height: 56px; border-radius: 50%; background: #d1fae5; margin-bottom: 16px;">
          <svg style="width: 28px; height: 28px; color: #10b981;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="3" width="18" height="18" rx="2"/>
            <line x1="9" y1="9" x2="9" y2="15"/>
            <line x1="15" y1="9" x2="15" y2="15"/>
            <line x1="9" y1="12" x2="15" y2="12"/>
          </svg>
        </div>
        <h2 style="font-size: 32px; font-weight: 800; color: #1f2937; margin-bottom: 8px;">Makrokalkylator</h2>
        <p style="color: #6b7280; font-size: 14px;">Beräkna dina dagliga makron baserat på din kropp och dina mål</p>
      </div>

      <!-- Input Form -->
      <div style="background: white; border-radius: 16px; padding: 24px; box-shadow: 0 2px 8px rgba(0,0,0,0.08); margin-bottom: 24px;">
        <div style="display: flex; align-items: center; gap: 8px; margin-bottom: 20px;">
          <svg style="width: 20px; height: 20px; color: #10b981;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M22 12h-4l-3 9L9 3l-3 9H2"/>
          </svg>
          <h3 style="font-size: 18px; font-weight: 700; color: #1f2937;">Dina uppgifter</h3>
        </div>

        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 16px;">
          <div>
            <label style="display: block; font-size: 14px; font-weight: 600; color: #374151; margin-bottom: 6px;">Vikt (kg)</label>
            <input 
              v-model="weight"
              type="number"
              placeholder="70"
              style="width: 100%; padding: 12px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 16px; outline: none;"
              @focus="$event.target.style.borderColor = '#10b981'"
              @blur="$event.target.style.borderColor = '#e5e7eb'"
            />
          </div>
          <div>
            <label style="display: block; font-size: 14px; font-weight: 600; color: #374151; margin-bottom: 6px;">Längd (cm)</label>
            <input 
              v-model="height"
              type="number"
              placeholder="175"
              style="width: 100%; padding: 12px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 16px; outline: none;"
              @focus="$event.target.style.borderColor = '#10b981'"
              @blur="$event.target.style.borderColor = '#e5e7eb'"
            />
          </div>
        </div>

        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 20px;">
          <div>
            <label style="display: block; font-size: 14px; font-weight: 600; color: #374151; margin-bottom: 6px;">Ålder</label>
            <input 
              v-model="age"
              type="number"
              placeholder="25"
              style="width: 100%; padding: 12px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 16px; outline: none;"
              @focus="$event.target.style.borderColor = '#10b981'"
              @blur="$event.target.style.borderColor = '#e5e7eb'"
            />
          </div>
          <div>
            <label style="display: block; font-size: 14px; font-weight: 600; color: #374151; margin-bottom: 6px;">Kön</label>
            <select 
              v-model="sex"
              style="width: 100%; padding: 12px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 16px; outline: none; background: white; cursor: pointer;"
            >
              <option value="male">Man</option>
              <option value="female">Kvinna</option>
            </select>
          </div>
        </div>

        <button 
          @click="calculateMacros"
          class="btn btn-primary"
          style="width: 100%; display: flex; align-items: center; justify-content: center; gap: 8px; font-size: 16px;"
        >
          <svg style="width: 20px; height: 20px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="3" width="18" height="18" rx="2"/>
            <line x1="9" y1="9" x2="9" y2="15"/>
            <line x1="15" y1="9" x2="15" y2="15"/>
            <line x1="9" y1="12" x2="15" y2="12"/>
          </svg>
          Beräkna makron
        </button>
      </div>

      <!-- Results -->
      <div v-if="results" style="animation: fadeIn 0.5s ease;">
        <div style="display: flex; align-items: center; gap: 8px; margin-bottom: 20px;">
          <svg style="width: 20px; height: 20px; color: #10b981;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="12" cy="12" r="10"/>
            <path d="M12 6v6l4 2"/>
          </svg>
          <h3 style="font-size: 20px; font-weight: 700; color: #1f2937;">Dina resultat</h3>
        </div>

        <!-- Tabs -->
        <div style="background: white; border-radius: 12px; padding: 6px; display: flex; gap: 4px; margin-bottom: 16px; box-shadow: 0 2px 8px rgba(0,0,0,0.08);">
          <button
            v-for="group in workoutGroups"
            :key="group.days"
            @click="activeWorkout = group.days"
            style="flex: 1; padding: 10px 8px; border: none; border-radius: 8px; font-weight: 600; font-size: 12px; cursor: pointer; transition: all 0.2s;"
            :style="activeWorkout === group.days 
              ? 'background: #10b981; color: white; box-shadow: 0 2px 8px rgba(16, 185, 129, 0.3);' 
              : 'background: transparent; color: #6b7280;'"
          >
            {{ group.label }}
          </button>
        </div>

        <!-- Active workout info -->
        <p style="font-size: 14px; color: #6b7280; margin-bottom: 16px; text-align: center;">
          {{ workoutGroups.find(g => g.days === activeWorkout)?.description }}
        </p>

        <!-- Goal Cards -->
        <div style="display: flex; flex-direction: column; gap: 12px;">
          <div 
            v-for="goal in goals"
            :key="goal.value"
            style="background: white; border-radius: 12px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.08); border-left: 4px solid;"
            :style="`border-left-color: ${goal.color};`"
          >
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px;">
              <div style="display: flex; align-items: center; gap: 8px;">
                <svg :style="`width: 20px; height: 20px; color: ${goal.color};`" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <component :is="'path'" :d="goal.icon" />
                </svg>
                <span style="font-weight: 600; color: #1f2937; font-size: 16px;">{{ goal.label }}</span>
              </div>
              <span style="font-size: 24px; font-weight: 800; color: #10b981;">
                {{ getResult(activeWorkout, goal.value).calories }} kcal
              </span>
            </div>

            <div style="display: grid; grid-template-columns: 1fr 1fr 1fr; gap: 8px;">
              <div style="background: #f9fafb; border-radius: 8px; padding: 12px; text-align: center;">
                <p style="font-size: 11px; color: #6b7280; margin-bottom: 4px; text-transform: uppercase; letter-spacing: 0.5px;">Protein</p>
                <p style="font-size: 18px; font-weight: 700; color: #1f2937;">{{ getResult(activeWorkout, goal.value).protein }}g</p>
              </div>
              <div style="background: #f9fafb; border-radius: 8px; padding: 12px; text-align: center;">
                <p style="font-size: 11px; color: #6b7280; margin-bottom: 4px; text-transform: uppercase; letter-spacing: 0.5px;">Kolhydrater</p>
                <p style="font-size: 18px; font-weight: 700; color: #1f2937;">{{ getResult(activeWorkout, goal.value).carbs }}g</p>
              </div>
              <div style="background: #f9fafb; border-radius: 8px; padding: 12px; text-align: center;">
                <p style="font-size: 11px; color: #6b7280; margin-bottom: 4px; text-transform: uppercase; letter-spacing: 0.5px;">Fett</p>
                <p style="font-size: 18px; font-weight: 700; color: #1f2937;">{{ getResult(activeWorkout, goal.value).fat }}g</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const weight = ref('')
const height = ref('')
const age = ref('')
const sex = ref('male')
const results = ref(null)
const activeWorkout = ref(0)

const workoutGroups = [
  { days: 0, label: '0', description: 'Ingen träning (Stillasittande)' },
  { days: 2, label: '1-2', description: '1-2 dagar/vecka (Lätt aktivitet)' },
  { days: 4, label: '3-4', description: '3-4 dagar/vecka (Måttlig aktivitet)' },
  { days: 6, label: '5-6', description: '5-6 dagar/vecka (Hög aktivitet)' }
]

const goals = [
  { 
    value: 'lose', 
    label: 'Gå ner i vikt', 
    color: '#ef4444',
    icon: 'M3 17l6-6 4 4 8-8M21 7v6h-6'
  },
  { 
    value: 'maintain', 
    label: 'Behåll vikt', 
    color: '#10b981',
    icon: 'M5 12h14'
  },
  { 
    value: 'gain', 
    label: 'Öka i vikt', 
    color: '#3b82f6',
    icon: 'M3 17l6-6 4 4 8-8M17 11h4v4'
  }
]

const activityMultipliers = {
  0: 1.2,
  1: 1.375,
  2: 1.375,
  3: 1.55,
  4: 1.55,
  5: 1.725,
  6: 1.725
}

const goalAdjustments = {
  lose: -500,
  maintain: 0,
  gain: 500
}

function calculateBMR() {
  const w = parseFloat(weight.value)
  const h = parseFloat(height.value)
  const a = parseFloat(age.value)

  if (isNaN(w) || isNaN(h) || isNaN(a)) return null

  // Mifflin-St Jeor Equation
  if (sex.value === 'male') {
    return 10 * w + 6.25 * h - 5 * a + 5
  } else {
    return 10 * w + 6.25 * h - 5 * a - 161
  }
}

function calculateMacrosForCalories(calories) {
  const protein = Math.round((calories * 0.3) / 4)
  const carbs = Math.round((calories * 0.4) / 4)
  const fat = Math.round((calories * 0.3) / 9)
  
  return { 
    calories: Math.round(calories), 
    protein, 
    carbs, 
    fat 
  }
}

function calculateMacros() {
  const bmr = calculateBMR()
  if (!bmr) {
    alert('Vänligen fyll i alla fält')
    return
  }

  const allResults = {}

  workoutGroups.forEach(group => {
    const tdee = bmr * activityMultipliers[group.days]
    allResults[group.days] = {}

    goals.forEach(goal => {
      const adjustedCalories = tdee + goalAdjustments[goal.value]
      allResults[group.days][goal.value] = calculateMacrosForCalories(adjustedCalories)
    })
  })

  results.value = allResults
  activeWorkout.value = 0
}

function getResult(workoutDays, goal) {
  if (!results.value) return { calories: 0, protein: 0, carbs: 0, fat: 0 }
  return results.value[workoutDays][goal]
}
</script>

<style>
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>