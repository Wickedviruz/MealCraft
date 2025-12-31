<template>
  <div id="app">
    <header class="app-header">
      <h1 class="app-logo">MealCraft</h1>
    </header>
    
    <main class="app-main">
      <!-- Discover View -->
      <div v-if="currentView === 'discover'" class="discover-view">
        <div v-if="currentRecipe || nextRecipe" class="swipe-container">
          <!-- Card stack - next card behind -->
          <div class="card-stack">
            <div v-if="nextRecipe" class="card-stack-item card-back">
              <img :src="nextRecipe.image" :alt="nextRecipe.title" />
            </div>
            <div v-if="thirdRecipe" class="card-stack-item card-far-back">
              <img :src="thirdRecipe.image" :alt="thirdRecipe.title" />
            </div>
          </div>
          
          <!-- Current card -->
          <Transition name="card-slide" mode="out-in">
            <RecipeCard 
              v-if="currentRecipe"
              :key="currentRecipe.id"
              :recipe="currentRecipe"
              @swipe-left="handleDislike"
              @swipe-right="handleLike"
            />
          </Transition>
          
          <SwipeButtons 
            @like="handleLike"
            @dislike="handleDislike"
          />
        </div>
        <div v-else class="empty-state">
          <div class="empty-icon">🎉</div>
          <h2>No more recipes!</h2>
          <p>You've seen all available recipes</p>
          <button @click="resetRecipes" class="btn-reset">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="23 4 23 10 17 10"/>
              <path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/>
            </svg>
            Start Over
          </button>
        </div>
      </div>
      
      <!-- Saved View -->
      <SavedRecipes 
        v-if="currentView === 'saved'"
        :recipes="savedRecipes"
        @select-recipe="viewRecipeDetail"
        @remove-recipe="removeSavedRecipe"
      />
      
      <!-- Browse View -->
      <RecipeBrowser 
        v-if="currentView === 'browse'"
        :recipes="allRecipes"
        :savedRecipeIds="savedRecipes.map(r => r.id)"
        @select-recipe="viewRecipeDetail"
        @toggle-save="toggleSaveRecipe"
      />
      
      <!-- Add Recipe View -->
      <AddRecipe 
        v-if="currentView === 'add'"
        @save="addNewRecipe"
        @cancel="currentView = 'discover'"
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
    
    <!-- Recipe Detail Modal -->
    <Transition name="modal-fade">
      <div v-if="selectedRecipeForDetail" class="modal-overlay" @click="closeRecipeDetail">
        <div class="modal-wrapper" @click.stop>
          <RecipeDetail 
            :recipe="selectedRecipeForDetail"
            :isSaved="savedRecipes.some(r => r.id === selectedRecipeForDetail.id)"
            @close="closeRecipeDetail"
            @save="toggleSaveRecipe(selectedRecipeForDetail.id)"
            @add-to-calendar="addRecipeToCalendarFromDetail"
          />
        </div>
      </div>
    </Transition>
    
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
import RecipeBrowser from './components/RecipeBrowser.vue'
import RecipeDetail from './components/RecipeDetail.vue'
import AddRecipe from './components/AddRecipe.vue'

// State
const currentView = ref('discover')
const currentRecipeIndex = ref(0)
const savedRecipes = ref([])
const calendarMeals = ref({})
const shoppingItems = ref([])
const showMealSelector = ref(false)
const selectedMealSlot = ref(null)
const selectedRecipeForDetail = ref(null)

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

const nextRecipe = computed(() => {
  return allRecipes.value[currentRecipeIndex.value + 1]
})

const thirdRecipe = computed(() => {
  return allRecipes.value[currentRecipeIndex.value + 2]
})

// Recipe swiping
function handleLike() {
  if (currentRecipe.value) {
    savedRecipes.value.push({ ...currentRecipe.value })
    saveToLocalStorage()
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

function removeSavedRecipe(recipeId) {
  savedRecipes.value = savedRecipes.value.filter(r => r.id !== recipeId)
  saveToLocalStorage()
}

function toggleSaveRecipe(recipeId) {
  const index = savedRecipes.value.findIndex(r => r.id === recipeId)
  if (index > -1) {
    savedRecipes.value.splice(index, 1)
  } else {
    const recipe = allRecipes.value.find(r => r.id === recipeId)
    if (recipe) {
      savedRecipes.value.push({ ...recipe })
    }
  }
  saveToLocalStorage()
}

function viewRecipeDetail(recipe) {
  selectedRecipeForDetail.value = recipe
}

function closeRecipeDetail() {
  selectedRecipeForDetail.value = null
}

function addNewRecipe(recipe) {
  allRecipes.value.push(recipe)
  savedRecipes.value.push(recipe)
  saveToLocalStorage()
  currentView.value = 'browse'
}

function addRecipeToCalendarFromDetail(recipe) {
  closeRecipeDetail()
  currentView.value = 'calendar'
  // TODO: Show meal selector automatically
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
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
}

.app-logo {
  font-size: 1.75rem;
  font-weight: 700;
  color: #2c3e50;
  background: linear-gradient(135deg, #8CB369, #6a9e4d);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
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
  position: relative;
}

.swipe-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  position: relative;
  width: 100%;
  max-width: 400px;
}

/* Card stack effect */
.card-stack {
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 100%;
  max-width: 400px;
  z-index: 0;
}

.card-stack-item {
  position: absolute;
  width: 100%;
  height: 550px;
  border-radius: 24px;
  overflow: hidden;
  transition: all 0.5s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.card-stack-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  filter: brightness(0.7);
}

.card-back {
  transform: translateY(20px) scale(0.95);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
  z-index: 1;
}

.card-far-back {
  transform: translateY(40px) scale(0.9);
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.1);
  opacity: 0.5;
  z-index: 0;
}

/* Card transition animations */
.card-slide-enter-active {
  animation: slideIn 0.6s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.card-slide-leave-active {
  position: absolute;
  animation: slideOut 0.5s ease-out;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-30px) scale(0.9);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

@keyframes slideOut {
  from {
    opacity: 1;
    transform: scale(1);
  }
  to {
    opacity: 0;
    transform: scale(0.8);
  }
}

.empty-state {
  text-align: center;
  color: #888;
  animation: fadeIn 0.6s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.empty-icon {
  font-size: 5rem;
  margin-bottom: 1rem;
  animation: bounce 2s ease-in-out infinite;
}

@keyframes bounce {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-20px);
  }
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
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  box-shadow: 0 4px 15px rgba(140, 195, 105, 0.3);
}

.btn-reset:hover {
  background: #7aa359;
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(140, 195, 105, 0.4);
}

.btn-reset:active {
  transform: translateY(0);
}

.btn-reset svg {
  width: 20px;
  height: 20px;
}

/* Modal animations */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
  animation: fadeIn 0.3s ease-out;
  backdrop-filter: blur(5px);
}

.modal-content {
  background: white;
  border-radius: 20px;
  padding: 2rem;
  max-width: 600px;
  width: 100%;
  max-height: 80vh;
  overflow-y: auto;
  animation: modalSlideIn 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
}

@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: translateY(-50px) scale(0.9);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
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
  transition: all 0.3s ease;
}

.selector-item:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
}

.selector-item:active {
  transform: translateY(-2px);
}

.selector-item img {
  width: 100%;
  height: 120px;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.selector-item:hover img {
  transform: scale(1.1);
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
  transition: all 0.3s ease;
}

.btn-close:hover {
  background: #c0d9b3;
  transform: translateY(-2px);
}

/* Smooth scrolling */
html {
  scroll-behavior: smooth;
}

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: #f1f1f1;
}

::-webkit-scrollbar-thumb {
  background: #8CB369;
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: #7aa359;
}

/* Recipe Detail Modal */
.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: all 0.3s ease;
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}

.modal-fade-enter-from .modal-wrapper,
.modal-fade-leave-to .modal-wrapper {
  transform: translateY(50px) scale(0.95);
}

.modal-wrapper {
  max-width: 900px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  background: white;
  border-radius: 20px 20px 0 0;
  position: fixed;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  transition: transform 0.3s ease;
}

@media (min-width: 768px) {
  .modal-wrapper {
    border-radius: 20px;
    position: relative;
    left: 0;
    transform: none;
    max-height: 85vh;
  }
}
</style>