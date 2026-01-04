<template>
  <div style="min-height: 100vh; background: #fefdf8;">
    <!-- Header -->
    <div style="background: white; border-bottom: 1px solid #e5e7eb; position: sticky; top: 0; z-index: 50;">
      <div style="max-width: 1280px; margin: 0 auto; padding: 24px 16px;">
        <h1 style="font-size: 32px; font-weight: 800; color: #1f2937; margin-bottom: 20px;">Upptäck Recept</h1>
        
        <!-- Search -->
        <div style="position: relative;">
          <svg style="position: absolute; left: 16px; top: 50%; transform: translateY(-50%); width: 20px; height: 20px; color: #9ca3af;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="11" cy="11" r="8"/>
            <path d="m21 21-4.35-4.35"/>
          </svg>
          <input 
            v-model="searchQuery"
            type="text"
            placeholder="Sök recept..."
            style="width: 100%; padding: 14px 16px 14px 48px; background: #f9fafb; border: 2px solid transparent; border-radius: 12px; font-size: 16px; outline: none; transition: all 0.2s;"
            @focus="$event.target.style.background = 'white'; $event.target.style.borderColor = '#10b981'"
            @blur="$event.target.style.background = '#f9fafb'; $event.target.style.borderColor = 'transparent'"
          />
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div style="background: white; border-bottom: 1px solid #e5e7eb;">
      <div style="max-width: 1280px; margin: 0 auto; padding: 16px;">
        <div style="display: flex; gap: 12px; overflow-x: auto; padding-bottom: 8px;">
          <button
            @click="activeCategory = null"
            style="padding: 10px 24px; border-radius: 20px; font-weight: 600; white-space: nowrap; border: none; cursor: pointer; transition: all 0.2s; font-size: 14px;"
            :style="activeCategory === null 
              ? 'background: #10b981; color: white; box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);' 
              : 'background: #f3f4f6; color: #4b5563;'"
          >
            Alla
          </button>
          
          <button
            v-for="cat in categories"
            :key="cat.id"
            @click="activeCategory = cat.id"
            style="padding: 10px 24px; border-radius: 20px; font-weight: 600; white-space: nowrap; border: none; cursor: pointer; transition: all 0.2s; font-size: 14px;"
            :style="activeCategory === cat.id 
              ? 'background: #10b981; color: white; box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);' 
              : 'background: #f3f4f6; color: #4b5563;'"
          >
            {{ cat.emoji }} {{ cat.label }}
          </button>

          <!-- Filter Toggle -->
          <button 
            @click="showFilters = !showFilters"
            style="margin-left: auto; padding: 10px 24px; border-radius: 20px; font-weight: 600; white-space: nowrap; border: 2px solid #e5e7eb; background: white; color: #4b5563; cursor: pointer; transition: all 0.2s; font-size: 14px;"
          >
            <svg style="width: 16px; height: 16px; display: inline; margin-right: 8px; vertical-align: middle;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="4" y1="6" x2="20" y2="6"/>
              <line x1="4" y1="12" x2="20" y2="12"/>
              <line x1="4" y1="18" x2="14" y2="18"/>
            </svg>
            Filter
          </button>
        </div>
      </div>
    </div>

    <!-- Advanced Filters -->
    <Transition name="slide">
      <div v-if="showFilters" style="background: white; border-bottom: 1px solid #e5e7eb; box-shadow: 0 4px 12px rgba(0,0,0,0.05);">
        <div style="max-width: 1280px; margin: 0 auto; padding: 24px 16px;">
          <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 16px;">
            <!-- Time -->
            <div>
              <label style="display: block; font-size: 14px; font-weight: 700; color: #374151; margin-bottom: 8px;">Tillagningstid</label>
              <select v-model="timeFilter" style="width: 100%; padding: 10px 16px; background: #f9fafb; border: 1px solid #e5e7eb; border-radius: 8px; font-size: 14px; cursor: pointer; outline: none;">
                <option value="">Alla tider</option>
                <option value="quick">⚡ Under 30 min</option>
                <option value="medium">⏱️ 30-60 min</option>
                <option value="long">🕐 Över 60 min</option>
              </select>
            </div>
            
            <!-- Difficulty -->
            <div>
              <label style="display: block; font-size: 14px; font-weight: 700; color: #374151; margin-bottom: 8px;">Svårighetsgrad</label>
              <select v-model="difficultyFilter" style="width: 100%; padding: 10px 16px; background: #f9fafb; border: 1px solid #e5e7eb; border-radius: 8px; font-size: 14px; cursor: pointer; outline: none;">
                <option value="">Alla nivåer</option>
                <option value="Enkel">👌 Enkel</option>
                <option value="Medel">🔥 Medel</option>
                <option value="Avancerad">👨‍🍳 Avancerad</option>
              </select>
            </div>
            
            <!-- Sort -->
            <div>
              <label style="display: block; font-size: 14px; font-weight: 700; color: #374151; margin-bottom: 8px;">Sortera</label>
              <select v-model="sortBy" style="width: 100%; padding: 10px 16px; background: #f9fafb; border: 1px solid #e5e7eb; border-radius: 8px; font-size: 14px; cursor: pointer; outline: none;">
                <option value="newest">🆕 Nyast</option>
                <option value="popular">⭐ Populärast</option>
                <option value="quickest">⚡ Snabbast</option>
                <option value="name">🔤 A-Ö</option>
              </select>
            </div>
          </div>
          
          <button 
            v-if="hasActiveFilters"
            @click="clearAllFilters" 
            style="margin-top: 16px; font-size: 14px; font-weight: 600; color: #10b981; background: none; border: none; cursor: pointer;"
          >
            ✕ Rensa alla filter
          </button>
        </div>
      </div>
    </Transition>

    <!-- Results -->
    <div style="max-width: 1280px; margin: 0 auto; padding: 24px 16px;">
      <p style="font-size: 14px; color: #6b7280; margin-bottom: 24px;">
        <span style="font-weight: 700; color: #1f2937;">{{ filteredRecipes.length }}</span> recept
      </p>

      <!-- Grid -->
      <div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 20px;">
        <div 
          v-for="recipe in filteredRecipes" 
          :key="recipe.id"
          @click="$emit('select', recipe)"
          style="background: white; border-radius: 16px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.08); cursor: pointer; transition: all 0.3s ease;"
          @mouseenter="$event.currentTarget.style.transform = 'translateY(-4px)'; $event.currentTarget.style.boxShadow = '0 12px 24px rgba(0,0,0,0.12)'"
          @mouseleave="$event.currentTarget.style.transform = 'translateY(0)'; $event.currentTarget.style.boxShadow = '0 2px 8px rgba(0,0,0,0.08)'"
        >
          <!-- Image -->
          <div style="position: relative; width: 100%; padding-bottom: 75%; background: #f3f4f6; overflow: hidden;">
            <img 
              :src="recipe.imageUrl" 
              :alt="recipe.title" 
              style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover; transition: transform 0.5s ease;"
              @mouseenter="$event.target.style.transform = 'scale(1.1)'"
              @mouseleave="$event.target.style.transform = 'scale(1)'"
            />
            
            <!-- Time badge -->
            <div style="position: absolute; top: 12px; left: 12px; display: flex; gap: 8px;">
              <span style="padding: 6px 12px; background: rgba(255,255,255,0.95); backdrop-filter: blur(10px); border-radius: 20px; font-size: 12px; font-weight: 700; color: #1f2937; box-shadow: 0 2px 8px rgba(0,0,0,0.1);">
                ⏱️ {{ recipe.cookTime }} min
              </span>
            </div>

            <!-- Save button -->
            <button 
              @click.stop="$emit('toggle-save', recipe.id)"
              style="position: absolute; top: 12px; right: 12px; width: 40px; height: 40px; border-radius: 50%; display: flex; align-items: center; justify-content: center; border: none; cursor: pointer; transition: all 0.2s; box-shadow: 0 2px 8px rgba(0,0,0,0.1);"
              :style="savedIds.includes(recipe.id)
                ? 'background: #10b981; color: white; transform: scale(1.1);'
                : 'background: rgba(255,255,255,0.95); backdrop-filter: blur(10px); color: #6b7280;'"
              @mouseenter="$event.target.style.transform = 'scale(1.15)'"
              @mouseleave="$event.target.style.transform = savedIds.includes(recipe.id) ? 'scale(1.1)' : 'scale(1)'"
            >
              <svg style="width: 20px; height: 20px;" viewBox="0 0 24 24" :fill="savedIds.includes(recipe.id) ? 'currentColor' : 'none'" stroke="currentColor" stroke-width="2">
                <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
              </svg>
            </button>
          </div>
          
          <!-- Content -->
          <div style="padding: 20px;">
            <h3 style="font-size: 18px; font-weight: 700; color: #1f2937; margin-bottom: 8px; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; min-height: 56px;">
              {{ recipe.title }}
            </h3>
            
            <div style="display: flex; align-items: center; gap: 12px; font-size: 14px; color: #6b7280;">
              <span v-if="recipe.servings">👥 {{ recipe.servings }}</span>
              <span v-if="recipe.difficulty">{{ getDifficultyEmoji(recipe.difficulty) }} {{ recipe.difficulty }}</span>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Empty State -->
      <div v-if="!filteredRecipes.length" style="text-align: center; padding: 80px 20px;">
        <div style="font-size: 80px; margin-bottom: 24px;">🔍</div>
        <h3 style="font-size: 28px; font-weight: 800; color: #1f2937; margin-bottom: 12px;">Hittade inga recept</h3>
        <p style="color: #6b7280; margin-bottom: 24px; max-width: 400px; margin-left: auto; margin-right: auto; line-height: 1.6;">
          Inga recept matchade dina filter. Prova att ändra sökning eller filter.
        </p>
        <button @click="clearAllFilters" class="btn btn-primary">
          Visa alla recept
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  recipes: {
    type: Array,
    required: true
  },
  savedIds: {
    type: Array,
    default: () => []
  }
})

defineEmits(['select', 'toggle-save'])

const searchQuery = ref('')
const activeCategory = ref(null)
const showFilters = ref(false)
const timeFilter = ref('')
const difficultyFilter = ref('')
const sortBy = ref('newest')

const categories = [
  { id: 'breakfast', emoji: '🥐', label: 'Frukost' },
  { id: 'lunch', emoji: '🥗', label: 'Lunch' },
  { id: 'dinner', emoji: '🍝', label: 'Middag' },
  { id: 'dessert', emoji: '🍰', label: 'Dessert' },
]

const hasActiveFilters = computed(() => {
  return timeFilter.value || difficultyFilter.value || searchQuery.value || activeCategory.value
})

const filteredRecipes = computed(() => {
  let result = [...props.recipes]
  
  // Search
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    result = result.filter(r => 
      r.title.toLowerCase().includes(q) ||
      (r.description && r.description.toLowerCase().includes(q)) ||
      (r.tags && r.tags.some(tag => tag.toLowerCase().includes(q)))
    )
  }
  
  // Category
  if (activeCategory.value) {
    const cat = categories.find(c => c.id === activeCategory.value)
    if (cat) {
      result = result.filter(r => r.tags && r.tags.some(tag => 
        tag.toLowerCase().includes(cat.label.toLowerCase())
      ))
    }
  }
  
  // Time
  if (timeFilter.value === 'quick') result = result.filter(r => r.cookTime <= 30)
  if (timeFilter.value === 'medium') result = result.filter(r => r.cookTime > 30 && r.cookTime <= 60)
  if (timeFilter.value === 'long') result = result.filter(r => r.cookTime > 60)
  
  // Difficulty
  if (difficultyFilter.value) {
    result = result.filter(r => r.difficulty === difficultyFilter.value)
  }
  
  // Sort
  if (sortBy.value === 'quickest') result.sort((a, b) => a.cookTime - b.cookTime)
  if (sortBy.value === 'popular') result.sort((a, b) => (b.rating || 0) - (a.rating || 0))
  if (sortBy.value === 'name') result.sort((a, b) => a.title.localeCompare(b.title, 'sv'))
  
  return result
})

function getDifficultyEmoji(difficulty) {
  const map = { 'Enkel': '👌', 'Medel': '🔥', 'Avancerad': '👨‍🍳' }
  return map[difficulty] || ''
}

function clearAllFilters() {
  searchQuery.value = ''
  activeCategory.value = null
  timeFilter.value = ''
  difficultyFilter.value = ''
  sortBy.value = 'newest'
  showFilters.value = false
}
</script>

<style scoped>
.slide-enter-active,
.slide-leave-active {
  transition: all 0.3s ease;
}

.slide-enter-from {
  opacity: 0;
  transform: translateY(-10px);
}

.slide-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}
</style>