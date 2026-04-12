<script setup>
import { ref, onMounted, computed, watch } from "vue";
import { useRoute } from "vue-router";
import { useCartStore } from "../stores/cartStore";
import { getProduct, getProducts } from "../api/products.js";
import ProductCard from "../components/ProductCard.vue";

const props = defineProps({
  id: {
    type: [String, Number],
    required: true,
  },
});

const route = useRoute();
const cartStore = useCartStore();
const product = ref(null);
const relatedProducts = ref([]);
const isLoading = ref(true);
const isError = ref(false);
const activeImageIndex = ref(0);
const selectedSize = ref("");
const selectedColor = ref("");
const quantity = ref(1);

const displaySizes = computed(() => {
  const sizes = product.value?.availableSizes;
  return sizes && sizes.length > 0 ? sizes : ["One Size"];
});

const fetchProductData = async (productId) => {
  isLoading.value = true;
  isError.value = false;
  try {
    const data = await getProduct(productId);
    product.value = mapProduct(data);

    // Set defaults
    if (product.value.availableSizes?.length > 0) {
      selectedSize.value = product.value.availableSizes[0];
    } else {
      selectedSize.value = "One Size";
    }
    if (product.value.colors?.length > 0) {
      selectedColor.value = product.value.colors[0].name;
    }

    // Fetch related products
    fetchRelated(data.category, data.subCategory, productId);
  } catch (err) {
    console.error("Error fetching product:", err);
    isError.value = true;
    cartStore.triggerToast("Failed to load product details", "error");
  } finally {
    isLoading.value = false;
  }
};

const fetchRelated = async (category, subCategory, currentId) => {
  try {
    const data = await getProducts({ category, subcategory: subCategory });
    // Filter out current product and take first 4
    relatedProducts.value = (data.items || data)
      .filter((p) => p.id !== parseInt(currentId))
      .slice(0, 4)
      .map(mapProduct);
  } catch (err) {
    console.error("Error fetching related products:", err);
  }
};

const mapProduct = (p) => ({
  ...p,
  colors:
    p.availableColors?.map((c) => ({ name: c, hex: getColorHex(c) })) || [],
  price: p.displayPrice || p.price,
  originalPrice: p.price,
  onSale: p.isOnSale,
});

const getColorHex = (colorName) => {
  const colors = {
    White: "#ffffff",
    Black: "#1a1a1a",
    Navy: "#000080",
    Grey: "#808080",
    Olive: "#556b2f",
    Blue: "#0000ff",
    Pink: "#ffc0cb",
    Green: "#008000",
    Beige: "#f5f5dc",
    Brown: "#a52a2a",
    Gold: "#ffd700",
    Floral: "#ff69b4",
    Red: "#ff0000",
    Pastel: "#decba4",
    Yellow: "#ffd700",
    Multicolor: "#a78bfa",
    Khaki: "#c3b091",
    "Light Blue": "#add8e6",
    "Dark Wash": "#1a2a3a",
    "Medium Wash": "#4a6080",
    "Light Wash": "#a0b8cc",
  };
  return colors[colorName] || "#666666";
};

const updateQuantity = (delta) => {
  const newVal = quantity.value + delta;
  if (newVal >= 1 && newVal <= 10) {
    quantity.value = newVal;
  }
};

onMounted(() => {
  fetchProductData(props.id);
});

// Refetch if ID changes (e.g. from related products)
watch(
  () => props.id,
  (newId) => {
    fetchProductData(newId);
    activeImageIndex.value = 0;
    quantity.value = 1;
  },
);
</script>

<template>
  <div class="product-view">
    <div v-if="isLoading" class="loading-overlay">
      <div class="spinner"></div>
    </div>

    <div v-else-if="isError" class="error-container">
      <h2 class="serif">Product not found</h2>
      <router-link to="/" class="back-link">Return to Home</router-link>
    </div>

    <div v-else-if="product" class="pdp-container">
      <div class="pdp-layout">
        <div class="mobile-interaction-area">
          <!-- Left: Image Gallery -->
          <div class="gallery-section">
            <div class="main-image-container mobile-exact-size">
              <img
                :src="product.imageUrls[activeImageIndex]"
                :alt="product.name"
                class="main-image"
              />
            </div>
            <div
              v-if="product.imageUrls.length > 1"
              class="thumbnail-strip hide-on-mobile"
            >
              <button
                v-for="(img, idx) in product.imageUrls"
                :key="idx"
                class="thumb-btn"
                :class="{ active: activeImageIndex === idx }"
                @click="activeImageIndex = idx"
              >
                <img :src="img" alt="Thumbnail" />
              </button>
            </div>
          </div>

          <!-- Right: Product Details -->
          <div class="details-section">
            <div class="details-glass mobile-exact-size">
              <h1 class="serif">{{ product.name }}</h1>

              <div class="price-row">
                <span v-if="product.onSale" class="price-original"
                  >${{ product.originalPrice.toFixed(2) }}</span
                >
                <span class="price-current" :class="{ sale: product.onSale }">
                  ${{ product.price.toFixed(2) }}
                </span>
                <span v-if="product.onSale" class="discount-pill">
                  -{{ product.discountPercent }}%
                </span>
              </div>
              <div v-if="product.colors.length > 0" class="selector-group">
                <span class="selector-label"
                  >Color:
                  <span class="selected-val">{{ selectedColor }}</span></span
                >
                <div class="color-options">
                  <button
                    v-for="color in product.colors"
                    :key="color.name"
                    class="color-circle"
                    :class="{ active: selectedColor === color.name }"
                    :style="{ backgroundColor: color.hex }"
                    @click="selectedColor = color.name"
                    :title="color.name"
                  ></button>
                </div>
              </div>

              <!-- Size Selector -->
              <div class="selector-group">
                <span class="selector-label"
                  >Size:
                  <span class="selected-val">{{ selectedSize }}</span></span
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

              <!-- Quantity & Add to Cart -->
              <div class="action-row">
                <div class="quantity-selector">
                  <button @click="updateQuantity(-1)" :disabled="quantity <= 1">
                    -
                  </button>
                  <input
                    type="number"
                    v-model.number="quantity"
                    min="1"
                    max="10"
                    readonly
                  />
                  <button @click="updateQuantity(1)" :disabled="quantity >= 10">
                    +
                  </button>
                </div>
                <button
                  class="add-btn"
                  :disabled="product.stockQuantity === 0"
                  @click="cartStore.addToCart(product, selectedSize, selectedColor, quantity)"
                >
                  {{
                    product.stockQuantity === 0 ? "Out of Stock" : "Add to Bag"
                  }}
                </button>
              </div>

              <div class="extra-info">
                <p>✓ Free Standard Shipping</p>
                <p>✓ 30-Day Easy Returns</p>
              </div>
            </div>
          </div>
        </div>

        <!-- SKU & Description stationary below the entire interactive area -->
        <div class="details-footer">
          <h2 class="footer-title">Product Description</h2>
          <div class="description-text">
            <p>{{ product.description || "No description available yet." }}</p>
          </div>
          <p class="sku">SKU: {{ product.sku || "N/A" }}</p>
        </div>
      </div>

      <!-- Related Products Section -->
      <section v-if="relatedProducts.length > 0" class="related-section">
        <h2 class="serif section-title">You might also like</h2>
        <div class="related-grid">
          <ProductCard
            v-for="rel in relatedProducts"
            :key="rel.id"
            :product="rel"
          />
        </div>
      </section>
    </div>
  </div>
</template>

<style scoped>
.product-view {
  min-height: 100vh;
  padding-top: 120px;
  background: #0d0d0f;
  color: #e0e0e0;
}

.pdp-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 5vw 4rem;
  box-sizing: border-box;
}

.pdp-layout {
  display: grid;
  grid-template-columns: 0.85fr 1.15fr; 
  gap: clamp(2rem, 5vw, 4rem);
  align-items: start;
}

.mobile-interaction-area {
  display: contents; 
}

@media (max-width: 768px) {
  .pdp-container {
    padding: 0 1rem 4rem; 
    display: flex;
    flex-direction: column;
    align-items: center; 
    width: 100%;
  }

  .pdp-layout {
    display: flex;
    flex-direction: column;
    gap: 0;
    width: 100%;
    align-items: center; 
  }

  .mobile-interaction-area {
    display: flex;
    flex-direction: column;
    align-items: center;
    position: relative;
    width: 100%;
  }
}

/* Gallery Style */
.gallery-section {
  max-width: 600px;
  width: 100%;
}

@media (max-width: 768px) {
  .mobile-exact-size {
    --max-w: calc(100vw - 2rem);
    --max-h: 70vh;
    --ratio-w: calc(var(--max-h) * (3 / 4));
    --ratio-h: calc(var(--max-w) * (4 / 3));

    width: min(var(--max-w), var(--ratio-w));
    height: min(var(--max-h), var(--ratio-h));
    margin: 0 auto;
    flex-shrink: 0;
  }

  .gallery-section {
    position: sticky;
    top: 120px; 
    z-index: 1;
    width: 100%;
    padding: 0;
    display: flex;
    flex-direction: column;
    align-items: center;
  }

  .main-image-container {
    border-radius: 20px;
    border: none;
    overflow: hidden;
  }
}

.main-image-container {
  aspect-ratio: 3/4;
  border-radius: 20px;
  overflow: hidden;
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
}

.main-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}

.thumbnail-strip {
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
  overflow-x: auto;
  padding-bottom: 0.75rem;
  scrollbar-width: thin;
  scrollbar-color: rgba(255, 255, 254, 0.1) transparent;
}

.thumbnail-strip::-webkit-scrollbar {
  height: 4px;
}

.thumbnail-strip::-webkit-scrollbar-track {
  background: transparent;
}

.thumbnail-strip::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
}

.thumb-btn {
  width: 80px;
  height: 100px;
  flex-shrink: 0;
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid rgba(255, 255, 255, 0.1);
  padding: 0;
  cursor: pointer;
  background: none;
  opacity: 0.6;
  transition: all 0.2s ease;
}

.thumb-btn.active,
.thumb-btn:hover {
  opacity: 1;
  border-color: var(--accent);
}

.thumb-btn img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

/* Details Section */
.details-glass {
  background: color-mix(in srgb, var(--universal-tint), black 60%);
  backdrop-filter: blur(var(--universal-blur));
  -webkit-backdrop-filter: blur(var(--universal-blur));
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 24px;
  padding: 2.5rem;
}

@media (max-width: 768px) {
  .details-section {
    position: relative;
    z-index: 2;
    background: transparent;
    margin-top: 0;
    min-height: auto;
    padding-bottom: 0;
    width: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
  }

  .details-glass {
    position: sticky;
    top: 120px;

    border-radius: 20px;
    border: 1px solid rgba(255, 255, 255, 0.1);
    box-sizing: border-box;
    padding: 1rem;
    overflow-y: scroll;

    display: flex;
    flex-direction: column;
    justify-content: flex-start;
    gap: clamp(
      0.15rem,
      0.6vh,
      0.4rem
    ); 
  }

  .details-glass h1.serif {
    font-size: clamp(1rem, 3vh, 1.7em);
    margin: 0;
    line-height: 1.1;
    padding-bottom: 10px;
  }
  .price-row {
    margin-bottom: 0;
    gap: 1rem;
  }
  .price-current {
    font-size: clamp(0.7rem, 2.5vh, 1.3rem);
  }
  .selector-group {
    margin-bottom: 0;
  }
  .selector-label {
    margin-bottom: clamp(0.2rem, 1vh, 0.4rem);
    font-size: clamp(0.7rem, 2vh, 0.8rem);
  }
  .color-circle {
    width: clamp(20px, 3.5vh, 26px);
    height: clamp(20px, 3.5vh, 26px);
  }
  .color-options {
    gap: clamp(0.5rem, 1.5vw, 1rem);
  }
  .size-pill {
    min-width: clamp(35px, 9vw, 42px);
    height: clamp(30px, 4.5vh, 35px);
    font-size: clamp(0.7rem, 2vh, 0.8rem);
  }
  .size-options {
    gap: clamp(0.4rem, 1.2vw, 0.75rem);
  }
  .action-row {
    margin-top: 0; 
    gap: clamp(0.5rem, 2vw, 1rem);
  }
  .add-btn,
  .quantity-selector button {
    height: clamp(34px, 4.5vh, 40px);
  }
  .quantity-selector input {
    width: clamp(30px, 8vw, 38px);
    font-size: clamp(0.9rem, 2vh, 1rem);
  }
  .extra-info {
    margin-top: 0;
    padding-top: clamp(0.1rem, 0.5vh, 0.3rem);
    gap: 0;
  }
  .extra-info p {
    font-size: clamp(0.55rem, 1.4vh, 0.65rem);
    margin: 0;
    line-height: 1;
    color: #888;
  }
}

.serif {
  font-family: "Playfair Display", serif; 
  font-weight: 500;
}

h1.serif {
  font-size: 2.5rem;
  margin: 0 0 1rem;
  color: #fff;
}

.price-row {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  margin-bottom: 1.5rem;
}

.price-current {
  font-size: clamp(0.8rem, 3vh, 2.3rem);
  font-weight: 600;
  color: #fff;
}

.price-current.sale {
  color: #ff4d4d;
}

.price-original {
  font-size: 1.2rem;
  color: #9e9e9e;
  text-decoration: line-through;
}

.discount-pill {
  background: #ff4d4d;
  color: #fff;
  font-size: 0.8rem;
  font-weight: 700;
  padding: 0.3rem 0.7rem;
  border-radius: 4px;
}

.details-footer {
  grid-column: 1 / 2; 
  margin-top: 2.5rem;
  padding: 0 1rem;
}

.footer-title {
  font-family: inherit;
  font-size: 0.85rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  color: #666;
  margin-bottom: 1rem;
}

.description-text {
  font-size: 0.95rem;
  color: #aaa;
  line-height: 1.7;
  max-width: 60ch;
}

.details-footer .sku {
  margin-top: 1.5rem;
  font-size: 0.7rem;
  color: #444;
  text-transform: uppercase;
  letter-spacing: 0.1em;
}

.selector-group {
  margin-bottom: 2rem;
}

.selector-label {
  display: block;
  font-size: 0.85rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  color: #888;
  margin-bottom: 1rem;
}

.selected-val {
  color: #fff;
}

.color-options {
  display: flex;
  gap: 1rem;
}

.color-circle {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  border: 1px solid rgba(255, 255, 255, 0.2);
  cursor: pointer;
  padding: 0;
  transition: all 0.2s ease;
}

.color-circle.active {
  border-color: #fff;
  outline: 2px solid var(--accent);
  outline-offset: 2px;
  transform: scale(1.1);
  box-shadow:
    0 0 0 2px #000,
    0 0 0 4px #fff;
}

.size-options {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
}

.size-pill {
  min-width: 60px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  color: #fff;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s ease;
}

.size-pill:hover {
  background: rgba(255, 255, 255, 0.1);
}

.size-pill.active {
  background: #fff;
  color: #000;
  border-color: #fff;
}

.action-row {
  display: flex;
  gap: 1.5rem;
  margin-top: 0;
}

.quantity-selector {
  display: flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  overflow: hidden;
}

.quantity-selector button {
  width: 44px;
  height: 52px;
  background: none;
  border: none;
  color: #fff;
  font-size: 1.2rem;
  cursor: pointer;
}

.quantity-selector button:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.1);
}

.quantity-selector button:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}

.quantity-selector input {
  width: 40px;
  text-align: center;
  background: none;
  border: none;
  color: #fff;
  font-size: 1rem;
  font-weight: 600;
}

.add-btn {
  flex-grow: 1;
  height: 52px;
  background: var(--accent);
  border: none;
  border-radius: 12px;
  color: #fff;
  font-weight: 700;
  font-size: 1rem;
  letter-spacing: 0.05em;
  text-transform: uppercase;
  cursor: pointer;
  transition: all 0.3s ease;
}

.add-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  filter: brightness(1.2);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.4);
}

.add-btn:disabled {
  background: #333;
  color: #666;
  cursor: not-allowed;
}

.extra-info {
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.extra-info p {
  font-size: 0.8rem;
  color: #777;
}

/* Related Section */
.related-section {
  margin-top: 8rem;
}

.section-title {
  font-size: 2.2rem;
  margin-bottom: 3rem;
  text-align: center;
}

.related-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 2rem;
}

@media (max-width: 1200px) {
  .related-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

@media (max-width: 768px) {
  .related-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
  }
}

/* States */
.loading-overlay {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 60vh;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid rgba(255, 255, 255, 0.1);
  border-top-color: var(--accent);
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.error-container {
  text-align: center;
  padding: 5rem 0;
}

.back-link {
  color: var(--accent);
  text-decoration: none;
  font-weight: 600;
  margin-top: 1rem;
  display: inline-block;
}
.hide-on-mobile {
  @media (max-width: 768px) {
    display: none;
  }
}

.show-on-mobile {
  @media (min-width: 769px) {
    display: none;
  }
}

@media (max-width: 768px) {
  .footer-title,
  .description-text,
  .details-footer .sku {
    padding: 0 1rem;
  }

  .details-footer {
    padding-bottom: 3rem;
  }
}
</style>
