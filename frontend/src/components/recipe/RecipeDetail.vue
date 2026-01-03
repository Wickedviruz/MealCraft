<template>
  <div class="modal-overlay" @click="$emit('close')">
    <div class="recipe-detail" @click.stop>
      <button @click="$emit('close')" class="btn-close">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <line x1="18" y1="6" x2="6" y2="18"/>
          <line x1="6" y1="6" x2="18" y2="18"/>
        </svg>
      </button>
      
      <div class="detail-image">
        <img :src="recipe.imageUrl" :alt="recipe.title" />
      </div>
      
      <div class="detail-content">
        <header class="detail-header">
          <h1>{{ recipe.title }}</h1>
          <button @click="$emit('toggle-save')" class="save-btn" :class="{ saved: isSaved }">
            <svg viewBox="0 0 24 24" fill="currentColor">
              <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
            </svg>
          </button>
        </header>
        
        <p v-if="recipe.description" class="description">{{ recipe.description }}</p>
        
        <div class="meta-grid">
          <div class="meta-item">
            <span class="meta-label">Tid</span>
            <span class="meta-value">{{ recipe.cookTime }} min</span>
          </div>
          <div class="meta-item">
            <span class="meta-label">Portioner</span>
            <span class="meta-value">{{ recipe.servings }}</span>
          </div>
          <div v-if="recipe.difficulty" class="meta-item">
            <span class="meta-label">Svårighet</span>
            <span class="meta-value">{{ recipe.difficulty }}</span>
          </div>
          <div v-if="recipe.calories" class="meta-item">
            <span class="meta-label">Kalorier</span>
            <span class="meta-value">{{ recipe.calories }} kcal</span>
          </div>
        </div>
        
        <div v-if="recipe.tags && recipe.tags.length" class="tags">
          <span v-for="tag in recipe.tags" :key="tag" class="tag">{{ tag }}</span>
        </div>
        
        <section class="section">
          <h2>Ingredienser</h2>
          <ul class="ingredients-list">
            <li v-for="ing in recipe.ingredients" :key="ing.id">
              <span class="amount">{{ ing.amount }} {{ ing.unit }}</span>
              <span class="name">{{ ing.name }}</span>
            </li>
          </ul>
        </section>
        
        <section class="section">
          <h2>Instruktioner</h2>
          <ol class="instructions-list">
            <li v-for="(step, index) in recipe.instructions" :key="index">
              {{ step.text }}
            </li>
          </ol>
        </section>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  recipe: {
    type: Object,
    required: true
  },
  isSaved: {
    type: Boolean,
    default: false
  }
})

defineEmits(['close', 'toggle-save'])
</script>

<style scoped>
.recipe-detail {
  background: var(--surface);
  border-radius: var(--radius-xl) var(--radius-xl) 0 0;
  max-width: 900px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  position: fixed;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  animation: slideUp 0.3s ease;
}

@keyframes slideUp {
  from {
    transform: translateX(-50%) translateY(100%);
  }
  to {
    transform: translateX(-50%) translateY(0);
  }
}

@media (min-width: 768px) {
  .recipe-detail {
    border-radius: var(--radius-xl);
    position: relative;
    left: 0;
    transform: none;
    max-height: 85vh;
  }
}

.btn-close {
  position: absolute;
  top: var(--space-md);
  right: var(--space-md);
  width: 40px;
  height: 40px;
  border-radius: var(--radius-full);
  background: rgba(255, 255, 255, 0.9);
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10;
  transition: var(--transition);
}

.btn-close:hover {
  background: white;
  transform: scale(1.1);
}

.btn-close svg {
  width: 20px;
  height: 20px;
}

.detail-image {
  width: 100%;
  height: 300px;
  overflow: hidden;
}

.detail-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.detail-content {
  padding: var(--space-xl);
}

.detail-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: var(--space-md);
  margin-bottom: var(--space-md);
}

.detail-header h1 {
  font-size: 2rem;
  font-weight: 700;
  flex: 1;
}

.save-btn {
  width: 50px;
  height: 50px;
  border-radius: var(--radius-full);
  border: 2px solid var(--border);
  background: var(--surface);
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
  border-color: var(--primary);
  color: white;
}

.save-btn svg {
  width: 24px;
  height: 24px;
}

.description {
  color: var(--text-light);
  line-height: 1.6;
  margin-bottom: var(--space-lg);
}

.meta-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
  gap: var(--space-md);
  margin-bottom: var(--space-lg);
}

.meta-item {
  display: flex;
  flex-direction: column;
  gap: var(--space-xs);
}

.meta-label {
  font-size: 0.85rem;
  color: var(--text-light);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.meta-value {
  font-size: 1.1rem;
  font-weight: 700;
  color: var(--text);
}

.tags {
  display: flex;
  gap: var(--space-sm);
  flex-wrap: wrap;
  margin-bottom: var(--space-lg);
}

.tag {
  padding: 0.5rem 1rem;
  background: var(--primary-light);
  color: var(--primary-dark);
  border-radius: var(--radius-md);
  font-size: 0.9rem;
  font-weight: 600;
}

.section {
  margin-bottom: var(--space-xl);
}

.section h2 {
  font-size: 1.5rem;
  font-weight: 700;
  margin-bottom: var(--space-md);
}

.ingredients-list {
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: var(--space-sm);
}

.ingredients-list li {
  display: flex;
  gap: var(--space-md);
  padding: var(--space-sm);
  border-radius: var(--radius-sm);
  transition: var(--transition);
}

.ingredients-list li:hover {
  background: var(--background);
}

.ingredients-list .amount {
  font-weight: 700;
  color: var(--primary);
  min-width: 80px;
}

.instructions-list {
  counter-reset: step;
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: var(--space-md);
}

.instructions-list li {
  counter-increment: step;
  display: flex;
  gap: var(--space-md);
  padding-left: calc(var(--space-xl) + var(--space-md));
  position: relative;
}

.instructions-list li::before {
  content: counter(step);
  position: absolute;
  left: 0;
  width: var(--space-xl);
  height: var(--space-xl);
  background: var(--primary);
  color: white;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
}
</style>