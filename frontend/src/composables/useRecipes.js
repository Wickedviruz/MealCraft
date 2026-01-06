// src/composables/useRecipes.js

import { ref } from 'vue'
import { recipeApi } from '../services/api.js'

export function useRecipes() {
  const allRecipes = ref([])
  const isLoading = ref(false)
  const error = ref(null)

  async function loadRecipes() {
    isLoading.value = true
    error.value = null
    
    try {
      const recipes = await recipeApi.getAll()
      allRecipes.value = recipes
      console.log('Loaded recipes:', recipes.length)
    } catch (err) {
      console.error('Failed to load recipes:', err)
      error.value = 'Kunde inte ladda recept från servern'
    } finally {
      isLoading.value = false
    }
  }

  async function createRecipe(recipeData) {
    try {
      const saved = await recipeApi.create(recipeData)
      allRecipes.value.push(saved)
      console.log('✅ Recipe created:', saved.title)
      return saved
    } catch (err) {
      console.error('Failed to create recipe:', err)
      throw new Error('Kunde inte spara recept: ' + err.message)
    }
  }

  async function updateRecipe(id, updates) {
    try {
      const updated = await recipeApi.update(id, updates)
      const index = allRecipes.value.findIndex(r => r.id === id)
      if (index > -1) {
        allRecipes.value[index] = updated
      }
      return updated
    } catch (err) {
      console.error('Failed to update recipe:', err)
      throw new Error('Kunde inte uppdatera recept')
    }
  }

  async function deleteRecipe(id) {
    try {
      await recipeApi.delete(id)
      allRecipes.value = allRecipes.value.filter(r => r.id !== id)
    } catch (err) {
      console.error('Failed to delete recipe:', err)
      throw new Error('Kunde inte ta bort recept')
    }
  }

  function retryLoad() {
    error.value = null
    loadRecipes()
  }

  return {
    allRecipes,
    isLoading,
    error,
    loadRecipes,
    createRecipe,
    updateRecipe,
    deleteRecipe,
    retryLoad
  }
}