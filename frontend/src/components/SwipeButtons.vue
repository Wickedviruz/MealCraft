<template>
  <div class="flex justify-center gap-6 sm:gap-8 mt-6 relative z-10">
    <!-- Dislike Button -->
    <button 
      @click="$emit('dislike')" 
      class="w-16 h-16 sm:w-[70px] sm:h-[70px] rounded-full border-none bg-white text-red-500 flex items-center justify-center transition-all duration-200 shadow-md hover:bg-red-500 hover:text-white hover:-translate-y-1 hover:shadow-lg active:scale-95"
    >
      <svg class="w-7 h-7 sm:w-8 sm:h-8" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <line x1="18" y1="6" x2="6" y2="18"/>
        <line x1="6" y1="6" x2="18" y2="18"/>
      </svg>
    </button>
    
    <!-- Like Button -->
    <button 
      @click="$emit('like')" 
      class="w-16 h-16 sm:w-[70px] sm:h-[70px] rounded-full border-none bg-emerald-500 text-white flex items-center justify-center transition-all duration-200 shadow-md hover:bg-emerald-700 hover:-translate-y-1 hover:shadow-lg active:scale-95"
    >
      <svg class="w-7 h-7 sm:w-8 sm:h-8" viewBox="0 0 24 24" fill="currentColor">
        <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
      </svg>
    </button>
  </div>
</template>

<script setup>
defineEmits(['like', 'dislike'])
</script>