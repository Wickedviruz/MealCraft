// src/composables/useShoppingList.js

import { ref, onMounted } from 'vue'

const STORAGE_KEY = 'mealcraft-shopping'

export function useShoppingList() {
  const shoppingItems = ref([])

  function generateFromMeals(meals) {
    const ingredientsMap = {}
    
    Object.values(meals).forEach(recipe => {
      if (!recipe.ingredients) return
      
      recipe.ingredients.forEach(ing => {
        const key = ing.name.toLowerCase()
        
        if (ingredientsMap[key]) {
          ingredientsMap[key].amount += ing.amount
        } else {
          ingredientsMap[key] = {
            id: `${Date.now()}-${Math.random()}`,
            name: ing.name,
            amount: ing.amount,
            unit: ing.unit,
            category: ing.category || 'Other',
            checked: false
          }
        }
      })
    })
    
    shoppingItems.value = Object.values(ingredientsMap)
      .sort((a, b) => a.category.localeCompare(b.category))
    
    saveToStorage()
  }

  function toggleItem(itemId) {
    const item = shoppingItems.value.find(i => i.id === itemId)
    if (item) {
      item.checked = !item.checked
      saveToStorage()
    }
  }

  function clearCheckedItems() {
    shoppingItems.value = shoppingItems.value.filter(item => !item.checked)
    saveToStorage()
  }

  function addItem(name, amount, unit, category = 'Other') {
    const item = {
      id: `${Date.now()}-${Math.random()}`,
      name,
      amount,
      unit,
      category,
      checked: false
    }
    shoppingItems.value.push(item)
    saveToStorage()
  }

  function removeItem(itemId) {
    shoppingItems.value = shoppingItems.value.filter(i => i.id !== itemId)
    saveToStorage()
  }

  function saveToStorage() {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(shoppingItems.value))
  }

  function loadFromStorage() {
    try {
      const saved = localStorage.getItem(STORAGE_KEY)
      if (saved) {
        shoppingItems.value = JSON.parse(saved)
      }
    } catch (err) {
      console.error('Failed to load shopping list:', err)
    }
  }

  onMounted(() => {
    loadFromStorage()
  })

  return {
    shoppingItems,
    generateFromMeals,
    toggleItem,
    clearCheckedItems,
    addItem,
    removeItem
  }
}