const getApiBase = () => {
  const hostname = window.location.hostname;
  
  // Om vi kör lokalt via localhost, använd localhost för API
  if (hostname === 'localhost' || hostname === '127.0.0.1') {
    return 'http://localhost:5000/api';
  }
  
  // Annars, använd samma IP som vi nådde frontend på
  return `http://${hostname}:5000/api`;
};

const API_BASE = getApiBase();

console.log('API Base URL:', API_BASE);

export const recipeApi = {
  // Get all recipes
  async getAll(limit = 50, offset = 0) {
    const response = await fetch(`${API_BASE}/recipes?limit=${limit}&offset=${offset}`);
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }
    return response.json();
  },

  // Get single recipe
  async getById(id) {
    const response = await fetch(`${API_BASE}/recipes/${id}`);
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: Recipe not found`);
    }
    return response.json();
  },

  // Create recipe
  async create(recipe) {
    const response = await fetch(`${API_BASE}/recipes`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(recipe)
    });
    if (!response.ok) {
      const error = await response.text();
      throw new Error(`Failed to create recipe: ${error}`);
    }
    return response.json();
  },

  // Update recipe
  async update(id, recipe) {
    const response = await fetch(`${API_BASE}/recipes/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(recipe)
    });
    if (!response.ok) {
      throw new Error('Failed to update recipe');
    }
    return response.json();
  },

  // Delete recipe
  async delete(id) {
    const response = await fetch(`${API_BASE}/recipes/${id}`, {
      method: 'DELETE'
    });
    if (!response.ok) {
      throw new Error('Failed to delete recipe');
    }
  },

  // Search recipes
  async search(query, limit = 50) {
    const response = await fetch(`${API_BASE}/recipes/search?query=${encodeURIComponent(query)}&limit=${limit}`);
    if (!response.ok) {
      throw new Error('Search failed');
    }
    return response.json();
  }
};

export const mealPlanApi = {
  // Get meal plans for date range
  async getPlans(startDate, endDate) {
    const response = await fetch(
      `${API_BASE}/mealplans?startDate=${startDate}&endDate=${endDate}`
    );
    if (!response.ok) {
      throw new Error('Failed to fetch meal plans');
    }
    return response.json();
  },

  // Add meal to calendar
  async add(date, mealType, recipeId) {
    const response = await fetch(`${API_BASE}/mealplans`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ date, mealType, recipeId })
    });
    if (!response.ok) {
      throw new Error('Failed to add meal plan');
    }
    return response.json();
  },

  // Remove meal from calendar
  async remove(date, mealType) {
    const response = await fetch(
      `${API_BASE}/mealplans?date=${date}&mealType=${mealType}`,
      { method: 'DELETE' }
    );
    if (!response.ok) {
      throw new Error('Failed to remove meal plan');
    }
  }
};

export const userApi = {
  // Register new user
  async register(username, email, password) {
    const response = await fetch(`${API_BASE}/users/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, email, password })
    });
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Registration failed');
    }
    return response.json();
  },

  // Login
  async login(email, password) {
    const response = await fetch(`${API_BASE}/users/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    });
    if (!response.ok) {
      throw new Error('Invalid credentials');
    }
    return response.json();
  }
};