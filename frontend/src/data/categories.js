// Centralized category definitions for Men, Women, and Kids sections.
// Each category has a slug (used in routes), a display label, a gradient, and
// a list of filters available when browsing that subcategory.

export const sections = {
  men: {
    label: 'Men',
    path: '/men',
    categories: [
      {
        slug: 'tshirts',
        label: 'T-Shirts',
        icon: '👕',
        image: '/images/categories/men/tshirts.jpg',
        gradient: 'linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'shirts',
        label: 'Shirts',
        icon: '👔',
        image: '/images/categories/men/shirts.jpg',
        gradient: 'linear-gradient(135deg, #0d1b2a 0%, #1b263b 50%, #415a77 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'jeans',
        label: 'Jeans',
        icon: '👖',
        image: '/images/categories/men/jeans.jpg',
        gradient: 'linear-gradient(135deg, #0a0908 0%, #22333b 50%, #32648a 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'accessories',
        label: 'Accessories',
        icon: '⌚',
        image: '/images/categories/men/accessories.jpg',
        gradient: 'linear-gradient(135deg, #1c0221 0%, #3b1f4e 50%, #6a1e8c 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'sale',
        label: 'Sale',
        icon: '🏷️',
        image: '/images/categories/men/sale.jpg',
        gradient: 'linear-gradient(135deg, #1a0000 0%, #7f0000 50%, #cc0000 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'novelties',
        label: 'New Arrivals',
        icon: '✨',
        image: '/novelties_icon.png', 
        gradient: 'linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #334155 100%)',
        accent: '#F2CC8F',
      },
    ],
  },

  women: {
    label: 'Women',
    path: '/women',
    categories: [
      {
        slug: 'dresses',
        label: 'Dresses',
        icon: '👗',
        image: '/images/categories/women/dresses.jpg',
        gradient: 'linear-gradient(135deg, #2d0036 0%, #6a0572 50%, #ab83a1 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'tops',
        label: 'Tops',
        icon: '👚',
        image: '/images/categories/women/tops.jpg',
        gradient: 'linear-gradient(135deg, #1a0533 0%, #44107a 50%, #991b81 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'pants',
        label: 'Pants',
        icon: '🩱',
        image: '/images/categories/women/pants.jpg',
        gradient: 'linear-gradient(135deg, #12032e 0%, #2c1654 50%, #534294 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'premium-collection',
        label: 'Premium Collection',
        icon: '✨',
        image: '/images/categories/women/premium-collection.jpg',
        gradient: 'linear-gradient(135deg, #0d0d0d 0%, #2a1a0e 50%, #8b6914 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'sale',
        label: 'Sale',
        icon: '🏷️',
        image: '/images/categories/women/sale.jpg',
        gradient: 'linear-gradient(135deg, #1a0000 0%, #7f0000 50%, #cc0000 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'novelties',
        label: 'New Arrivals',
        icon: '✨',
        image: '/novelties_icon.png',
        gradient: 'linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #415a77 100%)',
        accent: '#F2CC8F',
      },
    ],
  },

  kids: {
    label: 'Kids',
    path: '/kids',
    categories: [
      {
        slug: 'newborn',
        label: 'Newborn',
        icon: '🍼',
        image: '/images/categories/kids/newborn.jpg',
        gradient: 'linear-gradient(135deg, #fff0f5 0%, #ffd6e0 50%, #ffb3c6 100%)',
        accent: '#F2CC8F',
        dark: true,
      },
      {
        slug: 'girl',
        label: 'Girl',
        icon: '🎀',
        image: '/images/categories/kids/girl.jpg',
        gradient: 'linear-gradient(135deg, #1a0331 0%, #7b2d8b 50%, #c44dff 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'boy',
        label: 'Boy',
        icon: '🚀',
        image: '/images/categories/kids/boy.jpg',
        gradient: 'linear-gradient(135deg, #001a33 0%, #003d80 50%, #0077cc 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'shoes',
        label: 'Shoes',
        icon: '👟',
        image: '/images/categories/kids/shoes.jpg',
        gradient: 'linear-gradient(135deg, #0a0a0a 0%, #1e3040 50%, #285f7e 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'sale',
        label: 'Sale',
        icon: '🏷️',
        image: '/images/categories/kids/sale.jpg',
        gradient: 'linear-gradient(135deg, #1a0000 0%, #7f0000 50%, #cc0000 100%)',
        accent: '#F2CC8F',
      },
      {
        slug: 'novelties',
        label: 'New Arrivals',
        icon: '✨',
        image: '/novelties_icon.png',
        gradient: 'linear-gradient(135deg, #0b1527 0%, #142a4a 50%, #1e416e 100%)',
        accent: '#F2CC8F',
      },
    ],
  },
};

/**
 * Util: find a category by section slug + category slug.
 */
export function findCategory(sectionSlug, categorySlug) {
  const section = sections[sectionSlug];
  if (!section) return null;
  return section.categories.find((c) => c.slug === categorySlug) ?? null;
}
