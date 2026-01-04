<template>
  <div class="flex items-center justify-center min-h-[calc(100vh-180px)] p-6">
    <!-- Swipe Container -->
    <div v-if="currentRecipe || nextRecipe" style="width: 100%; max-width: 400px;">
      <div class="relative" style="width: 100%;">
        <!-- Background cards -->
        <div style="position: absolute; inset: 0; display: flex; align-items: center; justify-content: center; pointer-events: none;">
          <div 
            v-if="thirdRecipe" 
            style="position: absolute; width: 95%; height: 550px; border-radius: 24px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.1); transform: translateY(16px) scale(0.95); opacity: 0.3; transition: all 0.3s ease;"
          >
            <img :src="thirdRecipe.imageUrl" :alt="thirdRecipe.title" style="width: 100%; height: 100%; object-fit: cover;" />
          </div>
          
          <div 
            v-if="nextRecipe" 
            style="position: absolute; width: 97.5%; height: 550px; border-radius: 24px; overflow: hidden; box-shadow: 0 15px 40px rgba(0,0,0,0.15); transform: translateY(8px) scale(0.975); opacity: 0.6; transition: all 0.3s ease;"
          >
            <img :src="nextRecipe.imageUrl" :alt="nextRecipe.title" style="width: 100%; height: 100%; object-fit: cover;" />
          </div>
        </div>
        
        <!-- Current Card -->
        <Transition 
          @enter="onEnter"
          @leave="onLeave"
        >
          <div
            v-if="currentRecipe"
            :key="currentRecipe.id"
            ref="cardRef"
            style="position: relative; width: 100%; height: 550px; background: white; border-radius: 24px; overflow: hidden; box-shadow: 0 20px 50px rgba(0,0,0,0.2); cursor: grab; user-select: none;"
            :style="[cardStyle, { transition: isDragging ? 'none' : 'all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1)' }]"
            @touchstart="handleTouchStart"
            @touchmove="handleTouchMove"
            @touchend="handleTouchEnd"
            @mousedown="handleMouseDown"
            @mousemove="handleMouseMove"
            @mouseup="handleMouseEnd"
            @mouseleave="handleMouseEnd"
          >
            <!-- Image -->
            <img 
              :src="currentRecipe.imageUrl" 
              :alt="currentRecipe.title" 
              style="width: 100%; height: 100%; object-fit: cover;"
            />
            
            <!-- Swipe Indicators -->
            <div 
              style="position: absolute; top: 50%; left: 24px; transform: translateY(-50%); width: 80px; height: 80px; border-radius: 50%; background: rgba(239, 68, 68, 0.9); display: flex; align-items: center; justify-content: center; transition: opacity 0.2s ease;"
              :style="{ opacity: swipeDirection === 'left' ? 1 : 0 }"
            >
              <svg style="width: 40px; height: 40px; color: white;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                <line x1="18" y1="6" x2="6" y2="18"/>
                <line x1="6" y1="6" x2="18" y2="18"/>
              </svg>
            </div>
            
            <div 
              style="position: absolute; top: 50%; right: 24px; transform: translateY(-50%); width: 80px; height: 80px; border-radius: 50%; background: rgba(16, 185, 129, 0.9); display: flex; align-items: center; justify-content: center; transition: opacity 0.2s ease;"
              :style="{ opacity: swipeDirection === 'right' ? 1 : 0 }"
            >
              <svg style="width: 40px; height: 40px; color: white;" viewBox="0 0 24 24" fill="currentColor">
                <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
              </svg>
            </div>
            
            <!-- Gradient -->
            <div style="position: absolute; inset: 0; background: linear-gradient(to top, rgba(0,0,0,0.85) 0%, rgba(0,0,0,0.3) 40%, transparent 70%);"></div>
            
            <!-- Content -->
            <div style="position: absolute; bottom: 0; left: 0; right: 0; padding: 28px; color: white;">
              <!-- Tags -->
              <div v-if="currentRecipe.tags && currentRecipe.tags.length" style="display: flex; gap: 8px; margin-bottom: 14px; flex-wrap: wrap;">
                <span 
                  v-for="tag in currentRecipe.tags.slice(0, 2)" 
                  :key="tag"
                  style="padding: 6px 14px; background: rgba(255,255,255,0.25); backdrop-filter: blur(10px); border-radius: 20px; font-size: 12px; font-weight: 600; text-transform: capitalize;"
                >
                  {{ tag }}
                </span>
              </div>
              
              <!-- Title -->
              <h2 style="font-size: 32px; font-weight: 800; margin-bottom: 14px; line-height: 1.1; text-shadow: 0 2px 8px rgba(0,0,0,0.3);">
                {{ currentRecipe.title }}
              </h2>
              
              <!-- Meta -->
              <div style="display: flex; gap: 18px; font-size: 14px; flex-wrap: wrap;">
                <span style="display: flex; align-items: center; gap: 6px;">
                  <svg style="width: 16px; height: 16px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <circle cx="12" cy="12" r="10"/>
                    <polyline points="12 6 12 12 16 14"/>
                  </svg>
                  {{ currentRecipe.cookTime }} min
                </span>
                
                <span v-if="currentRecipe.servings" style="display: flex; align-items: center; gap: 6px;">
                  <svg style="width: 16px; height: 16px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
                    <circle cx="9" cy="7" r="4"/>
                  </svg>
                  {{ currentRecipe.servings }} port
                </span>
                
                <span v-if="currentRecipe.difficulty" style="display: flex; align-items: center; gap: 6px;">
                  <svg style="width: 16px; height: 16px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/>
                  </svg>
                  {{ currentRecipe.difficulty }}
                </span>
              </div>
            </div>
          </div>
        </Transition>
      </div>
      
      <!-- Buttons -->
      <div style="display: flex; justify-content: center; align-items: center; gap: 28px; margin-top: 36px;">
        <button 
          @click="handleDislike" 
          @mouseenter="hoverDislike = true"
          @mouseleave="hoverDislike = false"
          style="width: 84px; height: 84px; border-radius: 50%; background: white; color: #ef4444; box-shadow: 0 8px 20px rgba(0,0,0,0.12); border: 3px solid #fee2e2; display: flex; align-items: center; justify-content: center; cursor: pointer; transition: all 0.25s cubic-bezier(0.34, 1.56, 0.64, 1);"
          :style="{ 
            transform: hoverDislike ? 'scale(1.15) translateY(-4px)' : 'scale(1)',
            background: hoverDislike ? '#ef4444' : 'white',
            color: hoverDislike ? 'white' : '#ef4444',
            boxShadow: hoverDislike ? '0 12px 30px rgba(239, 68, 68, 0.3)' : '0 8px 20px rgba(0,0,0,0.12)'
          }"
        >
          <svg style="width: 34px; height: 34px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <line x1="18" y1="6" x2="6" y2="18"/>
            <line x1="6" y1="6" x2="18" y2="18"/>
          </svg>
        </button>
        
        <button 
          @click="handleLike" 
          @mouseenter="hoverLike = true"
          @mouseleave="hoverLike = false"
          style="width: 84px; height: 84px; border-radius: 50%; background: #10b981; color: white; box-shadow: 0 12px 30px rgba(16, 185, 129, 0.35); border: 3px solid #6ee7b7; display: flex; align-items: center; justify-content: center; cursor: pointer; transition: all 0.25s cubic-bezier(0.34, 1.56, 0.64, 1);"
          :style="{ 
            transform: hoverLike ? 'scale(1.15) translateY(-6px)' : 'scale(1)',
            boxShadow: hoverLike ? '0 16px 40px rgba(16, 185, 129, 0.45)' : '0 12px 30px rgba(16, 185, 129, 0.35)'
          }"
        >
          <svg style="width: 42px; height: 42px;" viewBox="0 0 24 24" fill="currentColor">
            <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
          </svg>
        </button>
      </div>
    </div>
    
    <!-- Empty State -->
    <div v-else style="text-align: center; max-width: 400px; animation: fadeIn 0.5s ease;">
      <div style="width: 100px; height: 100px; margin: 0 auto 28px; border-radius: 50%; background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%); display: flex; align-items: center; justify-content: center; box-shadow: 0 8px 24px rgba(16, 185, 129, 0.2); animation: pulse 2s ease-in-out infinite;">
        <svg style="width: 52px; height: 52px; color: #10b981;" viewBox="0 0 24 24" fill="currentColor">
          <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
        </svg>
      </div>
      
      <h2 style="font-size: 28px; font-weight: 800; margin-bottom: 14px; color: #1f2937;">All caught up!</h2>
      <p style="color: #6b7280; margin-bottom: 28px; font-size: 16px; line-height: 1.6;">
        You've swiped through all recipes. Check your saved recipes or reset to start again.
      </p>
      
      <button 
        @click="$emit('reset')" 
        class="btn btn-primary"
        style="display: inline-flex; align-items: center; gap: 10px; font-size: 16px;"
      >
        <svg style="width: 20px; height: 20px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <polyline points="23 4 23 10 17 10"/>
          <path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/>
        </svg>
        Start Over
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

defineProps({
  currentRecipe: Object,
  nextRecipe: Object,
  thirdRecipe: Object
})

const emit = defineEmits(['like', 'dislike', 'reset'])

const isDragging = ref(false)
const startX = ref(0)
const currentX = ref(0)
const swipeDirection = ref(null)
const cardRef = ref(null)
const hoverLike = ref(false)
const hoverDislike = ref(false)

const SWIPE_THRESHOLD = 100

const cardStyle = computed(() => {
  if (!isDragging.value) return {}
  
  const diff = currentX.value - startX.value
  const rotation = diff / 20
  const opacity = 1 - Math.abs(diff) / 300
  
  return {
    transform: `translateX(${diff}px) rotate(${rotation}deg)`,
    opacity: opacity < 0.5 ? 0.5 : opacity
  }
})

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
    diff > 0 ? handleLike() : handleDislike()
  }
  resetSwipe()
}

function handleMouseDown(e) {
  isDragging.value = true
  startX.value = e.clientX
  if (cardRef.value) {
    cardRef.value.style.cursor = 'grabbing'
  }
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
    diff > 0 ? handleLike() : handleDislike()
  }
  resetSwipe()
  if (cardRef.value) {
    cardRef.value.style.cursor = 'grab'
  }
}

function handleLike() {
  emit('like')
  resetSwipe()
}

function handleDislike() {
  emit('dislike')
  resetSwipe()
}

function resetSwipe() {
  isDragging.value = false
  startX.value = 0
  currentX.value = 0
  swipeDirection.value = null
}

// Vue Transition hooks
function onEnter(el, done) {
  el.style.opacity = '0'
  el.style.transform = 'translateY(-20px) scale(0.95)'
  
  setTimeout(() => {
    el.style.transition = 'all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1)'
    el.style.opacity = '1'
    el.style.transform = 'translateY(0) scale(1)'
    setTimeout(done, 400)
  }, 10)
}

function onLeave(el, done) {
  el.style.transition = 'all 0.3s ease-out'
  el.style.opacity = '0'
  el.style.transform = 'translateX(300px) rotate(20deg) scale(0.8)'
  setTimeout(done, 300)
}
</script>

<style>
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes pulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.05); }
}

/* RESPONSIVA KORT-HÖJDER */
@media (max-height: 800px) {
  /* Tablets och mindre skärmar */
  div[style*="height: 550px"] {
    height: 450px !important;
  }
}

@media (max-height: 700px) {
  /* Mobil landscape eller små tablets */
  div[style*="height: 550px"] {
    height: 400px !important;
  }
}

@media (max-height: 600px) {
  /* Väldigt små skärmar */
  div[style*="height: 550px"] {
    height: 350px !important;
  }
}
</style>