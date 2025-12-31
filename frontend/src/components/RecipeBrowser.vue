<template>
  <div class="recipe-browser">
    <header class="browser-header">
      <h1>Alla Recept</h1>
      
      <div class="search-bar">
        <svg class="search-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="11" cy="11" r="8"/>
          <path d="m21 21-4.35-4.35"/>
        </svg>
        <input 
          v-model="searchQuery"
          type="text" 
          placeholder="Sök recept, ingredienser..." 
          class="search-input"
        />
        <button v-if="searchQuery" @click="searchQuery = ''" class="clear-btn">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="18" y1="6" x2="6" y2="18"/>
            <line x1="6" y1="6" x2="18" y2="18"/>
          </svg>
        </button>
      </div>
      
      <div class="filters">
        <button 
          @click="showFilters = !showFilters" 
          :class="['filter-toggle', { active: hasActiveFilters }]"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"/>
          </svg>
          Filter
          <span v-if="activeFilterCount" class="filter-badge">{{ activeFilterCount }}</span>
        </button>
        
        <select v-model="sortBy" class="sort-select">
          <option value="popular">Mest populära</option>
          <option value="time-asc">Snabbast först</option>
          <option value="time-desc">Längst tid</option>
          <option value="rating">Högst betyg</option>
          <option value="newest">Nyaste</option>
        </select>
      </div>
    </header>
    
    <Transition name="slide-down">
      <div v-if="showFilters" class="filter-panel">
        <div class="filter-group">
          <h3>Tid</h3>
          <div class="filter-chips">
            <button 
              v-for="time in timeFilters" 
              :key="time.value"
              @click="toggleFilter('time', time.value)"
              :class="['chip', { active: selectedFilters.time.includes(time.value) }]"
            >
              {{ time.label }}
            </button>
          </div>
        </div>
        
        <div class="filter-group">
          <h3>Kategori</h3>
          <div class="filter-chips">
            <button 
              v-for="cat in categories" 
              :key="cat"
              @click="toggleFilter('category', cat)"
              :class="['chip', { active: selectedFilters.category.includes(cat) }]"
            >
              {{ cat }}
            </button>
          </div>
        </div>
        
        <div class="filter-group">
          <h3>Svårighetsgrad</h3>
          <div class="filter-chips">
            <button 
              v-for="diff in difficulties" 
              :key="diff"
              @click="toggleFilter('difficulty', diff)"
              :class="['chip', { active: selectedFilters.difficulty.includes(diff) }]"
            >
              {{ diff }}
            </button>
          </div>
        </div>
        
        <button @click="clearFilters" class="clear-filters">
          Rensa alla filter
        </button>
      </div>
    </Transition>
    
    <div class="results-info">
      <p>{{ filteredRecipes.length }} recept</p>
    </div>
    
    <div v-if="filteredRecipes.length === 0" class="empty-results">
      <svg class="empty-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
        <circle cx="11" cy="11" r="8"/>
        <path d="m21 21-4.35-4.35"/>
      </svg>
      <h2>Inga recept hittades</h2>
      <p>Prova att ändra dina filter eller sökord</p>
    </div>
    
    <div v-else class="recipe-grid">
      <div 
        v-for="recipe in filteredRecipes" 
        :key="recipe.id"
        class="recipe-card"
        @click="emit('select-recipe', recipe)"
      >
        <div class="card-image-wrapper">
          <img :src="recipe.image" :alt="recipe.title" class="card-image" />
          <button 
            @click.stop="emit('toggle-save', recipe.id)"
            :class="['btn-save', { saved: savedRecipeIds.includes(recipe.id) }]"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
            </svg>
          </button>
          <div class="card-tags">
            <span v-for="tag in recipe.tags.slice(0, 2)" :key="tag" class="tag">{{ tag }}</span>
          </div>
        </div>
        
        <div class="card-content">
          <h3 class="card-title">{{ recipe.title }}</h3>
          
          <div class="card-meta">
            <span class="meta-item">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10"/>
                <polyline points="12 6 12 12 16 14"/>
              </svg>
              {{ recipe.cookTime }}m
            </span>
            <span class="meta-item">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
                <circle cx="9" cy="7" r="4"/>
              </svg>
              {{ recipe.servings }}
            </span>
            <span class="meta-item rating">
              ★ {{ recipe.rating || 4.5 }}
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  recipes: {
    type: Array,
    default: () => []
  },
  savedRecipeIds: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['select-recipe', 'toggle-save'])

const searchQuery = ref('')
const sortBy = ref('popular')
const showFilters = ref(false)
const selectedFilters = ref({
  time: [],
  category: [],
  difficulty: []
})

const timeFilters = [
  { value: 'quick', label: 'Under 30 min' },
  { value: 'medium', label: '30-60 min' },
  { value: 'long', label: 'Över 60 min' }
]

const categories = ['Kött', 'Fisk', 'Vegetariskt', 'Veganskt', 'Dessert', 'Frukost', 'Lunch', 'Middag']
const difficulties = ['Lätt', 'Medel', 'Svår']

const filteredRecipes = computed(() => {
  let results = [...props.recipes]
  
  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    results = results.filter(r => 
      r.title.toLowerCase().includes(query) ||
      r.tags?.some(t => t.toLowerCase().includes(query)) ||
      r.ingredients?.some(i => i.name.toLowerCase().includes(query))
    )
  }
  
  // Time filter
  if (selectedFilters.value.time.length > 0) {
    results = results.filter(r => {
      if (selectedFilters.value.time.includes('quick')) return r.cookTime < 30
      if (selectedFilters.value.time.includes('medium')) return r.cookTime >= 30 && r.cookTime <= 60
      if (selectedFilters.value.time.includes('long')) return r.cookTime > 60
      return true
    })
  }
  
  // Category filter
  if (selectedFilters.value.category.length > 0) {
    results = results.filter(r => 
      r.tags?.some(t => selectedFilters.value.category.includes(t))
    )
  }
  
  // Difficulty filter
  if (selectedFilters.value.difficulty.length > 0) {
    results = results.filter(r => 
      selectedFilters.value.difficulty.includes(r.difficulty || 'Medel')
    )
  }
  
  // Sorting
  switch (sortBy.value) {
    case 'time-asc':
      results.sort((a, b) => a.cookTime - b.cookTime)
      break
    case 'time-desc':
      results.sort((a, b) => b.cookTime - a.cookTime)
      break
    case 'rating':
      results.sort((a, b) => (b.rating || 4.5) - (a.rating || 4.5))
      break
    case 'newest':
      results.sort((a, b) => (b.createdAt || 0) - (a.createdAt || 0))
      break
  }
  
  return results
})

const activeFilterCount = computed(() => {
  return selectedFilters.value.time.length + 
         selectedFilters.value.category.length + 
         selectedFilters.value.difficulty.length
})

const hasActiveFilters = computed(() => activeFilterCount.value > 0)

function toggleFilter(type, value) {
  const index = selectedFilters.value[type].indexOf(value)
  if (index > -1) {
    selectedFilters.value[type].splice(index, 1)
  } else {
    selectedFilters.value[type].push(value)
  }
}

function clearFilters() {
  selectedFilters.value = {
    time: [],
    category: [],
    difficulty: []
  }
}
</script>

<style scoped>
.recipe-browser {
  padding: 1.5rem;
  padding-bottom: 100px;
  max-width: 1400px;
  margin: 0 auto;
}

.browser-header h1 {
  font-size: 2rem;
  font-weight: 700;
  color: #2c3e50;
  margin: 0 0 1.5rem 0;
}

.search-bar {
  position: relative;
  margin-bottom: 1rem;
}

.search-icon {
  position: absolute;
  left: 1rem;
  top: 50%;
  transform: translateY(-50%);
  width: 20px;
  height: 20px;
  color: #888;
  pointer-events: none;
}

.search-input {
  width: 100%;
  padding: 1rem 3rem 1rem 3.5rem;
  border: 2px solid #D4E7C5;
  border-radius: 12px;
  font-size: 1rem;
  transition: all 0.3s ease;
}

.search-input:focus {
  outline: none;
  border-color: #8CB369;
  box-shadow: 0 0 0 3px rgba(140, 195, 105, 0.1);
}

.clear-btn {
  position: absolute;
  right: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  width: 32px;
  height: 32px;
  border-radius: 50%;
  border: none;
  background: #f0f0f0;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.clear-btn:hover {
  background: #e0e0e0;
}

.clear-btn svg {
  width: 16px;
  height: 16px;
}

.filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
}

.filter-toggle {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: white;
  border: 2px solid #D4E7C5;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  position: relative;
}

.filter-toggle:hover,
.filter-toggle.active {
  background: #8CB369;
  color: white;
  border-color: #8CB369;
}

.filter-toggle svg {
  width: 18px;
  height: 18px;
}

.filter-badge {
  position: absolute;
  top: -8px;
  right: -8px;
  background: #ff6b6b;
  color: white;
  width: 24px;
  height: 24px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: 700;
}

.sort-select {
  flex: 1;
  padding: 0.75rem 1rem;
  border: 2px solid #D4E7C5;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  background: white;
  transition: all 0.3s;
}

.sort-select:focus {
  outline: none;
  border-color: #8CB369;
}

.filter-panel {
  background: white;
  border: 2px solid #D4E7C5;
  border-radius: 16px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
}

.slide-down-enter-active,
.slide-down-leave-active {
  transition: all 0.3s ease;
}

.slide-down-enter-from,
.slide-down-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}

.filter-group {
  margin-bottom: 1.5rem;
}

.filter-group:last-of-type {
  margin-bottom: 1rem;
}

.filter-group h3 {
  font-size: 0.95rem;
  font-weight: 600;
  color: #888;
  text-transform: uppercase;
  margin: 0 0 0.75rem 0;
  letter-spacing: 0.5px;
}

.filter-chips {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.chip {
  padding: 0.5rem 1rem;
  border: 2px solid #D4E7C5;
  border-radius: 20px;
  background: white;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.chip:hover {
  border-color: #8CB369;
  background: #f9fdf7;
}

.chip.active {
  background: #8CB369;
  color: white;
  border-color: #8CB369;
}

.clear-filters {
  width: 100%;
  padding: 0.75rem;
  background: #f9fdf7;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  color: #8CB369;
  cursor: pointer;
  transition: all 0.2s;
}

.clear-filters:hover {
  background: #D4E7C5;
}

.results-info {
  margin-bottom: 1.5rem;
  color: #888;
  font-weight: 500;
}

.empty-results {
  text-align: center;
  padding: 4rem 2rem;
  color: #888;
}

.empty-icon {
  width: 80px;
  height: 80px;
  margin: 0 auto 1.5rem;
  stroke: #D4E7C5;
  stroke-width: 1.5;
}

.recipe-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

.recipe-card {
  background: white;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  cursor: pointer;
  transition: all 0.3s ease;
}

.recipe-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.12);
}

.card-image-wrapper {
  position: relative;
  height: 200px;
  overflow: hidden;
}

.card-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}

.recipe-card:hover .card-image {
  transform: scale(1.1);
}

.btn-save {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.95);
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s;
  z-index: 10;
}

.btn-save:hover {
  background: #8CB369;
  color: white;
  transform: scale(1.1);
}

.btn-save.saved {
  background: #8CB369;
  color: white;
}

.btn-save svg {
  width: 20px;
  height: 20px;
}

.card-tags {
  position: absolute;
  bottom: 0.75rem;
  left: 0.75rem;
  display: flex;
  gap: 0.5rem;
}

.tag {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  padding: 0.3rem 0.75rem;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 600;
}

.card-content {
  padding: 1.25rem;
}

.card-title {
  font-size: 1.1rem;
  font-weight: 600;
  color: #2c3e50;
  margin: 0 0 0.75rem 0;
}

.card-meta {
  display: flex;
  gap: 1rem;
  font-size: 0.9rem;
  color: #888;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 0.3rem;
}

.meta-item svg {
  width: 16px;
  height: 16px;
}

.meta-item.rating {
  color: #FFB84D;
  font-weight: 600;
}
</style>