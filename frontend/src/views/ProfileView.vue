<script setup>
import { ref, onMounted } from 'vue';
import { useAuthStore } from '../stores/authStore';
import { useCartStore } from '../stores/cartStore';
import { BASE_URL } from '../api/client.js';

const authStore = useAuthStore();
const cartStore = useCartStore();

const form = ref({
  email: '',
  firstName: '',
  lastName: '',
  defaultAddress: '',
  defaultCity: '',
  defaultZip: ''
});

const isLoaded = ref(false);
const isSaving = ref(false);

const loadProfile = async () => {
  try {
    const res = await authStore.fetchWithAuth(`${BASE_URL}/api/user/profile`);
    if (res.ok) {
      const data = await res.json();
      form.value = { ...data };
    }
  } catch (err) {
    cartStore.triggerToast('Failed to load profile.', 'error');
  } finally {
    isLoaded.value = true;
  }
};

const saveProfile = async () => {
  isSaving.value = true;
  try {
    const res = await authStore.fetchWithAuth(`${BASE_URL}/api/user/profile`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(form.value)
    });
    
    if (!res.ok) throw new Error();
    cartStore.triggerToast('Profile updated successfully.', 'success');
  } catch (err) {
    cartStore.triggerToast('Failed to update profile.', 'error');
  } finally {
    isSaving.value = false;
  }
};

onMounted(() => {
  loadProfile();
});
</script>

<template>
  <div class="glass-form-container">
    <h1 class="page-title">Personal Information</h1>
    
    <div v-if="!isLoaded" class="loading">Loading...</div>

    <form v-else @submit.prevent="saveProfile" class="glass-form">
      <div class="form-group">
        <label>Email Address</label>
        <input type="email" v-model="form.email" disabled class="disabled-input" />
        <span class="hint">Your email address cannot be changed.</span>
      </div>

      <div class="form-row">
        <div class="form-group half">
          <label>First Name</label>
          <input type="text" v-model="form.firstName" placeholder="Jane" maxlength="50" />
        </div>
        <div class="form-group half">
          <label>Last Name</label>
          <input type="text" v-model="form.lastName" placeholder="Doe" maxlength="50" />
        </div>
      </div>

      <h3 class="section-divider">Default Shipping</h3>

      <div class="form-group">
        <label>Address</label>
        <input type="text" v-model="form.defaultAddress" placeholder="123 Main St" maxlength="150" />
      </div>

      <div class="form-row">
        <div class="form-group half">
          <label>City</label>
          <input type="text" v-model="form.defaultCity" placeholder="New York" maxlength="50" />
        </div>
        <div class="form-group half">
          <label>ZIP / Postal</label>
          <input type="text" v-model="form.defaultZip" placeholder="10001" maxlength="10" />
        </div>
      </div>

      <button type="submit" class="submit-btn" :disabled="isSaving">
        {{ isSaving ? 'Saving...' : 'Save Changes' }}
      </button>
    </form>
  </div>
</template>

<style scoped>
.glass-form-container {
  width: 100%;
}

.page-title {
  font-size: 1.8rem;
  font-weight: 800;
  margin-bottom: 25px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

@media (max-width: 600px) {
  .page-title {
    font-size: 1.4rem;
    text-align: center;
  }
}

.section-divider {
  font-size: 1.2rem;
  font-weight: 700;
  margin: 30px 0 20px;
  padding-bottom: 10px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.glass-form {
  background: rgba(255, 255, 255, 0.03);
  -webkit-backdrop-filter: blur(16px);
  backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 20px;
  padding: 30px;
  display: flex;
  flex-direction: column;
}

@media (max-width: 600px) {
  .glass-form {
    padding: 20px 15px;
  }
}

.form-row {
  display: flex;
  gap: 20px;
}

@media (max-width: 600px) {
  .form-row {
    flex-direction: column;
    gap: 0;
  }
}

.form-group {
  margin-bottom: 20px;
  display: flex;
  flex-direction: column;
}

.form-group.half {
  flex: 1;
}

.form-group label {
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: rgba(255, 255, 255, 0.6);
  margin-bottom: 8px;
}

.hint {
  font-size: 0.7rem;
  color: rgba(255, 255, 255, 0.4);
  margin-top: 6px;
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
  width: 100%;
  box-sizing: border-box;
}

.form-group input:focus:not(:disabled) {
  border-color: rgba(255, 255, 255, 0.4);
  background: rgba(0, 0, 0, 0.5);
}

.disabled-input {
  opacity: 0.5;
  cursor: not-allowed;
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
  width: max-content;
  min-width: 200px;
  align-self: flex-start;
}

@media (max-width: 600px) {
  .submit-btn {
    width: 100%;
    align-self: stretch;
  }
}

.submit-btn:hover:not(:disabled) {
  background: #eee;
  transform: translateY(-2px);
}

.loading {
  opacity: 0.5;
  padding: 20px 0;
}
</style>
