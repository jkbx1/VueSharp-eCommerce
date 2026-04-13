/**
 * VueSharp — Centralized API Client
 *
 * The single source of truth for the backend base URL.
 * All API modules and views must import BASE_URL from here.
 * The actual URL is injected at build time via Vite's env system:
 *   - Local dev:  .env              → http://localhost:5012
 *   - Production: .env.production   → https://api.yourdomain.com
 */
// Get the raw value from environment variables
const rawBaseUrl = import.meta.env.VITE_API_BASE_URL || '';

// Sanitize: Remove trailing slash to prevent double-slashes in requests (e.g., https://api.com//api/...)
export const BASE_URL = rawBaseUrl.replace(/\/$/, '');

if (!BASE_URL && import.meta.env.PROD) {
  console.warn(
    '[VueSharp] WARNING: VITE_API_BASE_URL is not defined in production. ' +
    'API requests will fall back to the current domain, which likely won\'t work ' +
    'unless you have a proxy configured on your host (e.g. Netlify/Vercel rewrites).'
  );
} else if (!BASE_URL) {
  console.info('[VueSharp] VITE_API_BASE_URL is empty; using relative paths for local development proxy.');
}
