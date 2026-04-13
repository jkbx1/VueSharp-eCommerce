<script setup>
import { ref, onMounted, onUnmounted } from 'vue';
import ProductCard from './ProductCard.vue';

const props = defineProps({
  products: {
    type: Array,
    required: true
  }
});

const carouselRef = ref(null);
const showPrev = ref(false);
const showNext = ref(false);

const checkScroll = () => {
  if (!carouselRef.value) return;
  const { scrollLeft, scrollWidth, clientWidth } = carouselRef.value;
  showPrev.value = scrollLeft > 0;
  showNext.value = scrollLeft + clientWidth < scrollWidth - 2;
};

const scroll = (direction) => {
  if (!carouselRef.value) return;
  const { clientWidth } = carouselRef.value;
  const scrollAmount = clientWidth * 0.8;
  carouselRef.value.scrollBy({
    left: direction === 'next' ? scrollAmount : -scrollAmount,
    behavior: 'smooth'
  });
};

onMounted(() => {
  if (carouselRef.value) {
    checkScroll();
    carouselRef.value.addEventListener('scroll', checkScroll);
    window.addEventListener('resize', checkScroll);
    
    setTimeout(checkScroll, 150);
  }
});

onUnmounted(() => {
  if (carouselRef.value) {
    carouselRef.value.removeEventListener('scroll', checkScroll);
  }
  window.removeEventListener('resize', checkScroll);
});
</script>

<template>
  <div class="carousel-container" @mouseenter="checkScroll">
    <button 
      class="carousel-nav prev" 
      v-show="showPrev" 
      @click="scroll('prev')"
      aria-label="Previous Items"
    >
      <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <polyline points="15 18 9 12 15 6"></polyline>
      </svg>
    </button>
    
    <div class="carousel-track" ref="carouselRef">
      <div 
        class="carousel-item" 
        v-for="product in products" 
        :key="product.id"
      >
        <ProductCard :product="product" :grid-columns="4" />
      </div>
    </div>
    
    <button 
      class="carousel-nav next" 
      v-show="showNext" 
      @click="scroll('next')"
      aria-label="Next Items"
    >
      <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <polyline points="9 18 15 12 9 6"></polyline>
      </svg>
    </button>
  </div>
</template>

<style scoped>
.carousel-container {
  position: relative;
  width: 100%;
}

.carousel-track {
  display: flex;
  gap: 24px;
  overflow-x: auto;
  scroll-behavior: smooth;
  scrollbar-width: none; 
  -ms-overflow-style: none; 
  padding-bottom: 24px; 
}

.carousel-track::-webkit-scrollbar {
  display: none; 
}

.carousel-item {
  flex: 0 0 calc(25% - 18px);
  min-width: 250px;
}

@media (max-width: 1024px) {
  .carousel-item {
    flex: 0 0 calc(33.333% - 16px);
  }
}

@media (max-width: 768px) {
  .carousel-item {
    flex: 0 0 calc(50% - 12px);
  }
}

@media (max-width: 480px) {
  .carousel-item {
    flex: 0 0 85%; 
  }
}

.carousel-nav {
  position: absolute;
  top: calc(50% - 12px);
  transform: translateY(-50%);
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background: rgba(20, 20, 20, 0.8);
  -webkit-backdrop-filter: blur(8px);
  backdrop-filter: blur(8px);
  border: 1px solid rgba(255, 255, 255, 0.15);
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  z-index: 10;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.4);
  transition: all 0.3s cubic-bezier(0.22, 1, 0.36, 1);
}

.carousel-nav svg {
  width: 20px;
  height: 20px;
}

.carousel-nav:hover {
  background: rgba(255, 255, 255, 0.15);
  border-color: rgba(255, 255, 255, 0.3);
  transform: translateY(-50%) scale(1.1);
}

.carousel-nav.prev {
  left: -22px;
}

.carousel-nav.next {
  right: -22px;
}

@media (max-width: 768px) {
  .carousel-nav.prev { left: -10px; }
  .carousel-nav.next { right: -10px; }
}


</style>
