<template>
  <div 
    :class="['recipe-card', { 'is-swiping': isDragging, 'swipe-exit': isExiting }]"
    :style="cardStyle"
    @mousedown="startDrag"
    @touchstart="startDrag"
  >
    <img :src="recipe.image" :alt="recipe.title" class="recipe-image" />
    
    <!-- Animated gradient overlay -->
    <div class="gradient-overlay"></div>
    
    <div class="card-overlay">
      <div class="tags">
        <span 
          v-for="(tag, index) in recipe.tags" 
          :key="tag" 
          class="tag"
          :style="{ animationDelay: `${index * 0.1}s` }"
        >
          {{ tag }}
        </span>
      </div>
      
      <h2 class="recipe-title">{{ recipe.title }}</h2>
      
      <div class="recipe-info">
        <span 
          v-for="(item, index) in infoItems" 
          :key="index"
          class="info-item"
          :style="{ animationDelay: `${0.2 + index * 0.1}s` }"
        >
          <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <component :is="item.iconPath" />
          </svg>
          {{ item.text }}
        </span>
      </div>
    </div>
    
    <!-- Swipe indicators with scale animation -->
    <Transition name="indicator">
      <div v-if="swipeDirection === 'left'" class="swipe-indicator dislike">
        <div class="indicator-content">✕</div>
      </div>
    </Transition>
    
    <Transition name="indicator">
      <div v-if="swipeDirection === 'right'" class="swipe-indicator like">
        <div class="indicator-content">♥</div>
      </div>
    </Transition>
    
    <!-- Sparkle effects on like -->
    <div v-if="swipeDirection === 'right'" class="sparkles">
      <div v-for="i in 6" :key="i" class="sparkle" :style="{ '--index': i }"></div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, h } from 'vue'

const props = defineProps({
  recipe: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['swipe-left', 'swipe-right'])

const isDragging = ref(false)
const isExiting = ref(false)
const startX = ref(0)
const currentX = ref(0)
const startY = ref(0)
const currentY = ref(0)

const infoItems = computed(() => [
  {
    iconPath: () => [
      h('circle', { cx: '12', cy: '12', r: '10' }),
      h('polyline', { points: '12 6 12 12 16 14' })
    ],
    text: `${props.recipe.cookTime} min`
  },
  {
    iconPath: () => [
      h('path', { d: 'M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2' }),
      h('circle', { cx: '9', cy: '7', r: '4' }),
      h('path', { d: 'M23 21v-2a4 4 0 0 0-3-3.87' }),
      h('path', { d: 'M16 3.13a4 4 0 0 1 0 7.75' })
    ],
    text: `${props.recipe.servings} servings`
  },
  {
    iconPath: () => [
      h('path', { d: 'M12 2L2 7l10 5 10-5-10-5z' }),
      h('path', { d: 'M2 17l10 5 10-5' }),
      h('path', { d: 'M2 12l10 5 10-5' })
    ],
    text: `${props.recipe.calories} kcal`
  }
])

const swipeDirection = computed(() => {
  const diff = currentX.value - startX.value
  if (Math.abs(diff) < 50) return null
  return diff > 0 ? 'right' : 'left'
})

const cardStyle = computed(() => {
  if (!isDragging.value && !isExiting.value) return {}
  
  const deltaX = currentX.value - startX.value
  const deltaY = currentY.value - startY.value
  const rotation = deltaX / 20
  const opacity = isExiting.value ? 0 : Math.max(0.3, 1 - Math.abs(deltaX) / 400)
  
  return {
    transform: `translate(${deltaX}px, ${deltaY}px) rotate(${rotation}deg) scale(${isExiting.value ? 0.8 : 1})`,
    opacity: opacity,
    transition: isExiting.value ? 'all 0.5s cubic-bezier(0.68, -0.55, 0.265, 1.55)' : 'none'
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
    isExiting.value = true
    
    setTimeout(() => {
      if (diff > 0) {
        emit('swipe-right', props.recipe)
      } else {
        emit('swipe-left', props.recipe)
      }
      isExiting.value = false
    }, 500)
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
@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes shimmer {
  0%, 100% {
    background-position: -100% 0;
  }
  50% {
    background-position: 200% 0;
  }
}

@keyframes float {
  0%, 100% {
    transform: translateY(0px);
  }
  50% {
    transform: translateY(-10px);
  }
}

@keyframes sparkle {
  0% {
    transform: translate(0, 0) scale(0) rotate(0deg);
    opacity: 1;
  }
  100% {
    transform: translate(var(--tx), var(--ty)) scale(1) rotate(720deg);
    opacity: 0;
  }
}

.recipe-card {
  width: 100%;
  max-width: 400px;
  height: 550px;
  border-radius: 24px;
  overflow: hidden;
  position: relative;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.2);
  cursor: grab;
  transition: transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1), 
              box-shadow 0.3s ease,
              opacity 0.3s ease;
  user-select: none;
  animation: fadeInUp 0.6s ease-out;
}

.recipe-card:hover {
  box-shadow: 0 25px 70px rgba(0, 0, 0, 0.25);
}

.recipe-card.is-swiping {
  cursor: grabbing;
  box-shadow: 0 30px 80px rgba(0, 0, 0, 0.3);
}

.recipe-card.swipe-exit {
  pointer-events: none;
}

.recipe-card:active {
  cursor: grabbing;
}

.recipe-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}

.recipe-card:hover .recipe-image {
  transform: scale(1.05);
}

.gradient-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, 
    rgba(140, 195, 105, 0.1) 0%, 
    transparent 50%,
    rgba(212, 231, 197, 0.15) 100%
  );
  background-size: 200% 200%;
  animation: shimmer 8s ease-in-out infinite;
  pointer-events: none;
}

.card-overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  background: linear-gradient(to top, 
    rgba(0, 0, 0, 0.9) 0%, 
    rgba(0, 0, 0, 0.7) 50%,
    transparent 100%
  );
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
  animation: fadeInUp 0.5s ease-out both;
  transition: all 0.3s ease;
}

.tag:hover {
  background: rgba(255, 255, 255, 0.35);
  transform: translateY(-2px);
}

.recipe-title {
  font-size: 1.75rem;
  font-weight: 700;
  margin: 0 0 1rem 0;
  line-height: 1.3;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.5);
  animation: fadeInUp 0.6s ease-out both;
  animation-delay: 0.1s;
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
  animation: fadeInUp 0.5s ease-out both;
  transition: transform 0.3s ease;
}

.info-item:hover {
  transform: translateY(-2px);
}

.icon {
  width: 18px;
  height: 18px;
  stroke-width: 2;
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.3));
}

.swipe-indicator {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  font-size: 5rem;
  font-weight: bold;
  pointer-events: none;
  z-index: 10;
}

.indicator-content {
  animation: float 1s ease-in-out infinite;
  filter: drop-shadow(0 4px 10px rgba(0, 0, 0, 0.3));
}

.swipe-indicator.like {
  right: 2rem;
  color: #8CB369;
}

.swipe-indicator.dislike {
  left: 2rem;
  color: #ff6b6b;
}

/* Transition for indicators */
.indicator-enter-active {
  animation: indicatorIn 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55);
}

.indicator-leave-active {
  animation: indicatorOut 0.3s ease-out;
}

@keyframes indicatorIn {
  from {
    opacity: 0;
    transform: translateY(-50%) scale(0);
  }
  to {
    opacity: 0.8;
    transform: translateY(-50%) scale(1);
  }
}

@keyframes indicatorOut {
  from {
    opacity: 0.8;
    transform: translateY(-50%) scale(1);
  }
  to {
    opacity: 0;
    transform: translateY(-50%) scale(0.5);
  }
}

/* Sparkle effects */
.sparkles {
  position: absolute;
  inset: 0;
  pointer-events: none;
}

.sparkle {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 8px;
  height: 8px;
  background: radial-gradient(circle, #fff, #8CB369);
  border-radius: 50%;
  animation: sparkle 1.5s ease-out infinite;
  animation-delay: calc(var(--index) * 0.1s);
  --tx: calc(cos(var(--index) * 60deg) * 150px);
  --ty: calc(sin(var(--index) * 60deg) * 150px);
}

/* Responsive */
@media (max-width: 480px) {
  .recipe-card {
    max-width: 100%;
    height: 500px;
  }
  
  .recipe-title {
    font-size: 1.5rem;
  }
  
  .swipe-indicator {
    font-size: 4rem;
  }
}
</style>