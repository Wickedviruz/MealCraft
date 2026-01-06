<template>
  <div style="padding: 24px 16px; max-width: 1280px; margin: 0 auto; background: #fefdf8; min-height: 100vh;">
    <!-- Header -->
    <div style="margin-bottom: 24px;">
      <h2 style="font-size: 32px; font-weight: 800; color: #1f2937; margin-bottom: 8px;">Sparade Recept</h2>
      <p style="color: #6b7280; font-size: 16px;">{{ recipes.length }} sparade</p>
    </div>
    
    <!-- Grid -->
    <div v-if="recipes.length" style="display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 20px;">
      <div 
        v-for="recipe in recipes" 
        :key="recipe.id"
        @click="$emit('select', recipe)"
        style="background: white; border-radius: 16px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.08); cursor: pointer; transition: all 0.3s ease; position: relative;"
        @mouseenter="$event.currentTarget.style.transform = 'translateY(-4px)'; $event.currentTarget.style.boxShadow = '0 12px 24px rgba(0,0,0,0.12)'"
        @mouseleave="$event.currentTarget.style.transform = 'translateY(0)'; $event.currentTarget.style.boxShadow = '0 2px 8px rgba(0,0,0,0.08)'"
      >
        <!-- Image -->
        <div style="position: relative; width: 100%; padding-bottom: 75%; background: #f3f4f6; overflow: hidden;">
          <img 
            :src="recipe.imageUrl" 
            :alt="recipe.title" 
            style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover; transition: transform 0.5s ease;"
            @mouseenter="$event.target.style.transform = 'scale(1.1)'"
            @mouseleave="$event.target.style.transform = 'scale(1)'"
          />
          
          <!-- Remove Button -->
          <button 
            @click.stop="$emit('remove', recipe.id)"
            title="Ta bort"
            style="position: absolute; top: 12px; right: 12px; width: 40px; height: 40px; border-radius: 50%; background: rgba(239, 68, 68, 0.95); color: white; border: none; cursor: pointer; display: flex; align-items: center; justify-content: center; transition: all 0.2s; opacity: 0; box-shadow: 0 2px 8px rgba(0,0,0,0.2);"
            @mouseenter="$event.target.style.transform = 'scale(1.15)'; $event.target.parentElement.parentElement.querySelector('button').style.opacity = '1'"
            @mouseleave="$event.target.style.transform = 'scale(1)'"
          >
            <svg style="width: 20px; height: 20px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="3 6 5 6 21 6"/>
              <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/>
            </svg>
          </button>
        </div>
        
        <!-- Content -->
        <div style="padding: 20px;">
          <h3 style="font-size: 18px; font-weight: 700; color: #1f2937; margin-bottom: 12px; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; min-height: 56px;">
            {{ recipe.title }}
          </h3>
          
          <div style="display: flex; gap: 12px; color: #6b7280; font-size: 14px; margin-bottom: 12px;">
            <span style="display: flex; align-items: center; gap: 6px;">
              <svg style="width: 16px; height: 16px; color: #10b981;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10"/>
                <polyline points="12 6 12 12 16 14"/>
              </svg>
              {{ recipe.cookTime }} min
            </span>
            <span v-if="recipe.rating" style="display: flex; align-items: center; gap: 6px;">
              <svg style="width: 16px; height: 16px; color: #10b981;" viewBox="0 0 24 24" fill="currentColor">
                <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/>
              </svg>
              {{ recipe.rating }}
            </span>
          </div>
          
          <div v-if="recipe.tags && recipe.tags.length" style="display: flex; gap: 8px; flex-wrap: wrap;">
            <span 
              v-for="tag in recipe.tags.slice(0, 2)" 
              :key="tag" 
              style="padding: 4px 10px; background: #d1fae5; color: #047857; border-radius: 6px; font-size: 12px; font-weight: 600;"
            >
              {{ tag }}
            </span>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Empty State -->
    <div v-else style="text-align: center; padding: 80px 20px;">
      <div style="font-size: 80px; margin-bottom: 24px; animation: pulse 2s ease-in-out infinite;">💚</div>
      <h3 style="font-size: 28px; font-weight: 800; color: #1f2937; margin-bottom: 12px;">Inga sparade recept</h3>
      <p style="color: #6b7280; font-size: 16px; max-width: 400px; margin: 0 auto; line-height: 1.6;">
        Swipa höger på recept du gillar för att spara dem här
      </p>
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

defineEmits(['select', 'remove'])
</script>

<style>
@keyframes pulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.1); }
}
</style>