<template>
  <div id="app">
    <header class="app-header">
      <h1 class="app-logo">MealCraft</h1>
    </header>
    
    <main class="app-main">
      <!-- Discover View -->
      <div v-if="currentView === 'discover'" class="discover-view">
        <div v-if="currentRecipe" class="swipe-container">
          <RecipeCard 
            :recipe="currentRecipe"
            @swipe-left="handleDislike"
            @swipe-right="handleLike"
          />
          <SwipeButtons 
            @like="handleLike"
            @dislike="handleDislike"
          />
        </div>
        <div v-else class="empty-state">
          <h2>No more recipes!</h2>
          <p>You've seen all available recipes</p>
          <button @click="resetRecipes" class="btn-reset">Start Over</button>
        </div>
      </div>
      
      <!-- Saved View -->
      <SavedRecipes 
        v-if="currentView === 'saved'"
        :recipes="savedRecipes"
        @remove-recipe="removeSavedRecipe"
      />
      
      <!-- Calendar View -->
      <WeeklyCalendar 
        v-if="currentView === 'calendar'"
        :meals="calendarMeals"
        @add-meal="openMealSelector"
        @remove-meal="removeMealFromCalendar"
      />
      
      <!-- Shopping List View -->
      <ShoppingList 
        v-if="currentView === 'shopping'"
        :items="shoppingItems"
        @toggle-item="toggleShoppingItem"
        @clear-checked="clearCheckedItems"
        @generate-list="generateShoppingList"
      />
    </main>
    
    <BottomNav v-model="currentView" />
    
    <!-- Meal Selector Modal -->
    <div v-if="showMealSelector" class="modal-overlay" @click="closeMealSelector">
      <div class="modal-content" @click.stop>
        <h2>Select a Recipe</h2>
        <div class="recipe-selector-grid">
          <div 
            v-for="recipe in savedRecipes" 
            :key="recipe.id"
            class="selector-item"
            @click="addMealToCalendar(recipe)"
          >
            <img :src="recipe.image" :alt="recipe.title" />
            <div class="selector-title">{{ recipe.title }}</div>
          </div>
        </div>
        <button @click="closeMealSelector" class="btn-close">Close</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import RecipeCard from './components/RecipeCard.vue'
import SwipeButtons from './components/SwipeButtons.vue'
import SavedRecipes from './components/SavedRecipes.vue'
import WeeklyCalendar from './components/WeeklyCalendar.vue'
import ShoppingList from './components/ShoppingList.vue'
import BottomNav from './components/BottomNav.vue'

// State
const currentView = ref('discover')
const currentRecipeIndex = ref(0)
const savedRecipes = ref([])
const calendarMeals = ref({})
const shoppingItems = ref([])
const showMealSelector = ref(false)
const selectedMealSlot = ref(null)

// Mock recipe data
const allRecipes = ref([
  {
    id: 1,
    title: 'Thai Basil Chicken Stir-Fry',
    image: 'https://images.unsplash.com/photo-1455619452474-d2be8b1e70cd?w=800',
    tags: ['Quick', 'Asian'],
    cookTime: 15,
    servings: 4,
    calories: 320,
    ingredients: [
      { name: 'Chicken breast', amount: 500, unit: 'g', category: 'Meat' },
      { name: 'Thai basil', amount: 1, unit: 'bunch', category: 'Produce' },
      { name: 'Soy sauce', amount: 3, unit: 'tbsp', category: 'Pantry' }
    ]
  },
  {
    id: 2,
    title: 'Mediterranean Quinoa Bowl',
    image: 'https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=800',
    tags: ['Healthy', 'Vegetarian'],
    cookTime: 25,
    servings: 2,
    calories: 380,
    ingredients: [
      { name: 'Quinoa', amount: 200, unit: 'g', category: 'Grains' },
      { name: 'Cherry tomatoes', amount: 200, unit: 'g', category: 'Produce' },
      { name: 'Feta cheese', amount: 100, unit: 'g', category: 'Dairy' }
    ]
  },
  {
    id: 3,
    title: 'Spicy Shrimp Tacos',
    image: 'https://images.unsplash.com/photo-1551504734-5ee1c4a1479b?w=800',
    tags: ['Mexican', 'Seafood'],
    cookTime: 20,
    servings: 4,
    calories: 280,
    ingredients: [
      { name: 'Shrimp', amount: 400, unit: 'g', category: 'Meat' },
      { name: 'Corn tortillas', amount: 8, unit: 'pcs', category: 'Grains' },
      { name: 'Avocado', amount: 2, unit: 'pcs', category: 'Produce' }
    ]
  },
  {
    id: 4,
    title: 'Creamy Mushroom Risotto',
    image: 'https://images.unsplash.com/photo-1476124369976-7cd74bf99ab6?w=800',
    tags: ['Italian', 'Comfort'],
    cookTime: 35,
    servings: 4,
    calories: 420,
    ingredients: [
      { name: 'Arborio rice', amount: 300, unit: 'g', category: 'Grains' },
      { name: 'Mushrooms', amount: 400, unit: 'g', category: 'Produce' },
      { name: 'Parmesan', amount: 100, unit: 'g', category: 'Dairy' }
    ]
  },
  {
    id: 5,
    title: 'Grilled Salmon with Asparagus',
    image: 'https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=800',
    tags: ['Healthy', 'Quick'],
    cookTime: 18,
    servings: 2,
    calories: 350,
    ingredients: [
      { name: 'Salmon fillet', amount: 400, unit: 'g', category: 'Meat' },
      { name: 'Asparagus', amount: 300, unit: 'g', category: 'Produce' },
      { name: 'Lemon', amount: 1, unit: 'pcs', category: 'Produce' }
    ]
  }
])

const currentRecipe = computed(() => {
  return allRecipes.value[currentRecipeIndex.value]
})

// Recipe swiping
function handleLike() {
  if (currentRecipe.value) {
    savedRecipes.value.push({ ...currentRecipe.value })
    saveToLocalStorage()
    nextRecipe()
  }
}

function handleDislike() {
  nextRecipe()
}

function nextRecipe() {
  currentRecipeIndex.value++
}

function resetRecipes() {
  currentRecipeIndex.value = 0
}

function removeSavedRecipe(recipeId) {
  savedRecipes.value = savedRecipes.value.filter(r => r.id !== recipeId)
  saveToLocalStorage()
}

// Calendar
function openMealSelector({ date, mealType }) {
  selectedMealSlot.value = { date, mealType }
  showMealSelector.value = true
}

function closeMealSelector() {
  showMealSelector.value = false
  selectedMealSlot.value = null
}

function addMealToCalendar(recipe) {
  if (selectedMealSlot.value) {
    const { date, mealType } = selectedMealSlot.value
    const key = `${date}-${mealType}`
    calendarMeals.value[key] = recipe
    saveToLocalStorage()
    closeMealSelector()
    generateShoppingList()
  }
}

function removeMealFromCalendar({ date, mealType }) {
  const key = `${date}-${mealType}`
  delete calendarMeals.value[key]
  saveToLocalStorage()
  generateShoppingList()
}

// Shopping list
function generateShoppingList() {
  const ingredientsMap = {}
  
  Object.values(calendarMeals.value).forEach(recipe => {
    recipe.ingredients.forEach(ingredient => {
      const key = ingredient.name
      if (ingredientsMap[key]) {
        ingredientsMap[key].amount += ingredient.amount
      } else {
        ingredientsMap[key] = {
          id: `${Date.now()}-${Math.random()}`,
          name: ingredient.name,
          amount: ingredient.amount,
          unit: ingredient.unit,
          category: ingredient.category,
          checked: false
        }
      }
    })
  })
  
  shoppingItems.value = Object.values(ingredientsMap)
  saveToLocalStorage()
}

function toggleShoppingItem(itemId) {
  const item = shoppingItems.value.find(i => i.id === itemId)
  if (item) {
    item.checked = !item.checked
    saveToLocalStorage()
  }
}

function clearCheckedItems() {
  shoppingItems.value = shoppingItems.value.filter(item => !item.checked)
  saveToLocalStorage()
}

// LocalStorage
function saveToLocalStorage() {
  localStorage.setItem('mealswipe-saved', JSON.stringify(savedRecipes.value))
  localStorage.setItem('mealswipe-calendar', JSON.stringify(calendarMeals.value))
  localStorage.setItem('mealswipe-shopping', JSON.stringify(shoppingItems.value))
}

function loadFromLocalStorage() {
  const saved = localStorage.getItem('mealswipe-saved')
  const calendar = localStorage.getItem('mealswipe-calendar')
  const shopping = localStorage.getItem('mealswipe-shopping')
  
  if (saved) savedRecipes.value = JSON.parse(saved)
  if (calendar) calendarMeals.value = JSON.parse(calendar)
  if (shopping) shoppingItems.value = JSON.parse(shopping)
}

onMounted(() => {
  loadFromLocalStorage()
})
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
  background: #FEFDF8;
  color: #2c3e50;
}

#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.app-header {
  background: white;
  padding: 1rem 2rem;
  border-bottom: 1px solid #D4E7C5;
  text-align: center;
  position: sticky;
  top: 0;
  z-index: 50;
}

.app-logo {
  font-size: 1.75rem;
  font-weight: 700;
  color: #2c3e50;
}

.app-main {
  flex: 1;
  padding-bottom: 80px;
}

.discover-view {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: calc(100vh - 180px);
  padding: 2rem;
}

.swipe-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.empty-state {
  text-align: center;
  color: #888;
}

.empty-state h2 {
  font-size: 1.75rem;
  color: #555;
  margin-bottom: 1rem;
}

.btn-reset {
  margin-top: 1.5rem;
  padding: 0.75rem 2rem;
  background: #8CB369;
  color: white;
  border: none;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-reset:hover {
  background: #7aa359;
  transform: translateY(-2px);
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
}

.modal-content {
  background: white;
  border-radius: 20px;
  padding: 2rem;
  max-width: 600px;
  width: 100%;
  max-height: 80vh;
  overflow-y: auto;
}

.modal-content h2 {
  margin-bottom: 1.5rem;
  color: #2c3e50;
}

.recipe-selector-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.selector-item {
  cursor: pointer;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: all 0.2s;
}

.selector-item:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.selector-item img {
  width: 100%;
  height: 120px;
  object-fit: cover;
}

.selector-title {
  padding: 0.75rem;
  font-size: 0.9rem;
  font-weight: 600;
  color: #2c3e50;
}

.btn-close {
  width: 100%;
  padding: 0.75rem;
  background: #D4E7C5;
  border: none;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-close:hover {
  background: #c0d9b3;
}
</style>