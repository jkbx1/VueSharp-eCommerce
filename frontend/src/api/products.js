import { BASE_URL } from './client.js';

/**
 * Fetch products from the backend with optional filtering.
 * All business logic (IsOnSale, DiscountPercent, DisplayPrice) is pre-computed server-side.
 *
 * @param {Object} params - Query params: category, subcategory, color, size, onSale, page, pageSize
 * @returns {Promise<Array>}
 */
export async function getProducts(params = {}) {
  const query = new URLSearchParams();
  
  if (Array.isArray(params.category)) {
    params.category.forEach((c) => query.append("category", c));
  } else if (params.category) {
    query.set("category", params.category);
  }

  if (Array.isArray(params.subcategory)) {
    params.subcategory.forEach((s) => query.append("subcategory", s));
  } else if (params.subcategory) {
    query.set("subcategory", params.subcategory);
  }

  if (Array.isArray(params.itemType)) {
    params.itemType.forEach((i) => query.append("itemType", i));
  } else if (params.itemType) {
    query.set("itemType", params.itemType);
  }

  if (Array.isArray(params.color)) {
    params.color.forEach((c) => query.append("color", c));
  } else if (params.color) {
    query.set("color", params.color);
  }

  if (Array.isArray(params.size)) {
    params.size.forEach((s) => query.append("size", s));
  } else if (params.size) {
    query.set("size", params.size);
  }

  if (params.onSale) query.set("onSale", "true");
  if (params.minPrice) query.set("minPrice", params.minPrice);
  if (params.maxPrice) query.set("maxPrice", params.maxPrice);
  if (params.q) query.set("q", params.q);
  
  if (Array.isArray(params.skus)) {
    params.skus.forEach((sku) => query.append("skus", sku));
  } else if (params.skus) {
    query.set("skus", params.skus);
  }

  if (params.sort) query.set("sort", params.sort);
  if (params.limit) query.set("limit", params.limit);
  if (params.page) query.set("page", params.page);
  if (params.pageSize) query.set("pageSize", params.pageSize);

  const res = await fetch(`${BASE_URL}/api/products?${query.toString()}`);
  if (!res.ok) throw new Error(`Failed to fetch products: ${res.status}`);
  return res.json();
}

/**
 * Fetch unique filter options for a category/subcategory (itemType, colors, sizes)
 * based on what's actually in the database.
 * @param {Object} params - { category, subcategory, onSale }
 * @returns {Promise<Object>}
 */
export async function getFilters(params = {}) {
  const query = new URLSearchParams();
  
  if (Array.isArray(params.category)) {
    params.category.forEach((c) => query.append("category", c));
  } else if (params.category) {
    query.set("category", params.category);
  }

  if (Array.isArray(params.subcategory)) {
    params.subcategory.forEach((s) => query.append("subcategory", s));
  } else if (params.subcategory) {
    query.set("subcategory", params.subcategory);
  }

  if (Array.isArray(params.itemType)) {
    params.itemType.forEach((i) => query.append("itemType", i));
  } else if (params.itemType) {
    query.set("itemType", params.itemType);
  }

  if (Array.isArray(params.color)) {
    params.color.forEach((c) => query.append("color", c));
  } else if (params.color) {
    query.set("color", params.color);
  }

  if (Array.isArray(params.size)) {
    params.size.forEach((s) => query.append("size", s));
  } else if (params.size) {
    query.set("size", params.size);
  }

  if (params.onSale) query.set("onSale", "true");
  if (params.minPrice) query.set("minPrice", params.minPrice);
  if (params.maxPrice) query.set("maxPrice", params.maxPrice);
  if (params.q) query.set("q", params.q);

  const res = await fetch(`${BASE_URL}/api/products/filters?${query.toString()}`);
  if (!res.ok) throw new Error(`Failed to fetch filters: ${res.status}`);
  return res.json();
}

/**
 * Fetch a single product by ID.
 * @param {number} id
 * @returns {Promise<Object>}
 */
export async function getProduct(id) {
  const res = await fetch(`${BASE_URL}/api/products/${id}`);
  if (!res.ok) throw new Error(`Failed to fetch product ${id}: ${res.status}`);
  return res.json();
}
