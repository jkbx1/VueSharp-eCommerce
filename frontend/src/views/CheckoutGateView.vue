<script setup>
</script>

<template>
  <div class="gate-page">
    <div class="glass-container">
      <h1 class="page-title">Checkout Options</h1>

      <div class="gate-options">
        <div class="auth-section">
          <h2>Shopped with us before?</h2>
          <p>Login to use your saved shipping arrangements securely.</p>
          <router-link to="/login?redirect=/checkout" class="gate-btn primary-btn">
            Sign In
          </router-link>

          <div class="register-prompt">
            <span>New customer?</span>
            <router-link to="/register?redirect=/checkout" class="text-link">Create an Account</router-link>
          </div>
        </div>

        <div class="divider">
          <span>OR</span>
        </div>

        <div class="guest-section">
          <h2>Continue as Guest</h2>
          <p>You can effortlessly create an account after placing your order if you choose.</p>
          <router-link to="/checkout" class="gate-btn guest-btn">
            Guest Checkout
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.gate-page {
  padding: 120px 20px 60px;
  min-height: 100vh;
  background: var(--bg-color, #0a0a0a);
  color: #fff;
  display: flex;
  justify-content: center;
  align-items: flex-start;
}

@media (max-width: 768px) {
  .gate-page {
    padding: 80px 15px 40px;
  }
}

.glass-container {
  width: 100%;
  max-width: 800px;
  background: rgba(255, 255, 255, 0.03);
  -webkit-backdrop-filter: blur(16px);
  backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 20px;
  padding: 50px 40px;
  box-sizing: border-box;
}

@media (max-width: 600px) {
  .glass-container {
    padding: 30px 20px;
  }
}

.page-title {
  font-size: 2.2rem;
  font-weight: 800;
  margin-bottom: 40px;
  text-align: center;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  line-height: 1.1;
}

@media (max-width: 600px) {
  .page-title {
    font-size: 1.6rem;
    margin-bottom: 30px;
  }
}

.gate-options {
  display: flex;
  justify-content: space-between;
  align-items: stretch;
  gap: 40px;
}

@media (max-width: 768px) {
  .gate-options {
    flex-direction: column;
    gap: 0;
  }
}

.auth-section, .guest-section {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
}

h2 {
  font-size: 1.4rem;
  font-weight: 700;
  margin-bottom: 15px;
}

@media (max-width: 600px) {
  h2 {
    font-size: 1.2rem;
    text-align: center;
  }
}

p {
  font-size: 0.95rem;
  color: rgba(255, 255, 255, 0.6);
  line-height: 1.5;
  margin-bottom: 30px;
  min-height: 45px;
}

@media (max-width: 600px) {
  p {
    text-align: center;
    margin-bottom: 20px;
    min-height: auto;
  }
}

.gate-btn {
  display: block;
  text-align: center;
  padding: 16px;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  text-decoration: none;
  transition: all 0.3s;
  margin-bottom: 20px;
}

.primary-btn {
  background: #fff;
  color: #000;
}

.primary-btn:hover {
  background: #eee;
  transform: translateY(-2px);
}

.guest-btn {
  background: rgba(255, 255, 255, 0.1);
  color: #fff;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.guest-btn:hover {
  background: rgba(255, 255, 255, 0.15);
  transform: translateY(-2px);
}

.register-prompt {
  text-align: center;
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.6);
}

.text-link {
  color: #fff;
  text-decoration: underline;
  margin-left: 8px;
  font-weight: 600;
}

.text-link:hover {
  color: var(--accent, #ccc);
}

.divider {
  display: flex;
  flex-direction: column;
  align-items: center;
  position: relative;
  min-width: 40px;
}

.divider span {
  background: #1a1a1a; 
  padding: 10px;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.4);
  z-index: 1;
  border-radius: 50%;
}

.divider::before {
  content: '';
  position: absolute;
  top: 0;
  bottom: 0;
  left: 50%;
  width: 1px;
  background: rgba(255, 255, 255, 0.1);
}

@media (max-width: 768px) {
  .divider {
    flex-direction: row;
    justify-content: center;
    margin: 30px 0;
  }
  
  .divider::before {
    top: 50%;
    bottom: auto;
    left: 0;
    right: 0;
    width: auto;
    height: 1px;
  }
}
</style>
