<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useCartStore } from '../stores/cartStore';
import { useAuthStore } from '../stores/authStore';
import { BASE_URL } from '../api/client.js';

const router = useRouter();
const route = useRoute();
const cartStore = useCartStore();
const authStore = useAuthStore();

const email = ref('');
const password = ref('');
const confirmPassword = ref('');
const errorMsg = ref('');
const isSubmitting = ref(false);
const passwordTouched = ref(false);

onMounted(() => {
  if (route.query.email) {
    email.value = route.query.email;
  }
});

// ── Password Policy Validation ────────────────────────────────────────────
const PASSWORD_REGEX = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

const passwordRules = computed(() => [
  { label: 'At least 8 characters',        pass: password.value.length >= 8 },
  { label: 'At least one uppercase letter', pass: /[A-Z]/.test(password.value) },
  { label: 'At least one lowercase letter', pass: /[a-z]/.test(password.value) },
  { label: 'At least one number',           pass: /\d/.test(password.value) },
  { label: 'At least one special character (@$!%*?&)', pass: /[@$!%*?&]/.test(password.value) },
]);

const passwordValid = computed(() => PASSWORD_REGEX.test(password.value));
const showPasswordHints = computed(() => passwordTouched.value && !passwordValid.value);

const handleRegister = async () => {
  errorMsg.value = '';
  passwordTouched.value = true;

  if (!email.value || !password.value) {
    errorMsg.value = 'Please fill out all fields.';
    return;
  }

  if (!passwordValid.value) {
    errorMsg.value = 'Password does not meet the security requirements below.';
    return;
  }

  if (password.value !== confirmPassword.value) {
    errorMsg.value = 'Passwords do not match.';
    return;
  }

  isSubmitting.value = true;

  try {
    const res = await fetch(`${BASE_URL}/api/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: email.value, password: password.value })
    });

    if (!res.ok) {
      const data = await res.json().catch(() => ({}));
      throw new Error(data.message || 'Registration failed.');
    }

    // Auto-login
    const loginRes = await fetch(`${BASE_URL}/api/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: email.value, password: password.value })
    });

    if (loginRes.ok) {
      const data = await loginRes.json();
      authStore.setAuth(data.token, data.user);

      // Claim Order
      if (route.query.claimOrder) {
        try {
          await authStore.fetchWithAuth(`${BASE_URL}/api/orders/${route.query.claimOrder}/claim`, { method: 'POST' });
          cartStore.triggerToast('Account created and order claimed successfully!', 'success');
        } catch (e) {
          cartStore.triggerToast('Account created, but order claiming failed.', 'warning');
        }
      } else {
        cartStore.triggerToast('Successfully registered!', 'success');
      }
    } else {
      cartStore.triggerToast('Registered successfully, but auto-login failed. Please log in.', 'warning');
    }

    if (route.query.redirect) {
      router.push(route.query.redirect);
    } else {
      router.push(authStore.isAuthenticated ? '/account/profile' : '/login');
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
      <h1 class="page-title">Register</h1>
      
      <div v-if="errorMsg" class="error-msg">{{ errorMsg }}</div>

      <form @submit.prevent="handleRegister" class="glass-form">
        <div class="form-group">
          <label>Email Address</label>
          <input type="email" v-model="email" placeholder="you@example.com" maxlength="100" />
        </div>
        
        <div class="form-group">
          <label>Password</label>
          <input
            type="password"
            v-model="password"
            placeholder="••••••••"
            @blur="passwordTouched = true"
            :class="{ 'input-error': showPasswordHints }"
            maxlength="50"
          />
          <transition name="hints-fade">
            <ul v-if="showPasswordHints" class="password-hints">
              <li
                v-for="rule in passwordRules"
                :key="rule.label"
                :class="rule.pass ? 'rule-pass' : 'rule-fail'"
              >
                <span class="rule-icon">{{ rule.pass ? '✓' : '✗' }}</span>
                {{ rule.label }}
              </li>
            </ul>
          </transition>
        </div>

        <div class="form-group">
          <label>Confirm Password</label>
          <input type="password" v-model="confirmPassword" placeholder="••••••••" maxlength="50" />
        </div>

        <button type="submit" class="submit-btn" :disabled="isSubmitting">
          {{ isSubmitting ? 'Creating Account...' : 'Sign Up' }}
        </button>
      </form>

      <div class="auth-links">
        <p>Already have an account? <router-link to="/login">Sign in here</router-link></p>
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

.input-error {
  border-color: rgba(255, 80, 80, 0.6) !important;
}

.password-hints {
  list-style: none;
  padding: 12px 16px;
  margin: 8px 0 0;
  background: rgba(255, 50, 50, 0.06);
  border: 1px solid rgba(255, 50, 50, 0.2);
  border-radius: 10px;
  backdrop-filter: blur(8px);
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.password-hints li {
  font-size: 0.8rem;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: color 0.2s;
}

.rule-icon {
  font-size: 0.75rem;
  font-weight: 800;
  width: 14px;
  flex-shrink: 0;
}

.rule-pass {
  color: #4ecb71;
}

.rule-fail {
  color: rgba(255, 255, 255, 0.5);
}

.hints-fade-enter-active,
.hints-fade-leave-active {
  transition: all 0.25s ease;
}

.hints-fade-enter-from,
.hints-fade-leave-to {
  opacity: 0;
  transform: translateY(-6px);
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
