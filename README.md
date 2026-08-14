# HisabDo Web API - Capstone Project

**Day 13 - HisabDo Internship**

A .NET-based web application that mirrors the HisabDo mobile app (Khata/Ledger application).

ASP.NET Core Web API with Clean Architecture, EF Core, SQL Server and working CRUD modules.

**Developer:** Qamar Zaman
**Track:** .NET

> **Also check the [Screenshot folder](screenshots) for all Swagger, Postman and SQL Server test screenshots.**

---

# Day 13 - Authentication & Authorization (Complete)

## Authorization

- **All business APIs are now protected** with `[Authorize]` (Customers, Transactions, Categories, Settings).
- The logged-in user's ID is read from the JWT `sub` claim — every user only sees **their own** data (data isolation).
- **Role-based access**: `GET /api/v1/admin/users` requires the `Admin` role → `403 Forbidden` for normal users, `200` for admins.
- Demo account: `demo@hisabdo.com` / `Demo@123` (Role: Admin).

To call protected endpoints in **Swagger**: click **Authorize**, paste `Bearer <token>`. In **Postman**: Authorization tab → type `Bearer Token` → paste the token.

## How to Run

1. Edit the connection string in `src/HisabDo.API/appsettings.json` if needed:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=HisabDoDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

2. Create the database:

```bash
dotnet ef database update --project src/HisabDo.Infrastructure --startup-project src/HisabDo.API
```

3. Run the API:

```bash
dotnet run --project src/HisabDo.API
```

Open `http://localhost:5181/swagger` to test (the API opens Swagger automatically in the browser).

## Solution Structure (Clean Architecture)

```text
hisabdo-web-api/
├── src/
│   ├── HisabDo.API/            # Controllers, middleware, Program.cs, config
│   ├── HisabDo.Application/    # Services, DTOs, repository interfaces
│   ├── HisabDo.Domain/         # Entities, enums, base class
│   └── HisabDo.Infrastructure/ # EF Core, DbContext, repositories (implementations)
├── tests/                      # (future)
├── docs/ERD.md                 # Database diagram
└── README.md
```

Project references (one direction only):

```text
API -> Application -> Domain
API -> Infrastructure -> Domain
Infrastructure -> Application (implements repository interfaces)
```

## Entities (Domain)

- `User` - account owner (seeded demo user)
- `Customer` - Khata partner
- `Category` - groups transactions (seeded: Sales, Purchase, Rent, Food, Transport, Salary, Others)
- `Transaction` - the core entity (Type: 1 = Receivable, 2 = Payable)
- `Setting` - user preferences

All business tables have `UserId` and use soft delete (`IsDeleted`).

## Working CRUD Modules

### Customers (Day 9)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/v1/customers | List all customers |
| GET | /api/v1/customers/{id} | Get customer by ID |
| POST | /api/v1/customers | Add customer |
| PUT | /api/v1/customers/{id} | Update customer |
| DELETE | /api/v1/customers/{id} | Delete customer (soft) |

### Transactions (Day 9 + Day 11)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/v1/transactions | List transactions with filters (type, customerId, categoryId, fromDate, toDate) |
| GET | /api/v1/categories/{id}/transactions | List transactions of one category (relationship endpoint) |
| GET | /api/v1/transactions/{id} | Get transaction by ID |
| POST | /api/v1/transactions | Add Receivable/Payable |
| PUT | /api/v1/transactions/{id} | Update transaction |
| DELETE | /api/v1/transactions/{id} | Delete transaction (soft) |

Day 11 - the Transactions module was completed as the second core module with the Category relation:
- `Category` (first module) has one-to-many relationship with `Transaction` (second module): one category has many transactions.
- Database relationship: `Transactions.CategoryId` foreign key with `DeleteBehavior.Restrict` (category used by transactions cannot be deleted).
- New relationship endpoint: `GET /api/v1/categories/{id}/transactions`.
- New query filters on the list API: `type` (1 = Receivable, 2 = Payable), `customerId`, `categoryId`, `fromDate`, `toDate`.

### Categories (Day 10)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/v1/categories | List all categories |
| GET | /api/v1/categories/{id} | Get category by ID |
| POST | /api/v1/categories | Add category |
| PUT | /api/v1/categories/{id} | Update category |
| DELETE | /api/v1/categories/{id} | Delete category (soft) |

Category rules:
- Name is required (2-50 characters) and must be unique per user (case-insensitive).
- Seeded default categories (Sales, Purchase, Rent, Food, Transport, Salary, Others) cannot be updated or deleted.
- A category that is used by transactions cannot be deleted.
- HTTP status codes: 200 OK, 201 Created, 204 No Content, 400 Bad Request (validation / rules), 404 Not Found.

### Sample requests

```json
POST /api/v1/customers
{
  "name": "Ahmed Khan",
  "phone": "03123456789",
  "email": "ahmed@example.com",
  "notes": "Shop owner"
}

POST /api/v1/transactions
{
  "customerId": 1,
  "categoryId": 1,
  "type": 1,
  "amount": 5000,
  "note": "Credit sale",
  "transactionDate": "2026-08-08T10:00:00Z"
}

POST /api/v1/categories
{
  "name": "Electricity"
}
```

`type`: 1 = Receivable, 2 = Payable.

### Error handling

All errors return a consistent JSON response:

```json
{ "message": "No customer found with ID: 5" }
```

- 400 Bad Request - invalid operation (e.g. customer/category not found)
- 401 Unauthorized - invalid email/password or missing/invalid token
- 404 Not Found - resource not found
- 500 Internal Server Error - unexpected error

### Settings (Day 12)

One settings row per user (1-to-1 relationship with `User`, unique index on `UserId`).

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/v1/settings | Get current user settings |
| PUT | /api/v1/settings | Create or update settings (upsert) |
| DELETE | /api/v1/settings | Reset/delete settings (soft) |

### Authentication & Authorization (Day 13)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/v1/auth/register | Create account + return JWT token |
| POST | /api/v1/auth/login | Login + return JWT token |
| GET | /api/v1/auth/me | Get current user profile (requires Bearer token) |
| PUT | /api/v1/auth/me | Update current user profile (name, business, phone) |
| POST | /api/v1/auth/change-password | Change password (requires old password) |
| GET | /api/v1/admin/users | List all users (**Admin only**, 403 for User role) |

Authentication architecture:
- Passwords hashed with **BCrypt** (never stored in plain text).
- **Password policy**: 8-64 characters, must include uppercase, lowercase, digit, and special character (applied on register and change-password).
- JWT token contains: `sub` (userId), `email`, `name`, `role` claims; expires after 24 hours (configurable in `appsettings.json` -> `Jwt`).
- **Role-based authorization**: `[Authorize(Roles = "Admin")]` restricts endpoints (e.g. `/api/v1/admin/users` -> 403 for User role).
- All business endpoints (Customers, Transactions, Categories, Settings) require a valid token (401 without it).
- Demo account: `demo@hisabdo.com` / `Demo@123` (Role: Admin).

Registration sample:

```json
POST /api/v1/auth/register
{
  "fullName": "Qamar Zaman",
  "businessName": "My Shop",
  "email": "qamar@example.com",
  "phone": "03001234567",
  "password": "Strong@123"
}
```

Login sample (returns the token):

```json
POST /api/v1/auth/login
{
  "email": "qamar@example.com",
  "password": "Strong@123"
}
```

To call protected endpoints in Swagger click **Authorize** and paste `Bearer <token>`.

### Notes

- Business CRUD controllers read the user ID from the JWT token (`sub` claim) — each user only accesses their own data.
- Feedback report for the product analysis sub-task: [feedback/feedback-report.md](feedback/feedback-report.md)

## Screenshots

### Swagger - Day 9

![Swagger 1](screenshots/Day-9-Task/Swagger_Day_9/Screenshot%202026-08-09%20130002.png)
![Swagger 2](screenshots/Day-9-Task/Swagger_Day_9/Screenshot%202026-08-09%20130141.png)
![Swagger 3](screenshots/Day-9-Task/Swagger_Day_9/Screenshot%202026-08-09%20130214.png)
![Swagger 4](screenshots/Day-9-Task/Swagger_Day_9/Screenshot%202026-08-09%20130229.png)
![Swagger 5](screenshots/Day-9-Task/Swagger_Day_9/Screenshot%202026-08-09%20130320.png)
![Swagger 6](screenshots/Day-9-Task/Swagger_Day_9/Screenshot%202026-08-09%20130342.png)
![Swagger 7](screenshots/Day-9-Task/Swagger_Day_9/Screenshot%202026-08-09%20130411.png)
![Swagger 8](screenshots/Day-9-Task/Swagger_Day_9/Screenshot%202026-08-09%20130935.png)
![Swagger 9](screenshots/Day-9-Task/Swagger_Day_9/Screenshot%202026-08-09%20131008.png)
![Swagger 10](screenshots/Day-9-Task/Swagger_Day_9/Screenshot%202026-08-09%20131031.png)
![Swagger 11](screenshots/Day-9-Task/Swagger_Day_9/Screenshot%202026-08-09%20131137.png)
![Swagger 12](screenshots/Day-9-Task/Swagger_Day_9/Screenshot%202026-08-09%20131201.png)

### Postman - Day 9

![Postman 1](screenshots/Day-9-Task/Postman_Day_9/Screenshot%202026-08-09%20131955.png)
![Postman 2](screenshots/Day-9-Task/Postman_Day_9/Screenshot%202026-08-09%20132106.png)
![Postman 3](screenshots/Day-9-Task/Postman_Day_9/Screenshot%202026-08-09%20132324.png)
![Postman 4](screenshots/Day-9-Task/Postman_Day_9/Screenshot%202026-08-09%20132350.png)
![Postman 5](screenshots/Day-9-Task/Postman_Day_9/Screenshot%202026-08-09%20132906.png)
![Postman 6](screenshots/Day-9-Task/Postman_Day_9/Screenshot%202026-08-09%20134221.png)
![Postman 7](screenshots/Day-9-Task/Postman_Day_9/Screenshot%202026-08-09%20134520.png)
![Postman 8](screenshots/Day-9-Task/Postman_Day_9/Screenshot%202026-08-09%20134539.png)

### SQL Server - Day 9

![SQL Server 1](screenshots/Day-9-Task/SqlServer_Day_9/Screenshot%202026-08-09%20134751.png)
![SQL Server 2](screenshots/Day-9-Task/SqlServer_Day_9/Screenshot%202026-08-09%20134820.png)
![SQL Server 3](screenshots/Day-9-Task/SqlServer_Day_9/Screenshot%202026-08-09%20134841.png)
![SQL Server 4](screenshots/Day-9-Task/SqlServer_Day_9/Screenshot%202026-08-09%20134853.png)
![SQL Server 5](screenshots/Day-9-Task/SqlServer_Day_9/Screenshot%202026-08-09%20134903.png)

### Swagger - Day 10

![Swagger 1](screenshots/Day-10-Task/Swagger_Day_10/Screenshot%202026-08-10%20125722.png)
![Swagger 2](screenshots/Day-10-Task/Swagger_Day_10/Screenshot%202026-08-10%20125746.png)
![Swagger 3](screenshots/Day-10-Task/Swagger_Day_10/Screenshot%202026-08-10%20130148.png)
![Swagger 4](screenshots/Day-10-Task/Swagger_Day_10/Screenshot%202026-08-10%20130212.png)
![Swagger 5](screenshots/Day-10-Task/Swagger_Day_10/Screenshot%202026-08-10%20130316.png)
![Swagger 6](screenshots/Day-10-Task/Swagger_Day_10/Screenshot%202026-08-10%20130347.png)

### Postman - Day 10

![Postman 1](screenshots/Day-10-Task/Postman_Day_10/Screenshot%202026-08-10%20130506.png)
![Postman 2](screenshots/Day-10-Task/Postman_Day_10/Screenshot%202026-08-10%20130627.png)
![Postman 3](screenshots/Day-10-Task/Postman_Day_10/Screenshot%202026-08-10%20130657.png)
![Postman 4](screenshots/Day-10-Task/Postman_Day_10/Screenshot%202026-08-10%20130858.png)
![Postman 5](screenshots/Day-10-Task/Postman_Day_10/Screenshot%202026-08-10%20131544.png)
![Postman 6](screenshots/Day-10-Task/Postman_Day_10/Screenshot%202026-08-10%20131601.png)
![Postman 7](screenshots/Day-10-Task/Postman_Day_10/Screenshot%202026-08-10%20131620.png)

### SQL Server - Day 10

![SQL Server 1](screenshots/Day-10-Task/SqlServer_Day_10/Screenshot%202026-08-10%20131939.png)
![SQL Server 2](screenshots/Day-10-Task/SqlServer_Day_10/Screenshot%202026-08-10%20131953.png)

### Swagger - Day 11

![Swagger 1](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20142952.png)
![Swagger 2](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20143016.png)
![Swagger 3](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20143818.png)
![Swagger 4](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20143835.png)
![Swagger 5](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20144109.png)
![Swagger 6](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20144148.png)
![Swagger 7](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20144312.png)
![Swagger 8](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20144325.png)
![Swagger 9](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20144452.png)
![Swagger 10](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20144513.png)
![Swagger 11](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20145051.png)
![Swagger 12](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20145058.png)
![Swagger 13](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20150126.png)
![Swagger 14](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20150206.png)
![Swagger 15](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20150259.png)
![Swagger 16](screenshots/Day-11-Task/Swagger_Day_11/Screenshot%202026-08-11%20150456.png)

### Postman - Day 11

![Postman 1](screenshots/Day-11-Task/Postman_Day_11/Screenshot%202026-08-11%20150933.png)
![Postman 2](screenshots/Day-11-Task/Postman_Day_11/Screenshot%202026-08-11%20151721.png)
![Postman 3](screenshots/Day-11-Task/Postman_Day_11/Screenshot%202026-08-11%20151752.png)
![Postman 4](screenshots/Day-11-Task/Postman_Day_11/Screenshot%202026-08-11%20151819.png)
![Postman 5](screenshots/Day-11-Task/Postman_Day_11/Screenshot%202026-08-11%20153553.png)
![Postman 6](screenshots/Day-11-Task/Postman_Day_11/Screenshot%202026-08-11%20153951.png)
![Postman 7](screenshots/Day-11-Task/Postman_Day_11/Screenshot%202026-08-11%20154859.png)
![Postman 8](screenshots/Day-11-Task/Postman_Day_11/Screenshot%202026-08-11%20154918.png)
![Postman 9](screenshots/Day-11-Task/Postman_Day_11/Screenshot%202026-08-11%20154946.png)

### SQL Server - Day 11

![SQL Server 1](screenshots/Day-11-Task/SqlServer_Day_11/Screenshot%202026-08-11%20155531.png)

### Swagger - Day 13

![Swagger 1](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20151509.png)
![Swagger 2](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20151518.png)
![Swagger 3](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20153559.png)
![Swagger 4](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20153636.png)
![Swagger 5](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20155227.png)
![Swagger 6](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20155257.png)
![Swagger 7](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20155402.png)
![Swagger 8](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20155409.png)
![Swagger 9](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20163828.png)
![Swagger 10](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20163839.png)
![Swagger 11](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20163857.png)
![Swagger 12](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20163924.png)
![Swagger 13](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20164024.png)
![Swagger 14](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20164054.png)
![Swagger 15](screenshots/Day-13-Task/Swagger_Day_13/Screenshot%202026-08-13%20170005.png)

### SQL Server - Day 13

![SQL Server 1](screenshots/Day-13-Task/SqlServer_Day_13/Screenshot%202026-08-13%20170844.png)

