<script setup>
import { useAuthStore } from '../stores/authStore';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();

// Just an added guard layer manually routing outside if not valid
if (!authStore.isAuthenticated) {
  router.push('/login');
}
</script>

<template>
  <div class="dashboard-layout">
    <div class="dashboard-sidebar glass-panel">
      <div class="sidebar-header">
        <h2>My Account</h2>
        <p v-if="authStore.user">{{ authStore.user.email }}</p>
      </div>

      <nav class="sidebar-nav">
        <router-link to="/account/profile" class="nav-btn" active-class="active">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path><circle cx="12" cy="7" r="4"></circle></svg>
          Profile
        </router-link>
        
        <router-link to="/account/orders" class="nav-btn" active-class="active">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line></svg>
          Order History
        </router-link>

        <router-link to="/account/security" class="nav-btn" active-class="active">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather"><rect x="3" y="11" width="18" height="11" rx="2" ry="2"></rect><path d="M7 11V7a5 5 0 0 1 10 0v4"></path></svg>
          Security
        </router-link>
      </nav>
    </div>

    <div class="dashboard-content">
      <router-view></router-view>
    </div>
  </div>
</template>

<style scoped>
.dashboard-layout {
  padding: 100px 20px 40px;
  max-width: 1200px;
  margin: 0 auto;
  min-height: 100vh;
  display: flex;
  gap: 30px;
  box-sizing: border-box;
}

@media (max-width: 800px) {
  .dashboard-layout {
    flex-direction: column;
    padding: 80px 15px 40px;
    gap: 20px;
  }
}

.glass-panel {
  background: rgba(255, 255, 255, 0.03);
  -webkit-backdrop-filter: blur(16px);
  backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 20px;
}

.dashboard-sidebar {
  width: 260px;
  flex-shrink: 0;
  padding: 30px 20px;
  height: fit-content;
  position: sticky;
  top: 100px;
  box-sizing: border-box;
}

@media (max-width: 800px) {
  .dashboard-sidebar {
    width: 100%;
    position: static;
    padding: 20px;
  }
}

.sidebar-header {
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

@media (max-width: 800px) {
  .sidebar-header {
    margin-bottom: 15px;
    padding-bottom: 15px;
  }
}

.sidebar-header h2 {
  font-size: 1.4rem;
  font-weight: 800;
  margin-bottom: 4px;
}

.sidebar-header p {
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.5);
  word-break: break-all;
  overflow-wrap: break-word;
}

.sidebar-nav {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

@media (max-width: 800px) {
  .sidebar-nav {
    flex-direction: row;
    flex-wrap: wrap;
  }
  
  .nav-btn {
    flex: 1;
    min-width: 140px;
    justify-content: center;
  }
}

.nav-btn {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 16px;
  border-radius: 12px;
  color: rgba(255, 255, 255, 0.7);
  text-decoration: none;
  font-size: 0.95rem;
  font-weight: 600;
  transition: all 0.2s ease;
}

.nav-btn:hover {
  background: rgba(255, 255, 255, 0.05);
  color: #fff;
}

.nav-btn.active {
  background: rgba(255, 255, 255, 0.1);
  color: #fff;
  border-left: 3px solid var(--accent, #fff);
  border-radius: 0 12px 12px 0;
}

@media (max-width: 800px) {
  .nav-btn.active {
    border-left: none;
    border-bottom: 3px solid var(--accent, #fff);
    border-radius: 12px;
  }
}

.dashboard-content {
  flex: 1;
  min-width: 0; 
}
</style>
