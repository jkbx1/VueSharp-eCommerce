<script setup>
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/authStore';
import { useCartStore } from '../stores/cartStore';
import { BASE_URL } from '../api/client.js';

const authStore = useAuthStore();
const cartStore = useCartStore();
const router = useRouter();

const passForm = ref({
  currentPassword: '',
  newPassword: ''
});
const isSavingPass = ref(false);
const passError = ref('');
const newPasswordTouched = ref(false);

// ── Password Policy Validation ────────────────────────────────────────────
const PASSWORD_REGEX = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

const newPasswordRules = computed(() => [
  { label: 'At least 8 characters',        pass: passForm.value.newPassword.length >= 8 },
  { label: 'At least one uppercase letter', pass: /[A-Z]/.test(passForm.value.newPassword) },
  { label: 'At least one lowercase letter', pass: /[a-z]/.test(passForm.value.newPassword) },
  { label: 'At least one number',           pass: /\d/.test(passForm.value.newPassword) },
  { label: 'At least one special character (@$!%*?&)', pass: /[@$!%*?&]/.test(passForm.value.newPassword) },
]);

const newPasswordValid = computed(() => PASSWORD_REGEX.test(passForm.value.newPassword));
const showNewPasswordHints = computed(() => newPasswordTouched.value && !newPasswordValid.value);

const isDeleting = ref(false);
const showDeleteConfirm = ref(false);
const deletePassword = ref('');
const deleteError = ref('');

const changePassword = async () => {
  passError.value = '';
  newPasswordTouched.value = true;

  if (!passForm.value.currentPassword || !passForm.value.newPassword) {
    passError.value = 'Please provide both current and new passwords.';
    return;
  }

  if (!newPasswordValid.value) {
    passError.value = 'New password does not meet the security requirements below.';
    return;
  }

  isSavingPass.value = true;
  try {
    const res = await authStore.fetchWithAuth(`${BASE_URL}/api/user/change-password`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(passForm.value)
    });

    if (!res.ok) {
      const data = await res.json().catch(() => ({}));
      throw new Error(data.message || 'Password update failed.');
    }

    cartStore.triggerToast('Password changed successfully!', 'success');
    passForm.value.currentPassword = '';
    passForm.value.newPassword = '';
    newPasswordTouched.value = false;
  } catch (err) {
    passError.value = err.message;
  } finally {
    isSavingPass.value = false;
  }
};

const triggerDeleteSequence = () => {
  showDeleteConfirm.value = true;
  deletePassword.value = '';
  deleteError.value = '';
};

const confirmDeletion = async () => {
  deleteError.value = '';
  if (!deletePassword.value) {
    deleteError.value = 'You must prove identity using your password.';
    return;
  }

  isDeleting.value = true;
  try {
    const res = await authStore.fetchWithAuth(`${BASE_URL}/api/user/account`, {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ password: deletePassword.value })
    });
    
    if (!res.ok) {
      const data = await res.json().catch(() => ({}));
      throw new Error(data.message || 'Verification failed.');
    }
    
    cartStore.triggerToast('Account successfully purged.', 'success');
    authStore.logout();
    router.push('/');
  } catch (err) {
    deleteError.value = err.message;
  } finally {
    isDeleting.value = false;
  }
};
</script>

<template>
  <div class="glass-form-container">
    <h1 class="page-title">Security Settings</h1>
    
    <!-- Change Password Block -->
    <div class="glass-form">
      <h3 class="section-divider">Change Password</h3>
      
      <div v-if="passError" class="error-msg">{{ passError }}</div>
      
      <form @submit.prevent="changePassword">
        <div class="form-group">
          <label>Current Password</label>
          <input type="password" v-model="passForm.currentPassword" placeholder="••••••••" maxlength="50" />
        </div>
        <div class="form-group">
          <label>New Password</label>
          <input
            type="password"
            v-model="passForm.newPassword"
            placeholder="••••••••"
            @blur="newPasswordTouched = true"
            :class="{ 'input-error': showNewPasswordHints }"
            maxlength="50"
          />
          <transition name="hints-fade">
            <ul v-if="showNewPasswordHints" class="password-hints">
              <li
                v-for="rule in newPasswordRules"
                :key="rule.label"
                :class="rule.pass ? 'rule-pass' : 'rule-fail'"
              >
                <span class="rule-icon">{{ rule.pass ? '✓' : '✗' }}</span>
                {{ rule.label }}
              </li>
            </ul>
          </transition>
        </div>
        
        <button type="submit" class="submit-btn" :disabled="isSavingPass">
          {{ isSavingPass ? 'Updating...' : 'Update Password' }}
        </button>
      </form>
    </div>

    <!-- Danger Zone: Account Deletion -->
    <div class="glass-form danger-zone">
      <h3 class="section-divider text-danger">Danger Zone</h3>
      <p class="warning-text">
        Once you delete your account, there is no going back. Please be certain.
        For financial compliance, history of generated purchase invoices will remain mapped anonymously within our data centers.
      </p>

      <button v-if="!showDeleteConfirm" @click="triggerDeleteSequence" class="delete-init-btn">
        Delete My Account
      </button>

      <div v-else class="delete-confirm-block">
        <div v-if="deleteError" class="error-msg">{{ deleteError }}</div>
        
        <div class="form-group">
          <label>Verify Password to continue</label>
          <input type="password" v-model="deletePassword" placeholder="••••••••" maxlength="50" />
        </div>
        
        <div class="confirm-actions">
          <button @click="showDeleteConfirm = false" class="cancel-btn">Cancel</button>
          <button @click="confirmDeletion" class="delete-btn" :disabled="isDeleting">
            {{ isDeleting ? 'Purging...' : 'Confirm Deletion' }}
          </button>
        </div>
      </div>
    </div>
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

.glass-form {
  background: rgba(255, 255, 255, 0.03);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 20px;
  padding: 30px;
  margin-bottom: 30px;
  display: flex;
  flex-direction: column;
}

@media (max-width: 600px) {
  .glass-form {
    padding: 20px 15px;
  }
}

.section-divider {
  font-size: 1.2rem;
  font-weight: 700;
  margin-bottom: 20px;
}

.text-danger {
  color: #ff6b6b;
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
  width: 100%;
  box-sizing: border-box;
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
  transition: all 0.3s;
  width: max-content;
  min-width: 200px;
}

@media (max-width: 600px) {
  .submit-btn {
    width: 100%;
    min-width: auto;
  }
}

.submit-btn:hover:not(:disabled) {
  background: #eee;
  transform: translateY(-2px);
}

.error-msg {
  background: rgba(255, 50, 50, 0.1);
  border: 1px solid rgba(255, 50, 50, 0.3);
  color: #ff6b6b;
  padding: 15px;
  border-radius: 12px;
  margin-bottom: 20px;
  font-weight: 500;
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

.rule-pass { color: #4ecb71; }
.rule-fail { color: rgba(255, 255, 255, 0.5); }

.hints-fade-enter-active, .hints-fade-leave-active { transition: all 0.25s ease; }
.hints-fade-enter-from, .hints-fade-leave-to { opacity: 0; transform: translateY(-6px); }

.danger-zone {
  border-color: rgba(255, 107, 107, 0.3);
  background: rgba(255, 107, 107, 0.03);
}

.warning-text {
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: 25px;
  line-height: 1.5;
}

.delete-init-btn {
  background: transparent;
  color: #ff6b6b;
  border: 1px solid #ff6b6b;
  border-radius: 12px;
  padding: 16px;
  font-size: 1rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  cursor: pointer;
  transition: all 0.3s;
  width: max-content;
  min-width: 200px;
}

@media (max-width: 600px) {
  .delete-init-btn {
    width: 100%;
    min-width: auto;
  }
}

.delete-init-btn:hover {
  background: rgba(255, 107, 107, 0.1);
}

.delete-confirm-block {
  background: rgba(0, 0, 0, 0.2);
  padding: 20px;
  border-radius: 15px;
  border: 1px solid rgba(255, 107, 107, 0.2);
}

@media (max-width: 600px) {
  .delete-confirm-block {
    padding: 15px;
  }
}

.confirm-actions {
  display: flex;
  gap: 15px;
  margin-top: 15px;
}

@media (max-width: 600px) {
  .confirm-actions {
    flex-direction: column;
    gap: 10px;
  }
}

.cancel-btn {
  background: rgba(255, 255, 255, 0.1);
  color: #fff;
  border: none;
  border-radius: 12px;
  padding: 14px 24px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  cursor: pointer;
  transition: background 0.2s;
  flex: 1;
}

.cancel-btn:hover {
  background: rgba(255, 255, 255, 0.2);
}

.delete-btn {
  background: #ff6b6b;
  color: #fff;
  border: none;
  border-radius: 12px;
  padding: 14px 24px;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  cursor: pointer;
  transition: background 0.2s;
  flex: 2;
  white-space: nowrap; 
}

@media (max-width: 400px) {
  .delete-btn {
    white-space: normal;
    line-height: 1.2;
  }
}

.delete-btn:hover:not(:disabled) {
  background: #ff5252;
}

.delete-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>
