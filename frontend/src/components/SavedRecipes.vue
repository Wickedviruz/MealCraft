<template>
  <div class="saved-recipes">
    <header class="page-header">
      <h1>Saved Recipes</h1>
      <p class="subtitle">{{ recipes.length }} recipes saved</p>
    </header>
    
    <div v-if="recipes.length === 0" class="empty-state">
      <svg class="empty-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
        <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
      </svg>
      <h2>No saved recipes yet</h2>
      <p>Start swiping to save recipes you love!</p>
    </div>
    
    <div v-else class="recipe-grid">
      <div 
        v-for="recipe in recipes" 
        :key="recipe.id" 
        class="recipe-item"
        @click="emit('select-recipe', recipe)"
      >
        <img :src="recipe.image" :alt="recipe.title" class="recipe-thumb" />
        <div class="recipe-content">
          <h3 class="recipe-name">{{ recipe.title }}</h3>
          <div class="recipe-meta">
            <span>⏱️ {{ recipe.cookTime }}m</span>
            <span>👥 {{ recipe.servings }}</span>
          </div>
        </div>
        <button 
          class="btn-remove" 
          @click.stop="emit('remove-recipe', recipe.id)"
          aria-label="Remove recipe"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="18" y1="6" x2="6" y2="18"/>
            <line x1="6" y1="6" x2="18" y2="18"/>
          </svg>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  recipes: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['select-recipe', 'remove-recipe'])
</script>

<style scoped>
.saved-recipes {
  padding: 1.5rem;
  padding-bottom: 100px;
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2rem;
  font-weight: 700;
  color: #2c3e50;
  margin: 0 0 0.5rem 0;
}

.subtitle {
  color: #888;
  font-size: 0.95rem;
  margin: 0;
}

.empty-state {
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

.empty-state h2 {
  font-size: 1.5rem;
  color: #555;
  margin: 0 0 0.5rem 0;
}

.empty-state p {
  margin: 0;
}

.recipe-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.recipe-item {
  background: white;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
}

.recipe-item:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.12);
}

.recipe-thumb {
  width: 100%;
  height: 200px;
  object-fit: cover;
}

.recipe-content {
  padding: 1.25rem;
}

.recipe-name {
  font-size: 1.25rem;
  font-weight: 600;
  color: #2c3e50;
  margin: 0 0 0.75rem 0;
}

.recipe-meta {
  display: flex;
  gap: 1rem;
  font-size: 0.9rem;
  color: #888;
}

.btn-remove {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.95);
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.recipe-item:hover .btn-remove {
  opacity: 1;
}

.btn-remove:hover {
  background: #ff6b6b;
  color: white;
}

.btn-remove svg {
  width: 18px;
  height: 18px;
}
</style>