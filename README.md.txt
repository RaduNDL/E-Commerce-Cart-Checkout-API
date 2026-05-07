# E-Commerce Cart & Checkout API

A fullstack e-commerce application built with **ASP.NET Core** (.NET backend) and **Angular** (frontend), using **MS SQL Server** as the database.

## Tech Stack

- **Backend:** ASP.NET Core Web API, ADO.NET (no ORM), JWT Authentication, BCrypt
- **Frontend:** Angular 21, Bootstrap 5, RxJS BehaviorSubject
- **Database:** Microsoft SQL Server
- **Tests:** xUnit (backend)

---

## Prerequisites

Make sure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) and npm
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (any edition)
- [SQL Server Management Studio (SSMS)](https://aka.ms/ssmsfullsetup)
- [Angular CLI](https://angular.io/cli): `npm install -g @angular/cli`

---

## 1. Restore the Database

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your local SQL Server instance
3. Right-click **Databases** → **Restore Database...**
4. Select **Device** → click `...` → **Add** → browse to `ECommerceApi.bak` in the root of this repository
5. Click **OK** → **OK** to restore
6. The database `ECommerceApi` will appear in your Databases list with all tables and data populated

Alternatively, run this query in SSMS (adjust the path):

```sql
RESTORE DATABASE ECommerceApi
FROM DISK = 'C:\path\to\ECommerceApi.bak'
WITH REPLACE;
```

---

## 2. Configure & Run the Backend

### Connection String

Open `ECommerceApi/ECommerceApi/appsettings.json` and update the connection string to match your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ECommerceApi;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyHereMustBe32Chars!!",
    "Issuer": "ECommerceApi"
  }
}
```

Replace `YOUR_SERVER_NAME` with your SQL Server instance name (e.g. `localhost`, `.\SQLEXPRESS`, or `DESKTOP-XXXX`).

### Run

```bash
cd ECommerceApi/ECommerceApi
dotnet restore
dotnet run
```

The API will start on `http://localhost:5025`.

You can verify it's working by opening: `http://localhost:5025/api/products`

---

## 3. Run the Frontend

```bash
cd FrontEnd/ecommerce-frontend
npm install
ng serve
```

The Angular app will be available at `http://localhost:4200`.

---

## 4. Run Backend Unit Tests

```bash
cd ECommerceApi/ECommerceApi.Tests
dotnet test
```

Expected output: **7 passed**.

---

## API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/register` | No | Register a new user |
| POST | `/api/auth/login` | No | Login and receive JWT token |
| GET | `/api/products` | No | Get all products |
| GET | `/api/products/{id}` | No | Get product by ID |
| POST | `/api/orders/checkout` | Yes (JWT) | Place an order |

---

## Project Structure

```
E-Commerce-Cart-Checkout-API/
├── ECommerceApi/                  # .NET Backend
│   ├── ECommerceApi/              # Main API project
│   │   ├── Controllers/           # AuthController, ProductsController, OrdersController
│   │   ├── Models/                # Product, Order, OrderItem, User, DTOs
│   │   ├── Repositories/          # Repository pattern (no ORM)
│   │   └── Program.cs             # App configuration, DI, CORS, JWT
│   └── ECommerceApi.Tests/        # xUnit tests (7 tests)
├── FrontEnd/
│   └── ecommerce-frontend/        # Angular 21 SPA
│       └── src/app/
│           ├── components/        # ProductList, Cart, Checkout, Login, Register, Navbar
│           ├── services/          # AuthService, CartService, ProductService
│           └── models/            # TypeScript interfaces
└── ECommerceApi.bak               # SQL Server database backup (with populated products)
```