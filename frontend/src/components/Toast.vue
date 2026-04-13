<script setup>
import { useCartStore } from "../stores/cartStore";

const cartStore = useCartStore();
</script>

<template>
  <transition name="toast-fade">
    <div
      v-if="cartStore.showToast"
      :class="['toast-wrapper', cartStore.toastType]"
    >
      <div class="toast-content">
        <div class="toast-icon">
          <!-- Checkmark for Success -->
          <svg
            v-if="cartStore.toastType === 'success'"
            xmlns="http://www.w3.org/2000/svg"
            width="14"
            height="14"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="3.5"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <polyline points="20 6 9 17 4 12"></polyline>
          </svg>
          <!-- X for Error -->
          <svg
            v-else
            xmlns="http://www.w3.org/2000/svg"
            width="14"
            height="14"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="3.5"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <line x1="18" y1="6" x2="6" y2="18"></line>
            <line x1="6" y1="6" x2="18" y2="18"></line>
          </svg>
        </div>
        <span class="toast-text">{{ cartStore.toastMessage }}</span>
      </div>
    </div>
  </transition>
</template>

<style scoped>
.toast-wrapper {
  position: fixed;
  bottom: 6.5rem; 
  right: 2rem; 
  z-index: 10000;
  pointer-events: none;
  border-radius: 999px;
  background: var(--universal-tint);
  -webkit-backdrop-filter: blur(var(--universal-blur, 12px)) saturate(300%);
  backdrop-filter: blur(var(--universal-blur, 12px)) saturate(300%);
}

.toast-content {
  padding: 10px 18px;
  border-radius: 999px; 
  display: flex;
  align-items: center;
  gap: 10px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.4);
  pointer-events: auto;
  border: 1px solid rgba(255, 255, 255, 0.1);
  transition: all 0.3s ease;
}

/* Success State - Green Glass Tint */
.toast-wrapper.success .toast-content {
  background: rgba(34, 197, 94, 0.12); 
  border-color: rgba(34, 197, 94, 0.3);
}

.toast-wrapper.success .toast-icon {
  background: #22c55e;
}

/* Error State - Red Glass Tint */
.toast-wrapper.error .toast-content {
  background: rgba(239, 68, 68, 0.12); 
  border-color: rgba(239, 68, 68, 0.3);
  box-shadow: 0 10px 30px rgba(239, 68, 68, 0.15);
}

.toast-wrapper.error .toast-icon {
  background: #ef4444;
}

.toast-icon {
  width: 22px;
  height: 22px;
  color: #fff;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.toast-text {
  color: #fff;
  font-size: 0.82rem;
  font-weight: 600;
  letter-spacing: 0.01em;
  white-space: nowrap;
}

/* Transitions */
.toast-fade-enter-active,
.toast-fade-leave-active {
  transition: all 0.4s cubic-bezier(0.22, 1, 0.36, 1);
}

.toast-fade-enter-from {
  opacity: 0;
  transform: translateY(20px) scale(0.9);
}

.toast-fade-leave-to {
  opacity: 0;
  transform: translateY(10px) scale(0.95);
}
</style>
