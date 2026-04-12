<script setup>
import { ref, computed, onMounted, onUnmounted, watch } from "vue";
import { getProducts } from "../api/products.js";
import ProductCard from "../components/ProductCard.vue";
import ProductCarousel from "../components/ProductCarousel.vue";

// Array of hero slides
const heroSlides = ref([
  {
    id: 1,
    imageUrl: "/hero_man.png",
    subtitle: "LATEST COLLECTION: DARK URBAN",
    buttonText: "STEAL THE LOOK",
    buttonLink: "/men",
  },
  {
    id: 2,
    imageUrl: "/hero_woman.png",
    subtitle: "WOMEN: SPRING ELEGANCE",
    buttonText: "SHOP WOMEN",
    buttonLink: "/women",
  },
  {
    id: 3,
    imageUrl: "/promo_banner.png",
    subtitle: "SUMMER SALE: UP TO 50% OFF",
    buttonText: "SHOP SALE",
    buttonLink: "/sale",
  },
]);

const currentSlideIndex = ref(0);

const currentHero = computed(() => {
  return heroSlides.value[currentSlideIndex.value];
});

let slideInterval;

const resetSlideInterval = () => {
  if (slideInterval) {
    clearInterval(slideInterval);
  }
  slideInterval = setInterval(nextSlide, 10000);
};

const nextSlide = () => {
  currentSlideIndex.value =
    (currentSlideIndex.value + 1) % heroSlides.value.length;
  resetSlideInterval();
};

const prevSlide = () => {
  currentSlideIndex.value =
    (currentSlideIndex.value - 1 + heroSlides.value.length) %
    heroSlides.value.length;
  resetSlideInterval();
};

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
    Emerald: "#50c878",
    Lavender: "#e6e6fa",
    Champagne: "#f7e7ce",
    Silver: "#c0c0c0",
    Mint: "#98ff98",
    Neon: "#ccff00",
    Tortoise: "#734822",
  };
  return colors[colorName] || "#666666";
};

const mapProduct = (p) => ({
  id: p.id,
  name: p.name,
  price: p.displayPrice,
  originalPrice: p.price,
  onSale: p.isOnSale,
  discountPercent: p.discountPercent,
  stockQuantity: p.stockQuantity,
  description: p.description,
  imageUrls: p.imageUrls,
  availableSizes: p.availableSizes,
  colors: p.availableColors.map((c) => ({ name: c, hex: getColorHex(c) })),
});

const novelties = ref([]);
const saleItems = ref([]);
const outfitItems = ref([]);
const isLoading = ref(true);
const errorMsg = ref(null);

const activeOutfitTab = ref("Women");

const outfitConfigs = {
  Women: {
    skus: ["WOM-TOP-002", "WOM-PNT-002"],
    image: "/outfit_inspiration_1775695622732.png",
    title: "The Silk & Structure Look",
    desc: "Minimalist chic meets ultimate comfort.",
  },
  Men: {
    skus: ["MEN-SHT-001", "MEN-JNS-001", "MEN-ACC-001"],
    image: "/men_outfit_inspiration_1775698951495.png",
    title: "The Urban Classic Look",
    desc: "Effortless style with a perfect fit.",
  },
  Kids: {
    skus: ["KID-BOY-001", "KID-SHO-001"],
    image: "/kids_outfit_inspiration_1775698969925.png",
    title: "The Playground Ready Look",
    desc: "Durable, comfortable, and stylish.",
  },
};

const currentOutfit = computed(() => outfitConfigs[activeOutfitTab.value]);

watch(activeOutfitTab, async (newTab) => {
  outfitItems.value = [];
  try {
    const data = await getProducts({ skus: outfitConfigs[newTab].skus });
    outfitItems.value = (data.items || data).map(mapProduct);
  } catch (error) {
    console.error("Failed to load outfit items:", error);
    errorMsg.value = "Failed to load seasonal outfits. Please try again later.";
  }
});

onMounted(async () => {
  slideInterval = setInterval(nextSlide, 10000); 

  try {
    const [novData, saleData, outfitData] = await Promise.all([
      getProducts({ sort: "newest", limit: 10 }),
      getProducts({ onSale: true, limit: 10 }),
      getProducts({ skus: currentOutfit.value.skus }),
    ]);

    novelties.value = (novData.items || novData).map(mapProduct);
    saleItems.value = (saleData.items || saleData).map(mapProduct);
    outfitItems.value = (outfitData.items || outfitData).map(mapProduct);
  } catch (error) {
    console.error("Failed to load home page products:", error);
    errorMsg.value =
      "We're having trouble loading products right now. Please try again later.";
  } finally {
    isLoading.value = false;
  }
});

onUnmounted(() => {
  if (slideInterval) {
    clearInterval(slideInterval);
  }
});
</script>

<template>
  <div class="home-wrapper">
    <!-- Global API Error State -->
    <div v-if="errorMsg" class="global-error-state">
      <div class="error-glass">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="32"
          height="32"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
          class="error-icon"
        >
          <circle cx="12" cy="12" r="10"></circle>
          <line x1="12" y1="8" x2="12" y2="12"></line>
          <line x1="12" y1="16" x2="12.01" y2="16"></line>
        </svg>
        <p>{{ errorMsg }}</p>
        <button @click="window.location.reload()" class="retry-btn">
          Retry Connection
        </button>
      </div>
    </div>
    <!-- Hero Section -->
    <div class="hero-section">
      <transition-group
        name="hero-fade"
        tag="div"
        class="hero-images-container"
      >
        <img
          v-for="slide in heroSlides"
          :key="slide.id"
          v-show="slide.id === currentHero.id"
          class="hero-bg"
          :src="slide.imageUrl"
          alt="Hero Image"
        />
      </transition-group>
      <div class="hero-overlay"></div>
      <div class="hero-content">
        <div class="hero-action-area">
          <transition name="hero-content-fade" mode="out-in">
            <p :key="'sub-' + currentHero.id" class="hero-subtitle">
              {{ currentHero.subtitle }}
            </p>
          </transition>

          <router-link :to="currentHero.buttonLink" class="hero-btn">
            <transition name="hero-content-fade" mode="out-in">
              <span :key="'btn-' + currentHero.id">{{
                currentHero.buttonText
              }}</span>
            </transition>
          </router-link>
        </div>
      </div>

      <!-- Navigation Arrows -->
      <button
        class="hero-nav-btn prev"
        @click="prevSlide"
        aria-label="Previous Slide"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="24"
          height="24"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
          class="feather feather-chevron-left"
        >
          <polyline points="15 18 9 12 15 6"></polyline>
        </svg>
      </button>
      <button
        class="hero-nav-btn next"
        @click="nextSlide"
        aria-label="Next Slide"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="24"
          height="24"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
          class="feather feather-chevron-right"
        >
          <polyline points="9 18 15 12 9 6"></polyline>
        </svg>
      </button>
    </div>

    <!-- Scrollable content below hero -->
    <div class="scroll-content">
      <!-- Novelties Section -->
      <section class="home-section novelties-section">
        <div class="content-container">
          <div class="section-header">
            <h2 class="section-title">Novelties</h2>
            <p class="section-desc">
              Fresh off the line. Discover our latest curated pieces for the
              season.
            </p>
            <router-link to="/novelties" class="section-link"
              >View All Novelties</router-link
            >
          </div>
          <ProductCarousel :products="novelties" />
        </div>
      </section>

      <!-- Composed Outfit Section -->
      <section class="home-section outfit-section">
        <div class="content-container">
          <div class="outfit-header-row">
            <div>
              <h2 class="section-title">Shop the Look</h2>
              <p class="section-desc">
                Recreate this premium editorial look with carefully paired
                pieces.
              </p>
            </div>
            <div class="outfit-tabs">
              <button
                :class="{ active: activeOutfitTab === 'Women' }"
                @click="activeOutfitTab = 'Women'"
              >
                Women
              </button>
              <button
                :class="{ active: activeOutfitTab === 'Men' }"
                @click="activeOutfitTab = 'Men'"
              >
                Men
              </button>
              <button
                :class="{ active: activeOutfitTab === 'Kids' }"
                @click="activeOutfitTab = 'Kids'"
              >
                Kids
              </button>
            </div>
          </div>

          <div class="outfit-wrapper">
            <div class="outfit-image-container">
              <Transition name="fade-slide-outfit" mode="out-in">
                <div :key="activeOutfitTab" class="outfit-image-content">
                  <img
                    :src="currentOutfit.image"
                    :alt="currentOutfit.title"
                    class="outfit-image"
                  />
                  <div class="outfit-image-overlay">
                    <h3>{{ currentOutfit.title }}</h3>
                    <p>{{ currentOutfit.desc }}</p>
                  </div>
                </div>
              </Transition>
            </div>
            <div class="outfit-products">
              <!-- Added unique key and transition for smooth product swapping -->
              <Transition name="fade-slide-outfit" mode="out-in">
                <ProductCarousel
                  :products="outfitItems"
                  v-if="outfitItems.length > 0"
                  :key="activeOutfitTab"
                />
              </Transition>
            </div>
          </div>
        </div>
      </section>

      <!-- Sale Section -->
      <section class="home-section sale-section">
        <div class="content-container">
          <div class="section-header">
            <h2 class="section-title">Summer Sale</h2>
            <p class="section-desc">
              Limited time offers on our most sought-after styles.
            </p>
            <router-link to="/sale" class="section-link"
              >Shop All Sale</router-link
            >
          </div>
          <ProductCarousel :products="saleItems" />
        </div>
      </section>
    </div>
  </div>
</template>

<style scoped>
.home-wrapper {
  position: relative;
  width: 100%;
}

.global-error-state {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100vh;
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.8);
  backdrop-filter: blur(8px);
  padding: 20px;
}

.error-glass {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(16px);
  padding: 40px;
  border-radius: 24px;
  text-align: center;
  max-width: 400px;
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.5);
}

.error-icon {
  color: #ff4d4d;
  margin-bottom: 20px;
}

.error-glass p {
  font-size: 1.1rem;
  line-height: 1.6;
  margin-bottom: 24px;
  color: rgba(255, 255, 255, 0.8);
}

.retry-btn {
  background: #fff;
  color: #000;
  border: none;
  border-radius: 12px;
  padding: 12px 24px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  cursor: pointer;
  transition: all 0.3s;
}

.retry-btn:hover {
  background: #eee;
  transform: translateY(-2px);
}

.hero-section {
  position: relative;
  width: 100%;
  height: 100vh;
  display: flex;
  align-items: flex-end;
  justify-content: flex-start;
  padding: 4rem 4rem 6rem 8rem; 
  box-sizing: border-box;
  overflow: hidden;
}

.hero-bg {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: top center; 
}

.hero-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    to top right,
    rgba(0, 0, 0, 0.5) 0%,
    rgba(0, 0, 0, 0.1) 40%,
    transparent 100%
  );
}

.hero-content {
  position: relative;
  width: 100%;
}

.hero-action-area {
  margin-bottom: 2rem;
  max-width: 700px;
}

.hero-subtitle {
  color: #fff;
  font-family: "Playfair Display", serif;
  font-size: clamp(2.5rem, 6vw, 4.5rem);
  line-height: 0.95;
  letter-spacing: -0.04em;
  margin-bottom: 2rem;
  text-transform: uppercase;
  font-weight: 700;
  text-shadow: 0 10px 30px rgba(0, 0, 0, 0.5);
}

.hero-btn {
  position: relative;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  color: #fff;
  padding: 1.1rem 3.5rem;
  font-size: 0.8rem;
  font-weight: 700;
  text-decoration: none;
  border-radius: 50px;
  min-width: 180px; 
  letter-spacing: 0.2em;
  text-transform: uppercase;
  overflow: hidden; 
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition:
    border-color 0.4s ease,
    transform 0.4s ease,
    box-shadow 0.4s ease;
  z-index: 1; 
}

/* Pseudo-element strictly for the blurred background */
.hero-btn::before {
  content: "";
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.4) !important;
  backdrop-filter: blur(12px) !important;
  -webkit-backdrop-filter: blur(12px) !important;
  width: 100%;
  height: 100%;
  z-index: -1;
  border-radius: inherit;
  transition: background-color 0.4s ease;
}

.hero-btn span {
  position: relative;
  z-index: 2;
}

.hero-btn:hover {
  border-color: rgba(255, 255, 255, 0.6);
  transform: translateY(-4px) scale(1.02);
  box-shadow: 0 15px 40px rgba(0, 0, 0, 0.4);
}

.hero-btn:hover::before {
  background: rgba(255, 255, 255, 0.15) !important;
}

@media (max-width: 768px) {
  .hero-section {
    padding: 2rem 2rem 4rem max(2rem, env(safe-area-inset-left));
    align-items: flex-end;
  }

  .hero-subtitle {
    font-size: clamp(2rem, 10vw, 3rem);
    margin-bottom: 1.5rem;
  }

  .hero-btn {
    padding: 0.9rem 2.5rem;
    font-size: 0.75rem;
  }

  .hero-nav-btn {
    width: 42px;
    height: 42px;
  }

  .hero-nav-btn.prev {
    left: 1rem;
  }

  .hero-nav-btn.next {
    right: 1rem;
  }

  .hero-action-area {
    max-width: 100%;
  }
}

/* Landscape-specific optimization for small screens (iPhone Horizontal) */
@media (orientation: landscape) and (max-height: 500px) {
  .hero-section {
    padding: 2rem 2rem 2rem max(4rem, env(safe-area-inset-left));
  }

  .hero-subtitle {
    font-size: 2.2rem;
    margin-bottom: 1.2rem;
  }

  .hero-btn {
    padding: 0.7rem 2rem;
    font-size: 0.7rem;
  }

  
  .hero-nav-btn {
    width: 38px;
    height: 38px;
    top: auto;
    bottom: 2rem;
    transform: none; 
  }

  .hero-nav-btn.prev {
    left: 1rem;
  }

  .hero-nav-btn.next {
    right: 1rem;
  }

  .hero-nav-btn:hover {
    transform: scale(1.1);
  }

  .hero-action-area {
    margin-bottom: 0.5rem;
  }
}

.hero-btn:hover {
  background: rgba(255, 255, 255, 0.15) !important;
  border-color: rgba(255, 255, 255, 0.6);
  transform: translateY(-4px) scale(1.02);
  box-shadow: 0 15px 40px rgba(0, 0, 0, 0.4);
}

.scroll-content {
  position: relative;
  z-index: 10;
  background: #121212; 
  min-height: 150vh;
  padding: 6rem 0;
}

.content-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 2rem;
}

.section-title {
  font-family: "Playfair Display", serif;
  font-size: 3rem;
  margin-bottom: 1rem;
  color: #fff;
}

.section-desc {
  color: #a0a0a0;
  font-size: 1.1rem;
  margin-bottom: 4rem;
}

.product-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
}

.product-item {
  background: #1e1e1e;
  height: 400px;
  border-radius: 12px;
  transition: transform 0.3s ease;
}

.product-item:hover {
  transform: translateY(-10px);
}

/* Carousel Transitions */
.hero-images-container {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

.hero-fade-enter-active,
.hero-fade-leave-active {
  transition:
    opacity 0.6s ease,
    transform 0.6s ease;
}

.hero-fade-enter-from,
.hero-fade-leave-to {
  opacity: 0;
  transform: scale(1.02); 
}


.hero-content-fade-enter-active,
.hero-content-fade-leave-active {
  transition: opacity 0.5s ease;
}

.hero-content-fade-enter-from,
.hero-content-fade-leave-to {
  opacity: 0;
}

/* Hero Navigation Arrows */
.hero-nav-btn {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  z-index: 10;
  background: var(--universal-tint);
  backdrop-filter: blur(var(--universal-blur));
  -webkit-backdrop-filter: blur(var(--universal-blur));
  border: 1px solid rgba(255, 255, 255, 0.15);
  color: #fff;
  width: 54px;
  height: 54px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.22, 1, 0.36, 1);
  padding: 0;
}

.hero-nav-btn:hover {
  background: rgba(255, 255, 255, 0.2);
  border-color: rgba(255, 255, 255, 0.3);
  transform: translateY(-50%) scale(1.1);
}

.hero-nav-btn.prev {
  left: 2rem;
}

.hero-nav-btn.next {
  right: 2rem;
}

/* Home Sections Reset & Layout */
.home-section {
  padding: 4rem 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
}

.home-section:last-child {
  border-bottom: none;
}

.section-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  margin-bottom: 3rem;
}

.section-title {
  font-family: var(--font-primary);
  font-size: 2.5rem;
  letter-spacing: -0.02em;
  color: var(--text-primary);
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.section-desc {
  color: var(--text-secondary);
  font-size: 1.1rem;
  max-width: 600px;
  margin-bottom: 1.5rem;
}

.section-link {
  color: var(--text-primary);
  text-transform: uppercase;
  font-size: 0.85rem;
  letter-spacing: 0.1em;
  font-weight: 600;
  text-decoration: none;
  padding-bottom: 4px;
  border-bottom: 1px solid var(--text-primary);
  transition:
    color 0.3s,
    border-color 0.3s;
}

.section-link:hover {
  color: var(--text-secondary);
  border-color: var(--text-secondary);
}

.product-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 24px;
}

@media (max-width: 1024px) {
  .product-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .product-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 1.5rem;
  }
}

@media (max-width: 340px) {
  .product-grid {
    gap: 0.5rem;
  }
}

/* Outfit Section Specifics */
.outfit-wrapper {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  align-items: stretch;
  min-width: 0;
}

@media (max-width: 1024px) {
  .outfit-wrapper {
    grid-template-columns: 1fr;
    gap: 3rem;
  }
}

.outfit-image-container {
  position: relative;
  overflow: hidden;
  height: 100%;
  min-height: 400px;
  max-height: 600px;
  width: 100%;
  aspect-ratio: 4/5;
  border-radius: var(--radius-lg, 12px);
}

@media (max-width: 768px) {
  .outfit-image-container {
    height: 400px;
    min-height: unset;
    max-height: unset;
    aspect-ratio: unset;
  }
}

.outfit-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.8s ease;
}

.outfit-image-container:hover .outfit-image {
  transform: scale(1.05);
}

.outfit-image-content {
  position: relative;
  width: 100%;
  height: 100%;
}

.outfit-image-overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  padding: 3rem; 
  box-sizing: border-box;
  background: linear-gradient(
    to top,
    rgba(0, 0, 0, 0.8) 0%,
    rgba(0, 0, 0, 0.4) 60%,
    transparent 100%
  );
  color: white;
}

.outfit-image-overlay h3 {
  font-family: var(--font-primary);
  font-size: 1.8rem;
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.outfit-image-overlay p {
  font-size: 1rem;
  opacity: 0.9;
}

.outfit-products {
  position: relative;
  display: flex;
  flex-direction: column;
  min-width: 0;
  justify-content: center;
  min-height: 400px;
}

.outfit-header-row {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom: 2.5rem;
  width: 100%;
}

@media (max-width: 1024px) {
  .outfit-header-row {
    flex-direction: column;
    align-items: stretch;
    gap: 1.5rem;
  }
}

.outfit-tabs {
  display: flex;
  background: rgba(255, 255, 255, 0.05);
  padding: 4px;
  border-radius: 30px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  flex-shrink: 0;
  align-self: flex-start;
  overflow-x: auto;
  scrollbar-width: none;
}
.outfit-tabs::-webkit-scrollbar {
  display: none;
}

.outfit-tabs button {
  background: transparent;
  border: none;
  color: #a0a0a0;
  padding: 8px 20px;
  border-radius: 20px;
  font-family: var(--font-primary);
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.outfit-tabs button:hover {
  color: #fff;
}

.outfit-tabs button.active {
  background: var(--universal-tint);
  color: #fff;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.3);
}

.outfit-products .section-title {
  text-align: left;
}
.outfit-products .section-desc {
  text-align: left;
  margin-bottom: 0;
}

.product-list {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 24px;
}

@media (max-width: 600px) {
  .product-list {
    grid-template-columns: 1fr;
  }
}

/* Outfit Section Transitions */
.fade-slide-outfit-enter-active,
.fade-slide-outfit-leave-active {
  transition: all 0.5s cubic-bezier(0.4, 0, 0.2, 1);
  width: 100%;
}

.fade-slide-outfit-leave-active {
  position: absolute;
  top: 0;
  left: 0;
  z-index: 0;
}

.fade-slide-outfit-enter-active {
  z-index: 1;
}

.fade-slide-outfit-enter-from {
  opacity: 0;
  transform: translateY(20px);
}

.fade-slide-outfit-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}
</style>
