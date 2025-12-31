<template>
  <div class="recipe-detail">
    <button class="btn-back" @click="emit('close')">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <polyline points="15 18 9 12 15 6"/>
      </svg>
      Back
    </button>
    
    <div class="detail-hero">
      <img :src="recipe.image" :alt="recipe.title" class="hero-image" />
      <div class="hero-overlay">
        <h1 class="recipe-title">{{ recipe.title }}</h1>
        <div class="recipe-rating">
          <div class="stars">
            <span v-for="i in 5" :key="i" class="star">
              {{ i <= (recipe.rating || 4) ? '★' : '☆' }}
            </span>
          </div>
          <span class="rating-text">{{ recipe.rating || 4.8 }} • {{ recipe.reviews || 197 }} reviews</span>
        </div>
      </div>
    </div>
    
    <div class="detail-content">
      <div class="quick-info">
        <div class="info-badge">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="12" cy="12" r="10"/>
            <polyline points="12 6 12 12 16 14"/>
          </svg>
          <div>
            <div class="badge-label">Över {{ recipe.cookTime }} min</div>
            <div class="badge-value">Tid</div>
          </div>
        </div>
        
        <div class="info-badge">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
            <circle cx="9" cy="7" r="4"/>
          </svg>
          <div>
            <div class="badge-label">{{ recipe.servings }} ingredienser</div>
            <div class="badge-value">Portioner</div>
          </div>
        </div>
        
        <div class="info-badge">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M12 2L2 7l10 5 10-5-10-5z"/>
          </svg>
          <div>
            <div class="badge-label">{{ recipe.difficulty || 'Medel' }}</div>
            <div class="badge-value">Svårighet</div>
          </div>
        </div>
      </div>
      
      <div class="description">
        <p>{{ recipe.description || 'Klassiskt grundrecept med fantastisk smak.' }}</p>
      </div>
      
      <section class="ingredients-section">
        <div class="section-header">
          <h2>Ingredienser</h2>
          <div class="portion-selector">
            <button @click="decreasePortions" class="portion-btn">−</button>
            <span class="portion-count">{{ portions }} portioner</span>
            <button @click="increasePortions" class="portion-btn">+</button>
          </div>
        </div>
        
        <ul class="ingredients-list">
          <li v-for="(ingredient, index) in scaledIngredients" :key="index" class="ingredient-item">
            <span class="ingredient-amount">{{ ingredient.amount }} {{ ingredient.unit }}</span>
            <span class="ingredient-name">{{ ingredient.name }}</span>
          </li>
        </ul>
        
        <div class="servering-note" v-if="recipe.serving">
          <strong>Till servering:</strong> {{ recipe.serving }}
        </div>
      </section>
      
      <section class="instructions-section">
        <h2>Gör så här</h2>
        
        <div v-for="(step, index) in instructions" :key="index" class="instruction-step">
          <div class="step-checkbox">
            <input 
              type="checkbox" 
              :id="`step-${index}`"
              v-model="completedSteps[index]"
            />
            <label :for="`step-${index}`"></label>
          </div>
          
          <div class="step-content">
            <p :class="{ completed: completedSteps[index] }">{{ step.text }}</p>
            <div v-if="step.timer" class="step-timer">
              <button @click="startTimer(step.timer)" class="timer-btn">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <circle cx="12" cy="12" r="10"/>
                  <polyline points="12 6 12 12 16 14"/>
                </svg>
                {{ step.timer }}
              </button>
            </div>
          </div>
        </div>
      </section>
      
      <div class="action-buttons">
        <button @click="addToCalendar" class="btn-primary">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
            <line x1="16" y1="2" x2="16" y2="6"/>
            <line x1="8" y1="2" x2="8" y2="6"/>
            <line x1="3" y1="10" x2="21" y2="10"/>
          </svg>
          Lägg till i kalender
        </button>
        
        <button @click="toggleSave" :class="['btn-secondary', { saved: isSaved }]">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
          </svg>
          {{ isSaved ? 'Sparat' : 'Spara' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  recipe: {
    type: Object,
    required: true
  },
  isSaved: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['close', 'save', 'add-to-calendar'])

const portions = ref(props.recipe.servings || 4)
const completedSteps = ref([])

const instructions = computed(() => {
  return props.recipe.instructions || [
    { text: 'Lägg gelatinbladen i kallt vatten. Här hittar du en fin instruktionsfilm för hur du lagar din pannacotta.', timer: null },
    { text: 'Häll grädden i en kastrull. Dela vaniljstången på längden och skrapa ur fröna. Lägg fröna och stången i grädden tillsammans med sockret.', timer: null },
    { text: 'Koka upp grädden och koka den sakta i ca 2 minuter. Vispa så att vaniljfröna blandar sig med grädden. Ta upp vaniljstången.', timer: '00:02:00' },
    { text: 'Ta upp gelatinbladen och lägg ner dem i den varma grädden. Rör försiktigt så att gelatinbladen löser sig.', timer: null },
    { text: 'Fördela grädden i portionsglas eller formar.', timer: null },
    { text: 'Sätt formarna i kylen, minst 5 timmar.', timer: '05:00:00' },
    { text: 'Till servering: Dekorera med färska bär och servera.', timer: null }
  ]
})

const scaledIngredients = computed(() => {
  const scale = portions.value / (props.recipe.servings || 4)
  return (props.recipe.ingredients || []).map(ing => ({
    ...ing,
    amount: Math.round(ing.amount * scale * 10) / 10
  }))
})

function increasePortions() {
  portions.value++
}

function decreasePortions() {
  if (portions.value > 1) portions.value--
}

function startTimer(time) {
  alert(`Timer started: ${time}`)
  // TODO: Implementera riktig timer
}

function addToCalendar() {
  emit('add-to-calendar', props.recipe)
}

function toggleSave() {
  emit('save', props.recipe)
}
</script>

<style scoped>
.recipe-detail {
  max-width: 900px;
  margin: 0 auto;
  background: white;
  min-height: 100vh;
}

.btn-back {
  position: sticky;
  top: 70px;
  left: 1rem;
  z-index: 100;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: white;
  border: none;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
  margin: 1rem;
}

.btn-back:hover {
  transform: translateX(-4px);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.15);
}

.btn-back svg {
  width: 20px;
  height: 20px;
}

.detail-hero {
  position: relative;
  height: 400px;
  overflow: hidden;
}

.hero-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.hero-overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 2rem;
  background: linear-gradient(to top, rgba(0,0,0,0.8), transparent);
  color: white;
}

.recipe-title {
  font-size: 2.5rem;
  font-weight: 700;
  margin: 0 0 1rem 0;
}

.recipe-rating {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.stars {
  color: #FFD700;
  font-size: 1.25rem;
}

.rating-text {
  font-size: 0.95rem;
  opacity: 0.9;
}

.detail-content {
  padding: 2rem;
}

.quick-info {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.info-badge {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: #f9fdf7;
  border-radius: 12px;
  border: 1px solid #D4E7C5;
}

.info-badge svg {
  width: 32px;
  height: 32px;
  color: #8CB369;
  flex-shrink: 0;
}

.badge-label {
  font-weight: 600;
  color: #2c3e50;
}

.badge-value {
  font-size: 0.85rem;
  color: #888;
}

.description {
  font-size: 1.1rem;
  line-height: 1.6;
  color: #555;
  margin-bottom: 2rem;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.section-header h2 {
  font-size: 1.75rem;
  color: #2c3e50;
  margin: 0;
}

.portion-selector {
  display: flex;
  align-items: center;
  gap: 1rem;
  background: white;
  padding: 0.5rem;
  border-radius: 12px;
  border: 1px solid #D4E7C5;
}

.portion-btn {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  border: none;
  background: #8CB369;
  color: white;
  font-size: 1.25rem;
  cursor: pointer;
  transition: all 0.2s;
}

.portion-btn:hover {
  background: #7aa359;
}

.portion-count {
  font-weight: 600;
  min-width: 100px;
  text-align: center;
}

.ingredients-list {
  list-style: none;
  padding: 0;
  margin: 0 0 2rem 0;
}

.ingredient-item {
  display: flex;
  padding: 1rem;
  border-bottom: 1px solid #f0f0f0;
  transition: background 0.2s;
}

.ingredient-item:hover {
  background: #f9fdf7;
}

.ingredient-amount {
  min-width: 120px;
  font-weight: 600;
  color: #8CB369;
}

.ingredient-name {
  flex: 1;
  color: #2c3e50;
}

.servering-note {
  padding: 1rem;
  background: #fff9e6;
  border-left: 4px solid #FFB84D;
  border-radius: 8px;
  margin-top: 1rem;
}

.instructions-section {
  margin: 3rem 0;
}

.instructions-section h2 {
  font-size: 1.75rem;
  color: #2c3e50;
  margin-bottom: 1.5rem;
}

.instruction-step {
  display: flex;
  gap: 1rem;
  padding: 1.5rem 0;
  border-bottom: 1px solid #f0f0f0;
}

.step-checkbox input {
  width: 24px;
  height: 24px;
  cursor: pointer;
  accent-color: #8CB369;
}

.step-content {
  flex: 1;
}

.step-content p {
  margin: 0;
  line-height: 1.6;
  color: #2c3e50;
}

.step-content p.completed {
  text-decoration: line-through;
  opacity: 0.6;
}

.step-timer {
  margin-top: 0.75rem;
}

.timer-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: #FFB84D;
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.timer-btn:hover {
  background: #ff9f1a;
}

.timer-btn svg {
  width: 16px;
  height: 16px;
}

.action-buttons {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  margin-top: 3rem;
  padding-top: 2rem;
  border-top: 2px solid #f0f0f0;
}

.btn-primary,
.btn-secondary {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  padding: 1rem 2rem;
  border: none;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-primary {
  background: #8CB369;
  color: white;
}

.btn-primary:hover {
  background: #7aa359;
  transform: translateY(-2px);
}

.btn-secondary {
  background: white;
  color: #8CB369;
  border: 2px solid #8CB369;
}

.btn-secondary:hover {
  background: #f9fdf7;
}

.btn-secondary.saved {
  background: #8CB369;
  color: white;
}

.btn-primary svg,
.btn-secondary svg {
  width: 20px;
  height: 20px;
}

@media (max-width: 768px) {
  .recipe-title {
    font-size: 2rem;
  }
  
  .action-buttons {
    grid-template-columns: 1fr;
  }
}
</style>