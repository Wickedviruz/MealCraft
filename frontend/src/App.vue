<template>
  <div id="app">
    <Navbar />
    <main class="main-content">
      <h1>MealCraft</h1>
      <p>Smart meal planning coming soon...</p>
    </main>
    <Footer />
  </div>
</template>

<script setup>
import Footer from './components/Footer.vue'
import Navbar from './components/Navbar.vue';
</script>

<style scoped>
#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding: 2rem;
}

h1 {
  font-size: 3rem;
  color: #2c3e50;
  margin-bottom: 1rem;
}

p {
  font-size: 1.25rem;
  color: #7f8c8d;
}
</style>