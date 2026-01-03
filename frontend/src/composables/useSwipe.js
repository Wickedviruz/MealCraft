// src/composables/useSwipe.js

import { ref, computed } from 'vue'

export function useSwipe(allRecipes, onLike) {
  const currentRecipeIndex = ref(0)

  const currentRecipe = computed(() => {
    return allRecipes.value[currentRecipeIndex.value]
  })

  const nextRecipe = computed(() => {
    return allRecipes.value[currentRecipeIndex.value + 1]
  })

  const thirdRecipe = computed(() => {
    return allRecipes.value[currentRecipeIndex.value + 2]
  })

  const hasMoreRecipes = computed(() => {
    return currentRecipeIndex.value < allRecipes.value.length
  })

  function handleLike() {
    if (currentRecipe.value) {
      onLike(currentRecipe.value)
      goToNextRecipe()
    }
  }

  function handleDislike() {
    goToNextRecipe()
  }

  function goToNextRecipe() {
    currentRecipeIndex.value++
  }

  function resetRecipes() {
    currentRecipeIndex.value = 0
  }

  return {
    currentRecipe,
    nextRecipe,
    thirdRecipe,
    hasMoreRecipes,
    handleLike,
    handleDislike,
    resetRecipes
  }
}