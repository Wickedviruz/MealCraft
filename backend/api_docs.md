# MealSwipe API Documentation

This document outlines all the API endpoints required for the MealSwipe application backend.

## Base URL
```
https://api.mealswipe.app/v1
```

## Authentication

All endpoints (except auth endpoints) require a valid JWT token in the Authorization header:
```
Authorization: Bearer <token>
```

---

## 1. Authentication Endpoints

### POST /auth/signup
Create a new user account.

**Request Body:**
```json
{
  "email": "string (required, valid email)",
  "password": "string (required, min 6 characters)",
  "name": "string (required)"
}
```

**Response (201):**
```json
{
  "user": {
    "id": "uuid",
    "email": "string",
    "name": "string",
    "createdAt": "ISO8601 datetime"
  },
  "token": "JWT string",
  "refreshToken": "string"
}
```

**Errors:**
- `400` - Invalid input
- `409` - Email already exists

---

### POST /auth/login
Authenticate an existing user.

**Request Body:**
```json
{
  "email": "string (required)",
  "password": "string (required)"
}
```

**Response (200):**
```json
{
  "user": {
    "id": "uuid",
    "email": "string",
    "name": "string"
  },
  "token": "JWT string",
  "refreshToken": "string"
}
```

**Errors:**
- `401` - Invalid credentials

---

### POST /auth/refresh
Refresh the access token.

**Request Body:**
```json
{
  "refreshToken": "string (required)"
}
```

**Response (200):**
```json
{
  "token": "JWT string",
  "refreshToken": "string"
}
```

---

### POST /auth/logout
Invalidate the current session.

**Headers:** `Authorization: Bearer <token>`

**Response (204):** No content

---

### POST /auth/forgot-password
Request a password reset.

**Request Body:**
```json
{
  "email": "string (required)"
}
```

**Response (200):**
```json
{
  "message": "Password reset email sent"
}
```

---

### POST /auth/reset-password
Reset password with token.

**Request Body:**
```json
{
  "token": "string (required)",
  "newPassword": "string (required, min 6 characters)"
}
```

**Response (200):**
```json
{
  "message": "Password reset successful"
}
```

---

## 2. User Endpoints

### GET /users/me
Get the current user's profile.

**Response (200):**
```json
{
  "id": "uuid",
  "email": "string",
  "name": "string",
  "avatar": "string (URL, optional)",
  "createdAt": "ISO8601 datetime",
  "macroProfile": {
    "weight": "number (kg)",
    "height": "number (cm)",
    "age": "number",
    "sex": "male | female",
    "activityLevel": "number (1-5)",
    "goal": "lose | maintain | gain"
  }
}
```

---

### PATCH /users/me
Update the current user's profile.

**Request Body:**
```json
{
  "name": "string (optional)",
  "avatar": "string (optional, URL or base64)"
}
```

**Response (200):** Updated user object

---

### PUT /users/me/macro-profile
Update the user's macro profile.

**Request Body:**
```json
{
  "weight": "number (required, kg)",
  "height": "number (required, cm)",
  "age": "number (required)",
  "sex": "male | female (required)",
  "activityLevel": "number (required, 1-5)",
  "goal": "lose | maintain | gain (required)"
}
```

**Response (200):**
```json
{
  "macroProfile": { ... },
  "calculatedGoals": {
    "calories": "number",
    "protein": "number (grams)",
    "carbs": "number (grams)",
    "fat": "number (grams)",
    "water": "number (ml)"
  }
}
```

---

## 3. Recipe Endpoints

### GET /recipes
Get all recipes with optional filtering.

**Query Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `search` | string | Search in title and ingredients |
| `tags` | string[] | Filter by tags (comma-separated) |
| `maxCookTime` | number | Max cook time in minutes |
| `minCalories` | number | Minimum calories |
| `maxCalories` | number | Maximum calories |
| `minServings` | number | Minimum servings |
| `maxServings` | number | Maximum servings |
| `page` | number | Page number (default: 1) |
| `limit` | number | Items per page (default: 20, max: 100) |

**Response (200):**
```json
{
  "recipes": [
    {
      "id": "uuid",
      "title": "string",
      "image": "string (URL)",
      "cookTime": "string (e.g., '30 min')",
      "servings": "number",
      "calories": "number",
      "ingredients": [
        {
          "id": "uuid",
          "name": "string",
          "amount": "number | null",
          "unit": "string"
        }
      ],
      "tags": ["string"],
      "createdBy": "uuid (user ID)",
      "isPublic": "boolean",
      "createdAt": "ISO8601 datetime"
    }
  ],
  "pagination": {
    "page": "number",
    "limit": "number",
    "total": "number",
    "totalPages": "number"
  }
}
```

---

### GET /recipes/:id
Get a single recipe by ID.

**Response (200):**
```json
{
  "id": "uuid",
  "title": "string",
  "image": "string (URL)",
  "cookTime": "string",
  "servings": "number",
  "calories": "number",
  "protein": "number (optional)",
  "carbs": "number (optional)",
  "fat": "number (optional)",
  "ingredients": [
    {
      "id": "uuid",
      "name": "string",
      "amount": "number | null",
      "unit": "string"
    }
  ],
  "instructions": ["string (optional)"],
  "tags": ["string"],
  "createdBy": "uuid",
  "isPublic": "boolean",
  "createdAt": "ISO8601 datetime"
}
```

---

### POST /recipes
Create a new recipe.

**Request Body:**
```json
{
  "title": "string (required)",
  "image": "string (optional, URL or base64)",
  "cookTime": "string (required, e.g., '30 min')",
  "servings": "number (required)",
  "calories": "number (required)",
  "protein": "number (optional)",
  "carbs": "number (optional)",
  "fat": "number (optional)",
  "ingredients": [
    {
      "id": "uuid (optional, auto-generated)",
      "name": "string (required)",
      "amount": "number | null",
      "unit": "string"
    }
  ],
  "instructions": ["string (optional)"],
  "tags": ["string (optional)"],
  "isPublic": "boolean (default: false)"
}
```

**Response (201):** Created recipe object

---

### PATCH /recipes/:id
Update a recipe (only by creator).

**Request Body:** Same as POST (all fields optional)

**Response (200):** Updated recipe object

---

### DELETE /recipes/:id
Delete a recipe (only by creator).

**Response (204):** No content

---

## 4. Saved Recipes Endpoints

### GET /users/me/saved-recipes
Get the user's saved recipes.

**Response (200):**
```json
{
  "recipes": [Recipe objects]
}
```

---

### POST /users/me/saved-recipes
Save a recipe to favorites.

**Request Body:**
```json
{
  "recipeId": "uuid (required)"
}
```

**Response (201):**
```json
{
  "message": "Recipe saved"
}
```

---

### DELETE /users/me/saved-recipes/:recipeId
Remove a recipe from favorites.

**Response (204):** No content

---

## 5. Meal Plan Endpoints

### GET /meal-plans
Get meal plans for a date range.

**Query Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `startDate` | string | Start date (YYYY-MM-DD, required) |
| `endDate` | string | End date (YYYY-MM-DD, required) |
| `familyId` | uuid | Get family meal plan (optional) |

**Response (200):**
```json
{
  "mealPlans": {
    "2024-01-15": {
      "breakfast": { Recipe object or null },
      "lunch": { Recipe object or null },
      "dinner": { Recipe object or null },
      "snack-1": { Recipe object or null }
    }
  },
  "mealSlots": [
    { "id": "string", "name": "string", "order": "number" }
  ]
}
```

---

### PUT /meal-plans/:date/:slotId
Assign a recipe to a meal slot.

**Request Body:**
```json
{
  "recipeId": "uuid (required)",
  "familyId": "uuid (optional, for family plans)"
}
```

**Response (200):**
```json
{
  "date": "string",
  "slotId": "string",
  "recipe": { Recipe object }
}
```

---

### DELETE /meal-plans/:date/:slotId
Remove a recipe from a meal slot.

**Query Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `familyId` | uuid | For family plans (optional) |

**Response (204):** No content

---

### GET /meal-plans/summary
Get weekly meal plan summary with calorie totals and goal tracking.

**Query Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `weekStart` | string (YYYY-MM-DD) | Start of the week (required) |
| `familyId` | uuid | Include family combined goals (optional) |

**Response (200):**
```json
{
  "weekStart": "2024-01-15",
  "weekEnd": "2024-01-21",
  "summary": {
    "totalCalories": "number",
    "plannedMeals": "number",
    "totalSlots": "number",
    "dailyCalorieGoal": "number | null",
    "weeklyCalorieGoal": "number | null"
  },
  "familyGoals": {
    "membersWithProfiles": "number",
    "totalMembers": "number",
    "combinedDailyGoal": "number",
    "perMemberBreakdown": [
      {
        "userId": "uuid",
        "name": "string",
        "dailyCalorieGoal": "number",
        "hasProfile": "boolean"
      }
    ]
  }
}
```

**Notes:**
- If only some family members have macro profiles, the API calculates an average for members without profiles
- `dailyCalorieGoal` is null if no macro profile is set (user or family)
- For families, goals are combined for meal planning

---

## 6. Meal Slots Configuration

### GET /users/me/meal-slots
Get the user's configured meal slots.

**Response (200):**
```json
{
  "mealSlots": [
    { "id": "string", "name": "string", "order": "number" }
  ]
}
```

---

### PUT /users/me/meal-slots
Update the user's meal slot configuration.

**Request Body:**
```json
{
  "mealSlots": [
    { "id": "string (optional for new)", "name": "string", "order": "number" }
  ]
}
```

**Response (200):** Updated meal slots array

---

## 7. Shopping List Endpoints

### GET /shopping-lists
Get shopping lists.

**Query Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `familyId` | uuid | Get family shopping list (optional) |

**Response (200):**
```json
{
  "items": [
    {
      "id": "uuid",
      "name": "string",
      "checked": "boolean",
      "category": "string",
      "addedBy": "uuid"
    }
  ]
}
```

---

### POST /shopping-lists/generate
Generate shopping list from meal plan.

**Request Body:**
```json
{
  "startDate": "string (YYYY-MM-DD, required)",
  "endDate": "string (YYYY-MM-DD, required)",
  "familyId": "uuid (optional)"
}
```

**Response (201):**
```json
{
  "items": [ShoppingItem objects],
  "message": "Shopping list generated from X recipes"
}
```

---

### POST /shopping-lists/items
Add an item to the shopping list.

**Request Body:**
```json
{
  "name": "string (required)",
  "category": "string (optional)",
  "familyId": "uuid (optional)"
}
```

**Response (201):** Created item object

---

### PATCH /shopping-lists/items/:id
Update a shopping list item (e.g., toggle checked).

**Request Body:**
```json
{
  "checked": "boolean (optional)",
  "name": "string (optional)"
}
```

**Response (200):** Updated item object

---

### DELETE /shopping-lists/items/:id
Remove an item from the shopping list.

**Response (204):** No content

---

### DELETE /shopping-lists
Clear the entire shopping list.

**Query Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `familyId` | uuid | For family list (optional) |
| `checkedOnly` | boolean | Only clear checked items |

**Response (204):** No content

---

## 8. Family Endpoints

### POST /families
Create a new family group.

**Request Body:**
```json
{
  "name": "string (required)"
}
```

**Response (201):**
```json
{
  "id": "uuid",
  "name": "string",
  "ownerId": "uuid",
  "members": [
    {
      "userId": "uuid",
      "name": "string",
      "email": "string",
      "role": "owner",
      "joinedAt": "ISO8601 datetime"
    }
  ],
  "createdAt": "ISO8601 datetime"
}
```

---

### GET /families/:id
Get family details.

**Response (200):** Family object with members

---

### PATCH /families/:id
Update family settings (owner only).

**Request Body:**
```json
{
  "name": "string (optional)"
}
```

**Response (200):** Updated family object

---

### DELETE /families/:id
Delete a family (owner only).

**Response (204):** No content

---

### POST /families/:id/invites
Send an invitation to join the family.

**Request Body:**
```json
{
  "email": "string (required)"
}
```

**Response (201):**
```json
{
  "inviteId": "uuid",
  "email": "string",
  "expiresAt": "ISO8601 datetime",
  "message": "Invitation sent"
}
```

---

### GET /users/me/invites
Get pending family invitations for the current user.

**Response (200):**
```json
{
  "invites": [
    {
      "id": "uuid",
      "familyId": "uuid",
      "familyName": "string",
      "invitedBy": "string (name)",
      "createdAt": "ISO8601 datetime",
      "expiresAt": "ISO8601 datetime"
    }
  ]
}
```

---

### POST /invites/:id/accept
Accept a family invitation.

**Response (200):**
```json
{
  "family": { Family object },
  "message": "You have joined the family"
}
```

---

### POST /invites/:id/decline
Decline a family invitation.

**Response (204):** No content

---

### DELETE /families/:id/members/:userId
Remove a member from the family (owner/admin only).

**Response (204):** No content

---

### POST /families/:id/leave
Leave a family (non-owners only).

**Response (204):** No content

---

### PUT /families/:id/members/:userId/macro-profile
Update a family member's macro profile.

**Request Body:**
```json
{
  "weight": "number (kg)",
  "height": "number (cm)",
  "age": "number",
  "sex": "male | female",
  "activityLevel": "number (1-5)",
  "goal": "lose | maintain | gain"
}
```

**Response (200):**
```json
{
  "member": { Member object with macroProfile },
  "calculatedGoals": { Macro goals object }
}
```

---

## 9. Macro Calculator Endpoints

### POST /macros/calculate
Calculate macros without saving.

**Request Body:**
```json
{
  "weight": "number (kg, required)",
  "height": "number (cm, required)",
  "age": "number (required)",
  "sex": "male | female (required)",
  "activityLevel": "number (1-5, required)",
  "goal": "lose | maintain | gain (optional)"
}
```

**Response (200):**
```json
{
  "bmr": "number",
  "tdee": "number",
  "goals": {
    "lose": {
      "calories": "number",
      "protein": "number",
      "carbs": "number",
      "fat": "number"
    },
    "maintain": { ... },
    "gain": { ... }
  },
  "waterIntake": "number (ml)",
  "byWorkoutDays": {
    "0": { "calories": "number", "water": "number" },
    "1-2": { ... },
    "3-4": { ... },
    "5-6": { ... }
  }
}
```

---

### GET /families/:id/macros/summary
Get combined macro summary for family meal planning.

**Query Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `date` | string | Date for calculation (YYYY-MM-DD) |

**Response (200):**
```json
{
  "totalCalories": "number",
  "members": [
    {
      "userId": "uuid",
      "name": "string",
      "goals": { Macro goals },
      "consumed": { From meal plan }
    }
  ]
}
```

---

## Recipe Ratings Endpoints

### Rate a Recipe

`POST /recipes/{recipeId}/ratings`

**Request Body:**
```json
{
  "rating": "number (1-5)"
}
```

**Response (201):**
```json
{
  "id": "uuid",
  "recipeId": "uuid",
  "userId": "uuid | null",
  "rating": "number",
  "createdAt": "timestamp"
}
```

### Get Recipe Ratings

`GET /recipes/{recipeId}/ratings`

**Response (200):**
```json
{
  "averageRating": "number",
  "totalRatings": "number",
  "distribution": {
    "1": "number",
    "2": "number",
    "3": "number",
    "4": "number",
    "5": "number"
  }
}
```

---

## Recipe Comments Endpoints

### Get Recipe Comments

`GET /recipes/{recipeId}/comments`

**Query Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `page` | number | Page number (default: 1) |
| `limit` | number | Items per page (default: 20) |

**Response (200):**
```json
{
  "comments": [
    {
      "id": "uuid",
      "recipeId": "uuid",
      "userId": "uuid | null",
      "userName": "string",
      "userAvatar": "string | null",
      "content": "string",
      "createdAt": "timestamp"
    }
  ],
  "pagination": {
    "page": "number",
    "limit": "number",
    "total": "number",
    "totalPages": "number"
  }
}
```

### Add Comment to Recipe

`POST /recipes/{recipeId}/comments`

**Request Body:**
```json
{
  "content": "string (max 1000 chars)",
  "userName": "string (optional, for anonymous)",
  "userAvatar": "string (optional)"
}
```

**Response (201):**
```json
{
  "id": "uuid",
  "recipeId": "uuid",
  "userId": "uuid | null",
  "userName": "string",
  "userAvatar": "string | null",
  "content": "string",
  "createdAt": "timestamp"
}
```

### Delete Comment

`DELETE /recipes/{recipeId}/comments/{commentId}`

Requires authentication if the comment was made by a logged-in user.

**Response (204):** No content

---

## User Settings Endpoints

### Get Notification Preferences

`GET /users/me/notifications`

**Response (200):**
```json
{
  "mealReminders": "boolean",
  "reminderTime": "number (minutes before)",
  "shoppingListAlerts": "boolean",
  "familyActivity": "boolean"
}
```

### Update Notification Preferences

`PUT /users/me/notifications`

**Request Body:**
```json
{
  "mealReminders": "boolean (optional)",
  "reminderTime": "number (optional)",
  "shoppingListAlerts": "boolean (optional)",
  "familyActivity": "boolean (optional)"
}
```

**Response (200):** Updated preferences object

### Get Privacy Settings

`GET /users/me/privacy`

**Response (200):**
```json
{
  "profileVisible": "boolean",
  "shareRecipes": "boolean",
  "dataCollection": "boolean"
}
```

### Update Privacy Settings

`PUT /users/me/privacy`

**Request Body:**
```json
{
  "profileVisible": "boolean (optional)",
  "shareRecipes": "boolean (optional)",
  "dataCollection": "boolean (optional)"
}
```

**Response (200):** Updated privacy settings object

---

## Error Response Format

All errors follow this format:

```json
{
  "error": {
    "code": "string (e.g., VALIDATION_ERROR)",
    "message": "string (human-readable)",
    "details": { "field-specific errors (optional)" }
  }
}
```

**Common Error Codes:**
- `VALIDATION_ERROR` (400)
- `UNAUTHORIZED` (401)
- `FORBIDDEN` (403)
- `NOT_FOUND` (404)
- `CONFLICT` (409)
- `RATE_LIMITED` (429)
- `INTERNAL_ERROR` (500)

---

## Rate Limiting

- Standard endpoints: 100 requests per minute
- Auth endpoints: 10 requests per minute
- Calculation endpoints: 30 requests per minute

Rate limit headers are included in responses:
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1704067200
```

---

## Webhooks (Optional)

For real-time sync, implement webhooks:

### Family Events
- `family.member_joined`
- `family.member_left`
- `family.settings_updated`

### Meal Plan Events
- `meal_plan.updated`
- `meal_plan.recipe_assigned`
- `meal_plan.recipe_removed`

### Shopping List Events
- `shopping_list.item_added`
- `shopping_list.item_checked`
- `shopping_list.cleared`

---

## Database Schema Suggestion

```sql
-- Users
CREATE TABLE users (
  id UUID PRIMARY KEY,
  email VARCHAR(255) UNIQUE NOT NULL,
  password_hash VARCHAR(255) NOT NULL,
  name VARCHAR(255) NOT NULL,
  avatar_url TEXT,
  created_at TIMESTAMP DEFAULT NOW()
);

-- Macro Profiles
CREATE TABLE macro_profiles (
  user_id UUID REFERENCES users(id) PRIMARY KEY,
  weight DECIMAL(5,2),
  height DECIMAL(5,2),
  age INTEGER,
  sex VARCHAR(10),
  activity_level INTEGER,
  goal VARCHAR(20),
  updated_at TIMESTAMP DEFAULT NOW()
);

-- Recipes
CREATE TABLE recipes (
  id UUID PRIMARY KEY,
  title VARCHAR(255) NOT NULL,
  image_url TEXT,
  cook_time VARCHAR(50),
  servings INTEGER,
  calories INTEGER,
  protein INTEGER,
  carbs INTEGER,
  fat INTEGER,
  ingredients JSONB,
  instructions JSONB,
  tags TEXT[],
  created_by UUID REFERENCES users(id),
  is_public BOOLEAN DEFAULT false,
  created_at TIMESTAMP DEFAULT NOW()
);

-- Saved Recipes
CREATE TABLE saved_recipes (
  user_id UUID REFERENCES users(id),
  recipe_id UUID REFERENCES recipes(id),
  saved_at TIMESTAMP DEFAULT NOW(),
  PRIMARY KEY (user_id, recipe_id)
);

-- Meal Slots
CREATE TABLE meal_slots (
  id UUID PRIMARY KEY,
  user_id UUID REFERENCES users(id),
  name VARCHAR(50) NOT NULL,
  "order" INTEGER NOT NULL
);

-- Meal Plans
CREATE TABLE meal_plans (
  id UUID PRIMARY KEY,
  user_id UUID REFERENCES users(id),
  family_id UUID REFERENCES families(id),
  date DATE NOT NULL,
  slot_id VARCHAR(50) NOT NULL,
  recipe_id UUID REFERENCES recipes(id),
  UNIQUE (user_id, date, slot_id),
  UNIQUE (family_id, date, slot_id)
);

-- Shopping Items
CREATE TABLE shopping_items (
  id UUID PRIMARY KEY,
  user_id UUID REFERENCES users(id),
  family_id UUID REFERENCES families(id),
  name VARCHAR(255) NOT NULL,
  amount DECIMAL(10,2),
  unit VARCHAR(20),
  category VARCHAR(50),
  checked BOOLEAN DEFAULT false,
  added_by UUID REFERENCES users(id),
  created_at TIMESTAMP DEFAULT NOW()
);

-- Pantry Items
CREATE TABLE pantry_items (
  id UUID PRIMARY KEY,
  user_id UUID REFERENCES users(id),
  family_id UUID REFERENCES families(id),
  name VARCHAR(255) NOT NULL,
  amount DECIMAL(10,2),
  unit VARCHAR(20),
  added_at TIMESTAMP DEFAULT NOW()
);

-- Families
CREATE TABLE families (
  id UUID PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  owner_id UUID REFERENCES users(id) NOT NULL,
  created_at TIMESTAMP DEFAULT NOW()
);

-- Family Members
CREATE TABLE family_members (
  family_id UUID REFERENCES families(id) ON DELETE CASCADE,
  user_id UUID REFERENCES users(id) ON DELETE CASCADE,
  role VARCHAR(20) DEFAULT 'member',
  joined_at TIMESTAMP DEFAULT NOW(),
  PRIMARY KEY (family_id, user_id)
);

-- Family Invites
CREATE TABLE family_invites (
  id UUID PRIMARY KEY,
  family_id UUID REFERENCES families(id) ON DELETE CASCADE,
  email VARCHAR(255) NOT NULL,
  invited_by UUID REFERENCES users(id),
  expires_at TIMESTAMP NOT NULL,
  created_at TIMESTAMP DEFAULT NOW()
);
```

---

## 16. Pantry Endpoints

### GET /pantry
Get all pantry items for the current user or family.

**Query Parameters:**
- `familyId` (optional) - Filter by family

**Response (200):**
```json
{
  "items": [
    {
      "id": "uuid",
      "name": "string",
      "amount": "number | null",
      "unit": "string",
      "addedAt": "ISO8601 datetime"
    }
  ]
}
```

---

### POST /pantry
Add an item to the pantry.

**Request Body:**
```json
{
  "name": "string (required)",
  "amount": "number | null (optional - null means unlimited)",
  "unit": "string (optional)",
  "familyId": "uuid (optional)"
}
```

**Response (201):**
```json
{
  "id": "uuid",
  "name": "string",
  "amount": "number | null",
  "unit": "string",
  "addedAt": "ISO8601 datetime"
}
```

---

### PATCH /pantry/:id
Update a pantry item.

**Request Body:**
```json
{
  "amount": "number | null (optional)",
  "unit": "string (optional)"
}
```

**Response (200):**
```json
{
  "id": "uuid",
  "name": "string",
  "amount": "number | null",
  "unit": "string",
  "addedAt": "ISO8601 datetime"
}
```

---

### DELETE /pantry/:id
Remove an item from the pantry.

**Response (204):** No content

---

### POST /pantry/use
Deduct ingredients from pantry after completing a day's meals.

**Request Body:**
```json
{
  "date": "YYYY-MM-DD (required)",
  "ingredients": [
    {
      "name": "string",
      "amount": "number | null",
      "unit": "string"
    }
  ]
}
```

**Response (200):**
```json
{
  "message": "Ingredients deducted successfully",
  "removed": ["ingredient names that were fully used"],
  "updated": ["ingredient names with updated amounts"]
}
```
