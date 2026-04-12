<script setup>
import { useCartStore } from "../stores/cartStore";
import { useAuthStore } from "../stores/authStore";
import { useRouter } from "vue-router";

const authStore = useAuthStore();
const cartStore = useCartStore();
const router = useRouter();

const emit = defineEmits(["close"]);

const handleCheckout = () => {
  emit("close");
  if (authStore.isAuthenticated) {
    router.push('/checkout');
  } else {
    router.push('/checkout-gate');
  }
};
</script>

<template>
  <div class="cart-drawer">
    <div v-if="cartStore.items.length === 0" class="empty-state">
      <div class="empty-icon">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="24"
          height="24"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="1.5"
          stroke-linecap="round"
          stroke-linejoin="round"
        >
          <path d="M6 2L3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4z"></path>
          <line x1="3" y1="6" x2="21" y2="6"></line>
          <path d="M16 10a4 4 0 0 1-8 0"></path>
        </svg>
      </div>
      <p>Empty</p>
    </div>

    <div v-else class="cart-items">
      <div
        v-for="item in cartStore.items"
        :key="`${item.productId}-${item.selectedColor}-${item.selectedSize}`"
        class="cart-item"
      >
        <router-link
          :to="`/product/${item.productId}`"
          class="item-link"
          @click="emit('close')"
        >
          <div class="item-img">
            <img :src="item.imageUrl" :alt="item.name" />
          </div>
        </router-link>

        <div class="item-main">
          <router-link
            :to="`/product/${item.productId}`"
            class="item-link"
            @click="emit('close')"
          >
            <div class="name-marquee-container">
              <div
                class="name-scroller"
                :style="{
                  animationDuration: item.name.length > 20 ? '8s' : '0s',
                }"
              >
                {{ item.name }}
              </div>
            </div>
          </router-link>

          <div class="item-variant">
            {{ item.selectedColor }} / {{ item.selectedSize }}
          </div>

          <div class="item-actions">
            <div class="qty-control">
              <button
                @click="
                  cartStore.updateQuantity(
                    item.productId,
                    item.selectedSize,
                    item.selectedColor,
                    item.quantity - 1,
                  )
                "
                :disabled="item.quantity <= 1"
              >
                −
              </button>
              <span class="qty-num">{{ item.quantity }}</span>
              <button
                @click="
                  cartStore.updateQuantity(
                    item.productId,
                    item.selectedSize,
                    item.selectedColor,
                    item.quantity + 1,
                  )
                "
                :disabled="item.quantity >= 10"
              >
                +
              </button>
            </div>

            <div class="item-price">
              ${{ ((item.salePrice || item.price) * item.quantity).toFixed(2) }}
            </div>
          </div>

          <!-- Remove absolute button -->
          <button
            class="item-remove-btn"
            @click="
              cartStore.removeFromCart(
                item.productId,
                item.selectedSize,
                item.selectedColor,
              )
            "
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="12"
              height="12"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="3"
              stroke-linecap="round"
              stroke-linejoin="round"
            >
              <line x1="18" y1="6" x2="6" y2="18"></line>
              <line x1="6" y1="6" x2="18" y2="18"></line>
            </svg>
          </button>
        </div>
      </div>
    </div>

    <!-- Footer -->
    <div v-if="cartStore.items.length > 0" class="cart-footer">
      <div class="total-row">
        <span class="total-label">Subtotal</span>
        <span class="total-val">${{ cartStore.cartTotal.toFixed(2) }}</span>
      </div>
      <button class="checkout-btn" @click="handleCheckout">
        Secure Checkout
      </button>
    </div>
  </div>
</template>

<style scoped>
.cart-drawer {
  display: flex;
  flex-direction: column;
  width: 100%;
  overflow-x: hidden;
}

.empty-state {
  padding: 30px 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  opacity: 0.3;
}

.empty-state p {
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  margin-top: 8px;
}

.cart-items {
  max-height: 400px;
  overflow-y: auto;
  scrollbar-width: none;
}

@media (max-height: 700px) {
  .cart-items {
    max-height: calc(100vh - 260px);
  }
}

@media (max-height: 500px) {
  .cart-items {
    max-height: 100px;
  }
  .cart-item {
    padding: 8px 12px;
  }
}

.cart-items::-webkit-scrollbar {
  display: none;
}

.cart-item {
  position: relative;
  display: flex;
  gap: 12px;
  padding: 12px 16px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.03);
  align-items: center;
}

.item-img {
  width: 54px;
  height: 72px;
  border-radius: 8px;
  overflow: hidden;
  flex-shrink: 0;
  background: rgba(255, 255, 255, 0.05);
}

.item-img img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.item-main {
  flex: 1;
  min-width: 0; 
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.item-link {
  text-decoration: none;
  display: block;
}

.item-link:hover .name-scroller {
  color: var(--accent, #fff);
}

/* MARQUEE EFFECT */
.name-marquee-container {
  width: 100%;
  overflow: hidden;
  white-space: nowrap;
  mask-image: linear-gradient(to right, black 85%, transparent 100%);
  -webkit-mask-image: linear-gradient(to right, black 85%, transparent 100%);
}

.name-scroller {
  display: inline-block;
  font-size: 0.82rem;
  font-weight: 600;
  color: #fff;
  padding-right: 20px;
}


.name-scroller {
  animation: marquee linear infinite;
  animation-play-state: running;
}

@keyframes marquee {
  from {
    transform: translateX(0);
  }
  to {
    transform: translateX(-50%);
  }
}

.item-variant {
  font-size: 0.65rem;
  color: #888;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-top: -2px;
}

.item-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 6px;
}

.qty-control {
  display: flex;
  align-items: center;
  gap: 8px;
  background: rgba(255, 255, 255, 0.06);
  padding: 4px 10px;
  border-radius: 6px;
}

.qty-control button {
  background: transparent;
  border: none;
  color: #fff;
  font-size: 0.9rem;
  cursor: pointer;
  padding: 0 4px;
  opacity: 0.6;
  transition: opacity 0.2s;
}

.qty-control button:hover:not(:disabled) {
  opacity: 1;
}

.qty-num {
  font-size: 0.75rem;
  font-weight: 600;
  min-width: 14px;
  text-align: center;
}

.item-price {
  font-size: 0.9rem;
  font-weight: 800;
  color: #fff;
}

.item-remove-btn {
  position: absolute;
  top: 8px;
  right: 8px;
  background: transparent;
  border: none;
  color: rgba(255, 255, 255, 0.2);
  cursor: pointer;
  padding: 4px;
  transition: color 0.2s;
  opacity: 0; 
}

.cart-item:hover .item-remove-btn {
  opacity: 1;
}

@media (hover: none) {
  .item-remove-btn {
    opacity: 0.8;
  }
}

.item-remove-btn:hover {
  color: #ff4d4d;
}

.cart-footer {
  padding: 12px 16px;

  background: rgba(255, 255, 255, 0.02);
  border-top: 1px solid rgba(255, 255, 255, 0.05);
}

@media (max-height: 500px) {
  .cart-footer {
    padding: 8px 16px;
  }
  .total-row {
    margin-bottom: 4px;
  }
}

.total-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.total-label {
  font-size: 0.75rem;
  color: #666;
  text-transform: uppercase;
  letter-spacing: 0.1em;
}

.total-val {
  font-size: 1.1rem;
  font-weight: 700;
  color: #fff;
}

.checkout-btn {
  width: 100%;
  background: #fff;
  color: #000;
  border: none;
  border-radius: 10px;
  padding: 10px;

  font-size: 0.75rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  cursor: pointer;
  transition: all 0.3s;
}

.checkout-btn:hover {
  background: var(--accent);
  color: #fff;
  transform: translateY(-2px);
}
@media (max-width: 480px) {
  .cart-item {
    padding: 10px 12px;
    gap: 10px;
  }

  .item-img {
    width: 48px;
    height: 64px;
  }

  .name-scroller {
    font-size: 0.78rem;
  }

  .qty-control {
    padding: 3px 8px;
    gap: 6px;
  }

  .item-price {
    font-size: 0.85rem;
  }
}
</style>
