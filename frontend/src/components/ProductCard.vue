<script setup>
import { ref, computed } from "vue";
import { useCartStore } from "../stores/cartStore";

const props = defineProps({
  product: {
    type: Object,
    required: true,
  },
  activeCategoryIcon: {
    type: String,
    default: "",
  },
});

const cartStore = useCartStore();
const selectedColorIndex = ref(0);

const currentImageUrl = computed(() => {
  if (props.product.imageUrls && props.product.imageUrls.length > 0) {
    return props.product.imageUrls[
      Math.min(selectedColorIndex.value, props.product.imageUrls.length - 1)
    ];
  }
  return null;
});

const currentColorName = computed(() => {
  return props.product.colors[selectedColorIndex.value]?.name || "";
});

const placeholderGradient = computed(() => {
  const color =
    props.product.colors.length > 0
      ? props.product.colors[selectedColorIndex.value].hex
      : "#1a1a2e";
  return `linear-gradient(135deg, ${color}15, ${color}33)`;
});

const iconStyle = computed(() => {
  const hex = props.product.colors[selectedColorIndex.value]?.hex || "#ffffff";
  return {
    color: hex === "#ffffff" ? "#aaa" : hex,
    opacity: 0.5,
  };
});

const setProductColor = (idx) => {
  selectedColorIndex.value = idx;
};

const handleAddClick = () => {
  cartStore.openQuickAdd(props.product);
};
</script>

<template>
  <div
    class="product-card"
    :class="{
      'on-sale': product.onSale,
      'out-of-stock': product.stockQuantity === 0,
    }"
  >
    <RouterLink :to="`/product/${product.id}`" class="product-link">
      <div class="product-img-wrapper">
        <!-- Real image if available, gradient fallback otherwise -->
        <img
          v-if="currentImageUrl"
          :src="currentImageUrl"
          :alt="product.name"
          class="product-img"
        />
        <div
          v-else
          class="product-img-placeholder"
          :style="{ background: placeholderGradient }"
        >
          <span class="product-img-icon" :style="iconStyle">{{
            activeCategoryIcon
          }}</span>
        </div>

        <!-- Sale Badge with discount percent -->
        <span v-if="product.onSale" class="sale-badge">
          SALE<template v-if="product.discountPercent">
            –{{ product.discountPercent }}%</template
          >
        </span>

        <!-- Out of Stock Overlay -->
        <div v-if="product.stockQuantity === 0" class="out-of-stock-overlay">
          <span>Out of Stock</span>
        </div>
      </div>
    </RouterLink>

    <div class="product-info">
      <RouterLink :to="`/product/${product.id}`" class="product-name-link">
        <p class="product-name">
          {{ product.name }}
        </p>
      </RouterLink>

      <!-- Color Selection Row -->
      <div class="product-variants">
        <div class="color-options">
          <button
            v-for="(color, idx) in product.colors"
            :key="idx"
            class="color-square"
            :class="{ active: selectedColorIndex === idx }"
            :style="{ backgroundColor: color.hex }"
            @click.stop.prevent="setProductColor(idx)"
            :title="color.name"
          ></button>
        </div>
        <span v-if="product.colors.length > 0" class="selected-color-name">
          {{ currentColorName }}
        </span>
      </div>

      <div class="product-price-row">
        <span v-if="product.onSale" class="price-original"
          >${{ product.originalPrice.toFixed(2) }}</span
        >
        <span class="product-price" :class="{ 'sale-price': product.onSale }">
          ${{ product.price.toFixed(2) }}
        </span>
      </div>
    </div>
    <button
      class="product-add-btn"
      :disabled="product.stockQuantity === 0"
      @click.stop.prevent="handleAddClick"
    >
      {{ product.stockQuantity === 0 ? "Out of Stock" : "Add to bag" }}
    </button>
  </div>
</template>

<style scoped>
.product-card {
  position: relative;
  background: transparent;
  border: none;
  border-radius: 16px;
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.22, 1, 0.36, 1);
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 320px; 
  margin: 0 auto;
}

.product-card:hover {
  transform: translateY(-8px);
}

.product-link {
  text-decoration: none;
  display: block;
}

.product-img-wrapper {
  position: relative;
  width: 100%;
  aspect-ratio: 3/4;
  overflow: hidden;
  border-radius: 16px;
}

.product-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
  transition: transform 0.5s cubic-bezier(0.22, 1, 0.36, 1);
}

.product-card:hover .product-img {
  transform: scale(1.04);
}

.product-img-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--universal-tint);
  backdrop-filter: blur(var(--universal-blur));
  -webkit-backdrop-filter: blur(var(--universal-blur));
  border: 1px solid rgba(255, 255, 255, 0.08);
  transition: all 0.5s cubic-bezier(0.22, 1, 0.36, 1);
}

.product-card:hover .product-img-placeholder {
  transform: scale(1.03);
  border-color: rgba(255, 255, 255, 0.2);
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.5);
}

.product-img-icon {
  font-size: 3rem;
  opacity: 0.3;
}

.sale-badge {
  position: absolute;
  top: 12px;
  right: 12px;
  background: #ff4d4d;
  color: white;
  padding: 4px 10px;
  font-size: 0.65rem;
  font-weight: 800;
  border-radius: 6px;
  letter-spacing: 0.05em;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.3);
  z-index: 2;
}

.out-of-stock-overlay {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.55);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 3;
}

.out-of-stock-overlay span {
  color: #fff;
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.15em;
  text-transform: uppercase;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.25);
  padding: 0.4rem 1rem;
  border-radius: 20px;
  backdrop-filter: blur(6px);
}

.product-info {
  padding: 1rem 0.5rem 0;
  flex: 1;
}

.product-name-link {
  text-decoration: none;
}

.product-name {
  font-size: 0.9rem;
  color: #bbb;
  margin: 0 0 0.75rem;
  font-weight: 400;
  letter-spacing: 0.02em;
}

.product-variants {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.75rem;
}

.color-options {
  display: flex;
  gap: 6px;
}

.color-square {
  width: 14px;
  height: 14px;
  border-radius: 3px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  cursor: pointer;
  padding: 0;
  transition:
    transform 0.2s,
    border-color 0.2s;
}

.color-square:hover {
  transform: scale(1.2);
}

.color-square.active {
  border-color: #fff;
  outline: 1px solid #fff;
  outline-offset: 1px;
}

.selected-color-name {
  font-size: 0.7rem;
  color: #777;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.product-price-row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.product-price {
  font-family: inherit;
  font-size: 1.15rem;
  font-weight: 600;
  color: #fff;
  margin: 0;
}

.price-original {
  font-size: 0.9rem;
  color: #666;
  text-decoration: line-through;
}

.sale-price {
  color: #ff4d4d;
}

.product-add-btn {
  display: block;
  width: 100%;
  margin: 1.25rem 0 0.5rem;
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.1);
  color: #999;
  border-radius: 12px;
  padding: 0.75rem;
  font-size: 0.8rem;
  font-weight: 600;
  letter-spacing: 0.05em;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.22, 1, 0.36, 1);
}

.product-card:hover .product-add-btn {
  background: var(--accent);
  color: #000;
  border-color: transparent;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.4);
}

.product-add-btn:disabled {
  opacity: 0.45;
  cursor: not-allowed;
}

.product-add-btn:disabled:hover,
.product-card:hover .product-add-btn:disabled {
  background: rgba(255, 255, 255, 0.05);
  color: rgba(255, 255, 255, 0.4);
  border-color: rgba(255, 255, 255, 0.1);
  box-shadow: none;
}
</style>
