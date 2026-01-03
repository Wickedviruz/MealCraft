<template>
  <div 
    class="recipe-card"
    @touchstart="handleTouchStart"
    @touchmove="handleTouchMove"
    @touchend="handleTouchEnd"
    @mousedown="handleMouseDown"
    @mousemove="handleMouseMove"
    @mouseup="handleMouseEnd"
    :style="cardStyle"
  >
    <div class="card-image">
      <img :src="recipe.imageUrl" :alt="recipe.title" />
      
      <!-- Swipe Indicators -->
      <div class="swipe-indicator left" :class="{ active: swipeDirection === 'left' }">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <line x1="18" y1="6" x2="6" y2="18"/>
          <line x1="6" y1="6" x2="18" y2="18"/>
        </svg>
      </div>
      
      <div class="swipe-indicator right" :class="{ active: swipeDirection === 'right' }">
        <svg viewBox="0 0 24 24" fill="currentColor">
          <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
        </svg>
      </div>
    </div>
    
    <div class="card-content">
      <h2 class="recipe-title">{{ recipe.title }}</h2>
      
      <div class="recipe-meta">
        <span class="meta-item">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="12" cy="12" r="10"/>
            <polyline points="12 6 12 12 16 14"/>
          </svg>
          {{ recipe.cookTime }} min
        </span>
        
        <span class="meta-item">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
            <circle cx="9" cy="7" r="4"/>
            <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
            <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
          </svg>
          {{ recipe.servings }} portioner
        </span>
        
        <span v-if="recipe.rating" class="meta-item">
          <svg viewBox="0 0 24 24" fill="currentColor">
            <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/>
          </svg>
          {{ recipe.rating }}
        </span>
      </div>
      
      <div v-if="recipe.tags && recipe.tags.length" class="recipe-tags">
        <span v-for="tag in recipe.tags.slice(0, 3)" :key="tag" class="tag">
          {{ tag }}
        </span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  recipe: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['swipe-left', 'swipe-right'])

// Swipe state
const isDragging = ref(false)
const startX = ref(0)
const currentX = ref(0)
const swipeDirection = ref(null)

const SWIPE_THRESHOLD = 100

const cardStyle = computed(() => {
  if (!isDragging.value) return {}
  
  const diff = currentX.value - startX.value
  const rotation = diff / 20
  
  return {
    transform: `translateX(${diff}px) rotate(${rotation}deg)`,
    transition: 'none'
  }
})

// Touch handlers
function handleTouchStart(e) {
  isDragging.value = true
  startX.value = e.touches[0].clientX
}

function handleTouchMove(e) {
  if (!isDragging.value) return
  
  currentX.value = e.touches[0].clientX
  const diff = currentX.value - startX.value
  
  if (diff > 50) {
    swipeDirection.value = 'right'
  } else if (diff < -50) {
    swipeDirection.value = 'left'
  } else {
    swipeDirection.value = null
  }
}

function handleTouchEnd() {
  if (!isDragging.value) return
  
  const diff = currentX.value - startX.value
  
  if (Math.abs(diff) > SWIPE_THRESHOLD) {
    if (diff > 0) {
      emit('swipe-right')
    } else {
      emit('swipe-left')
    }
  }
  
  resetSwipe()
}

// Mouse handlers
function handleMouseDown(e) {
  isDragging.value = true
  startX.value = e.clientX
}

function handleMouseMove(e) {
  if (!isDragging.value) return
  
  currentX.value = e.clientX
  const diff = currentX.value - startX.value
  
  if (diff > 50) {
    swipeDirection.value = 'right'
  } else if (diff < -50) {
    swipeDirection.value = 'left'
  } else {
    swipeDirection.value = null
  }
}

function handleMouseEnd() {
  if (!isDragging.value) return
  
  const diff = currentX.value - startX.value
  
  if (Math.abs(diff) > SWIPE_THRESHOLD) {
    if (diff > 0) {
      emit('swipe-right')
    } else {
      emit('swipe-left')
    }
  }
  
  resetSwipe()
}

function resetSwipe() {
  isDragging.value = false
  startX.value = 0
  currentX.value = 0
  swipeDirection.value = null
}
</script>

<style scoped>
.recipe-card {
  width: 100%;
  max-width: 400px;
  height: 550px;
  background: var(--surface);
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-lg);
  overflow: hidden;
  cursor: grab;
  user-select: none;
  position: relative;
  z-index: 10;
  transition: var(--transition);
}

.recipe-card:active {
  cursor: grabbing;
}

.card-image {
  position: relative;
  width: 100%;
  height: 65%;
  overflow: hidden;
}

.card-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.swipe-indicator {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 100px;
  height: 100px;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.swipe-indicator.left {
  left: 20px;
  background: rgba(255, 107, 107, 0.9);
  color: white;
}

.swipe-indicator.right {
  right: 20px;
  background: rgba(140, 179, 105, 0.9);
  color: white;
}

.swipe-indicator.active {
  opacity: 1;
}

.swipe-indicator svg {
  width: 50px;
  height: 50px;
}

.card-content {
  padding: var(--space-lg);
  height: 35%;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.recipe-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--text);
  margin-bottom: var(--space-sm);
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.recipe-meta {
  display: flex;
  gap: var(--space-lg);
  flex-wrap: wrap;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: var(--space-xs);
  color: var(--text-light);
  font-size: 0.9rem;
}

.meta-item svg {
  width: 18px;
  height: 18px;
  color: var(--primary);
}

.recipe-tags {
  display: flex;
  gap: var(--space-sm);
  flex-wrap: wrap;
}

.tag {
  padding: 0.25rem 0.75rem;
  background: var(--primary-light);
  color: var(--primary-dark);
  border-radius: var(--radius-md);
  font-size: 0.85rem;
  font-weight: 600;
}
</style>