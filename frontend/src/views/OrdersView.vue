<script setup>
import { ref, onMounted } from 'vue';
import { useAuthStore } from '../stores/authStore';
import { useCartStore } from '../stores/cartStore';
import { BASE_URL } from '../api/client.js';

const authStore = useAuthStore();
const cartStore = useCartStore();

const orders = ref([]);
const isLoaded = ref(false);
const isCancelling = ref(false);

const cancelOrder = async (orderId) => {
  if (!confirm("Are you sure you want to cancel this order?")) return;
  
  isCancelling.value = true;
  try {
    const res = await authStore.fetchWithAuth(`${BASE_URL}/api/orders/${orderId}`, {
      method: 'DELETE'
    });
    
    if (res.ok) {
      cartStore.triggerToast('Order cancelled successfully.', 'success');
      orders.value = orders.value.filter(o => o.id !== orderId);
    } else {
      const data = await res.json().catch(() => ({}));
      throw new Error(data.message || 'Failed to cancel order.');
    }
  } catch (err) {
    cartStore.triggerToast(err.message, 'error');
  } finally {
    isCancelling.value = false;
  }
};

const loadOrders = async () => {
  try {
    const res = await authStore.fetchWithAuth(`${BASE_URL}/api/orders/my-orders`);
    if (res.ok) {
      const rawData = await res.json();
      console.log("Raw API Response for Orders:", rawData);
      
      // If the backend wraps the array in a data object (e.g. { data: [...] }), unwrap it here:
      if (rawData && rawData.data) {
        orders.value = rawData.data;
      } else {
        orders.value = rawData;
      }
    }
  } catch (err) {
    cartStore.triggerToast('Failed to load order history.', 'error');
  } finally {
    isLoaded.value = true;
  }
};

const formatDate = (dateStr) => {
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
};

const getPrimaryImage = (product) => {
  if (!product || !product.imageUrls) return '';
  if (Array.isArray(product.imageUrls)) return product.imageUrls[0] || '';
  try {
    const arr = JSON.parse(product.imageUrls);
    return arr[0] || '';
  } catch (e) {
    return '';
  }
};

onMounted(() => {
  loadOrders();
});
</script>

<template>
  <div class="glass-form-container">
    <h1 class="page-title">Order History</h1>
    
    <div v-if="!isLoaded" class="loading">Loading...</div>
    
    <div v-else-if="orders.length === 0" class="empty-state">
      <div class="icon-circle">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M6 2L3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4z"></path><line x1="3" y1="6" x2="21" y2="6"></line><path d="M16 10a4 4 0 0 1-8 0"></path></svg>
      </div>
      <p>You haven't placed any orders yet.</p>
      <router-link to="/" class="shop-btn">Continue Shopping</router-link>
    </div>

    <div v-else class="orders-list">
      <div v-for="order in orders" :key="order.id" class="glass-card order-card">
        <div class="order-header">
          <div class="order-info">
            <span class="order-id">#{{ order.id }}</span>
            <span class="order-date">{{ formatDate(order.orderDate) }}</span>
          </div>
          <div class="order-total-block">
            <div class="order-total">
              ${{ order.totalAmount.toFixed(2) }}
            </div>
            <button class="cancel-btn" @click="cancelOrder(order.id)" :disabled="isCancelling">
              Cancel Order
            </button>
          </div>
        </div>
        <div class="order-shipping">
          <div class="shipping-section-title">Shipping To</div>
          <div class="shipping-details">
            <strong>{{ order.name }}</strong><br/>
            {{ order.address }}<br/>
            {{ order.city }}, {{ order.zip }}<br/>
            <span class="shipping-email">{{ order.email }}</span>
          </div>
        </div>

        <div class="order-items">
          <div v-for="item in order.items" :key="item.id" class="item-row">
            <div class="item-info">
              <router-link :to="`/product/${item.productId}`" class="item-img-container">
                <img v-if="getPrimaryImage(item.product)" :src="getPrimaryImage(item.product)" :alt="item.product?.name" />
                <div v-else class="img-placeholder"></div>
              </router-link>
              <div class="item-details">
                <router-link :to="`/product/${item.productId}`" class="item-name-link">
                  <p class="item-name">{{ item.product ? item.product.name : `Product ID: ${item.productId}` }}</p>
                </router-link>
                <span class="item-var">{{ item.selectedColor }} / {{ item.selectedSize }}</span>
              </div>
            </div>
            <div class="item-price">
              {{ item.quantity }} × ${{ item.unitPrice.toFixed(2) }}
            </div>
          </div>
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

@media (max-width: 600px) {
  .page-title {
    font-size: 1.4rem;
    text-align: center;
  }
}

.loading {
  opacity: 0.5;
  padding: 20px 0;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  background: rgba(255, 255, 255, 0.03);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 20px;
  text-align: center;
}

.icon-circle {
  width: 70px;
  height: 70px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 20px;
  color: rgba(255, 255, 255, 0.8);
}

.shop-btn {
  margin-top: 20px;
  padding: 12px 24px;
  background: #fff;
  color: #000;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  text-decoration: none;
  border-radius: 12px;
  transition: all 0.3s;
}

.shop-btn:hover {
  background: #eee;
  transform: translateY(-2px);
}

.orders-list {
  display: flex;
  flex-direction: column;
  gap: 25px;
}

.order-card {
  background: rgba(255, 255, 255, 0.03);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 20px;
  overflow: hidden;
}

.order-header {
  padding: 20px;
  background: rgba(255, 255, 255, 0.03);
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

@media (max-width: 600px) {
  .order-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 15px;
    padding: 15px;
  }
}

.order-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.order-id {
  font-weight: 800;
  font-size: 1.1rem;
}

.order-date {
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.5);
}

.order-total-block {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 10px;
}

@media (max-width: 600px) {
  .order-total-block {
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
    width: 100%;
  }
}

.order-total {
  font-size: 1.2rem;
  font-weight: 800;
}

.cancel-btn {
  background: transparent;
  color: #ff6b6b;
  border: 1px solid #ff6b6b;
  border-radius: 8px;
  padding: 6px 12px;
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  cursor: pointer;
  transition: all 0.3s;
}

.cancel-btn:hover:not(:disabled) {
  background: rgba(255, 107, 107, 0.1);
}

.cancel-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.order-shipping {
  padding: 20px;
  background: rgba(0, 0, 0, 0.2);
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
}

@media (max-width: 600px) {
  .order-shipping {
    padding: 15px;
  }
}

.shipping-section-title {
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  color: rgba(255, 255, 255, 0.5);
  margin-bottom: 8px;
}

.shipping-details {
  font-size: 0.9rem;
  line-height: 1.5;
  color: rgba(255, 255, 255, 0.8);
}

.shipping-details strong {
  color: #fff;
}

.shipping-email {
  color: rgba(255, 255, 255, 0.5);
  word-break: break-all;
}

.order-items {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

@media (max-width: 600px) {
  .order-items {
    padding: 15px;
  }
}

.item-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 15px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  gap: 15px;
}

.item-row:last-child {
  border-bottom: none;
  padding-bottom: 0;
}

@media (max-width: 500px) {
  .item-row {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .item-price {
    align-self: flex-end;
  }
}

.item-info {
  display: flex;
  gap: 15px;
  align-items: center;
  min-width: 0;
}

.item-img-container {
  width: 60px;
  height: 80px;
  border-radius: 8px;
  overflow: hidden;
  flex-shrink: 0;
  background: rgba(255, 255, 255, 0.05);
  display: block;
}

.item-img-container img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s;
}

.item-img-container:hover img {
  transform: scale(1.05);
}

.img-placeholder {
  width: 100%;
  height: 100%;
  background: rgba(255, 255, 255, 0.1);
}

.item-details {
  min-width: 0;
}

.item-name-link {
  text-decoration: none;
  color: #fff;
  display: block;
}

.item-name-link:hover .item-name {
  color: var(--accent, #ccc);
}

.item-name {
  font-weight: 600;
  margin-bottom: 4px;
  font-size: 1rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.item-var {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.5);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.item-price {
  font-weight: 600;
  font-size: 0.95rem;
  flex-shrink: 0;
}
</style>
