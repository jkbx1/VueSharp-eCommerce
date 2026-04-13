<script setup>
import { computed, ref, watch, watchEffect, onMounted } from "vue";
import { useRoute, useRouter, RouterLink } from "vue-router";
import ProductCard from "./ProductCard.vue";
import { sections, findCategory } from "../data/categories.js";
import { getProducts, getFilters } from "../api/products.js";

const props = defineProps({
  sectionSlug: { type: String, default: null },
  isGlobalSale: { type: Boolean, default: false },
  isSearch: { type: Boolean, default: false },
  isNovelties: { type: Boolean, default: false },
});

const route = useRoute();
const router = useRouter();

const section = computed(() => {
  if (props.isGlobalSale) {
    return { label: "Summer Sale", path: "/sale" };
  }
  if (props.isNovelties) {
    return { label: "New Arrivals", path: "/novelties" };
  }
  return sections[props.sectionSlug] || null;
});

const categorySlug = computed(() => route.params.category ?? null);
const searchTerms = computed(() => route.query.q ?? "");

const activeCategory = computed(() => {
  if (props.isSearch)
    return {
      label: `Results for "${searchTerms.value}"`,
      slug: "search",
      icon: "🔍",
    };
  if (props.isGlobalSale) return { label: "Sale", slug: "sale", icon: "🏷️" };
  if (props.isNovelties)
    return { label: "Novelties", slug: "novelties", icon: "✨" };
  return categorySlug.value
    ? findCategory(props.sectionSlug, categorySlug.value)
    : null;
});

// ----- Breadcrumbs -----
const breadcrumbs = computed(() => {
  const crumbs = [{ label: "VueSharp", path: "/" }];

  if (props.isSearch) {
    crumbs.push({ label: "Search", path: null });
  } else if (props.isGlobalSale) {
    crumbs.push({ label: "Summer Sale", path: null });
  } else if (props.isNovelties) {
    crumbs.push({ label: "New Arrivals", path: null });
  } else if (section.value) {
    crumbs.push({ label: section.value.label, path: section.value.path });
    if (activeCategory.value) {
      crumbs.push({ label: activeCategory.value.label, path: null });
    }
  }
  return crumbs;
});

// ----- Filter state -----
const activeFilters = ref({});
const stagedFilters = ref({});
const isMobileFilterOpen = ref(false);

const cloneFilters = (source) => {
  const cloned = {};
  for (const group in source) {
    cloned[group] = new Set(source[group]);
  }
  return cloned;
};

// ----- Price Range State -----
const globalPriceRange = ref({ min: 0, max: 1000 });
const stagedPriceRange = ref({ min: 0, max: 1000 });
const activePriceRange = ref({ min: 0, max: 1000 });

// Ensure min <= max logic
watch(
  () => stagedPriceRange.value.min,
  (newMin) => {
    if (newMin > stagedPriceRange.value.max) {
      stagedPriceRange.value.max = newMin;
    }
  },
);

watch(
  () => stagedPriceRange.value.max,
  (newMax) => {
    if (newMax < stagedPriceRange.value.min) {
      stagedPriceRange.value.min = newMax;
    }
  },
);

// Reset filters when category changes
watch(categorySlug, () => {
  activeFilters.value = {};
  stagedFilters.value = {};
  isMobileFilterOpen.value = false;

  // Reset price range to global defaults until new filters load
  stagedPriceRange.value = { ...globalPriceRange.value };
  activePriceRange.value = { ...globalPriceRange.value };
});

// Reset filters and page when search query changes
watch(searchTerms, () => {
  if (props.isSearch) {
    activeFilters.value = {};
    stagedFilters.value = {};
    isMobileFilterOpen.value = false;

    stagedPriceRange.value = { ...globalPriceRange.value };
    activePriceRange.value = { ...globalPriceRange.value };
    currentPage.value = 1;
  }
});

// ----- Pagination State -----
const currentPage = ref(1);
const itemsPerPage = 12;
const totalProducts = ref(0);
const totalPages = computed(() =>
  Math.ceil(totalProducts.value / itemsPerPage),
);

const goToPage = (page) => {
  if (page < 1 || page > totalPages.value) return;
  currentPage.value = page;
  window.scrollTo({ top: 0, behavior: "smooth" });
};

// For initial load
watch(
  activeCategory,
  (newCat) => {
    if (newCat) {
      stagedFilters.value = cloneFilters(activeFilters.value);
    }
  },
  { immediate: true },
);

// Lock body scroll when mobile filters are open
watch(isMobileFilterOpen, (isOpen) => {
  if (isOpen) {
    document.body.style.overflow = "hidden";
  } else {
    document.body.style.overflow = "";
  }
});

const toggleFilter = (group, value) => {
  const newStaged = cloneFilters(stagedFilters.value);
  if (!newStaged[group]) {
    newStaged[group] = new Set();
  }
  if (newStaged[group].has(value)) {
    newStaged[group].delete(value);
    if (newStaged[group].size === 0) delete newStaged[group];
  } else {
    newStaged[group].add(value);
  }
  stagedFilters.value = newStaged;
};

// Immediate removal for the active pills above the grid
const removeActiveFilter = (group, value) => {
  const newActive = cloneFilters(activeFilters.value);
  if (newActive[group]) {
    newActive[group].delete(value);
    if (newActive[group].size === 0) delete newActive[group];
  }
  activeFilters.value = newActive;

  const newStaged = cloneFilters(stagedFilters.value);
  if (newStaged[group]) {
    newStaged[group].delete(value);
    if (newStaged[group].size === 0) delete newStaged[group];
  }
  stagedFilters.value = newStaged;
};

const isFilterApplied = (group, value) => {
  const isStaged = stagedFilters.value[group]?.has(value) ?? false;
  const isActive = activeFilters.value[group]?.has(value) ?? false;
  return isStaged && isActive;
};

const isFilterStaged = (group, value) => {
  const inStaged = stagedFilters.value[group]?.has(value) ?? false;
  const inActive = activeFilters.value[group]?.has(value) ?? false;
  return inStaged && !inActive;
};

const activeFilterCount = computed(() => {
  let count = 0;
  for (const group of Object.values(activeFilters.value)) {
    count += group.size;
  }
  // Count price range if it differs from global min/max
  if (
    activePriceRange.value.min > globalPriceRange.value.min ||
    activePriceRange.value.max < globalPriceRange.value.max
  ) {
    count++;
  }
  return count;
});

const stagedFilterCount = computed(() => {
  let count = 0;
  for (const group of Object.values(stagedFilters.value)) {
    count += group.size;
  }
  return count;
});

const applyFilters = () => {
  activeFilters.value = cloneFilters(stagedFilters.value);
  activePriceRange.value = { ...stagedPriceRange.value };
  isMobileFilterOpen.value = false;
};

const cancelFilters = () => {
  stagedFilters.value = cloneFilters(activeFilters.value);
  stagedPriceRange.value = { ...activePriceRange.value };
  isMobileFilterOpen.value = false;
};

const clearAllFilters = () => {
  stagedFilters.value = {};
  activeFilters.value = {};
  stagedPriceRange.value = { ...globalPriceRange.value };
  activePriceRange.value = { ...globalPriceRange.value };
};

const navigateToCategory = (cat) => {
  router.push(`${section.value.path}/${cat.slug}`);
};

// ----- Grid Columns State -----
const gridColumns = ref(3);

// ----- Color Hex Map (for color swatches) -----
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

// ----- Dynamic Filters from Backend -----
const dynamicFilters = ref({
  Category: [],
  SubCategory: [],
  Style: [],
  Color: [],
  Size: [],
});
const availableOptions = ref({});
const categoryMapping = ref({});
const isFiltersLoading = ref(false);

const updateAvailability = async () => {
  try {
    const isLocalSale = activeCategory.value?.slug === "sale";
    const isLocalNovelties = activeCategory.value?.slug === "novelties";

    // Only narrow down options based on top-level category/subcategory
    const params = {
      category:
        props.isGlobalSale || props.isSearch || props.isNovelties
          ? [...(stagedFilters.value["Category"] || [])]
          : section.value?.label,
      subcategory:
        props.isGlobalSale ||
        isLocalSale ||
        isLocalNovelties ||
        props.isSearch ||
        props.isNovelties
          ? [...(stagedFilters.value["SubCategory"] || [])]
          : activeCategory.value?.label,
      onSale: props.isGlobalSale || isLocalSale ? true : null,
      itemType: [...(stagedFilters.value["Style"] || [])],
      color: [...(stagedFilters.value["Color"] || [])],
      size: [...(stagedFilters.value["Size"] || [])],
      q: props.isSearch ? searchTerms.value : null,
      sort: props.isNovelties || isLocalNovelties ? "newest" : null,
    };

    const filters = await getFilters(params);

    availableOptions.value = {
      Category: filters.categories || [],
      SubCategory: filters.subCategories || [],
      Style: filters.itemTypes || [],
      Size: filters.sizes || [],
      Color: filters.colors || [],
    };
  } catch (e) {
    console.warn("Failed to update availability:", e);
  }
};

const isOptionDisabled = (group, value) => {
  // Grey-out logic ONLY makes sense for hierarchical filters (Category, SubCat, Style)
  const hierarchicalGroups = ["Category", "SubCategory", "Style"];
  if (!hierarchicalGroups.includes(group)) return false;

  if (Object.keys(availableOptions.value).length === 0) return false;
  if (stagedFilters.value[group]?.has(value)) return false;
  const validOptions = availableOptions.value[group];
  if (!validOptions) return false;
  return !validOptions.includes(value);
};

let availabilityTimeout;
watch(
  [stagedFilters, stagedPriceRange],
  () => {
    clearTimeout(availabilityTimeout);
    availabilityTimeout = setTimeout(updateAvailability, 300);
  },
  { deep: true },
);

// ----- Real Products State -----
const productsList = ref([]);
const isLoading = ref(false);
const isError = ref(false);

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

watchEffect(async () => {
  // Fetch dynamic filters contextually
  if (
    props.isGlobalSale ||
    props.isSearch ||
    props.isNovelties ||
    (section.value && activeCategory.value)
  ) {
    isFiltersLoading.value = true;
    try {
      const isLocalSale = activeCategory.value?.slug === "sale";
      const isLocalNovelties = activeCategory.value?.slug === "novelties";
      const params = {
        category:
          props.isGlobalSale || props.isSearch || props.isNovelties
            ? null
            : section.value?.label,
        subcategory:
          props.isGlobalSale ||
          isLocalSale ||
          isLocalNovelties ||
          props.isSearch ||
          props.isNovelties
            ? null
            : activeCategory.value?.label,
        onSale: props.isGlobalSale || isLocalSale ? true : null,
        q: props.isSearch ? searchTerms.value : null,
        sort: props.isNovelties || isLocalNovelties ? "newest" : null,
      };

      const filters = await getFilters(params);

      const finalFilters = {
        Category: filters.categories || [],
        SubCategory: filters.subCategories || [],
        Style: filters.itemTypes || [],
        Size: filters.sizes || [],
        Color: filters.colors || [],
      };

      // In Global/Search/Novelties hubs, "Category" (Men/Women/Kids) is needed for high-level browsing.
      // In Local listing pages (e.g., /women/tops), it's redundant.
      if (!props.isGlobalSale && !props.isSearch && !props.isNovelties) {
        delete finalFilters.Category;
      }

      dynamicFilters.value = finalFilters;
      categoryMapping.value = filters.categoryMapping || {};

      // Update price range boundaries
      globalPriceRange.value = { min: filters.minPrice, max: filters.maxPrice };

      // If price range hasn't been touched, sync it to global boundaries
      if (
        activePriceRange.value.min === 0 &&
        activePriceRange.value.max === 1000
      ) {
        stagedPriceRange.value = { ...globalPriceRange.value };
        activePriceRange.value = { ...globalPriceRange.value };
      }
    } catch (e) {
      console.error("Failed to fetch filters:", e);
    } finally {
      isFiltersLoading.value = false;
    }
  }
});

const fetchProducts = async () => {
  if (!activeCategory.value) {
    productsList.value = [];
    return;
  }

  isLoading.value = true;
  isError.value = false;

  const isLocalSale = activeCategory.value?.slug === "sale";
  const isLocalNovelties = activeCategory.value?.slug === "novelties";
  const params = {
    category:
      props.isGlobalSale || props.isSearch || props.isNovelties
        ? null
        : section.value?.label,
    subcategory:
      props.isGlobalSale ||
      isLocalSale ||
      isLocalNovelties ||
      props.isSearch ||
      props.isNovelties
        ? null
        : activeCategory.value?.label,
    onSale: props.isGlobalSale || isLocalSale ? true : null,
    q: props.isSearch ? searchTerms.value : null,
    sort: props.isNovelties || isLocalNovelties ? "newest" : null,
    page: currentPage.value,
    pageSize: itemsPerPage,
  };

  // Add selected filters (multi-select UNION support)
  if (activeFilters.value["Category"]?.size > 0) {
    params.category = [...activeFilters.value["Category"]];
  }
  if (activeFilters.value["SubCategory"]?.size > 0) {
    params.subcategory = [...activeFilters.value["SubCategory"]];
  }
  if (activeFilters.value["Style"]?.size > 0) {
    params.itemType = [...activeFilters.value["Style"]];
  }
  if (activeFilters.value["Color"]?.size > 0) {
    params.color = [...activeFilters.value["Color"]];
  }
  if (activeFilters.value["Size"]?.size > 0) {
    params.size = [...activeFilters.value["Size"]];
  }

  // Add price filters
  if (activePriceRange.value.min > globalPriceRange.value.min) {
    params.minPrice = activePriceRange.value.min;
  }
  if (activePriceRange.value.max < globalPriceRange.value.max) {
    params.maxPrice = activePriceRange.value.max;
  }

  try {
    const data = await getProducts(params);
    productsList.value = (data.items || data).map(mapProduct);
    totalProducts.value =
      data.totalCount ?? (Array.isArray(data) ? data.length : 0);
  } catch (e) {
    console.error("Failed to load products:", e);
    isError.value = true;
    productsList.value = [];
    totalProducts.value = 0;
  } finally {
    isLoading.value = false;
  }
};

// Reset page when filters change
watch([activeFilters, activePriceRange, categorySlug], () => {
  currentPage.value = 1;
});

// Explicitly watch the 'Applied' state and category/search changes
watch(
  [activeFilters, activePriceRange, categorySlug, searchTerms, currentPage],
  () => {
    fetchProducts();
  },
  { deep: true },
);

// Initial products load on mount
onMounted(() => {
  if (activeCategory.value) fetchProducts();
});
</script>

<template>
  <div class="page-wrapper">
    <!-- ===== Breadcrumb Trail ===== -->
    <div class="breadcrumb-bar">
      <nav class="breadcrumb" aria-label="breadcrumb">
        <template v-for="(crumb, idx) in breadcrumbs" :key="idx">
          <router-link v-if="crumb.path" :to="crumb.path" class="crumb-link">
            {{ crumb.label }}
          </router-link>
          <span v-else class="crumb-current">{{ crumb.label }}</span>
          <span
            v-if="idx < breadcrumbs.length - 1"
            class="crumb-sep"
            aria-hidden="true"
            >›</span
          >
        </template>
      </nav>
    </div>

    <!-- ===== SECTION ROOT: Category Tiles Grid ===== -->
    <div v-if="!activeCategory" class="tiles-section">
      <div class="section-header">
        <h1 class="section-title">{{ section?.label }}'s Collection</h1>
        <p class="section-subtitle">Choose a category to explore</p>
      </div>

      <div class="tiles-grid">
        <button
          v-for="(cat, index) in section?.categories"
          :key="cat.slug"
          class="category-tile"
          @click="navigateToCategory(cat)"
          :aria-label="`Browse ${cat.label}`"
          :style="{ '--animation-order': index }"
        >
          <div
            class="tile-bg-image"
            :style="{
              backgroundImage: `url(${cat.image})`,
              backgroundColor: cat.gradient ? 'transparent' : '#1a1a2e',
              ...(cat.gradient && !cat.image
                ? { background: cat.gradient }
                : {}),
            }"
          ></div>

          <div class="tile-content">
            <h2 class="tile-label">{{ cat.label }}</h2>
          </div>
        </button>
      </div>
    </div>

    <!-- ===== SUBCATEGORY VIEW: Minimal Header + Filters + Products ===== -->
    <div v-else class="subcategory-section">
      <div class="subcat-header">
        <div class="subcat-header-content">
          <div class="subcat-header-text">
            <h1 class="category-title">
              {{ activeCategory?.label ?? section?.label ?? "Search Results" }}
            </h1>
            <p class="category-subtitle">
              <template v-if="props.isSearch">
                Find what you're looking for in our curated collection.
              </template>
              <template v-else>
                {{ activeCategory?.label }}'s Collection
              </template>
            </p>
          </div>

          <!-- Manual grid toggles removed for strict deterministic layout -->
        </div>
      </div>

      <div class="subcat-body">
        <!-- Mobile filter toggle -->
        <div class="mobile-filter-bar">
          <button
            class="mobile-filter-toggle"
            @click="isMobileFilterOpen = true"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            >
              <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3" />
            </svg>
            Filters
            <span v-if="activeFilterCount > 0" class="filter-badge">{{
              activeFilterCount
            }}</span>
          </button>
          <button
            v-if="activeFilterCount > 0"
            class="clear-btn-mobile"
            @click="clearAllFilters"
          >
            Clear all
          </button>
        </div>

        <div class="subcat-layout">
          <!-- ===== Filter Sidebar ===== -->
          <aside
            class="filter-sidebar"
            :class="{ 'mobile-open': isMobileFilterOpen }"
          >
            <div class="filter-sidebar-header">
              <span class="filter-title">Filters</span>
              <div class="filter-header-actions">
                <button
                  v-if="activeFilterCount > 0 || stagedFilterCount > 0"
                  class="clear-all-btn"
                  @click="clearAllFilters"
                >
                  Clear
                </button>
                <button
                  class="close-filters-mobile"
                  @click="cancelFilters"
                  aria-label="Close filters"
                >
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
              </div>
            </div>

            <div class="filter-groups-wrapper">
              <!-- Price Filter Group (Special) -->
              <div class="filter-group">
                <h3 class="filter-group-title">Price Range</h3>
                <div class="price-range-container">
                  <div class="price-inputs">
                    <div class="price-input-box">
                      <span>$</span>
                      <input
                        type="number"
                        v-model.number="stagedPriceRange.min"
                        :min="globalPriceRange.min"
                        :max="stagedPriceRange.max"
                      />
                    </div>
                    <span class="price-sep">-</span>
                    <div class="price-input-box">
                      <span>$</span>
                      <input
                        type="number"
                        v-model.number="stagedPriceRange.max"
                        :min="stagedPriceRange.min"
                        :max="globalPriceRange.max"
                      />
                    </div>
                  </div>
                  <div class="range-slider">
                    <input
                      type="range"
                      v-model.number="stagedPriceRange.min"
                      :min="globalPriceRange.min"
                      :max="globalPriceRange.max"
                      step="1"
                    />
                    <input
                      type="range"
                      v-model.number="stagedPriceRange.max"
                      :min="globalPriceRange.min"
                      :max="globalPriceRange.max"
                      step="1"
                    />
                  </div>
                </div>
              </div>

              <!-- Loading Skeletons for filters -->
              <div v-if="isFiltersLoading" class="filter-group-skeleton">
                <div class="skeleton-title"></div>
                <div class="skeleton-chips">
                  <div class="skeleton-chip"></div>
                  <div class="skeleton-chip"></div>
                  <div class="skeleton-chip"></div>
                </div>
              </div>

              <div
                v-else
                v-for="(options, groupName) in dynamicFilters"
                v-show="
                  groupName !== 'SubCategory' ||
                  props.isSearch ||
                  props.isNovelties ||
                  !categorySlug ||
                  categorySlug === 'sale' ||
                  categorySlug === 'novelties' ||
                  isGlobalSale
                "
                :key="groupName"
                class="filter-group"
              >
                <h3 v-if="options.length > 0" class="filter-group-title">
                  {{ groupName }}
                </h3>
                <div class="filter-chips">
                  <button
                    v-for="option in options"
                    :key="option"
                    class="filter-chip"
                    :class="{
                      applied: isFilterApplied(groupName, option),
                      staged: isFilterStaged(groupName, option),
                      disabled: isOptionDisabled(groupName, option),
                    }"
                    @click="toggleFilter(groupName, option)"
                  >
                    <span
                      v-if="groupName === 'Color'"
                      class="color-dot"
                      :style="{ backgroundColor: getColorHex(option) }"
                    ></span>
                    {{ option }}
                    <span
                      v-if="isFilterApplied(groupName, option)"
                      class="applied-check"
                      >✓</span
                    >
                  </button>
                </div>
              </div>
            </div>

            <!-- Apply Buttons Section (Sticky at bottom on Mobile, fixed on Desktop) -->
            <div class="filter-apply-actions">
              <button
                class="apply-filters-btn"
                :class="{
                  'has-changes':
                    stagedFilterCount !== activeFilterCount ||
                    stagedPriceRange.min !== activePriceRange.min ||
                    stagedPriceRange.max !== activePriceRange.max,
                }"
                @click="applyFilters"
              >
                Apply Filters
                <span v-if="stagedFilterCount > 0"
                  >({{ stagedFilterCount }})</span
                >
              </button>
            </div>
          </aside>

          <!-- ===== Product Grid Area ===== -->
          <main class="product-area">
            <!-- Active Filter Pills -->
            <div v-if="activeFilterCount > 0" class="active-filters-row">
              <template v-for="(members, group) in activeFilters" :key="group">
                <div
                  v-for="value in members"
                  :key="value"
                  class="active-filter-pill"
                >
                  <span class="pill-label">{{ group }}:</span>
                  <span class="pill-value">{{ value }}</span>
                  <button
                    class="pill-remove"
                    @click="removeActiveFilter(group, value)"
                    aria-label="Remove filter"
                  >
                    ✕
                  </button>
                </div>
              </template>

              <!-- Active Price Pill -->
              <div
                v-if="
                  activePriceRange.min > globalPriceRange.min ||
                  activePriceRange.max < globalPriceRange.max
                "
                class="active-filter-pill"
              >
                <span class="pill-label">Price:</span>
                <span class="pill-value"
                  >${{ activePriceRange.min }} - ${{
                    activePriceRange.max
                  }}</span
                >
                <button
                  class="pill-remove"
                  @click="
                    stagedPriceRange = { ...globalPriceRange };
                    activePriceRange = { ...globalPriceRange };
                  "
                  aria-label="Remove price filter"
                >
                  ✕
                </button>
              </div>
            </div>

            <!-- Loading State -->
            <div v-if="isLoading" class="loading-state">
              <div class="loading-spinner"></div>
              <p>Loading products…</p>
            </div>

            <!-- Error State -->
            <div v-else-if="isError" class="error-state">
              <p>Failed to load products. Please try again.</p>
            </div>

            <!-- Empty State -->
            <div
              v-else-if="!isLoading && productsList.length === 0"
              class="empty-state"
            >
              <p>No products found for this category.</p>
            </div>

            <!-- Product Grid -->
            <div v-else class="products-container">
              <div class="product-grid">
                <ProductCard
                  v-for="product in productsList"
                  :key="product.id"
                  :product="product"
                  :active-category-icon="activeCategory.icon"
                />
              </div>
            </div>
          </main>

          <!-- Pagination Area: Sits in grid row 2, column 2 -->
          <div v-if="totalPages > 1" class="pagination-section">
            <div class="pagination-bar">
              <div class="pagination-info">
                Showing {{ (currentPage - 1) * itemsPerPage + 1 }}-{{
                  Math.min(currentPage * itemsPerPage, totalProducts)
                }}
                of {{ totalProducts }} items
              </div>

              <div class="pagination-controls">
                <button
                  class="pager-btn"
                  :disabled="currentPage === 1"
                  @click="goToPage(currentPage - 1)"
                  aria-label="Previous page"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    width="18"
                    height="18"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    stroke-width="2"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  >
                    <polyline points="15 18 9 12 15 6"></polyline>
                  </svg>
                </button>

                <div class="pager-numbers">
                  <button
                    v-for="p in totalPages"
                    :key="p"
                    class="pager-num"
                    :class="{ active: p === currentPage }"
                    @click="goToPage(p)"
                  >
                    {{ p }}
                  </button>
                </div>

                <button
                  class="pager-btn"
                  :disabled="currentPage === totalPages"
                  @click="goToPage(currentPage + 1)"
                  aria-label="Next page"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    width="18"
                    height="18"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    stroke-width="2"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  >
                    <polyline points="9 18 15 12 9 6"></polyline>
                  </svg>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* ====== Page Wrapper ====== */
.page-wrapper {
  min-height: 100vh;
  padding-top: 100px; /* account for fixed navbar */
  background: #0d0d0f;
  color: #e0e0e0;
}

/* ====== Breadcrumb ====== */
.breadcrumb-bar {
  padding: 0 clamp(1.5rem, 5vw, 4rem);
  margin: 2rem 0;
}

.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  flex-wrap: wrap;
}

.crumb-link {
  color: #888;
  text-decoration: none;
  font-size: 0.85rem;
  font-weight: 500;
  letter-spacing: 0.05em;
  transition: color 0.2s;
}

.crumb-link:hover {
  color: #e0e0e0;
}

.crumb-sep {
  color: #444;
  font-size: 0.9rem;
  user-select: none;
}

.crumb-current {
  color: #e0e0e0;
  font-size: 0.85rem;
  font-weight: 600;
  letter-spacing: 0.05em;
}

/* ====== Section Root: Tiles ====== */
.tiles-section {
  padding: 0 clamp(1rem, 3vw, 2rem) 3rem;
}

.section-header {
  margin-bottom: 3rem;
}

@media (max-width: 600px) {
  .section-header {
    margin-bottom: 1.5rem;
  }
}

.section-title {
  font-size: clamp(2rem, 5vw, 3.5rem);
  font-weight: 200;
  letter-spacing: -0.03em;
  color: #f0f0f0;
  margin: 0 0 0.5rem;
}

@media (max-width: 600px) {
  .section-title {
    font-size: 1.8rem;
  }
}

.section-subtitle {
  font-size: 1rem;
  color: #666;
  margin: 0;
  font-weight: 400;
  letter-spacing: 0.05em;
}

.tiles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

@media (min-width: 1400px) {
  .tiles-grid {
    grid-template-columns: repeat(4, 1fr);
  }
}
@media (min-width: 1800px) {
  .tiles-grid {
    grid-template-columns: repeat(5, 1fr);
  }
}
@media (max-width: 1399px) and (min-width: 901px) {
  .tiles-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}
@media (max-width: 900px) and (min-width: 601px) {
  .tiles-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}
@media (max-width: 600px) {
  .tiles-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 0.75rem;
  }
}

.category-tile {
  position: relative;
  overflow: hidden;
  border: none;
  border-radius: 16px;
  padding: 0;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  aspect-ratio: 3 / 4;
  transition:
    transform 0.4s cubic-bezier(0.25, 1, 0.5, 1),
    box-shadow 0.4s ease;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
  background-color: #1a1a2e;
  animation: slideFadeIn 0.6s cubic-bezier(0.22, 1, 0.36, 1) both;
  animation-delay: calc(var(--animation-order) * 0.1s);
}

@keyframes slideFadeIn {
  0% {
    opacity: 0;
    transform: translateY(30px);
  }
  100% {
    opacity: 1;
    transform: translateY(0);
  }
}

.category-tile:hover {
  transform: translateY(-8px);
  box-shadow: 0 16px 30px rgba(0, 0, 0, 0.4);
}

.tile-bg-image {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-size: cover;
  background-position: center;
  filter: grayscale(0%);
  transition:
    filter 0.8s cubic-bezier(0.22, 1, 0.36, 1),
    transform 1.2s cubic-bezier(0.22, 1, 0.36, 1);
  z-index: 1;
}

.category-tile:hover .tile-bg-image {
  filter: grayscale(100%);
  transform: scale(1.05);
}

.tile-content {
  position: absolute;
  top: 75%;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 3;
  text-align: center;

  width: 85%;
  max-width: 300px;
  height: auto;
  min-height: 56px;
  padding: 0.75rem 1.25rem;

  background: var(--universal-tint);
  -webkit-backdrop-filter: blur(var(--universal-blur));
  backdrop-filter: blur(var(--universal-blur));
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-radius: 40px;

  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.6s cubic-bezier(0.22, 1, 0.36, 1);
  pointer-events: none;
  overflow: hidden;
  box-sizing: border-box;
}

.category-tile:hover .tile-content {
  top: 50%;
  width: 100%;
  max-width: 100%;
  padding: 100% 1.25rem;
  border-radius: 0;
  background: var(--universal-tint);
  -webkit-backdrop-filter: blur(var(--universal-blur));
  backdrop-filter: blur(var(--universal-blur));
  border-color: transparent;
}

@media (max-width: 600px) {
  .tile-content {
    width: 90%;
    min-height: 44px;
    top: 80%;
    padding: 0.5rem 1rem;
  }
}

.tile-label {
  font-family: "Playfair Display", serif;
  font-size: 1.8rem;
  font-weight: 500;
  color: #fff;
  margin: 0;
  letter-spacing: 0.05em;
  text-shadow: 0 4px 12px rgba(0, 0, 0, 0.8);
  padding: 0 5px;
  white-space: normal;
  line-height: 1.2;
}

@media (max-width: 600px) {
  .tile-label {
    font-size: 1.2rem;
  }
}

.subcategory-section {
  min-height: calc(100vh - 100px);
  width: 100%;
  display: flex;
  flex-direction: column;
}

.subcat-header {
  position: relative;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
}

.subcat-header-content {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  padding: 2rem clamp(1rem, 4vw, 3rem) 2rem;
  width: 100%;
  box-sizing: border-box;
  flex-wrap: nowrap;
}

.subcat-header-left {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.subcat-icon {
  font-size: 3.5rem;
  line-height: 1;
  filter: drop-shadow(0 4px 12px rgba(0, 0, 0, 0.4));
}

.subcat-header-text {
  display: flex;
  flex-direction: column;
  flex: 1;
  min-width: 0;
}

.subcat-header-right {
  display: flex;
  align-items: flex-end;
  flex-shrink: 0;
}

.subcat-title {
  font-family: "Playfair Display", serif;
  font-size: clamp(2.5rem, 5vw, 4rem);
  font-weight: 500;
  letter-spacing: -0.02em;
  margin: 0;
  color: #fff;
  line-height: 1;
}

.subcat-desc {
  font-family: "Inter", sans-serif;
  font-size: 0.85rem;
  color: #666;
  margin-top: 0.75rem;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  font-weight: 600;
}

@media (max-width: 900px) {
  .subcat-header-content {
    flex-direction: column;
    align-items: flex-start;
    gap: 1.5rem;
  }
}

.subcat-body {
  padding: 0 clamp(1rem, 4vw, 3rem);
  width: 100%;
  flex: 1;
  display: flex;
  flex-direction: column;
  box-sizing: border-box;
}

/* ====== Mobile Filter Bar ====== */
.mobile-filter-bar {
  display: none;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  padding: 1rem 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
}

@media (max-width: 900px) {
  .mobile-filter-bar {
    display: flex;
  }
}

.mobile-filter-toggle {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: var(--universal-tint);
  border: 1px solid var(--accent-border);
  border-radius: 8px;
  color: #e0e0e0;
  padding: 0.5rem 1rem;
  font-size: 0.9rem;
  cursor: pointer;
  transition: background 0.2s;
}

.mobile-filter-toggle:hover {
  background: #2a2a3e;
}

.filter-badge {
  background: var(--accent);
  color: white;
  border-radius: 50%;
  width: 18px;
  height: 18px;
  font-size: 0.7rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
}

.clear-btn-mobile {
  background: none;
  border: none;
  color: #888;
  font-size: 0.85rem;
  cursor: pointer;
  text-decoration: underline;
}

/* ====== Subcategory Layout (MACRO GRID) ====== */
.subcat-layout {
  display: grid;
  grid-template-columns: 240px 1fr;
  grid-template-rows: auto auto;
  gap: 0 clamp(1.5rem, 3vw, 3rem);
  padding-top: 2rem;
  margin-bottom: 2rem;
  width: 100%;
}

@media (max-width: 900px) {
  .subcat-layout {
    grid-template-columns: 1fr;
    grid-template-rows: auto auto auto;
    gap: 2rem 0;
  }
}

/* Filter Skeleton */
.filter-group-skeleton {
  margin-bottom: 2rem;
}

.skeleton-title {
  width: 60px;
  height: 14px;
  background: rgba(255, 255, 255, 0.05);
  margin-bottom: 1rem;
  border-radius: 4px;
}

.skeleton-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.skeleton-chip {
  width: 70px;
  height: 32px;
  background: rgba(255, 255, 255, 0.03);
  border-radius: 8px;
}

.filter-sidebar {
  grid-column: 1;
  grid-row: 1 / span 2;
  position: relative;
  height: 0;
  min-height: 100%;
  padding: 1.5rem 1.5rem 2rem 1.5rem;

  display: flex;
  flex-direction: column;
  overflow: hidden;
  box-sizing: border-box;

  scrollbar-width: thin;
  scrollbar-color: rgba(255, 255, 255, 0.2) transparent;

  background: var(--universal-tint);
  -webkit-backdrop-filter: blur(var(--universal-blur));
  backdrop-filter: blur(var(--universal-blur));
  border-radius: 16px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  transition:
    transform 0.4s cubic-bezier(0.22, 1, 0.36, 1),
    opacity 0.3s ease;
}

@media (max-width: 900px) {
  .filter-sidebar {
    position: fixed;
    inset: 0;
    width: 100%;
    height: 100%;
    z-index: 1000;
    border-radius: 0;
    transform: translateX(-100%);
    opacity: 0;
    background: rgba(13, 13, 15, 0.8);
    -webkit-backdrop-filter: blur(20px);
    backdrop-filter: blur(20px);
    overflow-y: hidden;
    padding: 0;
    display: flex;
    flex-direction: column;
    box-sizing: border-box;
  }

  .filter-sidebar.mobile-open {
    transform: translateX(0);
    opacity: 1;
  }

  .close-filters-mobile {
    display: flex !important;
    align-items: center;
    justify-content: center;
  }
}

.filter-sidebar-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  flex-shrink: 0;
}

@media (max-width: 900px) {
  .filter-sidebar-header {
    padding: 2rem 1.5rem 1rem;
    margin: 0;
    border-bottom: 1px solid rgba(255, 255, 255, 0.08);
    z-index: 10;
  }
}

.filter-header-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.close-filters-mobile {
  display: none;
  background: none;
  border: none;
  color: #fff;
  cursor: pointer;
  padding: 4px;
  opacity: 0.7;
}

.close-filters-mobile:hover {
  opacity: 1;
}

.filter-groups-wrapper {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  padding-right: 0.5rem;
  margin-right: -0.5rem;
  padding-bottom: 5rem;
  scrollbar-width: thin;
  scrollbar-color: rgba(255, 255, 255, 0.1) transparent;
}

.filter-apply-actions {
  position: sticky;
  bottom: 0;
  margin-top: 1.5rem;
  background: var(--universal-tint);
  -webkit-backdrop-filter: blur(15px);
  backdrop-filter: blur(15px);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  border-bottom-left-radius: 16px;
  border-bottom-right-radius: 16px;
  box-shadow: 0 -10px 20px rgba(0, 0, 0, 0.2);
  z-index: 5;
  flex-shrink: 0;
}

@media (max-width: 900px) {
  .filter-apply-actions {
    position: relative;

    padding: 1.5rem;
    border-top: 1px solid rgba(255, 255, 255, 0.08);
    z-index: 10;
    margin: 0;
  }

  .filter-groups-wrapper {
    flex: 1;
    overflow-y: auto;
    padding: 1.5rem;
    padding-bottom: 3rem;
    scrollbar-width: thin;
    scrollbar-color: rgba(255, 255, 255, 0.1) transparent;
  }
}

.apply-filters-btn {
  width: 100%;
  background: rgba(255, 255, 255, 0.05);
  color: #888;
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  padding: 0.8rem;
  font-weight: 600;
  font-size: 0.85rem;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.22, 1, 0.36, 1);
}

.apply-filters-btn.has-changes {
  background: var(--accent);
  color: white;
  border-color: var(--accent-border);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
}

.apply-filters-btn:hover {
  transform: translateY(-2px);
  filter: brightness(1.2);
}

.filter-title {
  font-size: 1rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.18em;
  color: #fff;
}

.clear-all-btn {
  background: none;
  border: none;
  color: var(--accent);
  font-size: 0.8rem;
  cursor: pointer;
  padding: 0;
  font-weight: 600;
  transition:
    color 0.2s,
    filter 0.2s;
}

.clear-all-btn:hover {
  filter: brightness(1.2);
}

.filter-group {
  margin-bottom: 1.75rem;
}

.filter-group-title {
  font-size: 0.78rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.12em;
  color: #666;
  margin: 0 0 0.75rem;
}

.filter-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.filter-chip {
  background: rgba(255, 255, 255, 0.02); 
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 20px;
  color: rgba(255, 255, 255, 0.35); 
  font-size: 0.8rem;
  padding: 0.4rem 1rem;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.22, 1, 0.36, 1);
  font-family: inherit;
  display: flex;
  align-items: center;
  gap: 0.4rem;
}

.filter-chip:hover {
  background: rgba(255, 255, 255, 0.05);
  border-color: rgba(255, 255, 255, 0.2);
  color: #ddd;
}

/* State 1: Staged (Selected but unapplied) - Strong Vibrant Outline */
.filter-chip.staged {
  border-color: var(--accent-border);
  border-width: 2px;
  border-style: dashed;
  color: #fff;
  background: rgba(255, 255, 255, 0.04);
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);
  font-weight: 500;
}

/* State 2: Applied - High Contrast Solid State */
.filter-chip.applied {
  background: var(--accent);
  border-color: var(--accent);
  border-width: 1px;
  border-style: solid;
  color: #fff;
  font-weight: 700;
  box-shadow:
    0 4px 15px rgba(0, 0, 0, 0.4),
    0 0 10px rgba(0, 0, 0, 0.5);
}

.filter-chip.disabled {
  opacity: 0.3;
  pointer-events: none;
  border-style: dotted;
  filter: grayscale(1);
}

.applied-check {
  font-size: 0.7rem;
  font-weight: 900;
}

.color-dot {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

/* ====== Price Range Slider ====== */
.price-range-container {
  padding: 0.5rem 0.2rem;
}

.price-inputs {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1.5rem;
  gap: 0.5rem;
}

.price-input-box {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  padding: 0.4rem 0.6rem;
  flex: 1;
}

.price-input-box span {
  font-size: 0.8rem;
  color: #666;
  font-weight: 600;
}

.price-input-box input {
  background: none;
  border: none;
  color: #fff;
  font-size: 0.9rem;
  width: 100%;
  font-family: inherit;
  outline: none;
  padding: 0;
}

/* Chrome, Safari, Edge, Opera */
.price-input-box input::-webkit-outer-spin-button,
.price-input-box input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.price-sep {
  color: #444;
  font-weight: 300;
}

.range-slider {
  position: relative;
  height: 4px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 2px;
  margin: 0 10px;
}

.range-slider input[type="range"] {
  position: absolute;
  width: 100%;
  height: 4px;
  -webkit-appearance: none;
  appearance: none;
  background: none;
  pointer-events: none;
  top: 0;
  left: 0;
}

.range-slider input[type="range"]::-webkit-slider-thumb {
  height: 18px;
  width: 18px;
  border-radius: 50%;
  background: var(--accent);
  border: 3px solid #1a1a1f;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
  cursor: pointer;
  -webkit-appearance: none;
  appearance: none;
  pointer-events: auto;
  position: relative;
  z-index: 2;
}

.range-slider input[type="range"]::-moz-range-thumb {
  height: 18px;
  width: 18px;
  border-radius: 50%;
  background: var(--accent);
  border: 3px solid #1a1a1f;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
  cursor: pointer;
  pointer-events: auto;
}

/* ====== Product Area ====== */
.product-area {
  flex: 1;
  min-width: 0;
}

.active-filters-row {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
}

.active-filter-pill {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.12);
  color: #ddd;
  border-radius: 40px;
  padding: 0.4rem 0.9rem;
  font-size: 0.75rem;
  font-weight: 500;
  -webkit-backdrop-filter: blur(var(--universal-blur));
  backdrop-filter: blur(var(--universal-blur));
  transition: all 0.2s ease;
}

.active-filter-pill:hover {
  background: rgba(255, 255, 255, 0.12);
  border-color: rgba(255, 255, 255, 0.2);
  color: #fff;
}

.pill-remove {
  background: none;
  border: none;
  color: inherit;
  cursor: pointer;
  padding: 0;
  font-size: 0.7rem;
  opacity: 0.7;
  line-height: 1;
  display: flex;
  align-items: center;
}

.pill-remove:hover {
  opacity: 1;
}

/* ====== Product Grid: Deterministic Responsive Layout ====== */
/* Standard Mobile (2 columns) */
.product-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: clamp(0.75rem, 2vw, 2.5rem);
  width: 100%;
  justify-items: center;
  animation: gridAppear 0.8s cubic-bezier(0.22, 1, 0.36, 1);
}

@media (max-width: 400px) {
  .product-grid {
    gap: 0.5rem;
  }
}

/* Tablet & Standard Desktop (with sidebar) */
@media (min-width: 901px) and (max-width: 1440px) {
  .product-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

/* Large Desktop */
@media (min-width: 1441px) and (max-width: 1919px) {
  .product-grid {
    grid-template-columns: repeat(4, 1fr);
  }
}

/* Ultrawide Display */
@media (min-width: 1920px) {
  .product-grid {
    grid-template-columns: repeat(5, 1fr);
  }
}

@keyframes gridAppear {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
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

/* ====== Loading / Error / Empty States ====== */
.loading-state,
.error-state,
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  min-height: 300px;
  color: rgba(255, 255, 255, 0.4);
  font-size: 0.9rem;
  letter-spacing: 0.05em;
}
.loading-spinner {
  width: 36px;
  height: 36px;
  border: 3px solid rgba(255, 255, 255, 0.1);
  border-top-color: var(--accent);
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

/* ====== Pagination Styles ====== */
.products-container {
  display: flex;
  flex-direction: column;
  gap: 4rem;
  width: 100%;
}

.pagination-section {
  grid-column: 2;
  grid-row: 2;
  padding: 0 0 2rem 0;
}

@media (max-width: 900px) {
  .pagination-section {
    grid-column: 1;
    grid-row: 3;
    padding-bottom: 4rem;
  }
}

.pagination-bar {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
  padding-top: 2rem;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
}

.pagination-info {
  font-size: 0.85rem;
  color: #888;
  letter-spacing: 0.02em;
}

.pagination-controls {
  display: flex;
  align-items: center;
  gap: 1rem;
}

@media (max-width: 400px) {
  .pagination-controls {
    gap: 0.3rem;
  }
}

.pager-btn {
  width: 44px;
  height: 44px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 50%;
  color: #fff;
  cursor: pointer;
  transition: all 0.2s ease;
}

.pager-btn:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
}

.pager-btn:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}

.pager-numbers {
  display: flex;
  gap: 0.5rem;
}

.pager-num {
  width: 44px;
  height: 44px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: 1px solid transparent;
  border-radius: 50%;
  color: #aaa;
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.pager-num:hover:not(.active) {
  background: rgba(255, 255, 255, 0.05);
  color: #fff;
}

.pager-num.active {
  background: var(--accent, #6366f1);
  color: #fff;
  font-weight: 700;
  box-shadow: 0 4px 15px rgba(99, 102, 241, 0.4);
}

@media (max-width: 600px) {
  .pagination-bar {
    gap: 1rem;
  }

  .pager-btn,
  .pager-num {
    width: 38px;
    height: 38px;
  }
}
</style>
