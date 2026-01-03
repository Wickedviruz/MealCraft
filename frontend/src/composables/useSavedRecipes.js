// src/composables/useSavedRecipes.js

import { ref, onMounted } from 'vue'

const STORAGE_KEY = 'mealcraft-saved'

export function useSavedRecipes() {
  const savedRecipes = ref([])

  function isSaved(recipeId) {
    return savedRecipes.value.some(r => r.id === recipeId)
  }

  function toggleSaveRecipe(recipe) {
    const index = savedRecipes.value.findIndex(r => r.id === recipe.id)
    
    if (index > -1) {
      savedRecipes.value.splice(index, 1)
    } else {
      savedRecipes.value.push({ ...recipe })
    }
    
    saveToStorage()
  }

  function addSavedRecipe(recipe) {
    if (!isSaved(recipe.id)) {
      savedRecipes.value.push({ ...recipe })
      saveToStorage()
    }
  }

  function removeSavedRecipe(recipeId) {
    savedRecipes.value = savedRecipes.value.filter(r => r.id !== recipeId)
    saveToStorage()
  }

  function saveToStorage() {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(savedRecipes.value))
  }

  function loadFromStorage() {
    try {
      const saved = localStorage.getItem(STORAGE_KEY)
      if (saved) {
        savedRecipes.value = JSON.parse(saved)
      }
    } catch (err) {
      console.error('Failed to load saved recipes:', err)
    }
  }

  onMounted(() => {
    loadFromStorage()
  })

  return {
    savedRecipes,
    isSaved,
    toggleSaveRecipe,
    addSavedRecipe,
    removeSavedRecipe
  }
}