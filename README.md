# HisabDo Web API - Capstone Project

**Day 9 - HisabDo Internship**

A .NET-based web application that mirrors the HisabDo mobile app (Khata/Ledger application).

ASP.NET Core Web API with Clean Architecture, EF Core, SQL Server and working CRUD modules.

**Developer:** Qamar Zaman
**Track:** .NET

> **Also check the [Screenshot folder](screenshots) for Swagger test screenshots.**

---

# Day 9 - Project Implementation

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

Open `http://localhost:5xxx/swagger` to test.

## Solution Structure (Clean Architecture)

```text
hisabdo-web-api/
â”œâ”€â”€ src/
â”‚   â”œâ”€â”€ HisabDo.API/            # Controllers, middleware, Program.cs, config
â”‚   â”œâ”€â”€ HisabDo.Application/    # Services, DTOs, repository interfaces
â”‚   â”œâ”€â”€ HisabDo.Domain/         # Entities, enums, base class
â”‚   â””â”€â”€ HisabDo.Infrastructure/ # EF Core, DbContext, repositories (implementations)
â”œâ”€â”€ tests/                      # (future)
â”œâ”€â”€ docs/ERD.md                 # Database diagram
â””â”€â”€ README.md
```

Project references (one direction only):

```text
API -> Application -> Domain
API -> Infrastructure -> Domain
Infrastructure -> Application (implements repository interfaces)
```

## Entities (Domain)

- `User` - account owner (seeded demo user)
- `Customer` - Khata partner (logha lena / dena)
- `Category` - groups transactions (seeded: Sales, Purchase, Rent, Food, Transport, Salary, Others)
- `Transaction` - the core entity (Type: 1 = Receivable, 2 = Payable)
- `Setting` - user preferences

All business tables have `UserId` and use soft delete (`IsDeleted`).

## Working CRUD Modules (Day 9)

### Customers

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/v1/customers | List all customers |
| GET | /api/v1/customers/{id} | Get customer by ID |
| POST | /api/v1/customers | Add customer |
| PUT | /api/v1/customers/{id} | Update customer |
| DELETE | /api/v1/customers/{id} | Delete customer (soft) |

### Transactions

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/v1/transactions | List all transactions (with customer + category names) |
| GET | /api/v1/transactions/{id} | Get transaction by ID |
| POST | /api/v1/transactions | Add Receivable/Payable |
| PUT | /api/v1/transactions/{id} | Update transaction |
| DELETE | /api/v1/transactions/{id} | Delete transaction (soft) |

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
```

`type`: 1 = Receivable (logha lena), 2 = Payable (logha dena).

### Error handling

All errors return a consistent JSON response:

```json
{ "message": "No customer found with ID: 5" }
```

- 400 Bad Request - invalid operation (e.g. customer/category not found)
- 404 Not Found - resource not found
- 500 Internal Server Error - unexpected error

### Notes

- No JWT yet: controllers use the seeded demo user (UserId = 1). JWT auth comes in a later step.

