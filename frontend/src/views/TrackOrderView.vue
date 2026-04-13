<script setup>
import { ref } from 'vue';
import { BASE_URL } from '../api/client.js';

const orderId = ref('');
const email = ref('');
const order = ref(null);
const loading = ref(false);
const error = ref('');

const trackOrder = async () => {
    loading.value = true;
    error.value = '';
    order.value = null;

    try {
        const response = await fetch(`${BASE_URL}/api/orders/track?orderId=${orderId.value}&email=${encodeURIComponent(email.value)}`);
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            throw new Error(errorData.message || 'Order not found. Please check your details.');
        }
        order.value = await response.json();
    } catch (err) {
        error.value = err.message;
    } finally {
        loading.value = false;
    }
};

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
};
</script>

<template>
  <div class="track-order-page" style="margin-top: 100px;">
    <div class="glass-container">
      <h1 class="title">Track Your Order</h1>
      <p class="subtitle">Enter your order ID and email to see the status.</p>

      <form v-if="!order" @submit.prevent="trackOrder" class="track-form">
        <div class="input-group">
          <label>Order ID</label>
          <input type="number" v-model="orderId" required class="glass-input" placeholder="e.g. 1234" maxlength="10" />
        </div>
        <div class="input-group">
          <label>Email Address</label>
          <input type="email" v-model="email" required class="glass-input" placeholder="your@email.com" maxlength="100" />
        </div>
        
        <p v-if="error" class="error-msg">{{ error }}</p>

        <button type="submit" class="submit-btn" :disabled="loading">
          {{ loading ? 'Tracking...' : 'Track Order' }}
        </button>
      </form>

      <div v-else class="order-summary">
        <h2>Order #{{ order.id }}</h2>
        <div class="summary-details">
          <div class="detail-row"><span class="label">Date:</span><span>{{ formatDate(order.orderDate) }}</span></div>
          <div class="detail-row"><span class="label">Total:</span><span>${{ order.totalAmount.toFixed(2) }}</span></div>
          <div class="detail-row"><span class="label">Status:</span><span class="status-badge">Processing</span></div>
        </div>

        <h3 class="items-title">Items</h3>
        <div class="order-items">
          <div v-for="item in order.items" :key="item.id" class="item-card glass-input">
            <div class="item-info">
              <span class="item-name">{{ item.product?.name || 'Product ' + item.productId }}</span>
              <span class="item-meta">Size: {{ item.selectedSize }} | Color: {{ item.selectedColor }}</span>
              <span class="item-meta">Qty: {{ item.quantity }} × ${{ item.unitPrice.toFixed(2) }}</span>
            </div>
          </div>
        </div>

        <button @click="order = null; orderId = ''; email = ''" class="reset-btn">Track Another Order</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.track-order-page {
  min-height: calc(100vh - 400px); 
  display: flex;
  align-items: flex-start;
  justify-content: center;
  padding: 40px 20px 60px;
  background: var(--bg-color, #0a0a0a);
  color: #fff;
}

.glass-container {
  background: rgba(255, 255, 255, 0.03);
  -webkit-backdrop-filter: blur(16px);
  backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 24px;
  padding: 50px 40px;
  width: 100%;
  max-width: 500px;
  box-sizing: border-box;
}

.title {
  font-size: 2rem;
  font-weight: 800;
  margin-bottom: 10px;
  text-align: center;
}

.subtitle {
  color: rgba(255, 255, 255, 0.6);
  text-align: center;
  margin-bottom: 30px;
}

.track-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.input-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.input-group label {
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.8);
}

.glass-input {
  background: rgba(0, 0, 0, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.1);
  padding: 14px 16px;
  border-radius: 12px;
  color: white;
  font-family: inherit;
  font-size: 1rem;
  transition: border-color 0.3s;
}

.glass-input:focus {
  outline: none;
  border-color: rgba(255, 255, 255, 0.3);
}

.error-msg {
  color: #ff6b6b;
  font-size: 0.9rem;
  text-align: center;
}

.submit-btn, .reset-btn {
  background: #fff;
  color: #000;
  border: none;
  padding: 16px;
  border-radius: 12px;
  font-weight: 700;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.3s;
  text-transform: uppercase;
  margin-top: 10px;
}

.submit-btn:hover:not(:disabled), .reset-btn:hover {
  background: #eee;
  transform: translateY(-2px);
}

.submit-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.order-summary h2 {
  font-size: 1.5rem;
  margin-bottom: 20px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  padding-bottom: 10px;
}

.summary-details {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 30px;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  padding-bottom: 8px;
}

.detail-row .label {
  color: rgba(255, 255, 255, 0.6);
}

.status-badge {
  background: rgba(46, 213, 115, 0.2);
  color: #2ed573;
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
}

.items-title {
  font-size: 1.1rem;
  margin-bottom: 15px;
}

.order-items {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-bottom: 30px;
  max-height: 300px;
  overflow-y: auto;
}

.item-card {
  padding: 12px;
}

.item-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.item-name {
  font-weight: 600;
}

.item-meta {
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.6);
}

.reset-btn {
  width: 100%;
}
</style>
