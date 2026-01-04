<template>
  <div style="background: #fefdf8; min-height: 100vh; padding-bottom: 100px;">
    <!-- Header -->
    <header style="position: sticky; top: 0; z-index: 100; background: white; padding: 16px 24px; border-bottom: 1px solid #e5e7eb; display: flex; align-items: center; gap: 16px;">
      <button 
        @click="$emit('cancel')"
        style="width: 40px; height: 40px; border-radius: 50%; background: #f3f4f6; border: none; display: flex; align-items: center; justify-content: center; cursor: pointer; transition: all 0.2s;"
        @mouseenter="$event.target.style.background = '#e5e7eb'"
        @mouseleave="$event.target.style.background = '#f3f4f6'"
      >
        <svg style="width: 20px; height: 20px;" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <line x1="18" y1="6" x2="6" y2="18"/>
          <line x1="6" y1="6" x2="18" y2="18"/>
        </svg>
      </button>
      
      <h1 style="flex: 1; text-align: center; font-size: 24px; font-weight: 700; color: #1f2937;">
        Lägg till recept
      </h1>
      
      <button 
        @click="handleSave" 
        :disabled="!isValid || isSaving"
        class="btn btn-primary"
        :style="!isValid || isSaving ? 'opacity: 0.5; cursor: not-allowed;' : ''"
      >
        {{ isSaving ? 'Sparar...' : 'Spara' }}
      </button>
    </header>
    
    <!-- Error -->
    <div v-if="error" style="margin: 16px; padding: 16px; background: #fee2e2; border: 2px solid #ef4444; border-radius: 12px; color: #991b1b; font-weight: 600;">
      {{ error }}
    </div>
    
    <!-- Form -->
    <form @submit.prevent="handleSave" style="max-width: 800px; margin: 0 auto; padding: 24px; display: flex; flex-direction: column; gap: 24px;">
      
      <!-- Basic Info -->
      <section style="background: white; border-radius: 16px; padding: 24px; box-shadow: 0 2px 8px rgba(0,0,0,0.08);">
        <h2 style="font-size: 20px; font-weight: 700; margin-bottom: 20px; color: #1f2937;">Grundinfo</h2>
        
        <label style="display: block; margin-bottom: 20px;">
          <span style="display: block; margin-bottom: 8px; font-weight: 600; color: #374151;">Receptnamn *</span>
          <input 
            v-model="recipe.title" 
            type="text" 
            placeholder="T.ex. Pasta Carbonara" 
            required
            style="width: 100%; padding: 12px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 16px; outline: none; transition: border-color 0.2s;"
            @focus="$event.target.style.borderColor = '#10b981'"
            @blur="$event.target.style.borderColor = '#e5e7eb'"
          />
        </label>
        
        <label style="display: block; margin-bottom: 20px;">
          <span style="display: block; margin-bottom: 8px; font-weight: 600; color: #374151;">Beskrivning</span>
          <textarea 
            v-model="recipe.description" 
            placeholder="Beskriv receptet..." 
            rows="3"
            style="width: 100%; padding: 12px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 16px; font-family: inherit; outline: none; transition: border-color 0.2s; resize: vertical;"
            @focus="$event.target.style.borderColor = '#10b981'"
            @blur="$event.target.style.borderColor = '#e5e7eb'"
          ></textarea>
        </label>
        
        <label style="display: block; margin-bottom: 20px;">
          <span style="display: block; margin-bottom: 8px; font-weight: 600; color: #374151;">Bild URL</span>
          <input 
            v-model="recipe.imageUrl" 
            type="url" 
            placeholder="https://..." 
            style="width: 100%; padding: 12px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 16px; outline: none; transition: border-color 0.2s;"
            @focus="$event.target.style.borderColor = '#10b981'"
            @blur="$event.target.style.borderColor = '#e5e7eb'"
          />
        </label>
        
        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 20px;">
          <label>
            <span style="display: block; margin-bottom: 8px; font-weight: 600; color: #374151;">Tillagningstid (min) *</span>
            <input 
              v-model.number="recipe.cookTime" 
              type="number" 
              min="1" 
              required
              style="width: 100%; padding: 12px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 16px; outline: none; transition: border-color 0.2s;"
              @focus="$event.target.style.borderColor = '#10b981'"
              @blur="$event.target.style.borderColor = '#e5e7eb'"
            />
          </label>
          
          <label>
            <span style="display: block; margin-bottom: 8px; font-weight: 600; color: #374151;">Portioner *</span>
            <input 
              v-model.number="recipe.servings" 
              type="number" 
              min="1" 
              required
              style="width: 100%; padding: 12px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 16px; outline: none; transition: border-color 0.2s;"
              @focus="$event.target.style.borderColor = '#10b981'"
              @blur="$event.target.style.borderColor = '#e5e7eb'"
            />
          </label>
        </div>
        
        <label style="display: block; margin-bottom: 20px;">
          <span style="display: block; margin-bottom: 8px; font-weight: 600; color: #374151;">Svårighetsgrad</span>
          <select 
            v-model="recipe.difficulty"
            style="width: 100%; padding: 12px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 16px; outline: none; cursor: pointer; background: white; transition: border-color 0.2s;"
            @focus="$event.target.style.borderColor = '#10b981'"
            @blur="$event.target.style.borderColor = '#e5e7eb'"
          >
            <option value="Enkel">Enkel</option>
            <option value="Medel">Medel</option>
            <option value="Avancerad">Avancerad</option>
          </select>
        </label>
        
        <label>
          <span style="display: block; margin-bottom: 8px; font-weight: 600; color: #374151;">Taggar</span>
          <div style="display: flex; flex-wrap: wrap; gap: 8px; padding: 12px; border: 2px solid #e5e7eb; border-radius: 8px; min-height: 50px;">
            <span 
              v-for="(tag, index) in recipe.tags" 
              :key="index" 
              style="display: inline-flex; align-items: center; gap: 8px; padding: 6px 12px; background: #10b981; color: white; border-radius: 8px; font-size: 14px; font-weight: 600;"
            >
              {{ tag }}
              <button 
                @click="removeTag(index)" 
                type="button"
                style="background: none; border: none; color: white; font-size: 20px; cursor: pointer; padding: 0; line-height: 1;"
              >×</button>
            </span>
            <input 
              v-model="newTag" 
              @keydown.enter.prevent="addTag" 
              type="text" 
              placeholder="Lägg till tagg..." 
              style="flex: 1; border: none; outline: none; min-width: 150px; font-size: 16px;"
            />
          </div>
        </label>
      </section>
      
      <!-- Ingredients -->
      <section style="background: white; border-radius: 16px; padding: 24px; box-shadow: 0 2px 8px rgba(0,0,0,0.08);">
        <h2 style="font-size: 20px; font-weight: 700; margin-bottom: 20px; color: #1f2937;">Ingredienser</h2>
        
        <div 
          v-for="(ing, index) in recipe.ingredients" 
          :key="index" 
          style="display: flex; gap: 8px; margin-bottom: 12px; align-items: flex-start;"
        >
          <input 
            v-model.number="ing.amount" 
            type="number" 
            step="0.1" 
            placeholder="Mängd"
            style="width: 80px; padding: 10px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 14px; outline: none;"
          />
          <input 
            v-model="ing.unit" 
            type="text" 
            placeholder="g"
            style="width: 60px; padding: 10px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 14px; outline: none;"
          />
          <input 
            v-model="ing.name" 
            type="text" 
            placeholder="Ingrediens"
            style="flex: 1; padding: 10px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 14px; outline: none;"
          />
          <button 
            @click="removeIngredient(index)" 
            type="button"
            style="width: 40px; height: 40px; border-radius: 50%; background: #f3f4f6; border: none; font-size: 20px; cursor: pointer; transition: all 0.2s;"
            @mouseenter="$event.target.style.background = '#fecaca'; $event.target.style.color = '#ef4444'"
            @mouseleave="$event.target.style.background = '#f3f4f6'; $event.target.style.color = 'inherit'"
          >×</button>
        </div>
        
        <button 
          @click="addIngredient" 
          type="button"
          style="width: 100%; padding: 12px; border: 2px dashed #e5e7eb; border-radius: 8px; background: transparent; color: #10b981; font-weight: 600; cursor: pointer; transition: all 0.2s;"
          @mouseenter="$event.target.style.borderColor = '#10b981'; $event.target.style.background = '#d1fae5'"
          @mouseleave="$event.target.style.borderColor = '#e5e7eb'; $event.target.style.background = 'transparent'"
        >
          + Lägg till ingrediens
        </button>
      </section>
      
      <!-- Instructions -->
      <section style="background: white; border-radius: 16px; padding: 24px; box-shadow: 0 2px 8px rgba(0,0,0,0.08);">
        <h2 style="font-size: 20px; font-weight: 700; margin-bottom: 20px; color: #1f2937;">Instruktioner</h2>
        
        <div 
          v-for="(step, index) in recipe.instructions" 
          :key="index" 
          style="display: flex; gap: 12px; margin-bottom: 16px; align-items: flex-start;"
        >
          <span style="width: 32px; height: 32px; border-radius: 50%; background: #10b981; color: white; display: flex; align-items: center; justify-content: center; font-weight: 700; flex-shrink: 0; margin-top: 8px;">
            {{ index + 1 }}
          </span>
          <textarea 
            v-model="step.text" 
            placeholder="Beskriv steget..." 
            rows="2"
            style="flex: 1; padding: 10px; border: 2px solid #e5e7eb; border-radius: 8px; font-size: 14px; font-family: inherit; outline: none; resize: vertical;"
          ></textarea>
          <button 
            @click="removeInstruction(index)" 
            type="button"
            style="width: 40px; height: 40px; border-radius: 50%; background: #f3f4f6; border: none; font-size: 20px; cursor: pointer; transition: all 0.2s; margin-top: 8px;"
            @mouseenter="$event.target.style.background = '#fecaca'; $event.target.style.color = '#ef4444'"
            @mouseleave="$event.target.style.background = '#f3f4f6'; $event.target.style.color = 'inherit'"
          >×</button>
        </div>
        
        <button 
          @click="addInstruction" 
          type="button"
          style="width: 100%; padding: 12px; border: 2px dashed #e5e7eb; border-radius: 8px; background: transparent; color: #10b981; font-weight: 600; cursor: pointer; transition: all 0.2s;"
          @mouseenter="$event.target.style.borderColor = '#10b981'; $event.target.style.background = '#d1fae5'"
          @mouseleave="$event.target.style.borderColor = '#e5e7eb'; $event.target.style.background = 'transparent'"
        >
          + Lägg till steg
        </button>
      </section>
    </form>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const emit = defineEmits(['save', 'cancel'])

const recipe = ref({
  title: '',
  description: '',
  imageUrl: '',
  cookTime: null,
  servings: null,
  difficulty: 'Medel',
  tags: [],
  ingredients: [{ amount: null, unit: 'g', name: '' }],
  instructions: [{ text: '', timer: null }]
})

const newTag = ref('')
const isSaving = ref(false)
const error = ref(null)

const isValid = computed(() => {
  return recipe.value.title && 
         recipe.value.cookTime && 
         recipe.value.servings &&
         recipe.value.ingredients.some(i => i.name) &&
         recipe.value.instructions.some(i => i.text)
})

function addTag() {
  if (newTag.value.trim() && !recipe.value.tags.includes(newTag.value.trim())) {
    recipe.value.tags.push(newTag.value.trim())
    newTag.value = ''
  }
}

function removeTag(index) {
  recipe.value.tags.splice(index, 1)
}

function addIngredient() {
  recipe.value.ingredients.push({ amount: null, unit: 'g', name: '' })
}

function removeIngredient(index) {
  if (recipe.value.ingredients.length > 1) {
    recipe.value.ingredients.splice(index, 1)
  }
}

function addInstruction() {
  recipe.value.instructions.push({ text: '', timer: null })
}

function removeInstruction(index) {
  if (recipe.value.instructions.length > 1) {
    recipe.value.instructions.splice(index, 1)
  }
}

function handleSave() {
  if (!isValid.value) return
  
  isSaving.value = true
  error.value = null
  
  emit('save', recipe.value)
}
</script>