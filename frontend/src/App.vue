<template>
  <div id="app">
    <!-- Loading State -->
    <div v-if="isLoading" class="fixed inset-0 bg-[#fefdf8]/95 flex flex-col items-center justify-center z-[300]">
      <div class="spinner"></div>
      <p class="mt-4 text-gray-600">Laddar recept...</p>
    </div>

    <!-- Error State -->
    <div v-if="error" class="m-4 bg-red-50 border-2 border-red-400 rounded-lg p-4 text-center">
      <p class="mb-3 text-red-600">⚠️ {{ error }}</p>
      <button @click="retryLoad" class="btn btn-primary">Försök igen</button>
    </div>

    <!-- Header -->
    <AppHeader />
    
    <!-- Main Content -->
    <main class="flex-1 pb-24">
      <DiscoverView 
        v-if="currentView === 'discover'"
        :currentRecipe="currentRecipe"
        :nextRecipe="nextRecipe"
        :thirdRecipe="thirdRecipe"
        @like="handleLike"
        @dislike="handleDislike"
        @reset="resetRecipes"
      />
      
      <SavedRecipes 
        v-if="currentView === 'saved'"
        :recipes="savedRecipes"
        @select="viewRecipeDetail"
        @remove="removeSavedRecipe"
      />
      
      <RecipeBrowser 
        v-if="currentView === 'browse'"
        :recipes="allRecipes"
        :savedIds="savedRecipes.map(r => r.id)"
        @select="viewRecipeDetail"
        @toggle-save="toggleSaveFromId"
      />
      
      <AddRecipe 
        v-if="currentView === 'add'"
        @save="handleCreateRecipe"
        @cancel="currentView = 'discover'"
      />

      <MacroCalculator 
        v-if="currentView === 'macros'"
      />
      
      <WeeklyCalendar 
        v-if="currentView === 'calendar'"
        :meals="calendarMeals"
        @add-meal="openMealSelector"
        @remove-meal="handleRemoveMeal"
      />
      
      <ShoppingList 
        v-if="currentView === 'shopping'"
        :items="shoppingItems"
        @toggle="toggleItem"
        @clear-checked="clearCheckedItems"
        @generate="handleGenerateShoppingList"
      />
    </main>
    
    <!-- Bottom Navigation -->
    <BottomNav v-model="currentView" />
    
    <!-- Recipe Detail Modal -->
    <RecipeDetail 
      v-if="selectedRecipe"
      :recipe="selectedRecipe"
      :isSaved="isSaved(selectedRecipe.id)"
      @close="closeRecipeDetail"
      @toggle-save="toggleSaveRecipe(selectedRecipe)"
    />
    
    <!-- Meal Selector Modal -->
    <MealSelector
      v-if="showMealSelector"
      :recipes="savedRecipes"
      @select="handleMealSelect"
      @close="closeMealSelector"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import AppHeader from './components/layout/AppHeader.vue'
import BottomNav from './components/layout/BottomNav.vue'
import DiscoverView from './components/discover/DiscoverView.vue'
import SavedRecipes from './components/saved/SavedRecipes.vue'
import RecipeBrowser from './components/recipe/RecipeBrowser.vue'
import AddRecipe from './components/recipe/AddRecipe.vue'
import RecipeDetail from './components/recipe/RecipeDetail.vue'
import WeeklyCalendar from './components/calendar/WeeklyCalendar.vue'
import MealSelector from './components/calendar/MealSelector.vue'
import ShoppingList from './components/shopping/ShoppingList.vue'
import MacroCalculator from './components/macros/MacroCalculator.vue'

// Composables
import { useRecipes } from './composables/useRecipes.js'
import { useSavedRecipes } from './composables/useSavedRecipes.js'
import { useSwipe } from './composables/useSwipe.js'
import { useCalendar } from './composables/useCalendar.js'
import { useShoppingList } from './composables/useShoppingList.js'

// State
const currentView = ref('discover')
const selectedRecipe = ref(null)
const showMealSelector = ref(false)
const selectedMealSlot = ref(null)

// Recipes
const { allRecipes, isLoading, error, loadRecipes, createRecipe, retryLoad } = useRecipes()
const { savedRecipes, isSaved, toggleSaveRecipe, addSavedRecipe, removeSavedRecipe } = useSavedRecipes()
const { currentRecipe, nextRecipe, thirdRecipe, handleLike, handleDislike, resetRecipes } = useSwipe(
  allRecipes,
  addSavedRecipe
)

// Calendar
const { calendarMeals, addMeal, removeMeal } = useCalendar()

// Shopping List
const { shoppingItems, generateFromMeals, toggleItem, clearCheckedItems } = useShoppingList()

// Recipe Actions
function viewRecipeDetail(recipe) {
  selectedRecipe.value = recipe
}

function closeRecipeDetail() {
  selectedRecipe.value = null
}

function toggleSaveFromId(recipeId) {
  const recipe = allRecipes.value.find(r => r.id === recipeId)
  if (recipe) toggleSaveRecipe(recipe)
}

async function handleCreateRecipe(recipeData) {
  try {
    const saved = await createRecipe(recipeData)
    addSavedRecipe(saved)
    currentView.value = 'browse'
  } catch (err) {
    alert(err.message)
  }
}

// Calendar Actions
function openMealSelector({ date, mealType }) {
  selectedMealSlot.value = { date, mealType }
  showMealSelector.value = true
}

function closeMealSelector() {
  showMealSelector.value = false
  selectedMealSlot.value = null
}

function handleMealSelect(recipe) {
  if (selectedMealSlot.value) {
    const { date, mealType } = selectedMealSlot.value
    addMeal(date, mealType, recipe)
    closeMealSelector()
  }
}

function handleRemoveMeal({ date, mealType }) {
  removeMeal(date, mealType)
}

// Shopping List Actions
function handleGenerateShoppingList() {
  generateFromMeals(calendarMeals.value)
}

// Initialize
onMounted(() => {
  loadRecipes()
})
</script>