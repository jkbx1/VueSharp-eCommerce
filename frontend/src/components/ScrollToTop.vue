<script setup>
import { ref, onMounted, onUnmounted } from "vue";

const isVisible = ref(false);

const handleScroll = () => {
  
  isVisible.value = window.scrollY > 300;
};

const scrollToTop = () => {
  window.scrollTo({
    top: 0,
    behavior: "smooth",
  });
};

onMounted(() => {
  window.addEventListener("scroll", handleScroll);
});

onUnmounted(() => {
  window.removeEventListener("scroll", handleScroll);
});
</script>

<template>
  <transition name="fade-slide">
    <button
      v-if="isVisible"
      class="scroll-to-top-btn"
      @click="scrollToTop"
      aria-label="Scroll to top"
    >
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="18"
        height="18"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="2.5"
        stroke-linecap="round"
        stroke-linejoin="round"
        class="arrow-icon"
      >
        <line x1="12" y1="19" x2="12" y2="5"></line>
        <polyline points="5 12 12 5 19 12"></polyline>
      </svg>
      <span>To top</span>
    </button>
  </transition>
</template>

<style scoped>
.scroll-to-top-btn {
  position: fixed;
  bottom: 2rem;
  right: 2rem;
  z-index: 9999;

  display: flex;
  align-items: center;
  gap: 0.5rem;

  background: var(--universal-tint);
  -webkit-backdrop-filter: blur(var(--universal-blur, 8px));
  backdrop-filter: blur(var(--universal-blur, 8px));
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-radius: 40px;

  color: #fff;
  font-family: inherit;
  font-size: 0.9rem;
  font-weight: 600;
  padding: 0.7rem 1.2rem;
  cursor: pointer;

  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
  transition: all 0.3s cubic-bezier(0.25, 1, 0.5, 1);
}

.scroll-to-top-btn:hover {
  background: var(--accent, #fff);

  transform: translateY(-4px);
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.4);
}

.scroll-to-top-btn:hover .arrow-icon {
  transform: translateY(-2px);
}

.arrow-icon {
  transition: transform 0.3s ease;
}

/* Enter and leave transitions */
.fade-slide-enter-active,
.fade-slide-leave-active {
  transition:
    opacity 0.4s ease,
    transform 0.4s cubic-bezier(0.25, 1, 0.5, 1);
}

.fade-slide-enter-from,
.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(20px);
}
</style>
