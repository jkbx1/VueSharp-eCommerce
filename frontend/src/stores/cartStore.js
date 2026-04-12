import { defineStore } from 'pinia';
import { ref, computed, watch } from 'vue';

export const useCartStore = defineStore('cart', () => {
  // State
  const items = ref([]);
  const isQuickAddOpen = ref(false);
  const quickAddProduct = ref(null);
  const toastMessage = ref("");
  const toastType = ref("success");
  const showToast = ref(false);

  // Persistence: Load from localStorage
  const savedCart = localStorage.getItem('vuesharp_cart');
  if (savedCart) {
    try {
      items.value = JSON.parse(savedCart);
    } catch (e) {
      console.error("Failed to parse cart from localStorage", e);
      items.value = [];
    }
  }

  // Persistence: Watch and save
  watch(items, (newItems) => {
    localStorage.setItem('vuesharp_cart', JSON.stringify(newItems));
  }, { deep: true });

  // Getters
  const cartItemCount = computed(() => {
    return items.value.reduce((total, item) => total + item.quantity, 0);
  });

  const cartTotal = computed(() => {
    return items.value.reduce((total, item) => {
      const price = item.salePrice || item.price;
      return total + (price * item.quantity);
    }, 0);
  });

  // Actions
  const addToCart = (product, size, color, quantity = 1, imageUrl = "") => {
    // Validation: Ensure size and color are selected
    if (!size || !color) {
      triggerToast("Please select a size and color", "error");
      return;
    }

    // Check if item already exists with SAME size and color
    const existingItem = items.value.find(item => 
      item.productId === product.id && 
      item.selectedSize === size && 
      item.selectedColor === color
    );

    if (existingItem) {
      const newQty = existingItem.quantity + quantity;
      existingItem.quantity = Math.min(newQty, 10);
    } else {
      items.value.push({
        productId: product.id,
        name: product.name,
        price: product.originalPrice || product.price,
        salePrice: product.onSale ? product.price : null,
        selectedSize: size,
        selectedColor: color,
        imageUrl: imageUrl || (product.imageUrls && product.imageUrls[0]),
        quantity: Math.min(quantity, 10)
      });
    }

    triggerToast(`Added ${product.name} to bag`, "success");
  };

  const removeFromCart = (productId, size, color) => {
    items.value = items.value.filter(item => 
      !(item.productId === productId && item.selectedSize === size && item.selectedColor === color)
    );
  };

  const clearCart = () => {
    items.value = [];
  };

  const updateQuantity = (productId, size, color, newQuantity) => {
    const item = items.value.find(item => 
      item.productId === productId && item.selectedSize === size && item.selectedColor === color
    );
    if (item) {
      item.quantity = Math.max(1, Math.min(newQuantity, 10));
    }
  };

  const openQuickAdd = (product) => {
    quickAddProduct.value = product;
    isQuickAddOpen.value = true;
  };

  const closeQuickAdd = () => {
    isQuickAddOpen.value = false;
    setTimeout(() => {
      quickAddProduct.value = null;
    }, 300);
  };

  const triggerToast = (message, type = "success") => {
    toastMessage.value = message;
    toastType.value = type;
    showToast.value = true;
    setTimeout(() => {
      showToast.value = false;
    }, 3000);
  };

  return {
    items,
    isQuickAddOpen,
    quickAddProduct,
    toastMessage,
    toastType,
    showToast,
    cartItemCount,
    cartTotal,
    addToCart,
    removeFromCart,
    updateQuantity,
    clearCart,
    openQuickAdd,
    closeQuickAdd,
    triggerToast
  };
});
