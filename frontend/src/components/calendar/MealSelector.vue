<template>
  <div 
    style="position: fixed; inset: 0; background: rgba(0, 0, 0, 0.5); display: flex; align-items: center; justify-content: center; z-index: 200; padding: 16px; backdrop-filter: blur(5px);"
    @click="$emit('close')"
  >
    <div 
      @click.stop
      style="background: white; border-radius: 16px; max-width: 700px; width: 100%; max-height: 85vh; overflow: hidden; padding: 32px; position: relative;"
    >
      <!-- Header -->
      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; padding-bottom: 16px; border-bottom: 2px solid #e5e7eb;">
        <h2 style="font-size: 24px; font-weight: 700; color: #1f2937;">Välj ett recept</h2>
        
        <button 
          @click="$emit('close')"
          style="width: 40px; height: 40px; border-radius: 50%; background: #f3f4f6; border: none; display: flex; align-items: center; justify-content: center; cursor: pointer; transition: all 0.2s;"
          @mouseenter="$event.target.style.background = '#e5e7eb'"
          @mouseleave="$event.target.style.background = '#f3f4f6'"
        >
          <svg style="width: 20px; height: 20px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="18" y1="6" x2="6" y2="18"/>
            <line x1="6" y1="6" x2="18" y2="18"/>
          </svg>
        </button>
      </div>
      
      <!-- Grid -->
      <div v-if="recipes.length" style="display: grid; grid-template-columns: repeat(auto-fill, minmax(150px, 1fr)); gap: 16px; max-height: 60vh; overflow-y: auto;">
        <div 
          v-for="recipe in recipes" 
          :key="recipe.id"
          @click="$emit('select', recipe)"
          style="cursor: pointer; border-radius: 12px; overflow: hidden; background: #fefdf8; transition: all 0.3s; box-shadow: 0 2px 4px rgba(0,0,0,0.08);"
          @mouseenter="$event.target.style.transform = 'translateY(-4px)'; $event.target.style.boxShadow = '0 8px 16px rgba(0,0,0,0.12)'"
          @mouseleave="$event.target.style.transform = 'translateY(0)'; $event.target.style.boxShadow = '0 2px 4px rgba(0,0,0,0.08)'"
        >
          <div style="width: 100%; height: 120px; overflow: hidden;">
            <img 
              :src="recipe.imageUrl" 
              :alt="recipe.title"
              style="width: 100%; height: 100%; object-fit: cover; transition: transform 0.5s;"
              @mouseenter="$event.target.style.transform = 'scale(1.1)'"
              @mouseleave="$event.target.style.transform = 'scale(1)'"
            />
          </div>
          
          <div style="padding: 12px;">
            <h3 style="font-size: 14px; font-weight: 600; margin-bottom: 6px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; color: #1f2937;">
              {{ recipe.title }}
            </h3>
            <span style="font-size: 12px; color: #6b7280;">
              ⏱️ {{ recipe.cookTime }} min
            </span>
          </div>
        </div>
      </div>
      
      <!-- Empty State -->
      <div v-else style="text-align: center; padding: 60px 20px; color: #6b7280;">
        <div style="font-size: 60px; margin-bottom: 16px;">📭</div>
        <p style="margin-bottom: 8px; font-size: 16px; font-weight: 600; color: #1f2937;">Du har inga sparade recept än</p>
        <p style="font-size: 14px; color: #10b981;">Börja med att spara några recept i Upptäck-läget</p>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  recipes: {
    type: Array,
    required: true
  }
})

defineEmits(['select', 'close'])
</script>