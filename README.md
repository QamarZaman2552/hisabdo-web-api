# HisabDo Web API - Capstone Project

**Day 10 - HisabDo Internship**

A .NET-based web application that mirrors the HisabDo mobile app (Khata/Ledger application).

ASP.NET Core Web API with Clean Architecture, EF Core, SQL Server and working CRUD modules.

**Developer:** Qamar Zaman
**Track:** .NET

> **Also check the [Screenshot folder](screenshots) for all Swagger, Postman and SQL Server test screenshots.**

---

# Day 10 - Categories Module

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

### Transactions (Day 9)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/v1/transactions | List all transactions (with customer + category names) |
| GET | /api/v1/transactions/{id} | Get transaction by ID |
| POST | /api/v1/transactions | Add Receivable/Payable |
| PUT | /api/v1/transactions/{id} | Update transaction |
| DELETE | /api/v1/transactions/{id} | Delete transaction (soft) |

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
- 404 Not Found - resource not found
- 500 Internal Server Error - unexpected error

### Notes

- No JWT yet: controllers use the seeded demo user (UserId = 1). JWT auth comes in a later step.

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

