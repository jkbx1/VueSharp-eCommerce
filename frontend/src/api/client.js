/**
 * VueSharp — Centralized API Client
 *
 * The single source of truth for the backend base URL.
 * All API modules and views must import BASE_URL from here.
 * The actual URL is injected at build time via Vite's env system:
 *   - Local dev:  .env              → http://localhost:5012
 *   - Production: .env.production   → https://api.yourdomain.com
 */
export const BASE_URL = import.meta.env.VITE_API_BASE_URL;

if (!BASE_URL) {
  console.error(
    '[VueSharp] VITE_API_BASE_URL is not defined. ' +
    'Make sure a .env or .env.production file exists with this variable set.'
  );
}
