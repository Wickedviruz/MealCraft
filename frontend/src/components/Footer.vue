<template>
  <footer class="footer">
    <div class="footer-content">
      <p>&copy; {{ currentYear }} MealCraft. All rights reserved.</p>
      <div class="footer-links">
        <a href="https://github.com/Wickedviruz/MealCraft" target="_blank">GitHub</a>
        <span>|</span>
        <a href="https://www.johanivarsson.se" target="_blank">Contact</a>
      </div>
    </div>
  </footer>
</template>

<script setup>
import { computed } from 'vue'

const currentYear = computed(() => new Date().getFullYear())
</script>

<style scoped>
.footer {
  background-color: #2c3e50;
  color: #ecf0f1;
  padding: 2rem 1rem;
  margin-top: auto;
  text-align: center;
}

.footer-content {
  max-width: 1200px;
  margin: 0 auto;
}

.footer-links {
  margin-top: 0.5rem;
  display: flex;
  gap: 1rem;
  justify-content: center;
  align-items: center;
}

.footer-links a {
  color: #3498db;
  text-decoration: none;
  transition: color 0.3s;
}

.footer-links a:hover {
  color: #2980b9;
}

.footer-links span {
  color: #7f8c8d;
}
</style>