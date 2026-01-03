// src/composables/useCalendar.js

import { ref, onMounted } from 'vue'

const STORAGE_KEY = 'mealcraft-calendar'

export function useCalendar() {
  const calendarMeals = ref({})

  function getMeal(date, mealType) {
    const key = `${date}-${mealType}`
    return calendarMeals.value[key]
  }

  function addMeal(date, mealType, recipe) {
    const key = `${date}-${mealType}`
    calendarMeals.value[key] = recipe
    saveToStorage()
  }

  function removeMeal(date, mealType) {
    const key = `${date}-${mealType}`
    delete calendarMeals.value[key]
    saveToStorage()
  }

  function getMealsForDateRange(startDate, endDate) {
    return Object.entries(calendarMeals.value)
      .filter(([key]) => {
        const date = key.split('-')[0]
        return date >= startDate && date <= endDate
      })
      .map(([key, recipe]) => {
        const [date, mealType] = key.split('-')
        return { date, mealType, recipe }
      })
  }

  function saveToStorage() {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(calendarMeals.value))
  }

  function loadFromStorage() {
    try {
      const saved = localStorage.getItem(STORAGE_KEY)
      if (saved) {
        calendarMeals.value = JSON.parse(saved)
      }
    } catch (err) {
      console.error('Failed to load calendar:', err)
    }
  }

  onMounted(() => {
    loadFromStorage()
  })

  return {
    calendarMeals,
    getMeal,
    addMeal,
    removeMeal,
    getMealsForDateRange
  }
}