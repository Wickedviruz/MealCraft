<template>
  <nav class="navbar">
    <div class="navbar-content">
      <div class="logo">MealCraft</div>
      <ul class="nav-links">
        <li><a href="#recipes">Recept</a></li>
        <li><a href="#planner">Planering</a></li>
        <li><a href="#about">Om</a></li>
      </ul>
    </div>
  </nav>
</template>

<script>

</script>


<style scoped>
.navbar {
  background-color: #2c3e50;
  padding: 1rem 2rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.navbar-content {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.logo {
  color: white;
  font-size: 1.5rem;
  font-weight: bold;
}

.nav-links {
  justify-content: center;
  display: flex;
  gap: 2rem;
  list-style: none;
  margin: 0;
  padding: 0;
}

.nav-links a {
  color: white;
  text-decoration: none;
  transition: color 0.3s;
}

.nav-links a:hover {
  color: #3498db;
}
</style>