# 🛒 Retail Ordering System

A complete, beginner-friendly **ASP.NET Core 8 Web API** for a retail ordering system featuring Pizza, Cold Drinks, and Bread products.

Built for **learning**, **hackathon preparation**, and **interview preparation**.

---

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full SQL Server)
- Visual Studio 2022 or VS Code

### Run the Application

```bash
# 1. Clone / open the project
cd RetailOrderingSystem

# 2. Update connection string in appsettings.json
# Default uses SQL Server LocalDB — works out of the box

# 3. Run the application (migrations + seeding happen automatically)
dotnet run

# 4. Open Swagger UI
# https://localhost:{port}/swagger
```

### Default Credentials (Auto-Seeded)

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@retail.com | Admin@123 |
| Delivery Partner | delivery@retail.com | Delivery@123 |
| Customer | Register via `/api/auth/register` | Your choice |

---

## 🏗️ Architecture

```
RetailOrderingSystem/
├── Controllers/        → HTTP endpoints (thin layer, no business logic)
├── Services/           → Business logic (all rules live here)
├── Interfaces/         → Contracts for services and repositories
├── Repositories/       → Generic data access (EF Core)
├── Models/             → EF Core entities (map to DB tables)
├── DTOs/               → Request/Response shapes (separate from entities)
├── Data/               → AppDbContext (EF Core DbContext)
├── Middleware/         → Global exception handler, request logger
├── Helpers/            → JWT generator, BCrypt password helper
├── Validators/         → FluentValidation rules
├── Constants/          → App-wide constants (roles, statuses, rules)
├── SeedData/           → Database seeder (runs on startup)
└── Program.cs          → DI registration, middleware pipeline, startup
```

### Request Flow
```
Client → Middleware → Auth → Controller → Service → DbContext → SQL Server
```

---

## 🔐 Authentication

- **JWT Bearer Tokens** — 60-minute expiry
- **Refresh Tokens** — 7-day expiry, stored in DB
- **BCrypt** — Password hashing with work factor 12
- **Roles** — Admin, Customer, DeliveryPartner

### Login Flow
1. POST `/api/auth/login` → receive `accessToken` + `refreshToken`
2. Add header: `Authorization: Bearer {accessToken}`
3. When expired: POST `/api/auth/refresh` with `refreshToken`

---

## 📦 API Endpoints

### Auth
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | /api/auth/register | None | Register customer |
| POST | /api/auth/login | None | Login |
| POST | /api/auth/refresh | None | Refresh token |
| GET | /api/auth/profile | JWT | Get profile |
| PUT | /api/auth/profile | JWT | Update profile |
| PUT | /api/auth/change-password | JWT | Change password |
| GET | /api/auth/customers | Admin | List customers |

### Products
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | /api/products | None | Browse (search/filter) |
| GET | /api/products/{id} | None | Get by ID |
| POST | /api/products | Admin | Create |
| PUT | /api/products/{id} | Admin | Update |
| PATCH | /api/products/{id}/toggle-active | Admin | Enable/Disable |
| DELETE | /api/products/{id} | Admin | Soft delete |
| GET | /api/products/low-stock | Admin | Low stock alert |

### Cart (Customer only)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/cart | View cart |
| POST | /api/cart/items | Add item |
| PUT | /api/cart/items/{id} | Update quantity |
| DELETE | /api/cart/items/{id} | Remove item |
| DELETE | /api/cart | Clear cart |

### Orders
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | /api/orders | Customer | Place order |
| GET | /api/orders/my-orders | Customer | Order history |
| GET | /api/orders/{id} | Customer/Admin | Order details |
| GET | /api/orders/{id}/track | Customer | Track status |
| POST | /api/orders/{id}/reorder | Customer | Reorder |
| GET | /api/orders/all | Admin | All orders |
| PUT | /api/orders/{id}/status | Admin | Update status |
| PUT | /api/orders/{id}/assign-delivery | Admin | Assign delivery |

### Reports (Admin only)
| Endpoint | Description |
|----------|-------------|
| GET /api/reports/dashboard | Dashboard stats |
| GET /api/reports/sales/daily?date= | Daily sales |
| GET /api/reports/sales/monthly?year=&month= | Monthly sales |
| GET /api/reports/revenue?from=&to= | Revenue range |
| GET /api/reports/top-products | Top 10 products |
| GET /api/reports/top-categories | Categories by revenue |

---

## 🗄️ Database Design

### Tables
- **Users** — All users (Admin, Customer, DeliveryPartner)
- **Categories** — Pizza, Cold Drink, Bread
- **Products** — Items with stock tracking
- **Carts / CartItems** — Shopping cart
- **Orders / OrderItems** — Confirmed purchases
- **Coupons** — Discount codes
- **LoyaltyPoints** — Points per customer
- **InventoryTransactions** — Stock change audit log
- **EmailLogs** — Email send history

### Key Relationships
- User → Cart (One-to-One)
- User → Orders (One-to-Many)
- Category → Products (One-to-Many)
- Cart → CartItems → Products
- Order → OrderItems → Products

---

## 💡 Loyalty Points System

- **Earn**: 10 points per ₹100 spent
- **Redeem**: 100 points = ₹10 discount
- Applied at checkout via `loyaltyPointsToRedeem` in PlaceOrder request

---

## 🎟️ Coupon System

- **Percentage discount**: e.g., 20% off
- **Fixed discount**: e.g., ₹50 off
- **Minimum order**: Required cart total
- **Expiry date**: Auto-expires
- **Usage limit**: Single or multi-use

---

## 🔧 Configuration

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=RetailOrderingDB;..."
  },
  "JwtSettings": {
    "SecretKey": "your-32-char-secret-key-here!!!!"
  },
  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

---

## 🧪 Testing with Swagger

1. Run the app → open `/swagger`
2. Login via `POST /api/auth/login`
3. Copy the `accessToken`
4. Click **Authorize** button → enter `Bearer {token}`
5. Test any endpoint

---

## 📚 Tech Stack

| Technology | Purpose |
|-----------|---------|
| ASP.NET Core 8 | Web API framework |
| Entity Framework Core 8 | ORM (Code First) |
| SQL Server | Database |
| JWT Bearer | Authentication |
| BCrypt.Net | Password hashing |
| FluentValidation | Request validation |
| AutoMapper | DTO mapping |
| MailKit | Email sending |
| AspNetCoreRateLimit | Rate limiting |
| Swashbuckle | Swagger UI |
