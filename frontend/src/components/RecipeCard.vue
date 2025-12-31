<template>
  <div 
    class="recipe-card"
    :style="cardStyle"
    @mousedown="startDrag"
    @touchstart="startDrag"
  >
    <img :src="recipe.image" :alt="recipe.title" class="recipe-image" />
    
    <div class="card-overlay">
      <div class="tags">
        <span v-for="tag in recipe.tags" :key="tag" class="tag">{{ tag }}</span>
      </div>
      
      <h2 class="recipe-title">{{ recipe.title }}</h2>
      
      <div class="recipe-info">
        <span class="info-item">
          <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <circle cx="12" cy="12" r="10"/>
            <polyline points="12 6 12 12 16 14"/>
          </svg>
          {{ recipe.cookTime }} min
        </span>
        <span class="info-item">
          <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
            <circle cx="9" cy="7" r="4"/>
            <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
            <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
          </svg>
          {{ recipe.servings }} servings
        </span>
        <span class="info-item">
          <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M12 2L2 7l10 5 10-5-10-5z"/>
            <path d="M2 17l10 5 10-5"/>
            <path d="M2 12l10 5 10-5"/>
          </svg>
          {{ recipe.calories }} kcal
        </span>
      </div>
    </div>
    
    <!-- Swipe indicators -->
    <div v-if="swipeDirection === 'left'" class="swipe-indicator dislike">✕</div>
    <div v-if="swipeDirection === 'right'" class="swipe-indicator like">♥</div>
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

const isDragging = ref(false)
const startX = ref(0)
const currentX = ref(0)
const startY = ref(0)
const currentY = ref(0)

const swipeDirection = computed(() => {
  const diff = currentX.value - startX.value
  if (Math.abs(diff) < 50) return null
  return diff > 0 ? 'right' : 'left'
})

const cardStyle = computed(() => {
  if (!isDragging.value) return {}
  
  const deltaX = currentX.value - startX.value
  const deltaY = currentY.value - startY.value
  const rotation = deltaX / 20
  
  return {
    transform: `translate(${deltaX}px, ${deltaY}px) rotate(${rotation}deg)`,
    transition: 'none'
  }
})

function startDrag(e) {
  isDragging.value = true
  const touch = e.touches ? e.touches[0] : e
  startX.value = touch.clientX
  startY.value = touch.clientY
  currentX.value = touch.clientX
  currentY.value = touch.clientY
  
  document.addEventListener('mousemove', onDrag)
  document.addEventListener('mouseup', endDrag)
  document.addEventListener('touchmove', onDrag)
  document.addEventListener('touchend', endDrag)
}

function onDrag(e) {
  if (!isDragging.value) return
  const touch = e.touches ? e.touches[0] : e
  currentX.value = touch.clientX
  currentY.value = touch.clientY
}

function endDrag() {
  if (!isDragging.value) return
  
  const diff = currentX.value - startX.value
  
  if (Math.abs(diff) > 100) {
    if (diff > 0) {
      emit('swipe-right', props.recipe)
    } else {
      emit('swipe-left', props.recipe)
    }
  }
  
  isDragging.value = false
  currentX.value = 0
  currentY.value = 0
  
  document.removeEventListener('mousemove', onDrag)
  document.removeEventListener('mouseup', endDrag)
  document.removeEventListener('touchmove', onDrag)
  document.removeEventListener('touchend', endDrag)
}
</script>

<style scoped>
.recipe-card {
  width: 100%;
  max-width: 400px;
  height: 550px;
  border-radius: 24px;
  overflow: hidden;
  position: relative;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
  cursor: grab;
  transition: transform 0.3s ease;
  user-select: none;
}

.recipe-card:active {
  cursor: grabbing;
}

.recipe-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.card-overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  background: linear-gradient(to top, rgba(0, 0, 0, 0.85), transparent);
  padding: 2rem;
  color: white;
}

.tags {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
  flex-wrap: wrap;
}

.tag {
  background: rgba(255, 255, 255, 0.25);
  backdrop-filter: blur(10px);
  padding: 0.4rem 1rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 500;
}

.recipe-title {
  font-size: 1.75rem;
  font-weight: 700;
  margin: 0 0 1rem 0;
  line-height: 1.3;
}

.recipe-info {
  display: flex;
  gap: 1.5rem;
  font-size: 0.9rem;
  flex-wrap: wrap;
}

.info-item {
  display: flex;
  align-items: center;
  gap: 0.4rem;
}

.icon {
  width: 18px;
  height: 18px;
  stroke-width: 2;
}

.swipe-indicator {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  font-size: 5rem;
  font-weight: bold;
  opacity: 0.8;
  pointer-events: none;
}

.swipe-indicator.like {
  right: 2rem;
  color: #8CB369;
}

.swipe-indicator.dislike {
  left: 2rem;
  color: #ff6b6b;
}
</style>