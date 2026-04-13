<script setup>
import { defineProps } from 'vue';
import { useAuthStore } from '../stores/authStore';
import { useRoute } from 'vue-router';

const props = defineProps({
  id: {
    type: String,
    required: true
  }
});

const authStore = useAuthStore();
const route = useRoute();
const orderEmail = route.query.email || '';
</script>

<template>
  <div class="confirmation-page">
    <div class="confirmation-glass">
      <div class="icon-circle">
        <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
          <polyline points="20 6 9 17 4 12"></polyline>
        </svg>
      </div>
      <h1 class="title">Thank You!</h1>
      <p class="subtitle">Your order has been successfully placed.</p>
      
      <div class="order-id">
        <span class="label">Order Number</span>
        <span class="value">#{{ id }}</span>
      </div>

      <div v-if="!authStore.isAuthenticated" class="upsell-card">
        <h3>Want to track your package easily? Let's stay in touch!</h3>
        <p>Create an account using your email address to track all your orders in one place.</p>
        <router-link :to="{ name: 'register', query: { email: orderEmail, claimOrder: id, redirect: '/account/orders' } }" class="upsell-btn">
          Create Account
        </router-link>
      </div>

      <router-link to="/" class="continue-btn">
        Continue Shopping
      </router-link>
    </div>
  </div>
</template>

<style scoped>
.confirmation-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 100px 20px 60px;
  background: var(--bg-color, #0a0a0a);
  color: #fff;
}

.confirmation-glass {
  background: rgba(255, 255, 255, 0.03);
  -webkit-backdrop-filter: blur(16px);
  backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 24px;
  padding: 60px 40px;
  text-align: center;
  max-width: 500px;
  width: 100%;
  box-sizing: border-box;
}

@media (max-width: 600px) {
  .confirmation-glass {
    padding: 40px 20px;
  }
}

.icon-circle {
  width: 80px;
  height: 80px;
  background: rgba(46, 213, 115, 0.1);
  color: #2ed573;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 30px;
}

.title {
  font-size: 2.5rem;
  font-weight: 800;
  margin-bottom: 15px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  line-height: 1.1;
}

@media (max-width: 600px) {
  .title {
    font-size: 1.8rem;
    margin-bottom: 10px;
  }
}

.subtitle {
  color: rgba(255, 255, 255, 0.6);
  font-size: 1.1rem;
  margin-bottom: 40px;
  line-height: 1.4;
}

@media (max-width: 600px) {
  .subtitle {
    font-size: 1rem;
    margin-bottom: 30px;
  }
}

.order-id {
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 40px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.order-id .label {
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  color: rgba(255, 255, 255, 0.5);
}

.order-id .value {
  font-size: 1.5rem;
  font-weight: 700;
}

.continue-btn {
  display: inline-block;
  background: #fff;
  color: #000;
  text-decoration: none;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  padding: 16px 32px;
  border-radius: 12px;
  transition: all 0.3s;
}

.continue-btn:hover {
  background: #eee;
  transform: translateY(-2px);
}

.upsell-card {
  background: rgba(0, 0, 0, 0.4);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 30px;
}

.upsell-card h3 {
  font-size: 1.1rem;
  margin-bottom: 8px;
}

.upsell-card p {
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.9rem;
  margin-bottom: 15px;
}

.upsell-btn {
  display: inline-block;
  background: transparent;
  color: #fff;
  text-decoration: none;
  font-weight: 700;
  border: 1px solid rgba(255,255,255,0.3);
  padding: 10px 20px;
  border-radius: 8px;
  transition: all 0.3s;
}

.upsell-btn:hover {
  background: rgba(255,255,255,0.1);
}
</style>
