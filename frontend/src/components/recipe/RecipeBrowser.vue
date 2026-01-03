<template>
  <div class="recipe-browser">
    <div class="browser-header">
      <h2>Alla Recept</h2>
      <p>{{ recipes.length }} recept</p>
    </div>
    
    <div class="recipe-grid">
      <div 
        v-for="recipe in recipes" 
        :key="recipe.id"
        class="recipe-grid-item"
        @click="$emit('select', recipe)"
      >
        <div class="grid-item-image">
          <img :src="recipe.imageUrl" :alt="recipe.title" />
          <button 
            @click.stop="$emit('toggle-save', recipe.id)"
            class="save-btn"
            :class="{ saved: savedIds.includes(recipe.id) }"
          >
            <svg viewBox="0 0 24 24" fill="currentColor">
              <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
            </svg>
          </button>
        </div>
        
        <div class="grid-item-content">
          <h3>{{ recipe.title }}</h3>
          <div class="grid-meta">
            <span>{{ recipe.cookTime }} min</span>
            <span v-if="recipe.rating">★ {{ recipe.rating }}</span>
          </div>
          <div v-if="recipe.tags && recipe.tags.length" class="grid-tags">
            <span v-for="tag in recipe.tags.slice(0, 2)" :key="tag" class="tag">
              {{ tag }}
            </span>
          </div>
        </div>
      </div>
    </div>
    
    <div v-if="!recipes.length" class="empty-state">
      <p>Inga recept hittades</p>
    </div>
  </div>
</template>

<script setup>
defineProps({
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
</script>

<style scoped>
.recipe-browser {
  padding: var(--space-lg);
  max-width: 1200px;
  margin: 0 auto;
}

.browser-header {
  margin-bottom: var(--space-lg);
}

.browser-header h2 {
  font-size: 2rem;
  font-weight: 700;
  margin-bottom: var(--space-xs);
}

.browser-header p {
  color: var(--text-light);
}

.recipe-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: var(--space-lg);
}

.recipe-grid-item {
  background: var(--surface);
  border-radius: var(--radius-lg);
  overflow: hidden;
  cursor: pointer;
  transition: var(--transition);
  box-shadow: var(--shadow-sm);
}

.recipe-grid-item:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-md);
}

.grid-item-image {
  position: relative;
  width: 100%;
  height: 200px;
  overflow: hidden;
}

.grid-item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: var(--transition);
}

.recipe-grid-item:hover .grid-item-image img {
  transform: scale(1.05);
}

.save-btn {
  position: absolute;
  top: var(--space-sm);
  right: var(--space-sm);
  width: 40px;
  height: 40px;
  border-radius: var(--radius-full);
  border: none;
  background: rgba(255, 255, 255, 0.9);
  color: var(--text-light);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: var(--transition);
}

.save-btn:hover {
  transform: scale(1.1);
}

.save-btn.saved {
  background: var(--primary);
  color: white;
}

.save-btn svg {
  width: 20px;
  height: 20px;
}

.grid-item-content {
  padding: var(--space-md);
}

.grid-item-content h3 {
  font-size: 1.1rem;
  font-weight: 700;
  margin-bottom: var(--space-sm);
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.grid-meta {
  display: flex;
  gap: var(--space-md);
  color: var(--text-light);
  font-size: 0.9rem;
  margin-bottom: var(--space-sm);
}

.grid-tags {
  display: flex;
  gap: var(--space-xs);
  flex-wrap: wrap;
}

.tag {
  padding: 0.25rem 0.5rem;
  background: var(--primary-light);
  color: var(--primary-dark);
  border-radius: var(--radius-sm);
  font-size: 0.75rem;
  font-weight: 600;
}

.empty-state {
  text-align: center;
  padding: var(--space-xl);
  color: var(--text-light);
}
</style>