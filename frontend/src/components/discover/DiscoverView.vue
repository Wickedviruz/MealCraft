<template>
  <div class="discover-view">
    <div v-if="currentRecipe || nextRecipe" class="swipe-container">
      <!-- Card Stack -->
      <div class="card-stack">
        <div v-if="nextRecipe" class="card-stack-item card-back">
          <img :src="nextRecipe.imageUrl" :alt="nextRecipe.title" />
        </div>
        <div v-if="thirdRecipe" class="card-stack-item card-far-back">
          <img :src="thirdRecipe.imageUrl" :alt="thirdRecipe.title" />
        </div>
      </div>
      
      <!-- Current Card -->
      <Transition name="card-slide" mode="out-in">
        <RecipeCard 
          v-if="currentRecipe"
          :key="currentRecipe.id"
          :recipe="currentRecipe"
          @swipe-left="$emit('dislike')"
          @swipe-right="$emit('like')"
        />
      </Transition>
      
      <!-- Swipe Buttons -->
      <SwipeButtons 
        @like="$emit('like')"
        @dislike="$emit('dislike')"
      />
    </div>
    
    <!-- Empty State -->
    <div v-else class="empty-state">
      <div class="empty-icon">🎉</div>
      <h2>Inga fler recept!</h2>
      <p>Du har sett alla tillgängliga recept</p>
      <button @click="$emit('reset')" class="btn btn-primary">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <polyline points="23 4 23 10 17 10"/>
          <path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/>
        </svg>
        Börja om
      </button>
    </div>
  </div>
</template>

<script setup>
import RecipeCard from '../recipe/RecipeCard.vue'
import SwipeButtons from '../SwipeButtons.vue'

defineProps({
  currentRecipe: Object,
  nextRecipe: Object,
  thirdRecipe: Object
})

defineEmits(['like', 'dislike', 'reset'])
</script>

<style scoped>
.discover-view {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: calc(100vh - 180px);
  padding: var(--space-xl);
}

.swipe-container {
  position: relative;
  width: 100%;
  max-width: 400px;
}

.card-stack {
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 100%;
  z-index: 0;
}

.card-stack-item {
  position: absolute;
  width: 100%;
  height: 550px;
  border-radius: var(--radius-xl);
  overflow: hidden;
  transition: var(--transition);
}

.card-stack-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  filter: brightness(0.7);
}

.card-back {
  transform: translateY(20px) scale(0.95);
  box-shadow: var(--shadow-md);
  z-index: 1;
}

.card-far-back {
  transform: translateY(40px) scale(0.9);
  box-shadow: var(--shadow-sm);
  opacity: 0.5;
}

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
  to {
    opacity: 0;
    transform: scale(0.8);
  }
}

.empty-state {
  text-align: center;
  animation: fadeIn 0.6s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
}

.empty-icon {
  font-size: 5rem;
  margin-bottom: var(--space-md);
  animation: bounce 2s ease-in-out infinite;
}

@keyframes bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-20px); }
}

.empty-state h2 {
  font-size: 1.75rem;
  color: var(--text);
  margin-bottom: var(--space-sm);
}

.empty-state p {
  color: var(--text-light);
  margin-bottom: var(--space-lg);
}

.btn svg {
  width: 20px;
  height: 20px;
  margin-right: var(--space-sm);
}
</style>