<template>
  <div class="swipe-buttons">
    <button class="btn btn-dislike" @click="emit('dislike')" aria-label="Skip recipe">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <line x1="18" y1="6" x2="6" y2="18"/>
        <line x1="6" y1="6" x2="18" y2="18"/>
      </svg>
    </button>
    
    <button class="btn btn-like" @click="emit('like')" aria-label="Save recipe">
      <svg viewBox="0 0 24 24" fill="currentColor">
        <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
      </svg>
    </button>
  </div>
</template>

<script setup>
const emit = defineEmits(['like', 'dislike'])
</script>

<style scoped>
.swipe-buttons {
  display: flex;
  gap: 2rem;
  justify-content: center;
  margin-top: 2rem;
}

.btn {
  width: 70px;
  height: 70px;
  border-radius: 50%;
  border: none;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.btn:hover {
  transform: scale(1.1);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.15);
}

.btn:active {
  transform: scale(0.95);
}

.btn svg {
  width: 32px;
  height: 32px;
}

.btn-dislike {
  background: white;
  color: #ff6b6b;
}

.btn-dislike:hover {
  background: #fff5f5;
}

.btn-like {
  background: #8CB369;
  color: white;
}

.btn-like:hover {
  background: #7aa359;
}
</style>