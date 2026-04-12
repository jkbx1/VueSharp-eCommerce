<script setup>
import { ref } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useAuthStore } from '../stores/authStore';
import { useCartStore } from '../stores/cartStore';
import { BASE_URL } from '../api/client.js';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const cartStore = useCartStore();

const email = ref('');
const password = ref('');
const errorMsg = ref('');
const isSubmitting = ref(false);

const handleLogin = async () => {
  errorMsg.value = '';
  if (!email.value || !password.value) {
    errorMsg.value = 'Please enter your email and password.';
    return;
  }

  isSubmitting.value = true;
  
  try {
    const res = await fetch(`${BASE_URL}/api/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: email.value, password: password.value })
    });

    if (!res.ok) {
        const data = await res.json().catch(() => ({}));
        throw new Error(data.message || 'Invalid credentials.');
    }

    const data = await res.json();
    authStore.setAuth(data.token, data.user);
    cartStore.triggerToast('Successfully logged in!', 'success');
    
    if (route.query.redirect) {
      router.push(route.query.redirect);
    } else {
      router.push('/');
    }
  } catch (err) {
    errorMsg.value = err.message;
  } finally {
    isSubmitting.value = false;
  }
};
</script>

<template>
  <div class="auth-page">
    <div class="glass-form-container">
      <h1 class="page-title">Login</h1>
      
      <div v-if="errorMsg" class="error-msg">{{ errorMsg }}</div>

      <form @submit.prevent="handleLogin" class="glass-form">
        <div class="form-group">
          <label>Email Address</label>
          <input type="email" v-model="email" placeholder="you@example.com" maxlength="100" />
        </div>
        
        <div class="form-group">
          <label>Password</label>
          <input type="password" v-model="password" placeholder="••••••••" maxlength="50" />
        </div>

        <button type="submit" class="submit-btn" :disabled="isSubmitting">
          {{ isSubmitting ? 'Verifying...' : 'Sign In' }}
        </button>
      </form>

      <div class="auth-links">
        <p>Don't have an account? <router-link to="/register">Register here</router-link></p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.auth-page {
  padding: 120px 20px 60px;
  min-height: 100vh;
  background: var(--bg-color, #0a0a0a);
  color: #fff;
  display: flex;
  justify-content: center;
  align-items: flex-start;
}

.glass-form-container {
  width: 100%;
  max-width: 450px;
}

.page-title {
  font-size: 2.2rem;
  font-weight: 800;
  margin-bottom: 30px;
  text-align: center;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.error-msg {
  background: rgba(255, 50, 50, 0.1);
  border: 1px solid rgba(255, 50, 50, 0.3);
  color: #ff6b6b;
  padding: 15px;
  border-radius: 12px;
  margin-bottom: 20px;
  font-weight: 500;
  text-align: center;
}

.glass-form {
  background: rgba(255, 255, 255, 0.03);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 20px;
  padding: 40px 30px;
  display: flex;
  flex-direction: column;
}

.form-group {
  margin-bottom: 20px;
  display: flex;
  flex-direction: column;
}

.form-group label {
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: rgba(255, 255, 255, 0.6);
  margin-bottom: 8px;
}

.form-group input {
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(255, 255, 255, 0.1);
  color: #fff;
  padding: 14px 16px;
  border-radius: 10px;
  font-size: 1rem;
  outline: none;
  transition: all 0.3s ease;
}

.form-group input:focus {
  border-color: rgba(255, 255, 255, 0.4);
  background: rgba(0, 0, 0, 0.5);
}

.submit-btn {
  background: #fff;
  color: #000;
  border: none;
  border-radius: 12px;
  padding: 16px;
  font-size: 1rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  cursor: pointer;
  margin-top: 15px;
  transition: all 0.3s;
}

.submit-btn:hover:not(:disabled) {
  background: #eee;
  transform: translateY(-2px);
}

.submit-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.auth-links {
  text-align: center;
  margin-top: 25px;
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.6);
}

.auth-links a {
  color: #fff;
  text-decoration: underline;
  font-weight: 600;
}

.auth-links a:hover {
  color: var(--accent, #ccc);
}
</style>
