import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('vuesharp_jwt') || null);
  const user = ref(JSON.parse(localStorage.getItem('vuesharp_user')) || null);

  const isAuthenticated = computed(() => !!token.value);

  const setAuth = (newToken, newUser) => {
    token.value = newToken;
    user.value = newUser;
    localStorage.setItem('vuesharp_jwt', newToken);
    localStorage.setItem('vuesharp_user', JSON.stringify(newUser));
  };

  const logout = () => {
    token.value = null;
    user.value = null;
    localStorage.removeItem('vuesharp_jwt');
    localStorage.removeItem('vuesharp_user');
  };

  const fetchWithAuth = async (url, options = {}) => {
    const headers = { ...options.headers };
    
    if (token.value) {
      headers['Authorization'] = `Bearer ${token.value}`;
    }

    return fetch(url, { ...options, headers });
  };

  return {
    token,
    user,
    isAuthenticated,
    setAuth,
    logout,
    fetchWithAuth
  };
});
