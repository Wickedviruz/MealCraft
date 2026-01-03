<template>
  <div class="add-recipe">
    <header class="add-header">
      <button class="btn-icon" @click="$emit('cancel')">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <line x1="18" y1="6" x2="6" y2="18"/>
          <line x1="6" y1="6" x2="18" y2="18"/>
        </svg>
      </button>
      <h1>Lägg till recept</h1>
      <button @click="handleSave" class="btn btn-primary" :disabled="!isValid || isSaving">
        {{ isSaving ? 'Sparar...' : 'Spara' }}
      </button>
    </header>
    
    <div v-if="error" class="error-banner">
      {{ error }}
    </div>
    
    <form @submit.prevent="handleSave" class="recipe-form">
      <!-- Basic Info -->
      <section class="card">
        <h2 class="section-title">Grundinfo</h2>
        
        <label class="form-label">
          Receptnamn *
          <input v-model="recipe.title" type="text" placeholder="T.ex. Pasta Carbonara" class="form-input" required />
        </label>
        
        <label class="form-label">
          Beskrivning
          <textarea v-model="recipe.description" placeholder="Beskriv receptet..." class="form-textarea" rows="3"></textarea>
        </label>
        
        <label class="form-label">
          Bild URL
          <input v-model="recipe.imageUrl" type="url" placeholder="https://..." class="form-input" />
        </label>
        
        <div class="form-row">
          <label class="form-label">
            Tillagningstid (min) *
            <input v-model.number="recipe.cookTime" type="number" min="1" class="form-input" required />
          </label>
          
          <label class="form-label">
            Portioner *
            <input v-model.number="recipe.servings" type="number" min="1" class="form-input" required />
          </label>
        </div>
        
        <label class="form-label">
          Svårighetsgrad
          <select v-model="recipe.difficulty" class="form-select">
            <option value="Lätt">Lätt</option>
            <option value="Medel">Medel</option>
            <option value="Svår">Svår</option>
          </select>
        </label>
        
        <label class="form-label">
          Taggar
          <div class="tags-input">
            <span v-for="(tag, index) in recipe.tags" :key="index" class="tag">
              {{ tag }}
              <button @click="removeTag(index)" type="button" class="tag-remove">×</button>
            </span>
            <input v-model="newTag" @keydown.enter.prevent="addTag" type="text" placeholder="Lägg till tagg..." class="tag-input" />
          </div>
        </label>
      </section>
      
      <!-- Ingredients -->
      <section class="card">
        <h2 class="section-title">Ingredienser</h2>
        
        <div v-for="(ing, index) in recipe.ingredients" :key="index" class="ingredient-row">
          <input v-model.number="ing.amount" type="number" step="0.1" placeholder="Mängd" class="form-input small" />
          <input v-model="ing.unit" type="text" placeholder="g" class="form-input small" />
          <input v-model="ing.name" type="text" placeholder="Ingrediens" class="form-input flex-1" />
          <button @click="removeIngredient(index)" type="button" class="btn-icon">×</button>
        </div>
        
        <button @click="addIngredient" type="button" class="btn-add">+ Lägg till ingrediens</button>
      </section>
      
      <!-- Instructions -->
      <section class="card">
        <h2 class="section-title">Instruktioner</h2>
        
        <div v-for="(step, index) in recipe.instructions" :key="index" class="instruction-row">
          <span class="step-number">{{ index + 1 }}</span>
          <textarea v-model="step.text" placeholder="Beskriv steget..." class="form-textarea" rows="2"></textarea>
          <button @click="removeInstruction(index)" type="button" class="btn-icon">×</button>
        </div>
        
        <button @click="addInstruction" type="button" class="btn-add">+ Lägg till steg</button>
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
  ingredients: [{ amount: null, unit: 'g', name: '', category: 'Grains' }],
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
  recipe.value.ingredients.push({ amount: null, unit: 'g', name: '', category: 'Grains' })
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

<style scoped>
.add-recipe {
  background: var(--background);
  min-height: 100vh;
  padding-bottom: 100px;
}

.add-header {
  position: sticky;
  top: 0;
  z-index: var(--z-header);
  background: var(--surface);
  padding: var(--space-md) var(--space-lg);
  border-bottom: 1px solid var(--border);
  display: flex;
  align-items: center;
  gap: var(--space-md);
}

.add-header h1 {
  flex: 1;
  text-align: center;
  font-size: 1.5rem;
  font-weight: 700;
}

.recipe-form {
  max-width: 800px;
  margin: 0 auto;
  padding: var(--space-lg);
  display: flex;
  flex-direction: column;
  gap: var(--space-lg);
}

.section-title {
  font-size: 1.25rem;
  font-weight: 700;
  margin-bottom: var(--space-md);
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-md);
}

.tags-input {
  display: flex;
  flex-wrap: wrap;
  gap: var(--space-sm);
  padding: var(--space-sm);
  border: 2px solid var(--border);
  border-radius: var(--radius-sm);
  min-height: 50px;
}

.tag {
  display: inline-flex;
  align-items: center;
  gap: var(--space-sm);
  padding: 0.3rem 0.75rem;
  background: var(--primary);
  color: white;
  border-radius: var(--radius-md);
  font-size: 0.9rem;
}

.tag-remove {
  background: none;
  border: none;
  color: white;
  font-size: 1.25rem;
  cursor: pointer;
  padding: 0;
}

.tag-input {
  flex: 1;
  border: none;
  outline: none;
  min-width: 150px;
  font-size: 1rem;
}

.ingredient-row,
.instruction-row {
  display: flex;
  gap: var(--space-sm);
  margin-bottom: var(--space-sm);
  align-items: flex-start;
}

.form-input.small {
  width: 80px;
}

.form-input.flex-1 {
  flex: 1;
}

.step-number {
  width: 32px;
  height: 32px;
  border-radius: var(--radius-full);
  background: var(--primary);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  flex-shrink: 0;
  margin-top: 0.5rem;
}

.btn-add {
  width: 100%;
  padding: var(--space-sm);
  border: 2px dashed var(--border);
  border-radius: var(--radius-sm);
  background: transparent;
  color: var(--primary);
  font-weight: 600;
  cursor: pointer;
  transition: var(--transition);
}

.btn-add:hover {
  border-color: var(--primary);
  background: var(--primary-light);
}
</style>