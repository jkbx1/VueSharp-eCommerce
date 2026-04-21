<script setup>
import { computed } from "vue";
import { useCartStore } from "../stores/cartStore";

const cartStore = useCartStore();
const currentYear = computed(() => new Date().getFullYear());

const shopLinks = [
  { name: "Women", href: "/women" },
  { name: "Men", href: "/men" },
  { name: "Kids", href: "/kids" },
  { name: "New Arrivals", href: "/novelties" },
];

const supportLinks = [
  { name: "FAQ", href: "/faq" },
  { name: "Shipping & Returns", href: "/shipping-returns" },
  { name: "Order Tracking", href: "/track-order" },
  { name: "Contact Us", href: "/contact" },
];

const legalLinks = [
  { name: "Terms of Service", href: "/terms" },
  { name: "Privacy Policy", href: "/privacy" },
];

const socialLinks = [
  { name: "Instagram", icon: "instagram", href: "#" },
  { name: "Pinterest", icon: "pinterest", href: "#" },
  { name: "X", icon: "twitter", href: "#" },
];

const handleDemoClick = (name) => {
  cartStore.triggerToast(
    `The ${name} page is part of the demo and is not yet available.`,
    "success",
  );
};

const handleSubscribe = () => {
  cartStore.triggerToast(
    "Thank you for subscribing! (Demo project)",
    "success",
  );
};

const handleSocialClick = (social, event) => {
  if (!social.isPrimary) {
    event.preventDefault();
    handleDemoClick(social.name);
  }
};
</script>

<template>
  <footer class="footer">
    <div class="footer-container">
      <!-- Newsletter Section -->
      <div class="newsletter-section glass-element">
        <div class="newsletter-content">
          <h3>Elevate Your Wardrobe</h3>
          <p>
            Subscribe to receive exclusive offers, early access to new
            collections, and style inspiration.
          </p>
        </div>
        <form class="newsletter-form" @submit.prevent="handleSubscribe">
          <div class="input-wrapper">
            <input
              type="email"
              placeholder="email@example.com"
              class="newsletter-input"
              required
            />
            <button type="submit" class="newsletter-btn">Subscribe</button>
          </div>
        </form>
      </div>

      <!-- Main Footer Grid -->
      <div class="footer-grid">
        <!-- Brand Column -->
        <div class="footer-col brand-col">
          <router-link to="/" class="brand-logo">VueSharp</router-link>
          <p class="tagline">
            Redefining modern elegance with a minimalist touch. Your curated
            destination for premium essentials.
          </p>
          <div class="social-links">
            <a
              v-for="social in socialLinks"
              :key="social.name"
              href="#"
              class="social-icon"
              :aria-label="social.name"
              @click.prevent="handleDemoClick(social.name)"
            >
              <span v-if="social.name === 'Instagram'">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="20"
                  height="20"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="1.5"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                >
                  <rect x="2" y="2" width="20" height="20" rx="5" ry="5"></rect>
                  <path
                    d="M16 11.37A4 4 0 1 1 12.63 8 4 4 0 0 1 16 11.37z"
                  ></path>
                  <line x1="17.5" y1="6.5" x2="17.51" y2="6.5"></line>
                </svg>
              </span>
              <span v-else-if="social.name === 'Pinterest'">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="20"
                  height="20"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="1.5"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                >
                  <line x1="12" y1="8" x2="12" y2="16"></line>
                  <line x1="8" y1="12" x2="16" y2="12"></line>
                  <circle cx="12" cy="12" r="10"></circle>
                </svg>
              </span>
              <span v-else>
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="20"
                  height="20"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="1.5"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                >
                  <path
                    d="M23 3a10.9 10.9 0 0 1-3.14 1.53 4.48 4.48 0 0 0-7.86 3v1A10.66 10.66 0 0 1 3 4s-4 9 5 13a11.64 11.64 0 0 1-7 2c9 5 20 0 20-11.5a4.5 4.5 0 0 0-.08-.83A7.72 7.72 0 0 0 23 3z"
                  ></path>
                </svg>
              </span>
            </a>
          </div>
        </div>

        <!-- Shop Column -->
        <div class="footer-col">
          <h4>Shop</h4>
          <ul>
            <li v-for="link in shopLinks" :key="link.name">
              <router-link :to="link.href">{{ link.name }}</router-link>
            </li>
          </ul>
        </div>

        <!-- Support Column -->
        <div class="footer-col">
          <h4>Support</h4>
          <ul>
            <li v-for="link in supportLinks" :key="link.name">
              <router-link v-if="link.name === 'Order Tracking'" :to="link.href">
                {{ link.name }}
              </router-link>
              <a v-else href="#" @click.prevent="handleDemoClick(link.name)">
                {{ link.name }}
              </a>
            </li>
          </ul>
        </div>

        <!-- Legal Column -->
        <div class="footer-col">
          <h4>Legal</h4>
          <ul>
            <li v-for="link in legalLinks" :key="link.name">
              <a href="#" @click.prevent="handleDemoClick(link.name)">{{
                link.name
              }}</a>
            </li>
          </ul>
        </div>
      </div>

      <!-- Bottom Bar -->
      <div class="footer-bottom">
        <p>&copy; {{ currentYear }} VueSharp. All rights reserved.</p>
        <div class="payment-icons">
          <!-- Placeholder for payment icons -->
          <div class="payment-dot"></div>
          <div class="payment-dot"></div>
          <div class="payment-dot"></div>
          <div class="payment-dot"></div>
        </div>
      </div>

      <!-- Developer Bar -->
      <div class="developer-bar">
        <p>
          Crafted with precision by
          <span class="dev-name">Jakub Barszczak</span>. Explore my
          <a
            href="https://jakubbarszczak.vercel.app/"
            target="_blank"
            rel="noopener noreferrer"
            class="dev-link"
            >Portfolio</a
          >
          or view the source on
          <a
            href="https://github.com/jkbx1"
            target="_blank"
            rel="noopener noreferrer"
            class="dev-link"
            >GitHub</a
          >.
        </p>
      </div>
    </div>
  </footer>
</template>

<style scoped>
.footer {
  background-color: var(--bg-color, #121212);
  border-top: 1px solid var(--border, #2e303a);
  padding: 80px 0 40px;
  margin-top: auto;
  position: relative;
  overflow: hidden;
}

.footer-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 40px;
}

/* Newsletter Section */
.newsletter-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 40px 60px;
  margin-bottom: 80px;
  gap: 40px;
  background: var(--universal-tint);
  -webkit-backdrop-filter: blur(var(--universal-blur));
  backdrop-filter: blur(var(--universal-blur));
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 24px;
}

.newsletter-content h3 {
  font-family: "Playfair Display", serif;
  font-size: 32px;
  color: var(--text-h);
  margin-bottom: 8px;
}

.newsletter-content p {
  color: var(--text-main);
  opacity: 0.8;
  max-width: 500px;
}

.newsletter-form {
  flex-shrink: 0;
}

.input-wrapper {
  display: flex;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 50px;
  padding: 6px;
  width: 400px;
  transition: border-color 0.3s ease;
}

.input-wrapper:focus-within {
  border-color: var(--accent);
}

.newsletter-input {
  background: transparent;
  border: none;
  padding: 10px 20px;
  color: white;
  width: 100%;
  font-family: "Inter", sans-serif;
  font-size: 14px;
}

.newsletter-input:focus {
  outline: none;
}

.newsletter-btn {
  background: var(--accent);
  color: white;
  border: none;
  border-radius: 50px;
  padding: 10px 30px;
  font-family: "Inter", sans-serif;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.22, 1, 0.36, 1);
}

.newsletter-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(130, 102, 68, 0.3);
}

/* Footer Grid */
.footer-grid {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr 1fr;
  gap: 60px;
  margin-bottom: 80px;
}

.brand-col .brand-logo {
  font-family: "Playfair Display", serif;
  font-size: 28px;
  color: var(--text-h);
  text-decoration: none;
  display: block;
  margin-bottom: 20px;
  letter-spacing: 1px;
}

.brand-col .tagline {
  font-size: 15px;
  line-height: 1.6;
  opacity: 0.7;
  margin-bottom: 30px;
  max-width: 350px;
}

.social-links {
  display: flex;
  gap: 20px;
}

.social-icon {
  color: var(--text-main);
  opacity: 0.6;
  transition: all 0.3s ease;
}

.social-icon:hover {
  color: var(--accent);
  opacity: 1;
  transform: translateY(-3px);
}

.social-icon.primary-link {
  color: var(--accent);
  opacity: 1;
  position: relative;
}

.social-icon.primary-link::after {
  content: "";
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 32px;
  height: 32px;
  background: var(--accent);
  border-radius: 50%;
  filter: blur(12px);
  opacity: 0.15;
  z-index: -1;
  transition: opacity 0.3s ease;
}

.social-icon.primary-link:hover::after {
  opacity: 0.3;
}

.footer-col h4 {
  font-size: 14px;
  text-transform: uppercase;
  letter-spacing: 2px;
  color: var(--text-h);
  margin-bottom: 25px;
}

.footer-col ul {
  list-style: none;
  padding: 0;
}

.footer-col ul li {
  margin-bottom: 12px;
}

.footer-col ul li a {
  text-decoration: none;
  color: var(--text-main);
  font-size: 15px;
  opacity: 0.7;
  transition: all 0.3s ease;
}

.footer-col ul li a:hover {
  color: var(--accent);
  opacity: 1;
  padding-left: 5px;
}

/* Developer Bar */
.developer-bar {
  margin-top: 30px;
  padding-top: 20px;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
  text-align: center;
}

.developer-bar p {
  font-size: 14px;
  color: var(--text-main);
  opacity: 0.6;
  letter-spacing: 0.5px;
}

.dev-name {
  color: var(--text-h);
  font-weight: 500;
}

.dev-link {
  color: var(--accent);
  text-decoration: none;
  font-weight: 600;
  margin: 0 4px;
  position: relative;
  transition: all 0.3s ease;
}

.dev-link::after {
  content: "";
  position: absolute;
  bottom: -2px;
  left: 0;
  width: 0;
  height: 1px;
  background: var(--accent);
  transition: width 0.3s ease;
}

.dev-link:hover {
  opacity: 1;
  text-shadow: 0 0 8px rgba(130, 102, 68, 0.4);
}

.dev-link:hover::after {
  width: 100%;
}

/* Bottom Bar */
.footer-bottom {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 40px;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
}

.footer-bottom p {
  font-size: 13px;
  opacity: 0.5;
}

.payment-icons {
  display: flex;
  gap: 12px;
}

.payment-dot {
  width: 32px;
  height: 20px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 4px;
}

/* Responsive */
@media (max-width: 1024px) {
  .footer-grid {
    grid-template-columns: 1.5fr 1fr 1fr;
    gap: 40px;
  }

  .legal-col {
    grid-column: span 3;
  }

  .newsletter-section {
    flex-direction: column;
    text-align: center;
    padding: 40px;
  }

  .input-wrapper {
    width: 100%;
    max-width: 500px;
  }
}

@media (max-width: 768px) {
  .footer {
    padding: 60px 0 30px;
  }

  .footer-container {
    padding: 0 24px;
  }

  .footer-grid {
    grid-template-columns: 1fr;
    gap: 50px;
    text-align: center;
  }

  .brand-col .tagline {
    margin-left: auto;
    margin-right: auto;
  }

  .social-links {
    justify-content: center;
  }

  .footer-bottom {
    flex-direction: column;
    gap: 20px;
  }

  .newsletter-content h3 {
    font-size: 24px;
  }
}
</style>
