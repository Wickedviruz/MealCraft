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
@keyframes pulse {
  0%, 100% {
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  }
  50% {
    box-shadow: 0 4px 20px rgba(140, 195, 105, 0.4);
  }
}

@keyframes pulseDislike {
  0%, 100% {
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  }
  50% {
    box-shadow: 0 4px 20px rgba(255, 107, 107, 0.4);
  }
}

@keyframes heartbeat {
  0%, 100% {
    transform: scale(1);
  }
  25% {
    transform: scale(1.2);
  }
  50% {
    transform: scale(1);
  }
  75% {
    transform: scale(1.1);
  }
}

.swipe-buttons {
  display: flex;
  gap: 2rem;
  justify-content: center;
  margin-top: 2rem;
  position: relative;
  z-index: 10;
}

.btn {
  width: 70px;
  height: 70px;
  border-radius: 50%;
  border: none;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  position: relative;
  overflow: hidden;
}

.btn::before {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: 50%;
  background: radial-gradient(circle, rgba(255,255,255,0.3), transparent);
  transform: scale(0);
  transition: transform 0.5s ease;
}

.btn:active::before {
  transform: scale(2);
  transition: transform 0s;
}

.btn:hover {
  transform: scale(1.15);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.15);
}

.btn:active {
  transform: scale(0.95);
}

.btn svg {
  width: 32px;
  height: 32px;
  transition: all 0.3s ease;
  position: relative;
  z-index: 1;
}

.btn:hover svg {
  transform: scale(1.1);
}

.btn-dislike {
  background: white;
  color: #ff6b6b;
}

.btn-dislike:hover {
  background: #fff5f5;
  animation: pulseDislike 1.5s ease-in-out infinite;
}

.btn-like {
  background: linear-gradient(135deg, #8CB369 0%, #7aa359 100%);
  color: white;
  box-shadow: 0 4px 15px rgba(140, 195, 105, 0.3);
}

.btn-like:hover {
  background: linear-gradient(135deg, #7aa359 0%, #6a9e4d 100%);
  animation: pulse 1.5s ease-in-out infinite;
}

.btn-like:hover svg {
  animation: heartbeat 1s ease-in-out infinite;
}
</style>