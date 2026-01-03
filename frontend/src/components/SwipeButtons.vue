<template>
  <div class="swipe-buttons">
    <button @click="$emit('dislike')" class="swipe-btn dislike-btn">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <line x1="18" y1="6" x2="6" y2="18"/>
        <line x1="6" y1="6" x2="18" y2="18"/>
      </svg>
    </button>
    
    <button @click="$emit('like')" class="swipe-btn like-btn">
      <svg viewBox="0 0 24 24" fill="currentColor">
        <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
      </svg>
    </button>
  </div>
</template>

<script setup>
defineEmits(['like', 'dislike'])
</script>

<style scoped>
.swipe-buttons {
  display: flex;
  justify-content: center;
  gap: var(--space-xl);
  margin-top: var(--space-lg);
  position: relative;
  z-index: 10;
}

.swipe-btn {
  width: 70px;
  height: 70px;
  border-radius: var(--radius-full);
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: var(--transition);
  box-shadow: var(--shadow-md);
}

.swipe-btn:active {
  transform: scale(0.95);
}

.dislike-btn {
  background: white;
  color: #ff6b6b;
}

.dislike-btn:hover {
  background: #ff6b6b;
  color: white;
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg);
}

.like-btn {
  background: var(--primary);
  color: white;
}

.like-btn:hover {
  background: var(--primary-dark);
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg);
}

.swipe-btn svg {
  width: 32px;
  height: 32px;
}
</style>