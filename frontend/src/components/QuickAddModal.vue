<script setup>
import { ref, computed, watch } from "vue";
import { useCartStore } from "../stores/cartStore";

const cartStore = useCartStore();

const selectedSize = ref("");
const selectedColor = ref("");
const quantity = ref(1);

// Initialize selections when product changes
watch(
  () => cartStore.quickAddProduct,
  (product) => {
    if (product) {
      if (product.availableSizes?.length > 0) {
        selectedSize.value = product.availableSizes[0];
      } else {
        selectedSize.value = "One Size";
      }
      if (product.colors?.length > 0) {
        selectedColor.value = product.colors[0].name;
      }
      quantity.value = 1;
    }
  },
  { immediate: true },
);

const displaySizes = computed(() => {
  const sizes = cartStore.quickAddProduct?.availableSizes;
  return sizes && sizes.length > 0 ? sizes : ["One Size"];
});

const currentImageUrl = computed(() => {
  const product = cartStore.quickAddProduct;
  if (!product) return "";
  if (product.colors && product.colors.length > 0) {
    const idx = product.colors.findIndex((c) => c.name === selectedColor.value);
    if (idx !== -1 && product.imageUrls && product.imageUrls[idx]) {
      return product.imageUrls[idx];
    }
  }
  return product.imageUrls && product.imageUrls[0];
});

const addToCart = () => {
  if (!cartStore.quickAddProduct) return;

  cartStore.addToCart(
    cartStore.quickAddProduct,
    selectedSize.value,
    selectedColor.value,
    quantity.value,
    currentImageUrl.value,
  );

  cartStore.closeQuickAdd();
};

const updateQuantity = (delta) => {
  const newVal = quantity.value + delta;
  if (newVal >= 1 && newVal <= 10) {
    quantity.value = newVal;
  }
};

// Handle body scroll locking
watch(
  () => cartStore.isQuickAddOpen,
  (isOpen) => {
    if (isOpen) {
      document.body.style.overflow = "hidden";
    } else {
      document.body.style.overflow = "";
    }
  },
);
</script>

<template>
  <transition name="modal-fade">
    <div
      v-if="cartStore.isQuickAddOpen"
      class="modal-overlay"
      @click.self="cartStore.closeQuickAdd"
    >
      <div class="modal-content">
        <button class="close-btn" @click="cartStore.closeQuickAdd">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <line x1="18" y1="6" x2="6" y2="18"></line>
            <line x1="6" y1="6" x2="18" y2="18"></line>
          </svg>
        </button>

        <div v-if="cartStore.quickAddProduct" class="modal-body">
          <div class="product-preview">
            <div class="image-wrapper">
              <img
                :src="currentImageUrl"
                :alt="cartStore.quickAddProduct.name"
              />
            </div>
          </div>

          <div class="product-info">
            <h3 class="product-name">{{ cartStore.quickAddProduct.name }}</h3>

            <div class="price-row">
              <span
                v-if="cartStore.quickAddProduct.onSale"
                class="price-original"
              >
                ${{ cartStore.quickAddProduct.originalPrice.toFixed(2) }}
              </span>
              <span
                class="price-current"
                :class="{ sale: cartStore.quickAddProduct.onSale }"
              >
                ${{ cartStore.quickAddProduct.price.toFixed(2) }}
              </span>
            </div>

            <!-- Color Selection -->
            <div
              v-if="cartStore.quickAddProduct.colors?.length > 0"
              class="selector-group"
            >
              <label
                >Color: <span>{{ selectedColor }}</span></label
              >
              <div class="color-options">
                <button
                  v-for="color in cartStore.quickAddProduct.colors"
                  :key="color.name"
                  class="color-circle"
                  :class="{ active: selectedColor === color.name }"
                  :style="{ backgroundColor: color.hex }"
                  @click="selectedColor = color.name"
                  :title="color.name"
                ></button>
              </div>
            </div>

            <!-- Size Selection -->
            <div class="selector-group">
              <label
                >Size: <span>{{ selectedSize }}</span></label
              >
              <div class="size-options">
                <button
                  v-for="size in displaySizes"
                  :key="size"
                  class="size-pill"
                  :class="{ active: selectedSize === size }"
                  @click="selectedSize = size"
                >
                  {{ size }}
                </button>
              </div>
            </div>

            <!-- Quantity & Actions -->
            <div class="action-row">
              <div class="qty-selector">
                <button @click="updateQuantity(-1)" :disabled="quantity <= 1">
                  −
                </button>
                <input type="number" v-model.number="quantity" readonly />
                <button @click="updateQuantity(1)" :disabled="quantity >= 10">
                  +
                </button>
              </div>
              <button class="submit-btn" @click="addToCart">Add to Bag</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </transition>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  -webkit-backdrop-filter: blur(8px);
  backdrop-filter: blur(8px);
  z-index: 10000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.modal-content {
  background: rgba(20, 20, 20, 0.85);
  -webkit-backdrop-filter: blur(20px) saturate(180%);
  backdrop-filter: blur(20px) saturate(180%);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 24px;
  width: 100%;
  max-width: 800px;
  position: relative;
  overflow: hidden;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
}

.close-btn {
  position: absolute;
  top: 20px;
  right: 20px;
  background: rgba(255, 255, 255, 0.05);
  border: none;
  color: #fff;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  z-index: 10;
  transition: all 0.2s;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.15);
  transform: rotate(90deg);
}

.modal-body {
  display: grid;
  grid-template-columns: 1fr 1fr;
  min-height: 480px;
}

@media (max-width: 650px) {
  .modal-body {
    grid-template-columns: 1fr;
  }
  .product-preview {
    height: 300px;
  }
}

.product-preview {
  background: rgba(255, 255, 255, 0.02);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.image-wrapper {
  width: 100%;
  height: 100%;
  border-radius: 12px;
  overflow: hidden;
}

.image-wrapper img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.product-info {
  padding: 40px;
  display: flex;
  flex-direction: column;
}

.product-name {
  font-family: "Playfair Display", serif;
  font-size: 1.8rem;
  font-weight: 700;
  color: #fff;
  margin: 0 0 12px;
}

.price-row {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 32px;
}

.price-current {
  font-size: 1.4rem;
  font-weight: 600;
  color: #fff;
}

.price-current.sale {
  color: #ff4d4d;
}

.price-original {
  font-size: 1.1rem;
  color: #666;
  text-decoration: line-through;
}

.selector-group {
  margin-bottom: 24px;
}

.selector-group label {
  display: block;
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  color: #666;
  margin-bottom: 12px;
  letter-spacing: 0.1em;
}

.selector-group label span {
  color: #fff;
  margin-left: 4px;
}

.color-options {
  display: flex;
  gap: 12px;
}

.color-circle {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  border: 1px solid rgba(255, 255, 255, 0.2);
  cursor: pointer;
  padding: 0;
  transition: all 0.2s;
}

.color-circle.active {
  border-color: #fff;
  transform: scale(1.15);
  box-shadow:
    0 0 0 2px rgba(255, 255, 255, 0.1),
    0 0 10px rgba(255, 255, 255, 0.2);
}

.size-options {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.size-pill {
  min-width: 44px;
  height: 36px;
  border-radius: 8px;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  color: #fff;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.size-pill.active {
  background: #fff;
  color: #000;
  border-color: #fff;
}

.action-row {
  margin-top: auto;
  display: flex;
  gap: 16px;
}

.qty-selector {
  display: flex;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 12px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  overflow: hidden;
}

.qty-selector button {
  width: 40px;
  height: 48px;
  background: transparent;
  border: none;
  color: #fff;
  font-size: 1.1rem;
  cursor: pointer;
}

.qty-selector button:disabled {
  opacity: 0.3;
}

.qty-selector input {
  width: 32px;
  border: none;
  background: transparent;
  color: #fff;
  text-align: center;
  font-size: 0.95rem;
  font-weight: 700;
}

.submit-btn {
  flex: 1;
  background: var(--accent);
  color: #fff;
  border: none;
  border-radius: 12px;
  padding: 0 24px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  cursor: pointer;
  transition: all 0.3s;
}

.submit-btn:hover {
  filter: brightness(1.1);
  transform: translateY(-2px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3);
}

/* Modal Fade Transition */
.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.3s ease;
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}

.modal-fade-enter-active .modal-content,
.modal-fade-leave-active .modal-content {
  transition: transform 0.4s cubic-bezier(0.22, 1, 0.36, 1);
}

.modal-fade-enter-from .modal-content {
  transform: scale(0.9) translateY(40px);
}

.modal-fade-leave-to .modal-content {
  transform: scale(0.95) translateY(20px);
}
</style>
