<template>
  <div 
    style="position: fixed; inset: 0; background: rgba(0, 0, 0, 0.5); display: flex; align-items: center; justify-content: center; z-index: 200; padding: 16px; backdrop-filter: blur(5px);"
    @click="$emit('close')"
  >
    <div 
      @click.stop
      style="background: white; border-radius: 16px; max-width: 900px; width: 100%; max-height: 90vh; overflow-y: auto; position: relative; animation: slideUp 0.3s ease;"
    >
      <!-- Close Button -->
      <button 
        @click="$emit('close')"
        style="position: sticky; top: 16px; right: 16px; float: right; width: 44px; height: 44px; border-radius: 50%; background: rgba(255, 255, 255, 0.95); backdrop-filter: blur(10px); border: none; cursor: pointer; display: flex; align-items: center; justify-content: center; z-index: 10; transition: all 0.2s; box-shadow: 0 2px 8px rgba(0,0,0,0.1); margin: 16px 16px 0 0;"
        @mouseenter="$event.target.style.transform = 'scale(1.1)'; $event.target.style.background = 'white'"
        @mouseleave="$event.target.style.transform = 'scale(1)'; $event.target.style.background = 'rgba(255, 255, 255, 0.95)'"
      >
        <svg style="width: 20px; height: 20px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <line x1="18" y1="6" x2="6" y2="18"/>
          <line x1="6" y1="6" x2="18" y2="18"/>
        </svg>
      </button>
      
      <!-- Image -->
      <div style="width: 100%; height: 300px; overflow: hidden; border-radius: 16px 16px 0 0;">
        <img 
          :src="recipe.imageUrl" 
          :alt="recipe.title" 
          style="width: 100%; height: 100%; object-fit: cover;"
        />
      </div>
      
      <!-- Content -->
      <div style="padding: 32px;">
        <!-- Header -->
        <div style="display: flex; justify-content: space-between; align-items: flex-start; gap: 16px; margin-bottom: 20px;">
          <h1 style="font-size: 32px; font-weight: 800; color: #1f2937; flex: 1;">
            {{ recipe.title }}
          </h1>
          
          <button 
            @click="$emit('toggle-save')"
            style="width: 50px; height: 50px; border-radius: 50%; border: none; cursor: pointer; display: flex; align-items: center; justify-content: center; transition: all 0.2s; flex-shrink: 0;"
            :style="isSaved 
              ? 'background: #10b981; color: white; box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);'
              : 'background: #f3f4f6; color: #6b7280;'"
            @mouseenter="$event.target.style.transform = 'scale(1.1)'"
            @mouseleave="$event.target.style.transform = 'scale(1)'"
          >
            <svg style="width: 24px; height: 24px;" viewBox="0 0 24 24" fill="currentColor">
              <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
            </svg>
          </button>
        </div>
        
        <p v-if="recipe.description" style="color: #6b7280; line-height: 1.6; margin-bottom: 24px; font-size: 16px;">
          {{ recipe.description }}
        </p>
        
        <!-- Meta Grid -->
        <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(120px, 1fr)); gap: 16px; margin-bottom: 24px;">
          <div style="display: flex; flex-direction: column; gap: 4px;">
            <span style="font-size: 12px; color: #6b7280; text-transform: uppercase; letter-spacing: 0.5px; font-weight: 600;">Tid</span>
            <span style="font-size: 18px; font-weight: 700; color: #1f2937;">{{ recipe.cookTime }} min</span>
          </div>
          <div style="display: flex; flex-direction: column; gap: 4px;">
            <span style="font-size: 12px; color: #6b7280; text-transform: uppercase; letter-spacing: 0.5px; font-weight: 600;">Portioner</span>
            <span style="font-size: 18px; font-weight: 700; color: #1f2937;">{{ recipe.servings }}</span>
          </div>
          <div v-if="recipe.difficulty" style="display: flex; flex-direction: column; gap: 4px;">
            <span style="font-size: 12px; color: #6b7280; text-transform: uppercase; letter-spacing: 0.5px; font-weight: 600;">Svårighet</span>
            <span style="font-size: 18px; font-weight: 700; color: #1f2937;">{{ recipe.difficulty }}</span>
          </div>
          <div v-if="recipe.calories" style="display: flex; flex-direction: column; gap: 4px;">
            <span style="font-size: 12px; color: #6b7280; text-transform: uppercase; letter-spacing: 0.5px; font-weight: 600;">Kalorier</span>
            <span style="font-size: 18px; font-weight: 700; color: #1f2937;">{{ recipe.calories }} kcal</span>
          </div>
        </div>
        
        <!-- Tags -->
        <div v-if="recipe.tags && recipe.tags.length" style="display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 32px;">
          <span 
            v-for="tag in recipe.tags" 
            :key="tag" 
            style="padding: 8px 16px; background: #d1fae5; color: #047857; border-radius: 8px; font-size: 14px; font-weight: 600;"
          >
            {{ tag }}
          </span>
        </div>
        
        <!-- Ingredients -->
        <section style="margin-bottom: 32px;">
          <h2 style="font-size: 24px; font-weight: 700; margin-bottom: 16px; color: #1f2937;">Ingredienser</h2>
          <ul style="list-style: none; display: flex; flex-direction: column; gap: 8px;">
            <li 
              v-for="ing in recipe.ingredients" 
              :key="ing.id"
              style="display: flex; gap: 16px; padding: 12px; border-radius: 8px; transition: background 0.2s;"
              @mouseenter="$event.target.style.background = '#f9fafb'"
              @mouseleave="$event.target.style.background = 'transparent'"
            >
              <span style="font-weight: 700; color: #10b981; min-width: 80px;">
                {{ ing.amount }} {{ ing.unit }}
              </span>
              <span style="color: #1f2937;">{{ ing.name }}</span>
            </li>
          </ul>
        </section>
        
        <!-- Instructions -->
        <section>
          <h2 style="font-size: 24px; font-weight: 700; margin-bottom: 16px; color: #1f2937;">Instruktioner</h2>
          <ol style="list-style: none; display: flex; flex-direction: column; gap: 20px; counter-reset: step;">
            <li 
              v-for="(step, index) in recipe.instructions" 
              :key="index"
              style="display: flex; gap: 16px; padding-left: 52px; position: relative; counter-increment: step; line-height: 1.6; color: #374151;"
            >
              <span style="content: counter(step); position: absolute; left: 0; width: 36px; height: 36px; background: #10b981; color: white; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: 700; flex-shrink: 0;">
                {{ index + 1 }}
              </span>
              {{ step.text }}
            </li>
          </ol>
        </section>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  recipe: {
    type: Object,
    required: true
  },
  isSaved: {
    type: Boolean,
    default: false
  }
})

defineEmits(['close', 'toggle-save'])
</script>

<style>
@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>