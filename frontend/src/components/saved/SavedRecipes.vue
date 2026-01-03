<template>
  <div class="saved-recipes">
    <div class="saved-header">
      <h2>Sparade Recept</h2>
      <p>{{ recipes.length }} sparade</p>
    </div>
    
    <div v-if="recipes.length" class="recipe-grid">
      <div 
        v-for="recipe in recipes" 
        :key="recipe.id"
        class="recipe-card"
        @click="$emit('select', recipe)"
      >
        <div class="card-image">
          <img :src="recipe.imageUrl" :alt="recipe.title" />
          <button 
            @click.stop="$emit('remove', recipe.id)"
            class="remove-btn"
            title="Ta bort"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="3 6 5 6 21 6"/>
              <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/>
            </svg>
          </button>
        </div>
        
        <div class="card-content">
          <h3>{{ recipe.title }}</h3>
          <div class="card-meta">
            <span>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10"/>
                <polyline points="12 6 12 12 16 14"/>
              </svg>
              {{ recipe.cookTime }} min
            </span>
            <span v-if="recipe.rating">
              <svg viewBox="0 0 24 24" fill="currentColor">
                <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/>
              </svg>
              {{ recipe.rating }}
            </span>
          </div>
          <div v-if="recipe.tags && recipe.tags.length" class="card-tags">
            <span v-for="tag in recipe.tags.slice(0, 2)" :key="tag" class="tag">
              {{ tag }}
            </span>
          </div>
        </div>
      </div>
    </div>
    
    <div v-else class="empty-state">
      <div class="empty-icon">💚</div>
      <h3>Inga sparade recept</h3>
      <p>Swipa höger på recept du gillar för att spara dem här</p>
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

defineEmits(['select', 'remove'])
</script>

<style scoped>
.saved-recipes {
  padding: var(--space-lg);
  max-width: 1200px;
  margin: 0 auto;
}

.saved-header {
  margin-bottom: var(--space-lg);
}

.saved-header h2 {
  font-size: 2rem;
  font-weight: 700;
  margin-bottom: var(--space-xs);
}

.saved-header p {
  color: var(--text-light);
}

.recipe-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: var(--space-lg);
}

.recipe-card {
  background: var(--surface);
  border-radius: var(--radius-lg);
  overflow: hidden;
  cursor: pointer;
  transition: var(--transition);
  box-shadow: var(--shadow-sm);
}

.recipe-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-md);
}

.card-image {
  position: relative;
  width: 100%;
  height: 200px;
  overflow: hidden;
}

.card-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: var(--transition);
}

.recipe-card:hover .card-image img {
  transform: scale(1.05);
}

.remove-btn {
  position: absolute;
  top: var(--space-sm);
  right: var(--space-sm);
  width: 40px;
  height: 40px;
  border-radius: var(--radius-full);
  border: none;
  background: rgba(255, 107, 107, 0.9);
  color: white;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: var(--transition);
  opacity: 0;
}

.recipe-card:hover .remove-btn {
  opacity: 1;
}

.remove-btn:hover {
  background: #ff6b6b;
  transform: scale(1.1);
}

.remove-btn svg {
  width: 20px;
  height: 20px;
}

.card-content {
  padding: var(--space-md);
}

.card-content h3 {
  font-size: 1.1rem;
  font-weight: 700;
  margin-bottom: var(--space-sm);
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.card-meta {
  display: flex;
  gap: var(--space-md);
  color: var(--text-light);
  font-size: 0.9rem;
  margin-bottom: var(--space-sm);
}

.card-meta span {
  display: flex;
  align-items: center;
  gap: var(--space-xs);
}

.card-meta svg {
  width: 16px;
  height: 16px;
  color: var(--primary);
}

.card-tags {
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
}

.empty-icon {
  font-size: 5rem;
  margin-bottom: var(--space-md);
  animation: pulse 2s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.1); }
}

.empty-state h3 {
  font-size: 1.5rem;
  margin-bottom: var(--space-sm);
}

.empty-state p {
  color: var(--text-light);
}
</style>