<template>
  <div class="modal-overlay" @click="$emit('close')">
    <div class="modal-content meal-selector" @click.stop>
      <header class="selector-header">
        <h2>Välj ett recept</h2>
        <button @click="$emit('close')" class="btn-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="18" y1="6" x2="6" y2="18"/>
            <line x1="6" y1="6" x2="18" y2="18"/>
          </svg>
        </button>
      </header>
      
      <div v-if="recipes.length" class="recipes-grid">
        <div 
          v-for="recipe in recipes" 
          :key="recipe.id"
          class="recipe-item"
          @click="$emit('select', recipe)"
        >
          <div class="recipe-image">
            <img :src="recipe.imageUrl" :alt="recipe.title" />
          </div>
          <div class="recipe-info">
            <h3>{{ recipe.title }}</h3>
            <span class="recipe-time">{{ recipe.cookTime }} min</span>
          </div>
        </div>
      </div>
      
      <div v-else class="empty-message">
        <p>Du har inga sparade recept än</p>
        <p class="hint">Börja med att spara några recept i Discover-läget</p>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  recipes: {
    type: Array,
    required: true
  }
})

defineEmits(['select', 'close'])
</script>

<style scoped>
.meal-selector {
  max-width: 700px;
}

.selector-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--space-lg);
  padding-bottom: var(--space-md);
  border-bottom: 2px solid var(--border);
}

.selector-header h2 {
  font-size: 1.5rem;
  font-weight: 700;
}

.recipes-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: var(--space-md);
  max-height: 60vh;
  overflow-y: auto;
}

.recipe-item {
  cursor: pointer;
  border-radius: var(--radius-md);
  overflow: hidden;
  background: var(--background);
  transition: var(--transition);
  box-shadow: var(--shadow-sm);
}

.recipe-item:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-md);
}

.recipe-image {
  width: 100%;
  height: 120px;
  overflow: hidden;
}

.recipe-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: var(--transition);
}

.recipe-item:hover .recipe-image img {
  transform: scale(1.1);
}

.recipe-info {
  padding: var(--space-sm);
}

.recipe-info h3 {
  font-size: 0.9rem;
  font-weight: 600;
  margin-bottom: var(--space-xs);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.recipe-time {
  font-size: 0.8rem;
  color: var(--text-light);
}

.empty-message {
  text-align: center;
  padding: var(--space-xl);
  color: var(--text-light);
}

.empty-message p {
  margin-bottom: var(--space-sm);
}

.hint {
  font-size: 0.9rem;
  color: var(--primary);
}
</style>