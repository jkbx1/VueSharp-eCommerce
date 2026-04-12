<script setup>
import Navbar from './components/Navbar.vue';
import ScrollToTop from './components/ScrollToTop.vue';
import Toast from './components/Toast.vue';
import QuickAddModal from './components/QuickAddModal.vue';
import Footer from './components/Footer.vue';
</script>

<template>
  <div class="app-container">
    <Navbar />
    
    <main class="main-content">
      <router-view v-slot="{ Component }">
        <transition name="page-fade" mode="out-in">
          <component :is="Component" :key="$route.fullPath" />
        </transition>
      </router-view>
    </main>
    
    <Footer />

    <!-- Global Components -->
    <ScrollToTop />
    <Toast />
    <QuickAddModal />
  </div>
</template>

<style>

@import url('https://fonts.googleapis.com/css2?family=Playfair+Display:wght@500;600;700&display=swap');

:root {
  --bg-color: #121212;
  --text-main: #e0e0e0;
}

html {
  background-color: var(--bg-color);
}

body {
  margin: 0;
  background-color: var(--bg-color);
  color: var(--text-main);
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

.app-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-content {
  flex: 1;
}

/* Simple page transition */
.page-fade-enter-active,
.page-fade-leave-active {
  transition: opacity 0.3s ease, transform 0.3s ease;
}
.page-fade-enter-from {
  opacity: 0;
  transform: translateY(20px);
}
.page-fade-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}
</style>
