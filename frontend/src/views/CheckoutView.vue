<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useCartStore } from '../stores/cartStore';
import { useAuthStore } from '../stores/authStore';
import { BASE_URL } from '../api/client.js';

const cartStore = useCartStore();
const authStore = useAuthStore();
const router = useRouter();

const form = ref({
  name: '',
  email: '',
  address: '',
  city: '',
  zip: ''
});

const isSubmitting = ref(false);
const errorMsg = ref('');

const submitOrder = async () => {
  if (!form.value.name || !form.value.email || !form.value.address || !form.value.city || !form.value.zip) {
    errorMsg.value = "Please fill in all fields.";
    return;
  }
  
  if (cartStore.items.length === 0) {
    errorMsg.value = "Your cart is empty.";
    return;
  }

  isSubmitting.value = true;
  errorMsg.value = '';

  const payload = {
    name: form.value.name,
    email: form.value.email,
    address: form.value.address,
    city: form.value.city,
    zip: form.value.zip,
    items: cartStore.items.map(i => ({
      productId: i.productId,
      selectedSize: i.selectedSize,
      selectedColor: i.selectedColor,
      quantity: i.quantity
    }))
  };

  try {
    const res = await authStore.fetchWithAuth(`${BASE_URL}/api/orders`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(payload)
    });

    if (!res.ok) {
      const data = await res.json().catch(() => ({ message: 'A server error occurred' }));
      throw new Error(data.message || 'Payment failed validation.');
    }

    const result = await res.json();
    
    // Success
    cartStore.clearCart();
    router.push({ path: `/order-confirmation/${result.orderId}`, query: { email: form.value.email } });
    
  } catch (err) {
    errorMsg.value = err.message || 'Something went wrong. Please try again.';
  } finally {
    isSubmitting.value = false;
  }
};

import { onMounted } from 'vue';

onMounted(async () => {
  if (authStore.isAuthenticated) {
    try {
      const res = await authStore.fetchWithAuth(`${BASE_URL}/api/user/profile`);
      if (res.ok) {
        const data = await res.json();
        form.value.email = data.email || '';
        form.value.name = `${data.firstName || ''} ${data.lastName || ''}`.trim();
        form.value.address = data.defaultAddress || '';
        form.value.city = data.defaultCity || '';
        form.value.zip = data.defaultZip || '';
      }
    } catch (e) {
      // Silently fail allowing manual input
    }
  }
});
</script>

<template>
  <div class="checkout-page">
    <div class="checkout-container">
      <h1 class="page-title">Secure Checkout</h1>
      
      <div v-if="errorMsg" class="error-msg">
        {{ errorMsg }}
      </div>

      <div class="checkout-grid">
        <!-- Form Section -->
        <div class="form-section">
          <form @submit.prevent="submitOrder" class="glass-form">
            <div class="form-group">
              <label>Full Name</label>
              <input type="text" v-model="form.name" placeholder="John Doe" maxlength="50" />
            </div>
            
            <div class="form-group">
              <label>Email Address</label>
              <input type="email" v-model="form.email" placeholder="john@example.com" maxlength="100" />
            </div>
            
            <div class="form-group">
              <label>Shipping Address</label>
              <input type="text" v-model="form.address" placeholder="123 Main St" maxlength="150" />
            </div>
            
            <div class="form-row">
              <div class="form-group half">
                <label>City</label>
                <input type="text" v-model="form.city" placeholder="New York" maxlength="50" />
              </div>
              <div class="form-group half">
                <label>ZIP / Postal</label>
                <input type="text" v-model="form.zip" placeholder="10001" maxlength="10" />
              </div>
            </div>

            <button type="submit" class="submit-btn" :disabled="isSubmitting || cartStore.items.length === 0">
              {{ isSubmitting ? 'Processing...' : 'Place Order' }}
            </button>
          </form>
        </div>

        <!-- Summary Section -->
        <div class="summary-section">
          <div class="glass-summary">
            <h3>Order Summary</h3>
            <div class="summary-items">
              <div v-for="item in cartStore.items" :key="`${item.productId}-${item.selectedColor}-${item.selectedSize}`" class="summary-item">
                <div class="si-img">
                  <img :src="item.imageUrl" alt=""/>
                </div>
                <div class="si-info">
                  <div class="si-name">{{ item.name }}</div>
                  <div class="si-var">{{ item.selectedColor }} / {{ item.selectedSize }} - Qty: {{ item.quantity }}</div>
                </div>
                <div class="si-price">
                  ${{ ((item.salePrice || item.price) * item.quantity).toFixed(2) }}
                </div>
              </div>
            </div>
            <div class="summary-total">
              <span>Total</span>
              <span>${{ cartStore.cartTotal.toFixed(2) }}</span>
            </div>
            <div class="price-disclaimer">
              Final prices are calculated at checkout and may vary if sales have ended.
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.checkout-page {
  padding: 100px 20px 60px;
  min-height: 100vh;
  background: var(--bg-color, #0a0a0a);
  color: #fff;
  display: flex;
  justify-content: center;
  box-sizing: border-box;
}

.checkout-container {
  width: 100%;
  max-width: 1000px;
}

.page-title {
  font-size: 2rem;
  font-weight: 800;
  margin-bottom: 30px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

@media (max-width: 600px) {
  .page-title {
    font-size: 1.5rem;
    margin-bottom: 20px;
    text-align: center;
  }
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

.checkout-grid {
  display: grid;
  grid-template-columns: 1.5fr 1fr;
  gap: 40px;
}

@media (max-width: 900px) {
  .checkout-grid {
    gap: 25px;
  }
}

@media (max-width: 768px) {
  .checkout-grid {
    grid-template-columns: 1fr;
  }
}

.glass-form, .glass-summary {
  background: rgba(255, 255, 255, 0.03);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 20px;
  padding: 30px;
}

@media (max-width: 600px) {
  .glass-form, .glass-summary {
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

.form-group input {
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(255, 255, 255, 0.1);
  color: #fff;
  padding: 12px 16px;
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
  width: 100%;
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
  margin-top: 10px;
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

.glass-summary h3 {
  font-size: 1.2rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 20px;
  padding-bottom: 15px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.summary-items {
  max-height: 300px;
  overflow-y: auto;
  margin-bottom: 20px;
  padding-right: 5px;
}

.summary-items::-webkit-scrollbar {
  width: 4px;
}

.summary-items::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.2);
  border-radius: 4px;
}

.summary-item {
  display: flex;
  gap: 15px;
  margin-bottom: 15px;
  align-items: center;
}

.si-img {
  width: 50px;
  height: 65px;
  border-radius: 8px;
  overflow: hidden;
  flex-shrink: 0;
  background: rgba(255, 255, 255, 0.05);
}

.si-img img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.si-info {
  flex: 1;
  min-width: 0;
}

.si-name {
  font-size: 0.9rem;
  font-weight: 600;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.si-var {
  font-size: 0.7rem;
  color: rgba(255, 255, 255, 0.5);
  margin-top: 4px;
}

.si-price {
  font-weight: 700;
  font-size: 0.95rem;
  flex-shrink: 0;
}

.summary-total {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 20px;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  font-size: 1.2rem;
  font-weight: 800;
}

.price-disclaimer {
  margin-top: 15px;
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.4);
  font-style: italic;
  text-align: center;
  line-height: 1.4;
}
</style>
