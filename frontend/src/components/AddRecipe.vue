<template>
  <div class="add-recipe">
    <header class="add-header">
      <button class="btn-cancel" @click="emit('cancel')">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <line x1="18" y1="6" x2="6" y2="18"/>
          <line x1="6" y1="6" x2="18" y2="18"/>
        </svg>
      </button>
      <h1>Lägg till recept</h1>
      <button @click="saveRecipe" class="btn-save" :disabled="!isValid">
        Spara
      </button>
    </header>
    
    <form @submit.prevent="saveRecipe" class="recipe-form">
      <section class="form-section">
        <div class="image-upload">
          <input 
            type="file" 
            id="image-upload"
            accept="image/*"
            @change="handleImageUpload"
            class="hidden-input"
          />
          <label for="image-upload" class="upload-label">
            <div v-if="!recipe.image" class="upload-placeholder">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <rect x="3" y="3" width="18" height="18" rx="2" ry="2"/>
                <circle cx="8.5" cy="8.5" r="1.5"/>
                <polyline points="21 15 16 10 5 21"/>
              </svg>
              <span>Lägg till bild</span>
            </div>
            <img v-else :src="recipe.image" alt="Recipe preview" class="preview-image" />
          </label>
        </div>
      </section>
      
      <section class="form-section">
        <label class="form-label">
          Receptnamn *
          <input 
            v-model="recipe.title"
            type="text" 
            placeholder="T.ex. Pasta Carbonara"
            class="form-input"
            required
          />
        </label>
        
        <label class="form-label">
          Beskrivning
          <textarea 
            v-model="recipe.description"
            placeholder="Beskriv receptet..."
            class="form-textarea"
            rows="3"
          ></textarea>
        </label>
        
        <div class="form-row">
          <label class="form-label">
            Tillagningstid (min) *
            <input 
              v-model.number="recipe.cookTime"
              type="number" 
              min="1"
              placeholder="30"
              class="form-input"
              required
            />
          </label>
          
          <label class="form-label">
            Portioner *
            <input 
              v-model.number="recipe.servings"
              type="number" 
              min="1"
              placeholder="4"
              class="form-input"
              required
            />
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
            <span 
              v-for="(tag, index) in recipe.tags" 
              :key="index"
              class="tag-item"
            >
              {{ tag }}
              <button @click="removeTag(index)" type="button" class="tag-remove">×</button>
            </span>
            <input 
              v-model="newTag"
              @keydown.enter.prevent="addTag"
              @keydown.comma.prevent="addTag"
              type="text" 
              placeholder="Lägg till tagg..."
              class="tag-input"
            />
          </div>
          <small class="form-hint">Tryck Enter eller komma för att lägga till</small>
        </label>
      </section>
      
      <section class="form-section">
        <h2 class="section-title">Ingredienser</h2>
        
        <div 
          v-for="(ingredient, index) in recipe.ingredients" 
          :key="index"
          class="ingredient-row"
        >
          <input 
            v-model.number="ingredient.amount"
            type="number" 
            step="0.1"
            placeholder="Mängd"
            class="form-input small"
          />
          <input 
            v-model="ingredient.unit"
            type="text" 
            placeholder="Enhet"
            class="form-input small"
          />
          <input 
            v-model="ingredient.name"
            type="text" 
            placeholder="Ingrediens"
            class="form-input flex-1"
          />
          <select v-model="ingredient.category" class="form-select small">
            <option value="Produce">Grönsaker</option>
            <option value="Meat">Kött/Fisk</option>
            <option value="Dairy">Mejeri</option>
            <option value="Grains">Spannmål</option>
            <option value="Spices">Kryddor</option>
            <option value="Pantry">Skafferi</option>
          </select>
          <button @click="removeIngredient(index)" type="button" class="btn-remove">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="18" y1="6" x2="6" y2="18"/>
              <line x1="6" y1="6" x2="18" y2="18"/>
            </svg>
          </button>
        </div>
        
        <button @click="addIngredient" type="button" class="btn-add">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="12" y1="5" x2="12" y2="19"/>
            <line x1="5" y1="12" x2="19" y2="12"/>
          </svg>
          Lägg till ingrediens
        </button>
      </section>
      
      <section class="form-section">
        <h2 class="section-title">Instruktioner</h2>
        
        <div 
          v-for="(step, index) in recipe.instructions" 
          :key="index"
          class="instruction-row"
        >
          <span class="step-number">{{ index + 1 }}</span>
          <textarea 
            v-model="step.text"
            placeholder="Beskriv steget..."
            class="form-textarea"
            rows="2"
          ></textarea>
          <input 
            v-model="step.timer"
            type="text" 
            placeholder="00:00:00"
            class="form-input small"
          />
          <button @click="removeInstruction(index)" type="button" class="btn-remove">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="18" y1="6" x2="6" y2="18"/>
              <line x1="6" y1="6" x2="18" y2="18"/>
            </svg>
          </button>
        </div>
        
        <button @click="addInstruction" type="button" class="btn-add">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="12" y1="5" x2="12" y2="19"/>
            <line x1="5" y1="12" x2="19" y2="12"/>
          </svg>
          Lägg till steg
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
  image: '',
  cookTime: null,
  servings: null,
  difficulty: 'Medel',
  tags: [],
  ingredients: [{ amount: null, unit: '', name: '', category: 'Produce' }],
  instructions: [{ text: '', timer: null }]
})

const newTag = ref('')

const isValid = computed(() => {
  return recipe.value.title && 
         recipe.value.cookTime && 
         recipe.value.servings &&
         recipe.value.ingredients.some(i => i.name) &&
         recipe.value.instructions.some(i => i.text)
})

function handleImageUpload(event) {
  const file = event.target.files[0]
  if (file) {
    const reader = new FileReader()
    reader.onload = (e) => {
      recipe.value.image = e.target.result
    }
    reader.readAsDataURL(file)
  }
}

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
  recipe.value.ingredients.push({ amount: null, unit: '', name: '', category: 'Produce' })
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

function saveRecipe() {
  if (isValid.value) {
    emit('save', {
      ...recipe.value,
      id: Date.now(),
      createdAt: Date.now(),
      rating: 0,
      reviews: 0
    })
  }
}
</script>

<style scoped>
.add-recipe {
  background: #FEFDF8;
  min-height: 100vh;
  padding-bottom: 100px;
}

.add-header {
  position: sticky;
  top: 0;
  z-index: 50;
  background: white;
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #D4E7C5;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
}

.add-header h1 {
  flex: 1;
  text-align: center;
  font-size: 1.5rem;
  font-weight: 700;
  color: #2c3e50;
  margin: 0;
}

.btn-cancel {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  border: none;
  background: #f0f0f0;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.btn-cancel:hover {
  background: #e0e0e0;
}

.btn-cancel svg {
  width: 20px;
  height: 20px;
}

.btn-save {
  padding: 0.75rem 1.5rem;
  background: #8CB369;
  color: white;
  border: none;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-save:hover:not(:disabled) {
  background: #7aa359;
}

.btn-save:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.recipe-form {
  max-width: 800px;
  margin: 0 auto;
  padding: 1.5rem;
}

.form-section {
  background: white;
  border-radius: 16px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.section-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #2c3e50;
  margin: 0 0 1rem 0;
}

.image-upload {
  margin-bottom: 1rem;
}

.hidden-input {
  display: none;
}

.upload-label {
  display: block;
  cursor: pointer;
}

.upload-placeholder {
  height: 200px;
  border: 2px dashed #D4E7C5;
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  color: #888;
  transition: all 0.3s;
}

.upload-placeholder:hover {
  border-color: #8CB369;
  background: #f9fdf7;
}

.upload-placeholder svg {
  width: 48px;
  height: 48px;
}

.preview-image {
  width: 100%;
  height: 200px;
  object-fit: cover;
  border-radius: 12px;
}

.form-label {
  display: block;
  margin-bottom: 1rem;
  font-weight: 600;
  color: #2c3e50;
}

.form-input,
.form-textarea,
.form-select {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #D4E7C5;
  border-radius: 8px;
  font-size: 1rem;
  margin-top: 0.5rem;
  transition: all 0.3s;
}

.form-input:focus,
.form-textarea:focus,
.form-select:focus {
  outline: none;
  border-color: #8CB369;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.tags-input {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  padding: 0.75rem;
  border: 2px solid #D4E7C5;
  border-radius: 8px;
  margin-top: 0.5rem;
  min-height: 50px;
}

.tag-item {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.3rem 0.75rem;
  background: #8CB369;
  color: white;
  border-radius: 12px;
  font-size: 0.9rem;
}

.tag-remove {
  background: none;
  border: none;
  color: white;
  font-size: 1.25rem;
  cursor: pointer;
  padding: 0;
  line-height: 1;
}

.tag-input {
  flex: 1;
  border: none;
  outline: none;
  min-width: 150px;
  font-size: 1rem;
}

.form-hint {
  display: block;
  margin-top: 0.5rem;
  font-size: 0.85rem;
  color: #888;
  font-weight: 400;
}

.ingredient-row,
.instruction-row {
  display: flex;
  gap: 0.75rem;
  margin-bottom: 0.75rem;
  align-items: flex-start;
}

.form-input.small,
.form-select.small {
  width: 100px;
  margin-top: 0;
}

.form-input.flex-1 {
  flex: 1;
  margin-top: 0;
}

.instruction-row .form-textarea {
  flex: 1;
  margin-top: 0;
}

.step-number {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: #8CB369;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  flex-shrink: 0;
  margin-top: 0.5rem;
}

.btn-remove {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border: none;
  background: #f0f0f0;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  transition: all 0.2s;
  margin-top: 0.5rem;
}

.btn-remove:hover {
  background: #ff6b6b;
  color: white;
}

.btn-remove svg {
  width: 18px;
  height: 18px;
}

.btn-add {
  width: 100%;
  padding: 0.75rem;
  border: 2px dashed #D4E7C5;
  border-radius: 8px;
  background: transparent;
  color: #8CB369;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  transition: all 0.3s;
}

.btn-add:hover {
  border-color: #8CB369;
  background: #f9fdf7;
}

.btn-add svg {
  width: 20px;
  height: 20px;
}
</style>