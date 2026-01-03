<template>
  <header class="app-header">
    <h1 class="app-logo">MealCraft</h1>
  </header>
</template>

<style scoped>
.app-header {
  background: var(--surface);
  padding: var(--space-md) var(--space-xl);
  border-bottom: 1px solid var(--border);
  text-align: center;
  position: sticky;
  top: 0;
  z-index: var(--z-header);
  box-shadow: var(--shadow-sm);
}

.app-logo {
  font-size: 1.75rem;
  font-weight: 700;
  background: linear-gradient(135deg, var(--primary), var(--primary-dark));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}
</style>