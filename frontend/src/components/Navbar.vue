<script setup>
import { ref, onMounted, onUnmounted, computed } from "vue";
import { useRouter } from "vue-router";
import { useCartStore } from "../stores/cartStore";
import { useAuthStore } from "../stores/authStore";
import CartDrawer from "./CartDrawer.vue";
import { sections } from "../data/categories.js";

const router = useRouter();
const cartStore = useCartStore();
const authStore = useAuthStore();
const sectionsData = computed(() => sections || {});

const kidsCategories = computed(
  () => sectionsData.value.kids?.categories || [],
);
const womenCategories = computed(
  () => sectionsData.value.women?.categories || [],
);
const menCategories = computed(() => sectionsData.value.men?.categories || []);

const hoveredLink = ref(null);

const setHoveredLink = (link) => {
  hoveredLink.value = link;
};

const clearHoveredLink = () => {
  hoveredLink.value = null;
};

const isMobileMenuOpen = ref(false);
const expandedMobileCategory = ref(null);

const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value;
  if (isMobileMenuOpen.value) {
    document.body.style.overflow = "hidden";
    isBagExpanded.value = false;
    isAccountExpanded.value = false;
    isSearchExpanded.value = false;
  } else {
    document.body.style.overflow = "";
  }
};

const closeMobileMenu = () => {
  isMobileMenuOpen.value = false;
  document.body.style.overflow = "";
};

const toggleMobileCategory = (category) => {
  if (expandedMobileCategory.value === category) {
    expandedMobileCategory.value = null;
  } else {
    expandedMobileCategory.value = category;
  }
};

// Scroll logic for sticky navbar vs floating pill
const isScrolled = ref(false);

const onScroll = () => {
  const currentScrollPosition =
    window.scrollY || document.documentElement.scrollTop;
  if (currentScrollPosition > 50) {
    isScrolled.value = true;
  } else {
    isScrolled.value = false;
  }
};

onUnmounted(() => {
  window.removeEventListener("scroll", onScroll);
  document.removeEventListener("mousedown", handleOutsideClick);
});

// Search history and expansion logic
const isSearchExpanded = ref(false);
const isMobileSearchFocused = ref(false);
const isBagExpanded = ref(false);
const isAccountExpanded = ref(false);
const searchQuery = ref("");
const searchHistory = ref([]);

const toggleSearch = () => {
  isSearchExpanded.value = !isSearchExpanded.value;
  isBagExpanded.value = false;
  isAccountExpanded.value = false;
  if (isMobileMenuOpen.value) toggleMobileMenu();
};

const toggleBag = () => {
  isBagExpanded.value = !isBagExpanded.value;
  isSearchExpanded.value = false;
  isAccountExpanded.value = false;
  if (isMobileMenuOpen.value) toggleMobileMenu();
};

const toggleAccount = () => {
  if (!authStore.isAuthenticated) {
    router.push("/login");
    if (isMobileMenuOpen.value) closeMobileMenu();
    return;
  }
  isAccountExpanded.value = !isAccountExpanded.value;
  isSearchExpanded.value = false;
  isBagExpanded.value = false;
  if (isMobileMenuOpen.value) toggleMobileMenu();
};

const handleLogout = () => {
  authStore.logout();
  isAccountExpanded.value = false;
  router.push("/");
};

const expandSearch = () => {
  isSearchExpanded.value = true;
};

const handleOutsideClick = (event) => {
  const navContainer = document.querySelector(".navbar");
  if (navContainer && !navContainer.contains(event.target)) {
    isSearchExpanded.value = false;
    isBagExpanded.value = false;
    isAccountExpanded.value = false;
    if (isMobileMenuOpen.value) toggleMobileMenu();
  }
};

const loadSearchHistory = () => {
  const saved = localStorage.getItem("vuesharp_search_history");
  if (saved) {
    searchHistory.value = JSON.parse(saved);
  }
};

const saveSearch = () => {
  const query = searchQuery.value.trim();
  if (!query) return;

  let updated = [
    query,
    ...searchHistory.value.filter((item) => item !== query),
  ];

  updated = updated.slice(0, 5);

  searchHistory.value = updated;
  localStorage.setItem("vuesharp_search_history", JSON.stringify(updated));

  router.push({ path: "/search", query: { q: query } });

  searchQuery.value = "";
  isSearchExpanded.value = false;
  if (isMobileMenuOpen.value) closeMobileMenu();
};

const removeSearch = (index) => {
  searchHistory.value.splice(index, 1);
  localStorage.setItem(
    "vuesharp_search_history",
    JSON.stringify(searchHistory.value),
  );
};

const clearHistory = () => {
  searchHistory.value = [];
  localStorage.removeItem("vuesharp_search_history");
};

onMounted(() => {
  window.addEventListener("scroll", onScroll);
  document.addEventListener("mousedown", handleOutsideClick);
  loadSearchHistory();
});
</script>

<template>
  <nav class="navbar-wrapper" @mouseleave="clearHoveredLink">
    <div class="navbar" :class="{ 'scrolled-pill': isScrolled }">
      <!-- Brand Logo -->
      <div class="brand">
        <router-link to="/" class="brand-link">
          <svg
            width="32"
            height="32"
            viewBox="0 0 40 40"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
            class="brand-logo"
          >
            <defs>
              <linearGradient
                id="navBrandGradient"
                x1="0"
                y1="0"
                x2="40"
                y2="40"
                gradientUnits="userSpaceOnUse"
              >
                <stop stop-color="white" stop-opacity="0.6" />
                <stop offset="1" stop-color="white" stop-opacity="0.2" />
              </linearGradient>
            </defs>
            <path
              d="M10 5L20 32L30 5H35L20 37L5 5H10Z"
              fill="white"
              fill-opacity="0.1"
            />
            <path
              d="M10 5L20 32L30 5H12L20 28L28 5H10Z"
              fill="url(#navBrandGradient)"
              stroke="rgba(255,255,255,0.4)"
              stroke-width="0.8"
            />
          </svg>
          <span class="brand-text">VueSharp</span>
        </router-link>
      </div>

      <!-- Main Navigation Links -->
      <ul class="nav-links">
        <!-- Kids -->
        <li
          class="nav-item-wrapper"
          @mouseenter="setHoveredLink('kids')"
          :style="{ zIndex: hoveredLink === 'kids' ? 1020 : 10 }"
        >
          <div
            class="nav-expandable-unit category-unit"
            :class="{ expanded: hoveredLink === 'kids' }"
          >
            <div
              class="unit-top-bar"
              :class="{ 'expanded-header': hoveredLink === 'kids' }"
            >
              <div v-if="hoveredLink === 'kids'" class="empty-icon-slot"></div>

              <router-link
                to="/kids"
                class="nav-link"
                :class="{ 'expanded-title': hoveredLink === 'kids' }"
                >Kids</router-link
              >

              <button
                v-if="hoveredLink === 'kids'"
                class="popup-close-btn"
                @click.stop="setHoveredLink(null)"
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
                  class="feather"
                >
                  <line x1="18" y1="6" x2="6" y2="18"></line>
                  <line x1="6" y1="6" x2="18" y2="18"></line>
                </svg>
              </button>
            </div>
            <transition name="history-reveal">
              <div v-show="hoveredLink === 'kids'" class="unit-content">
                <ul class="dropdown-list">
                  <li v-for="cat in kidsCategories" :key="cat.slug">
                    <router-link
                      :to="`/kids/${cat.slug}`"
                      class="dropdown-link"
                      @click="setHoveredLink(null)"
                      >{{ cat.label }}</router-link
                    >
                  </li>
                </ul>
              </div>
            </transition>
          </div>
        </li>

        <!-- Women -->
        <li
          class="nav-item-wrapper"
          @mouseenter="setHoveredLink('women')"
          :style="{ zIndex: hoveredLink === 'women' ? 1020 : 10 }"
        >
          <div
            class="nav-expandable-unit category-unit"
            :class="{ expanded: hoveredLink === 'women' }"
          >
            <div
              class="unit-top-bar"
              :class="{ 'expanded-header': hoveredLink === 'women' }"
            >
              <div v-if="hoveredLink === 'women'" class="empty-icon-slot"></div>

              <router-link
                to="/women"
                class="nav-link"
                :class="{ 'expanded-title': hoveredLink === 'women' }"
                >Women</router-link
              >

              <button
                v-if="hoveredLink === 'women'"
                class="popup-close-btn"
                @click.stop="setHoveredLink(null)"
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
                  class="feather"
                >
                  <line x1="18" y1="6" x2="6" y2="18"></line>
                  <line x1="6" y1="6" x2="18" y2="18"></line>
                </svg>
              </button>
            </div>
            <transition name="history-reveal">
              <div v-show="hoveredLink === 'women'" class="unit-content">
                <ul class="dropdown-list">
                  <li v-for="cat in womenCategories" :key="cat.slug">
                    <router-link
                      :to="`/women/${cat.slug}`"
                      class="dropdown-link"
                      @click="setHoveredLink(null)"
                      >{{ cat.label }}</router-link
                    >
                  </li>
                </ul>
              </div>
            </transition>
          </div>
        </li>

        <!-- Men -->
        <li
          class="nav-item-wrapper"
          @mouseenter="setHoveredLink('men')"
          :style="{ zIndex: hoveredLink === 'men' ? 1020 : 10 }"
        >
          <div
            class="nav-expandable-unit category-unit"
            :class="{ expanded: hoveredLink === 'men' }"
          >
            <div
              class="unit-top-bar"
              :class="{ 'expanded-header': hoveredLink === 'men' }"
            >
              <div v-if="hoveredLink === 'men'" class="empty-icon-slot"></div>

              <router-link
                to="/men"
                class="nav-link"
                :class="{ 'expanded-title': hoveredLink === 'men' }"
                >Men</router-link
              >

              <button
                v-if="hoveredLink === 'men'"
                class="popup-close-btn"
                @click.stop="setHoveredLink(null)"
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
                  class="feather"
                >
                  <line x1="18" y1="6" x2="6" y2="18"></line>
                  <line x1="6" y1="6" x2="18" y2="18"></line>
                </svg>
              </button>
            </div>
            <transition name="history-reveal">
              <div v-show="hoveredLink === 'men'" class="unit-content">
                <ul class="dropdown-list">
                  <li v-for="cat in menCategories" :key="cat.slug">
                    <router-link
                      :to="`/men/${cat.slug}`"
                      class="dropdown-link"
                      @click="setHoveredLink(null)"
                      >{{ cat.label }}</router-link
                    >
                  </li>
                </ul>
              </div>
            </transition>
          </div>
        </li>
      </ul>

      <!-- Right Menu Area -->
      <div class="right-menu">
        <!-- Search Area: Expanded at top, collapsed to icon when scrolled or when other icons are clicked -->
        <div class="search-container-wrapper">
          <div
            class="search-container"
            :class="{
              collapsed: isScrolled || isBagExpanded || isAccountExpanded,
              expanded: isSearchExpanded,
            }"
          >
            <div class="search-top-bar">
              <input
                type="text"
                class="search-input"
                placeholder="Search"
                v-model="searchQuery"
                @focus="expandSearch"
                @keyup.enter="saveSearch"
              />
              <button class="search-icon-btn" @click="toggleSearch">
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
                  class="feather feather-search"
                >
                  <circle cx="11" cy="11" r="8"></circle>
                  <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
                </svg>
              </button>
            </div>

            <!-- Integrated Search History Content -->
            <transition name="history-reveal">
              <div
                v-if="isSearchExpanded && searchHistory.length > 0"
                class="search-history-content"
              >
                <div class="history-list">
                  <div
                    v-for="(item, index) in searchHistory"
                    :key="index"
                    class="history-item"
                  >
                    <span
                      class="history-text"
                      @click="
                        searchQuery = item;
                        saveSearch();
                      "
                      >{{ item }}</span
                    >
                    <button class="remove-btn" @click="removeSearch(index)">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="14"
                        height="14"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        class="feather feather-x"
                      >
                        <line x1="18" y1="6" x2="6" y2="18"></line>
                        <line x1="6" y1="6" x2="18" y2="18"></line>
                      </svg>
                    </button>
                  </div>
                </div>
                <button class="clear-history-btn" @click="clearHistory">
                  Clear Search History
                </button>
              </div>
            </transition>
          </div>
        </div>

        <!-- Icons Container -->
        <div class="nav-icons">
          <!-- Bag Expandable -->
          <div
            class="nav-icon-wrapper"
            :style="{ zIndex: isBagExpanded ? 1020 : 10 }"
          >
            <div
              class="nav-expandable-unit icon-unit pos-bag"
              :class="{ expanded: isBagExpanded }"
            >
              <div
                class="unit-top-bar center-icon"
                :class="{ 'expanded-header': isBagExpanded }"
              >
                <button class="icon-btn-integrated" @click="toggleBag">
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
                    class="feather"
                  >
                    <path
                      d="M6 2L3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4z"
                    ></path>
                    <line x1="3" y1="6" x2="21" y2="6"></line>
                    <path d="M16 10a4 4 0 0 1-8 0"></path>
                  </svg>
                  <span v-if="cartStore.cartItemCount > 0" class="cart-badge">{{
                    cartStore.cartItemCount
                  }}</span>
                </button>

                <span v-if="isBagExpanded" class="expanded-title">BAG</span>

                <button
                  v-if="isBagExpanded"
                  class="popup-close-btn"
                  @click.stop="toggleBag"
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
                    class="feather"
                  >
                    <line x1="18" y1="6" x2="6" y2="18"></line>
                    <line x1="6" y1="6" x2="18" y2="18"></line>
                  </svg>
                </button>
              </div>
              <transition name="history-reveal">
                <div v-show="isBagExpanded" class="unit-content icon-content">
                  <CartDrawer @close="isBagExpanded = false" />
                </div>
              </transition>
            </div>
          </div>

          <!-- Account Expandable -->
          <div
            class="nav-icon-wrapper"
            :style="{ zIndex: isAccountExpanded ? 1020 : 10 }"
          >
            <div
              class="nav-expandable-unit icon-unit pos-account"
              :class="{ expanded: isAccountExpanded }"
            >
              <div
                class="unit-top-bar center-icon"
                :class="{ 'expanded-header': isAccountExpanded }"
              >
                <button
                  class="icon-btn-integrated user-btn"
                  @click="toggleAccount"
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
                    class="feather"
                  >
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
                    <circle cx="12" cy="7" r="4"></circle>
                  </svg>
                </button>

                <span v-if="isAccountExpanded" class="expanded-title"
                  >ACCOUNT</span
                >

                <button
                  v-if="isAccountExpanded"
                  class="popup-close-btn"
                  @click.stop="toggleAccount"
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
                    class="feather"
                  >
                    <line x1="18" y1="6" x2="6" y2="18"></line>
                    <line x1="6" y1="6" x2="18" y2="18"></line>
                  </svg>
                </button>
              </div>
              <transition name="history-reveal">
                <div
                  v-show="isAccountExpanded"
                  class="unit-content icon-content"
                >
                  <ul class="dropdown-list">
                    <li>
                      <router-link
                        to="/account/profile"
                        class="dropdown-link"
                        @click="isAccountExpanded = false"
                        >Profile</router-link
                      >
                    </li>
                    <li>
                      <router-link
                        to="/account/orders"
                        class="dropdown-link"
                        @click="isAccountExpanded = false"
                        >Orders</router-link
                      >
                    </li>
                    <li>
                      <a
                        href="#"
                        class="dropdown-link"
                        @click.prevent="handleLogout"
                      >
                        Logout
                      </a>
                    </li>
                  </ul>
                </div>
              </transition>
            </div>
          </div>

          <!-- Menu Hamburger / Close Icon (Integrated Unit) -->
          <div class="nav-icon-wrapper mobile-toggle-wrapper">
            <div
              class="nav-expandable-unit icon-unit mobile-menu-unit pos-menu"
              :class="{ expanded: isMobileMenuOpen }"
            >
              <div
                class="unit-top-bar center-icon"
                :class="{ 'expanded-header': isMobileMenuOpen }"
                @click="!isMobileMenuOpen && toggleMobileMenu()"
              >
                <button
                  class="icon-btn mobile-toggle-btn"
                  :class="{ active: isMobileMenuOpen }"
                  :style="{
                    opacity: isMobileMenuOpen ? 0 : 1,
                    pointerEvents: isMobileMenuOpen ? 'none' : 'auto',
                  }"
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
                    class="feather"
                  >
                    <line x1="3" y1="12" x2="21" y2="12"></line>
                    <line x1="3" y1="6" x2="21" y2="6"></line>
                    <line x1="3" y1="18" x2="21" y2="18"></line>
                  </svg>
                </button>

                <div v-if="isMobileMenuOpen" class="empty-title-slot"></div>

                <button
                  v-if="isMobileMenuOpen"
                  class="popup-close-btn"
                  @click.stop="toggleMobileMenu"
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
                    class="feather"
                  >
                    <line x1="18" y1="6" x2="6" y2="18"></line>
                    <line x1="6" y1="6" x2="18" y2="18"></line>
                  </svg>
                </button>
              </div>

              <transition name="fade">
                <div v-show="isMobileMenuOpen" class="unit-content">
                  <!-- Mobile Search Bar -->
                  <div
                    class="mobile-search-wrapper"
                    :class="{ expanded: isMobileSearchFocused }"
                  >
                    <div class="search-top-bar">
                      <input
                        type="text"
                        class="search-input"
                        placeholder="Search"
                        v-model="searchQuery"
                        @keyup.enter="saveSearch"
                        @focus="isMobileSearchFocused = true"
                        @blur="isMobileSearchFocused = false"
                      />
                      <button
                        class="search-icon-btn"
                        @mousedown.prevent="saveSearch"
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
                          class="feather feather-search"
                        >
                          <circle cx="11" cy="11" r="8"></circle>
                          <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
                        </svg>
                      </button>
                    </div>

                    <!-- Mobile Search History Content -->
                    <transition name="history-reveal">
                      <div
                        v-if="searchHistory.length > 0 && isMobileSearchFocused"
                        class="search-history-content"
                      >
                        <div class="history-list">
                          <div
                            v-for="(item, index) in searchHistory"
                            :key="index"
                            class="history-item"
                            @mousedown.prevent
                          >
                            <span
                              class="history-text"
                              @click="
                                searchQuery = item;
                                saveSearch();
                                isMobileSearchFocused = false;
                              "
                              >{{ item }}</span
                            >
                            <button
                              class="remove-btn"
                              @mousedown.prevent
                              @click="removeSearch(index)"
                            >
                              <svg
                                xmlns="http://www.w3.org/2000/svg"
                                width="14"
                                height="14"
                                viewBox="0 0 24 24"
                                fill="none"
                                stroke="currentColor"
                                stroke-width="2"
                                stroke-linecap="round"
                                stroke-linejoin="round"
                                class="feather feather-x"
                              >
                                <line x1="18" y1="6" x2="6" y2="18"></line>
                                <line x1="6" y1="6" x2="18" y2="18"></line>
                              </svg>
                            </button>
                          </div>
                        </div>
                        <button
                          class="clear-history-btn"
                          @mousedown.prevent
                          @click="clearHistory"
                        >
                          Clear Search History
                        </button>
                      </div>
                    </transition>
                  </div>

                  <!-- Mobile Categories Accordion -->
                  <div
                    class="mobile-categories"
                    style="padding: 0 16px; white-space: normal"
                  >
                    <!-- Kids -->
                    <div class="mobile-category-item">
                      <div class="mobile-category-header">
                        <router-link
                          to="/kids"
                          class="mobile-category-link"
                          @click="closeMobileMenu"
                          >Kids</router-link
                        >
                        <button
                          class="mobile-category-toggle"
                          @click.stop="toggleMobileCategory('kids')"
                        >
                          <svg
                            :class="{
                              'rotate-icon': expandedMobileCategory === 'kids',
                            }"
                            xmlns="http://www.w3.org/2000/svg"
                            width="18"
                            height="18"
                            viewBox="0 0 24 24"
                            fill="none"
                            stroke="currentColor"
                            stroke-width="2"
                          >
                            <polyline points="6 9 12 15 18 9"></polyline>
                          </svg>
                        </button>
                      </div>
                      <transition name="accordion">
                        <ul
                          v-show="expandedMobileCategory === 'kids'"
                          class="mobile-subcategory-list"
                        >
                          <li v-for="cat in kidsCategories" :key="cat.slug">
                            <router-link
                              :to="`/kids/${cat.slug}`"
                              class="mobile-subcategory-link"
                              @click="closeMobileMenu"
                              >{{ cat.label }}</router-link
                            >
                          </li>
                        </ul>
                      </transition>
                    </div>

                    <!-- Women -->
                    <div class="mobile-category-item">
                      <div class="mobile-category-header">
                        <router-link
                          to="/women"
                          class="mobile-category-link"
                          @click="closeMobileMenu"
                          >Women</router-link
                        >
                        <button
                          class="mobile-category-toggle"
                          @click.stop="toggleMobileCategory('women')"
                        >
                          <svg
                            :class="{
                              'rotate-icon': expandedMobileCategory === 'women',
                            }"
                            xmlns="http://www.w3.org/2000/svg"
                            width="18"
                            height="18"
                            viewBox="0 0 24 24"
                            fill="none"
                            stroke="currentColor"
                            stroke-width="2"
                          >
                            <polyline points="6 9 12 15 18 9"></polyline>
                          </svg>
                        </button>
                      </div>
                      <transition name="accordion">
                        <ul
                          v-show="expandedMobileCategory === 'women'"
                          class="mobile-subcategory-list"
                        >
                          <li v-for="cat in womenCategories" :key="cat.slug">
                            <router-link
                              :to="`/women/${cat.slug}`"
                              class="mobile-subcategory-link"
                              @click="closeMobileMenu"
                              >{{ cat.label }}</router-link
                            >
                          </li>
                        </ul>
                      </transition>
                    </div>

                    <!-- Men -->
                    <div class="mobile-category-item">
                      <div class="mobile-category-header">
                        <router-link
                          to="/men"
                          class="mobile-category-link"
                          @click="closeMobileMenu"
                          >Men</router-link
                        >
                        <button
                          class="mobile-category-toggle"
                          @click.stop="toggleMobileCategory('men')"
                        >
                          <svg
                            :class="{
                              'rotate-icon': expandedMobileCategory === 'men',
                            }"
                            xmlns="http://www.w3.org/2000/svg"
                            width="18"
                            height="18"
                            viewBox="0 0 24 24"
                            fill="none"
                            stroke="currentColor"
                            stroke-width="2"
                          >
                            <polyline points="6 9 12 15 18 9"></polyline>
                          </svg>
                        </button>
                      </div>
                      <transition name="accordion">
                        <ul
                          v-show="expandedMobileCategory === 'men'"
                          class="mobile-subcategory-list"
                        >
                          <li v-for="cat in menCategories" :key="cat.slug">
                            <router-link
                              :to="`/men/${cat.slug}`"
                              class="mobile-subcategory-link"
                              @click="closeMobileMenu"
                              >{{ cat.label }}</router-link
                            >
                          </li>
                        </ul>
                      </transition>
                    </div>
                  </div>
                </div>
              </transition>
            </div>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>

<style scoped>
/* Wrapper to keep it fixed */
.navbar-wrapper {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  z-index: 1000;
  display: flex;
  justify-content: center;
  pointer-events: none;
}

/* Base Navbar (At Top) */
.navbar {
  position: relative;
  margin-top: 1rem;
  width: 95%;
  max-width: 1600px;
  height: 70px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 32px;
  pointer-events: auto;

  border-radius: 12px;
  transition:
    width 0.5s cubic-bezier(0.22, 1, 0.36, 1),
    max-width 0.5s cubic-bezier(0.22, 1, 0.36, 1),
    margin-top 0.5s cubic-bezier(0.22, 1, 0.36, 1),
    height 0.5s cubic-bezier(0.22, 1, 0.36, 1),
    border-radius 0.5s cubic-bezier(0.22, 1, 0.36, 1);
}

.navbar::before {
  content: "";
  position: absolute;
  inset: 0;
  z-index: -1;
  border-radius: inherit;
  background: transparent;
  border: 1px solid transparent;
  transition:
    background-color 0.5s cubic-bezier(0.22, 1, 0.36, 1),
    border-color 0.5s cubic-bezier(0.22, 1, 0.36, 1),
    box-shadow 0.5s cubic-bezier(0.22, 1, 0.36, 1);
}

/* Scrolled Navbar (Floating Pill) */
.navbar.scrolled-pill {
  width: 80%;
  max-width: 1100px;
  margin-top: 24px;
  height: 60px;
  border-radius: 50px;
}

/* Apply the pill visuals to the pseudo-element */
.navbar.scrolled-pill::before {
  background: var(--universal-tint);
  -webkit-backdrop-filter: blur(var(--universal-blur)) saturate(150%);
  backdrop-filter: blur(var(--universal-blur)) saturate(150%);
  border: 1px solid rgba(255, 255, 255, 0.08);
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
}

/* Typography & Brand */
.brand {
  height: 100%;
  display: flex;
  align-items: center;
}

.brand a.brand-link {
  display: flex;
  align-items: center;
  gap: 12px;
  text-decoration: none;
  transition: all 0.4s cubic-bezier(0.22, 1, 0.36, 1);
}

.brand-logo {
  filter: drop-shadow(0 0 8px rgba(255, 255, 255, 0.15));
  transition: transform 0.4s cubic-bezier(0.22, 1, 0.36, 1);
}

.brand-text {
  font-family: "Playfair Display", serif;
  font-size: 1.6rem;
  font-weight: 700;
  color: #fff;
  letter-spacing: -0.5px;
  line-height: 1;
  transition:
    opacity 0.4s ease,
    font-size 0.4s ease;
}

.brand-link:hover .brand-logo {
  transform: scale(1.1) rotate(-5deg);
}

.scrolled-pill .brand-text {
  font-size: 1.3rem;
}

.scrolled-pill .brand-logo {
  transform: scale(0.9);
}

/* Nav Links */
.nav-links {
  display: flex;
  list-style: none;
  margin: 0;
  padding: 0;
  gap: 8px;
  height: 100%;
  align-items: center;
}

.nav-expandable-unit {
  display: flex;
  flex-direction: column;
  background: var(--universal-tint);
  -webkit-backdrop-filter: blur(var(--universal-blur));
  backdrop-filter: blur(var(--universal-blur));
  border-radius: 24px;
  border: 1px solid rgba(255, 255, 255, 0.25);
  transition:
    width 0.5s cubic-bezier(0.22, 1, 0.36, 1),
    max-height 0.5s cubic-bezier(0.22, 1, 0.36, 1),
    background-color 0.5s cubic-bezier(0.22, 1, 0.36, 1),
    border-color 0.5s cubic-bezier(0.22, 1, 0.36, 1),
    border-radius 0.5s cubic-bezier(0.22, 1, 0.36, 1),
    right 0.5s cubic-bezier(0.22, 1, 0.36, 1),
    transform 0.5s cubic-bezier(0.22, 1, 0.36, 1);
  overflow: hidden;
  z-index: 1001;
  max-height: 48px;
}

.nav-expandable-unit.expanded {
  max-height: 600px;
  background: var(--universal-tint) !important;
  border-color: rgba(255, 255, 255, 0.3);
  border-radius: 20px;
}

@media (max-height: 700px) {
  .nav-expandable-unit.expanded {
    max-height: calc(100vh - 80px);
  }
}

.unit-top-bar {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 48px;
  min-height: 48px;
  padding: 0 16px;
  box-sizing: border-box;
}

.unit-top-bar.center-icon {
  padding: 0;
  width: 48px;
}

/* Expanded Header Grid Layout */
.unit-top-bar.expanded-header {
  display: grid;
  grid-template-columns: 48px 1fr 48px;
  width: 100%;
  padding: 0;
  align-items: center;
  justify-items: center;
}

.expanded-header > *:nth-child(1) {
  justify-self: center;
}
.expanded-header > *:nth-child(2) {
  justify-self: center;
  text-align: center;
}
.expanded-header > *:nth-child(3) {
  justify-self: center;
}

.expanded-title {
  color: #fff;
  font-size: 0.9rem;
  font-weight: 500;
  letter-spacing: 1px;
}

.popup-close-btn {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: transparent;
  color: rgba(255, 255, 255, 0.4);
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s;
}

.popup-close-btn:hover {
  background: rgba(255, 255, 255, 0.1);
  color: #fff;
}

.unit-content {
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  padding: 12px 0;
  width: 100%;
  white-space: nowrap;
}

@media (max-height: 500px) {
  .unit-content {
    padding: 4px 0;
  }
}

.icon-content {
  min-width: 200px;
}

.nav-item-wrapper,
.nav-icon-wrapper {
  position: relative;
  height: 48px;
  display: flex;
  align-items: flex-start;
  justify-content: center;
  transition: width 0.5s cubic-bezier(0.22, 1, 0.36, 1);
}

.nav-item-wrapper {
  min-width: 80px;
}

/* Category Specifics */
.category-unit {
  background: transparent;
  -webkit-backdrop-filter: none;
  backdrop-filter: none;
  border-color: transparent;
  width: 80px;
}

.category-unit.expanded {
  width: 220px;
  background: var(--universal-tint) !important;
  -webkit-backdrop-filter: blur(var(--universal-blur)) !important;
  backdrop-filter: blur(var(--universal-blur)) !important;
  border-color: rgba(255, 255, 255, 0.3) !important;
}

/* Icon Specifics */
.icon-unit {
  width: 48px;
}

.icon-unit.expanded {
  width: min(300px, calc(100vw - 32px));
}

.icon-unit .unit-top-bar {
  justify-content: center;
  padding: 0;
}

.nav-link {
  text-decoration: none;
  color: #d1d1d1;
  font-weight: 500;
  font-size: 0.95rem;
  transition:
    color 0.2s ease,
    font-size 0.5s ease;
  padding: 8px 0;
}

.scrolled-pill .nav-link {
  font-size: 0.9rem;
}

.nav-link:hover,
.nav-link.router-link-active {
  color: #fff;
}

/* Dropdown styling */
.dropdown-list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
}

.dropdown-list li {
  width: 100%;
}

.dropdown-list li:not(:last-child) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.06);
}

/* Dropdown links using unified style */
.dropdown-link {
  display: block;
  padding: 10px 20px;
  color: #ccc;
  text-decoration: none;
  font-size: 0.9rem;
  transition:
    background 0.2s ease,
    color 0.2s ease;
}

.dropdown-link:hover {
  background: rgba(255, 255, 255, 0.1);
  color: #fff;
}

.empty-state {
  padding: 20px;
  color: rgba(255, 255, 255, 0.3);
  font-size: 0.85rem;
  text-align: center;
}

/* Right Menu Area */
.right-menu {
  display: flex;
  align-items: center;

  gap: 8px;
}

/* Search Bar Transition */
.search-container {
  display: flex;
  flex-direction: column;
  position: absolute;
  top: 0;
  right: 0;
  background: var(--universal-tint);
  -webkit-backdrop-filter: blur(var(--universal-blur));
  backdrop-filter: blur(var(--universal-blur));
  border-radius: 24px;
  border: 1px solid rgba(255, 255, 255, 0.25);
  width: 260px;
  max-height: 48px;
  transition:
    width 0.5s cubic-bezier(0.7, 1, 0.5, 1),
    max-height 0.5s cubic-bezier(0.7, 1, 0.5, 1),
    padding 0.5s cubic-bezier(0.7, 1, 0.5, 1),
    background-color 0.5s cubic-bezier(0.7, 1, 0.5, 1),
    border-color 0.5s cubic-bezier(0.7, 1, 0.5, 1),
    border-radius 0.5s cubic-bezier(0.7, 1, 0.5, 1);
  overflow: hidden;
  z-index: 1001;
}

.search-top-bar {
  display: flex;
  align-items: center;
  width: 100%;
  height: 48px;
  min-height: 48px;
  padding: 0 8px;
  justify-content: center;
  box-sizing: border-box;
}

.search-container.expanded {
  max-height: 600px;
  background: var(--universal-tint) !important;
  border-color: rgba(255, 255, 255, 0.3);
  border-radius: 20px;
}

/* Scrolled (Pill) State Expansion logic */
.search-container.collapsed {
  width: 48px;
  padding: 0;
}

.search-container.collapsed.expanded {
  width: 260px;
  border-radius: 20px;
}

.search-container-wrapper {
  position: relative;
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
}

.search-input {
  flex: 1;
  background: transparent;
  border: none;
  outline: none;
  color: #fff;
  padding-left: 16px;
  font-size: 0.9rem;
  font-family: inherit;
  transition: opacity 0.3s ease;
}

.search-container.collapsed:not(.expanded) .search-input {
  opacity: 0;
  width: 0;
  padding: 0;
  pointer-events: none;
}

.search-input::placeholder {
  color: rgba(255, 255, 255, 0.6);
}

.search-icon-btn {
  background: transparent;
  border: none;
  color: #fff;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  flex-shrink: 0;
  transition: background 0.2s;
}

.search-icon-btn:hover {
  background: rgba(255, 255, 255, 0.1);
}

/* History Content inside the container */
.search-history-content {
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  padding: 8px 0;
  width: 100%;
}

.history-list {
  display: flex;
  flex-direction: column;
}

.history-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 20px;
  cursor: pointer;
  transition: background 0.2s;
}

.history-item:hover {
  background: rgba(255, 255, 255, 0.05);
}

.history-text {
  color: #ddd;
  font-size: 0.9rem;
  flex: 1;
}

.remove-btn {
  background: transparent;
  border: none;
  color: rgba(255, 255, 255, 0.4);
  cursor: pointer;
  padding: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: color 0.2s;
}

.remove-btn:hover {
  color: #fff;
}

.clear-history-btn {
  width: 100%;
  background: transparent;
  border: none;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  color: #888;
  padding: 12px;
  font-size: 0.8rem;
  cursor: pointer;
  transition: color 0.2s;
}

.clear-history-btn:hover {
  color: #fff;
}

/* History Reveal Animation */
.history-reveal-enter-active,
.history-reveal-leave-active {
  transition: opacity 0.5s ease;
}

.history-reveal-enter-from,
.history-reveal-leave-to {
  opacity: 0;
}

/* Icon Buttons Container */
.nav-icons {
  display: flex;
  align-items: center;
  gap: 8px;
}

.icon-btn-integrated {
  background: transparent;
  border: none;
  color: #ccc;
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: color 0.3s ease;
}

.icon-btn-integrated:hover {
  color: #fff;
}

.icon-btn-integrated {
  position: relative;
}

.cart-badge {
  position: absolute;
  top: 6px;
  right: 6px;
  background: var(--accent);
  color: #fff;
  font-size: 0.65rem;
  font-weight: 800;
  min-width: 16px;
  height: 16px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(255, 255, 255, 0.2);
  pointer-events: none;
}

.icon-btn {
  background: var(--universal-tint);
  -webkit-backdrop-filter: blur(var(--universal-blur));
  backdrop-filter: blur(var(--universal-blur));
  color: #ccc;
  border: 1px solid rgba(255, 255, 255, 0.25);
  width: 48px;
  height: 48px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition:
    background-color 0.3s cubic-bezier(0.22, 1, 0.36, 1),
    color 0.3s cubic-bezier(0.22, 1, 0.36, 1),
    transform 0.3s cubic-bezier(0.22, 1, 0.36, 1),
    border-color 0.3s cubic-bezier(0.22, 1, 0.36, 1);
}

.icon-btn:hover {
  color: #fff;
  background: rgba(255, 255, 255, 0.25);
  transform: scale(1.05);
}

/* Transitions */
.fade-enter-active,
.fade-leave-active {
  transition:
    opacity 0.2s ease,
    transform 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

/* Tablet / Smaller Desktop Adjustments */
@media (max-width: 1150px) {
  .navbar {
    padding: 0 16px;
  }

  .nav-links {
    gap: 4px;
  }

  .nav-item-wrapper {
    min-width: 80px;
    z-index: 10;
  }

  .category-unit {
    position: absolute;
    top: 0;
    right: 0;
    width: 80px;
  }
  .category-unit.expanded {
    width: 220px;
  }

  .nav-icon-wrapper {
    width: 48px;
    z-index: 10;
  }
  .icon-unit {
    position: absolute;
    right: 0;
    left: auto;
    margin-right: 10px;
    transform: none;
    transition: all 0.5s cubic-bezier(0.22, 1, 0.36, 1);
  }
  .icon-unit.expanded {
    width: min(280px, calc(100vw - 32px));
    margin-right: 0px;
    right: 0;
    left: auto;
    transform: none !important;
  }

  .search-container.collapsed.expanded {
    width: 200px;
  }

  .search-container.expanded:not(.collapsed) {
    width: 200px;
  }
}

/* Specific Thin Mobile Centering (Only for thin portrait phones/folded screens) */
@media (max-width: 500px) and (max-aspect-ratio: 0.7) {
  .nav-icon-wrapper {
    position: static;
  }

  .navbar {
    position: relative;
  }

  .brand-text {
    display: none;
  }

  .icon-unit.pos-menu:not(.expanded) {
    right: 0px;
  }
  .icon-unit.pos-account:not(.expanded) {
    right: 56px;
  }
  .icon-unit.pos-bag:not(.expanded) {
    right: 112px;
  }

  .icon-unit.expanded {
    right: 50% !important;
    transform: translateX(50%) !important;
    width: min(320px, calc(100vw - 32px)) !important;
  }

  .expanded-title {
    font-size: 0.75rem;
  }
}

@media (max-width: 420px) {
  .brand-text {
    display: none;
  }
}

/* Landscape Mobile Adjustments - Wider navbar to feel less cramped */
@media (orientation: landscape) and (max-height: 500px) {
  .navbar.scrolled-pill {
    width: 80%;
  }
}

/* --- Mobile Menu Styles --- */
.mobile-toggle-wrapper {
  display: none;
}

.mobile-toggle-btn {
  display: none;
}

@media (max-width: 1150px) {
  .nav-links {
    display: none;
  }

  .search-container {
    display: none;
  }

  .mobile-toggle-wrapper {
    display: flex;
  }

  .mobile-toggle-btn {
    display: flex !important;
  }
}

/* Mobile Menu Integrated Unit - Ensure pinning on horizontal/side */
.mobile-menu-unit.expanded {
  width: 320px;
  max-width: calc(100vw - 32px);
  max-height: 80vh;
  overflow-y: auto;
  right: 0;
  left: auto;
  transform: none;
}

/* Mobile Search */
.mobile-search-wrapper {
  margin: 0 16px 24px;
  display: flex;
  flex-direction: column;
  background: var(--universal-tint);
  -webkit-backdrop-filter: blur(var(--universal-blur));
  backdrop-filter: blur(var(--universal-blur));
  border-radius: 24px;
  border: 1px solid rgba(255, 255, 255, 0.25);
  max-height: 48px;
  transition:
    max-height 0.5s cubic-bezier(0.7, 1, 0.5, 1),
    background-color 0.5s cubic-bezier(0.7, 1, 0.5, 1),
    border-color 0.5s cubic-bezier(0.7, 1, 0.5, 1),
    border-radius 0.5s cubic-bezier(0.7, 1, 0.5, 1);
  overflow: hidden;
  box-sizing: border-box;
}

.mobile-search-wrapper.expanded {
  max-height: 600px;
  background: var(--universal-tint) !important;
  border-color: rgba(255, 255, 255, 0.3);
  border-radius: 20px;
}

/* Mobile Categories */
.mobile-categories {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.mobile-category-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  padding: 12px 8px;
}

.mobile-category-link {
  color: #fff;
  font-size: 1.2rem;
  font-weight: 500;
  text-decoration: none;
  flex: 1;
}

.mobile-category-toggle {
  background: var(--universal-tint);
  -webkit-backdrop-filter: blur(var(--universal-blur));
  backdrop-filter: blur(var(--universal-blur));
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 50%;
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: #fff;
  transition:
    background 0.2s,
    transform 0.3s ease;
  padding: 0;
}

.mobile-category-toggle:hover {
  background: rgba(255, 255, 255, 0.15);
}

.rotate-icon {
  transform: rotate(180deg);
}

.mobile-category-toggle svg {
  transition: transform 0.3s ease;
}

.mobile-subcategory-list {
  list-style: none;
  padding: 8px 0 16px 16px;
  margin: 0;
}

.mobile-subcategory-link {
  display: block;
  text-decoration: none;
  color: #a0a0a0;
  padding: 8px;
  font-size: 1rem;
  transition: color 0.2s;
}

.mobile-subcategory-link:hover {
  color: #fff;
}

/* Accordion Transition */
.accordion-enter-active,
.accordion-leave-active {
  transition:
    max-height 0.5s cubic-bezier(0.7, 1, 0.5, 1),
    opacity 0.5s ease,
    padding 0.5s ease;
  max-height: 600px;
  overflow: hidden;
}

.accordion-enter-from,
.accordion-leave-to {
  max-height: 0 !important;
  opacity: 0;
  padding-top: 0 !important;
  padding-bottom: 0 !important;
}
</style>
