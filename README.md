# VueSharp E-Commerce

A full-stack e-commerce application built with Vue 3 and .NET Core.

---

## Tech Stack

| Layer | Technology |
| :--- | :--- |
| Frontend Framework | Vue 3 (Composition API) |
| State Management | Pinia |
| Build Tool | Vite |
| Backend Framework | ASP.NET Core (.NET 10) |
| ORM | Entity Framework Core |
| Database | PostgreSQL (Supabase) |
| Authentication | JWT (JSON Web Tokens) |

---

## Key Features

- **Guest Checkout** — Users can complete a purchase without creating an account.
- **Post-Purchase Order Claiming** — After registering, a guest can associate their previous order with their new account using the order's claim token.
- **JWT Authentication** — Stateless token-based authentication for all protected endpoints.
- **Role-Based Access Control** — Separate authorization policies for `Customer` and `Admin` roles.
- **Rate Limiting** — Server-side rate limiting applied to checkout (`CheckoutLimiter`) to prevent brute force and inventory draining.
- **Database Query Optimization** — N+1 query issues resolved via eager loading (`Include`/`ThenInclude`) and batch product fetching. Read-only queries use `AsNoTracking`. Sorting and pagination are pushed to the database layer.

---

## Project Structure

```
VueSharp/
├── backend/          # ASP.NET Core Web API
│   ├── Controllers/  # API endpoint handlers
│   ├── Data/         # DbContext and EF Core configuration
│   ├── Migrations/   # EF Core database migrations
│   ├── Models/       # Entity and DTO definitions
│   └── Utilities/    # Shared helpers (rate limiter, etc.)
└── frontend/         # Vue 3 / Vite SPA
    └── src/
        ├── components/   # Reusable UI components
        ├── views/        # Page-level components
        ├── stores/       # Pinia state stores
        └── router/       # Vue Router configuration
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18+) and npm
- [PostgreSQL](https://www.postgresql.org/) (or a Supabase project)

### 1. Backend

```bash
cd backend

# Restore dependencies
dotnet restore

# Apply migrations to your database
# Ensure your Connection String is set in environment or User Secrets
dotnet ef database update

# Run the development server (default port: 5012)
dotnet run
```

The API will be available at `http://localhost:5012`.

> **Security Note:** The `appsettings.json` contains placeholders. You **must** set `JWT_SECRET_KEY` and `ConnectionStrings__DefaultConnection` as environment variables for the application to function correctly.

### 2. Frontend

```bash
cd frontend

# Install dependencies
npm install

# Copy the environment template and configure it
cp .env.example .env.local
# Edit .env.local and set VITE_API_BASE_URL to your backend address

# Run the development server (default port: 5173)
npm run dev
```

The frontend will be available at `http://localhost:5173`.

---

## Environment Variables

### Frontend (`frontend/.env.local`)

| Variable | Description | Example |
| :--- | :--- | :--- |
| `VITE_API_BASE_URL` | Base URL of the backend API | `http://localhost:5012` |

### Backend (System Environment Variables)

Backend configuration is managed via environment variables for security.

| Variable | Description | Default / Example |
| :--- | :--- | :--- |
| `ConnectionStrings__DefaultConnection` | PostgreSQL Connection String | `Server=...;Database=...;User Id=...;Password=...;` |
| `JWT_SECRET_KEY` | Secret key for JWT (min. 32 chars) | *[Required Site Secret]* |
| `ALLOWED_ORIGINS` | Comma-separated list of allowed CORS origins | `http://localhost:5173,https://your-app.vercel.app` |
| `Jwt__Issuer` | JWT issuer identifier | `VueSharpBackend` |
| `Jwt__Audience` | JWT audience identifier | `VueSharpFrontend` |

---