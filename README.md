# HisabDo Web API - Capstone Project

**Day 14 - HisabDo Internship**

A .NET-based web application that mirrors the HisabDo mobile app (Khata/Ledger application).

ASP.NET Core Web API with Clean Architecture, EF Core, SQL Server and working CRUD modules.

**Developer:** Qamar Zaman
**Track:** .NET

> **Also check the [Screenshot folder](screenshots) for all Swagger, Postman and SQL Server test screenshots.**

---

# Day 14 - Authentication & Authorization (Complete)

## Authorization

- **All business APIs are now protected** with `[Authorize]` (Customers, Transactions, Categories, Settings).
- The logged-in user's ID is read from the JWT `sub` claim - every user only sees **their own** data (data isolation).
- **Role-based access**: `GET /api/v1/admin/users` requires the `Admin` role -> `403 Forbidden` for normal users, `200` for admins.
- Demo account: `demo@hisabdo.com` / `Demo@123` (Role: Admin).

To call protected endpoints in **Swagger**: click **Authorize**, paste `Bearer <token>`. In **Postman**: Authorization tab -> type `Bearer Token` -> paste the token.

# Day 15-16 - Reports, Database Improvements & Finalized Auth (Complete)

## Reports / Dashboard

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/v1/reports/summary?period=week\|month\|3months\|year | Dashboard totals with period filter (requires token) |
| GET | /api/v1/reports/by-category | Per-category breakdown (requires token) |

## Database improvements

- **Indexes added** for report performance: `(UserId, Type)` and `(UserId, Type, TransactionDate)` on Transactions.
- **Unique index** on `(UserId, Name)` for Categories - enforces one category name per user at the database level.
- Amounts stored as `decimal(18,2)`.

## Authentication finalized

- JWT register/login, role-based authorization (`Admin` / `User`), user profile APIs (`GET/PUT /auth/me`, `POST /auth/change-password`), password policy validation - all verified end-to-end.
- Demo account: `demo@hisabdo.com` / `Demo@123` (Admin).

# Day 17 - Documentation & Progress Report

- **Postman collection**: import [docs/HisabDo-API.postman_collection.json](docs/HisabDo-API.postman_collection.json) into Postman - run "Auth - Login" first, the token is saved automatically and applied to every request.
- **Progress report**: [docs/ProgressReport-Day15-17.md](docs/ProgressReport-Day15-17.md) - team members, assigned modules, progress, challenges and blockers.

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
|-- src/
|   |-- HisabDo.API/            # Controllers, middleware, Program.cs, config
|   |-- HisabDo.Application/    # Services, DTOs, repository interfaces
|   |-- HisabDo.Domain/         # Entities, enums, base class
|   `-- HisabDo.Infrastructure/ # EF Core, DbContext, repositories (implementations)
|-- tests/                      # (future)
|-- docs/ERD.md                 # Database diagram
`-- README.md
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

### Customers

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/v1/customers?page=1&pageSize=50 | List all customers (paginated) |
| GET | /api/v1/customers/{id} | Get customer by ID |
| POST | /api/v1/customers | Add customer |
| PUT | /api/v1/customers/{id} | Update customer |
| DELETE | /api/v1/customers/{id} | Delete customer (soft) |

### Transactions

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/v1/transactions?Search=&Page=1&PageSize=50 | List transactions (paginated + search) |
| GET | /api/v1/transactions?Type=1&CustomerId=1&CategoryId=1&FromDate=&ToDate= | Filter by type, customer, category, date range |
| GET | /api/v1/categories/{id}/transactions | List transactions of one category |
| GET | /api/v1/transactions/{id} | Get transaction by ID |
| POST | /api/v1/transactions | Add Receivable/Payable |
| PUT | /api/v1/transactions/{id} | Update transaction |
| DELETE | /api/v1/transactions/{id} | Delete transaction (soft) |

### Categories

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/v1/categories?page=1&pageSize=50 | List all categories (paginated) |
| GET | /api/v1/categories/{id} | Get category by ID (ownership verified) |
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

### Error handling (RFC 7807 ProblemDetails)

All errors return a consistent **ProblemDetails** JSON response with `title`, `status`, `detail` and `traceId`:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5",
  "title": "Bad request",
  "status": 400,
  "detail": "Transaction date cannot be in the future.",
  "traceId": "0HNNUI07M2B14:00000001"
}
```

- 400 Bad Request - invalid operation (e.g. customer/category not found, future transaction date, bad amount)
- 401 Unauthorized - invalid email/password or missing/invalid token
- 404 Not Found - resource not found
- 500 Internal Server Error - unexpected error (generic message, real detail logged)

### API validation

- Model validation via DataAnnotations (`[Required]`, `[Range]`, `[StringLength]`, `[EmailAddress]`) on all create/update DTOs.
- Amount must be greater than 0, transaction type must be 1 or 2, transaction date cannot be in the future.
- Service-level checks: customer/category must exist before creating a transaction; category names are unique per user; default category cannot be updated or deleted; a category used by transactions cannot be deleted.

### Settings (Day 12)

One settings row per user (1-to-1 relationship with `User`, unique index on `UserId`).

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/v1/settings | Get current user settings |
| PUT | /api/v1/settings | Create or update settings (upsert) |
| DELETE | /api/v1/settings | Reset/delete settings (soft) |

### Authentication & Authorization (Day 14)

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

- Business CRUD controllers read the user ID from the JWT token (`sub` claim) - each user only accesses their own data.
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

### Swagger - Day 14

![Swagger 1](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20163638.png)
![Swagger 2](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20163658.png)
![Swagger 3](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20163809.png)
![Swagger 4](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20165217.png)
![Swagger 5](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20165227.png)
![Swagger 6](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20165400.png)
![Swagger 7](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20165417.png)
![Swagger 8](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20165502.png)
![Swagger 9](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20165537.png)
![Swagger 10](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20165659.png)
![Swagger 11](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20165714.png)
![Swagger 12](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20165817.png)
![Swagger 13](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20165936.png)
![Swagger 14](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20165951.png)
![Swagger 15](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20170015.png)
![Swagger 16](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20170042.png)
![Swagger 17](screenshots/Day-14-Task/Swagger_Day_14/Screenshot%202026-08-14%20170125.png)

### SQL Server - Day 14

![SQL Server 1](screenshots/Day-14-Task/SqlServer_Day_14/Screenshot%202026-08-13%20170844.png)
![SQL Server 2](screenshots/Day-14-Task/SqlServer_Day_14/Screenshot%202026-08-14%20170755.png)
![SQL Server 3](screenshots/Day-14-Task/SqlServer_Day_14/Screenshot%202026-08-14%20170814.png)

### Swagger - Day 15-16

![Swagger 1](screenshots/Day-15-16-Task/Swagger_Day_15-16/Screenshot%202026-08-17%20150626.png)
![Swagger 2](screenshots/Day-15-16-Task/Swagger_Day_15-16/Screenshot%202026-08-17%20150643.png)
![Swagger 3](screenshots/Day-15-16-Task/Swagger_Day_15-16/Screenshot%202026-08-17%20150736.png)
![Swagger 4](screenshots/Day-15-16-Task/Swagger_Day_15-16/Screenshot%202026-08-17%20151016.png)
![Swagger 5](screenshots/Day-15-16-Task/Swagger_Day_15-16/Screenshot%202026-08-17%20151047.png)

### SQL Server - Day 15-16

![SQL Server 1](screenshots/Day-15-16-Task/SqlServer_Day_15-16/Screenshot%202026-08-17%20151452.png)
![SQL Server 2](screenshots/Day-15-16-Task/SqlServer_Day_15-16/Screenshot%202026-08-17%20151525.png)

## Day 18-20 - Validation, Error Handling & DTO Improvements

- **RFC 7807 ProblemDetails** error responses (`title`, `status`, `detail`, `traceId`) for all error paths.
- **API validation**: transaction date cannot be in the future (create + update), plus existing DataAnnotations checks on all DTOs.
- **DTO improvements**: `CreatedAt` added to transaction, customer and category responses for better UI ordering/display.
- Verified via automated HTTP tests: future date -> 400, bad amount -> 400, missing category -> 400, missing customer -> 404, no token -> 401, valid create -> 201.

### Swagger - Day 18-20

![Swagger 1](screenshots/Day-18-20-Task/Swagger_Day_18-20/Screenshot%202026-08-20%20212413.png)
![Swagger 2](screenshots/Day-18-20-Task/Swagger_Day_18-20/Screenshot%202026-08-20%20212959.png)
![Swagger 3](screenshots/Day-18-20-Task/Swagger_Day_18-20/Screenshot%202026-08-20%20213010.png)
![Swagger 4](screenshots/Day-18-20-Task/Swagger_Day_18-20/Screenshot%202026-08-20%20213128.png)
![Swagger 5](screenshots/Day-18-20-Task/Swagger_Day_18-20/Screenshot%202026-08-20%20213143.png)
![Swagger 6](screenshots/Day-18-20-Task/Swagger_Day_18-20/Screenshot%202026-08-20%20213227.png)
![Swagger 7](screenshots/Day-18-20-Task/Swagger_Day_18-20/Screenshot%202026-08-20%20213329.png)
![Swagger 8](screenshots/Day-18-20-Task/Swagger_Day_18-20/Screenshot%202026-08-20%20213504.png)

## Bug Fixes (Day 21-22)

- **BOLA security fix** — all CRUD endpoints now verify resource ownership (userId check on update/delete)
- **Invalid token handling** — `GetUserId()` now throws `UnauthorizedAccessException` instead of returning 0
- **Soft-delete filters** — reports and admin list now exclude soft-deleted records
- **Cross-user protection** — transactions can only reference the user's own customers/categories
- **Exception messages** — production mode hides internal error details; only shows generic messages
- **RFC type URLs** — ProblemDetails now returns correct type for each status code
- **Email normalization** — registration stores emails in lowercase
- **Same password check** — changing to the same password is now rejected
- **Race condition protection** — category creation catches database constraint violations

# Day 22-24 - Backend Stabilization (Complete)

## Bug Fixes (Additional)

- **Category BOLA fix (Bug 12)** — `GetByIdAsync` now filters by both userId AND id; cross-user category access returns 404
- **Rate limiting (Bug 13/14)** — ASP.NET Core built-in rate limiting: 100 req/min (general), 10 req/min (auth endpoints). Returns 429 Too Many Requests
- **JWT secret validation (Bug 15)** — Startup check: minimum 32 characters required, warns if default value detected
- **Concurrency handling (Bug 16)** — All repository SaveChanges wrapped in try-catch for `DbUpdateConcurrencyException`
- **Pagination (Bug 17)** — All list endpoints now support `?page=&pageSize=` query params. Response wrapped in `PaginatedResult<T>` with `items`, `page`, `pageSize`, `totalCount`, `totalPages`, `hasNext`, `hasPrevious`

## New Features

- **Transaction search** — `GET /transactions?Search=text` filters by note content and customer name
- **Report period filters** — `GET /reports/summary?period=week|month|3months|year` for flexible reporting

## Pagination Format

All list endpoints now return:

```json
{
  "items": [...],
  "page": 1,
  "pageSize": 50,
  "totalCount": 120,
  "totalPages": 3,
  "hasPrevious": false,
  "hasNext": true
}
```

## Rate Limiting

| Policy | Limit | Window | Queue |
|--------|-------|--------|-------|
| `fixed` (default) | 100 requests | 1 minute | 10 |
| `auth` (login/register) | 10 requests | 1 minute | 0 |

Returns `429 Too Many Requests` when exceeded.

## File Attachment Upload

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/v1/transactions/{id}/attachment | Upload image (jpg/png/gif) or PDF, max 10MB |

- Allowed extensions: `.jpg`, `.jpeg`, `.png`, `.gif`, `.pdf`
- Files stored in `wwwroot/uploads/` with GUID filenames
- Returns `{ "attachmentUrl": "/uploads/{guid}.png", "fileName": "..." }`
- `AttachmentUrl` field included in all transaction responses

## Default Categories on Registration

New users automatically get 7 default categories: Sales, Purchase, Rent, Food, Transport, Salary, Others.

# Day 25 — Final Stabilization & SQA Handover

## Bug Fixes

- **BOLA on read endpoints (Categories/Customers/Transactions)** — `GetByIdAsync` now filters by `userId + id`; cross-user access returns 404
- **Soft-deleted user login blocked** — `GetByEmailAsync` + `LoginAsync` both check `!IsDeleted`
- **DELETE /auth/account** — Self-service account deletion (soft-delete + cascade)

## New Features

- **Notifications summary** — `GET /reports/notifications` returns Today + This Week `{receivable, payable, transactions}`

## Security

- All GET-by-ID endpoints now verify ownership (BOLA fixed)
- Rate limiting: 100 req/min global, 10 req/min auth endpoints
- CORS enabled for frontend integration
- JWT secret validation at startup

## Testing

- [SQA Handover Document](docs/SQA-Handover.md) — complete endpoint reference, test scenarios, credentials
- Postman collection updated with auto-save ID variables (`catId`, `custId`, `txId`)

## Endpoint Summary (36 total)

| Module | Endpoints |
|--------|-----------|
| Auth | 6 (register, login, me GET/PUT, change-password, **delete-account**) |
| Categories | 7 (CRUD + pagination + category transactions) |
| Customers | 6 (CRUD + pagination) |
| Transactions | 10 (CRUD + pagination + search + filters + **file upload**) |
| Reports | 6 (summary + by-category + **notifications**) |
| Settings | 3 (GET/PUT/DELETE) |
| Admin | 1 (users list) |

## Files for SQA

- **Postman**: `docs/HisabDo-API.postman_collection.json` (36 requests, auto-token, ID variables)
- **Handover**: `docs/SQA-Handover.md` (full endpoint reference, test scenarios, credentials)
- **Swagger**: `http://localhost:5181/swagger`


