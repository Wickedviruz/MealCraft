-- Users
CREATE TABLE Users (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Username VARCHAR(50) UNIQUE NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT NOW(),
    UpdatedAt TIMESTAMP DEFAULT NOW()
);

-- Recipes
CREATE TABLE Recipes (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    UserId UUID REFERENCES Users(Id),
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    ImageUrl VARCHAR(500),
    CookTime INT NOT NULL,
    Servings INT NOT NULL,
    Difficulty VARCHAR(20),
    Calories INT,
    Protein DECIMAL(5,1),
    Carbs DECIMAL(5,1),
    Fat DECIMAL(5,1),
    Rating DECIMAL(2,1) DEFAULT 0,
    ReviewCount INT DEFAULT 0,
    CreatedAt TIMESTAMP DEFAULT NOW(),
    UpdatedAt TIMESTAMP DEFAULT NOW()
);

-- Recipe Tags
CREATE TABLE RecipeTags (
    RecipeId UUID REFERENCES Recipes(Id) ON DELETE CASCADE,
    Tag VARCHAR(50) NOT NULL,
    PRIMARY KEY (RecipeId, Tag)
);

-- Ingredients
CREATE TABLE Ingredients (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name VARCHAR(100) UNIQUE NOT NULL,
    Category VARCHAR(50)
);

-- Recipe Ingredients
CREATE TABLE RecipeIngredients (
    RecipeId UUID REFERENCES Recipes(Id) ON DELETE CASCADE,
    IngredientId UUID REFERENCES Ingredients(Id),
    Amount DECIMAL(10,2) NOT NULL,
    Unit VARCHAR(20) NOT NULL,
    PRIMARY KEY (RecipeId, IngredientId)
);

-- Instructions
CREATE TABLE Instructions (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    RecipeId UUID REFERENCES Recipes(Id) ON DELETE CASCADE,
    StepNumber INT NOT NULL,
    Text TEXT NOT NULL,
    Timer VARCHAR(20)
);

-- Reviews
CREATE TABLE Reviews (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    RecipeId UUID REFERENCES Recipes(Id) ON DELETE CASCADE,
    UserId UUID REFERENCES Users(Id),
    Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    Comment TEXT,
    CreatedAt TIMESTAMP DEFAULT NOW()
);

-- Saved Recipes
CREATE TABLE SavedRecipes (
    UserId UUID REFERENCES Users(Id),
    RecipeId UUID REFERENCES Recipes(Id),
    SavedAt TIMESTAMP DEFAULT NOW(),
    PRIMARY KEY (UserId, RecipeId)
);

-- Meal Plans
CREATE TABLE MealPlans (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    UserId UUID REFERENCES Users(Id),
    Date DATE NOT NULL,
    MealType VARCHAR(20) NOT NULL,
    RecipeId UUID REFERENCES Recipes(Id),
    CreatedAt TIMESTAMP DEFAULT NOW(),
    UNIQUE(UserId, Date, MealType)
);

-- Shopping Lists
CREATE TABLE ShoppingLists (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    UserId UUID REFERENCES Users(Id),
    IngredientId UUID REFERENCES Ingredients(Id),
    Amount DECIMAL(10,2) NOT NULL,
    Unit VARCHAR(20) NOT NULL,
    Checked BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMP DEFAULT NOW()
);

-- Indexes
CREATE INDEX idx_recipes_user ON Recipes(UserId);
CREATE INDEX idx_recipes_rating ON Recipes(Rating DESC);
CREATE INDEX idx_mealplans_user_date ON MealPlans(UserId, Date);
CREATE INDEX idx_reviews_recipe ON Reviews(RecipeId);