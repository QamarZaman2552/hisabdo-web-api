# HisabDo Web API — SQA Handover Document

**Version**: Day 25 (Final)  
**Date**: 2025-08-25  
**Branch**: main  
**Commit**: latest  
**Base URL**: `http://localhost:5181/api/v1` (dev) / `https://localhost:7071/api/v1` (prod)

---

## 1. Quick Start

```bash
# 1. Configure connection string in src/HisabDo.API/appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=HisabDoDB;Trusted_Connection=True;TrustServerCertificate=True;"
}

# 2. Run migrations
dotnet ef database update --project src/HisabDo.Infrastructure --startup-project src/HisabDo.API

# 3. Start API
dotnet run --project src/HisabDo.API

# 4. Open Swagger
http://localhost:5181/swagger
```

---

## 2. Demo Credentials

| Email | Password | Role |
|-------|----------|------|
| demo@hisabdo.com | Demo@123 | Admin |

**New user registration**: auto-seeds 7 default categories (Sales, Purchase, Rent, Food, Transport, Salary, Others) + default settings (PKR/en).

---

## 3. Complete Endpoint Reference (39 Endpoints)

### Auth (6)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/auth/register` | ❌ | Register new user (auto-seeds categories + settings) |
| POST | `/auth/login` | ❌ | Login, returns JWT (rate limited: 10/min) |
| GET | `/auth/me` | ✅ | Current user profile |
| PUT | `/auth/me` | ✅ | Update profile |
| POST | `/auth/change-password` | ✅ | Change password (rejects same password) |
| **DELETE** | `/auth/account` | ✅ | **Soft-delete own account** |

### Categories (7)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/categories?page=1&pageSize=50` | ✅ | List categories (paginated) |
| GET | `/categories/{id}` | ✅ | Get category by ID (ownership verified) |
| POST | `/categories` | ✅ | Create category |
| PUT | `/categories/{id}` | ✅ | Update category (default categories locked) |
| DELETE | `/categories/{id}` | ✅ | Soft-delete (if no transactions) |
| GET | `/categories/{id}/transactions` | ✅ | Transactions for category |

### Customers (6)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/customers?page=1&pageSize=50` | ✅ | List customers (paginated) |
| GET | `/customers/{id}` | ✅ | Get customer by ID |
| POST | `/customers` | ✅ | Create customer |
| PUT | `/customers/{id}` | ✅ | Update customer |
| DELETE | `/customers/{id}` | ✅ | Soft-delete |

### Transactions (10)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/transactions?Page=1&PageSize=50&Search=text` | ✅ | List (paginated, search by note/customer) |
| GET | `/transactions?Type=1&CustomerId=2&CategoryId=1&FromDate=...` | ✅ | Filter by type/customer/category/date |
| GET | `/transactions/{id}` | ✅ | Get by ID |
| POST | `/transactions` | ✅ | Create (type 1=Receivable, 2=Payable) |
| PUT | `/transactions/{id}` | ✅ | Update |
| DELETE | `/transactions/{id}` | ✅ | Soft-delete (deletes attachment file) |
| **POST** | `/transactions/{id}/attachment` | ✅ | **Upload image/PDF (max 10MB)** |
| GET | `/categories/{id}/transactions` | ✅ | Transactions for category |

### Reports (6)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/reports/summary?period=month\|week\|3months\|year` | ✅ | Dashboard totals with period filter |
| GET | `/reports/by-category` | ✅ | Per-category breakdown |
| **GET** | `/reports/notifications` | ✅ | **Today + This Week summary** |

### Settings (3)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/settings` | ✅ | Get user settings |
| PUT | `/settings` | ✅ | Update currency/language |
| DELETE | `/settings` | ✅ | Reset settings |

### Admin (1)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/admin/users` | ✅ (Admin) | List all users (soft-deleted excluded) |

### Data — Backup/Restore/Danger Zone (3)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/data/backup` | ✅ | Export all data as JSON (settings, categories, customers, transactions) |
| POST | `/data/restore?replace=false\|true` | ✅ | Import backup JSON; IDs auto-remapped; `replace=true` wipes first (atomic) |
| DELETE | `/data/all` | ✅ | Danger Zone: clear all transactions/customers/categories (account stays) |

---

## 4. Response Formats

### Paginated Response (Categories, Customers, Transactions)
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

### Error Response (RFC 7807 ProblemDetails)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5",
  "title": "Bad request",
  "status": 400,
  "detail": "Transaction date cannot be in the future.",
  "traceId": "0HNNUI07M2B14:00000001"
}
```

### Notifications Summary
```json
{
  "today": { "receivable": 0, "payable": 0, "transactions": 0 },
  "thisWeek": { "receivable": 1500, "payable": 500, "transactions": 3 }
}
```

---

## 5. Security Features

| Feature | Implementation |
|---------|----------------|
| **Auth** | JWT Bearer (HS256), 30-min expiry |
| **Authorization** | `[Authorize]` on all business endpoints |
| **Data Isolation** | All queries filter by `UserId` from JWT `sub` claim |
| **BOLA Protection** | All GET-by-ID endpoints verify ownership (returns 404) |
| **Rate Limiting** | 100 req/min global, 10 req/min auth endpoints |
| **Soft Delete** | All entities use `IsDeleted` flag |
| **Password Policy** | Min 8 chars, uppercase, lowercase, digit, special |
| **CORS** | Production: locked to `AllowedOrigins` whitelist (no credentials leak). Development: `AllowAnyOrigin` for local testing |

---

## 6. Test Scenarios for SQA

### Authentication
- [ ] Register → Login → Token works
- [ ] Invalid credentials → 401
- [ ] Duplicate email → 400
- [ ] Weak password → 400
- [ ] Change password with same password → 400
- [ ] Login with soft-deleted user → 401
- [ ] Rate limit on login (10/min) → 429

### Data Isolation (BOLA)
- [ ] User A cannot GET/PUT/DELETE User B's category (expect 404)
- [ ] User A cannot GET/PUT/DELETE User B's customer (expect 404)
- [ ] User A cannot GET/PUT/DELETE User B's transaction (expect 404)
- [ ] User A cannot create transaction with User B's customer/category (expect 400/404)

### CRUD Validation
- [ ] Create category with duplicate name → 400
- [ ] Update default category → 400
- [ ] Delete category with transactions → 400
- [ ] Transaction date in future → 400
- [ ] Transaction amount ≤ 0 → 400
- [ ] Transaction with non-existent customer/category → 404

### Pagination & Search
- [ ] Categories page=1,pageSize=5 → correct page/items
- [ ] Transactions search="sale" → filters note/customer name
- [ ] Transactions filter by type/customer/category/date

### Reports
- [ ] Summary period=month/week/3months/year
- [ ] By-category breakdown
- [ ] Notifications: today + thisWeek structure

### File Upload
- [ ] Upload JPG/PNG/GIF/PDF ≤ 10MB → 200 with attachmentUrl
- [ ] Upload .txt/.exe → 400
- [ ] Re-upload replaces old file
- [ ] Delete transaction deletes attachment file

### Error Format (Unified RFC 7807)
- [ ] Every error response has shape: `type`, `title`, `status`, `detail`, `traceId` (no plain `{message}` anywhere)
- [ ] GET non-existent id (customer/category/transaction) -> 404 ProblemDetails
- [ ] Upload without file / wrong extension -> 400 ProblemDetails
- [ ] Duplicate email register -> 400; DB-level duplicate race -> 409

### JWT Negative Cases
- [ ] No Authorization header -> 401
- [ ] Garbage token string -> 401
- [ ] Tampered signature -> 401
- [ ] Token without "Bearer " prefix -> 401
- [ ] Non-admin user on /admin/users -> 403
- [ ] Expired token (24h, ClockSkew=Zero) -> 401

### Referential Integrity
- [ ] DELETE customer that HAS transactions -> 400 "Customer has transactions and cannot be deleted."
- [ ] DELETE category that HAS transactions -> 400
- [ ] Create transaction with soft-deleted customer/category -> 400
- [ ] After clear-all (/data/all), same-name categories can be created again (filtered unique index)

### Rate Limiting (verified live)
- [ ] >100 requests/min from one IP on any endpoint -> 429 ProblemDetails (type rfc9110#15.5.9)
- [ ] >10 requests/min on /auth/login or /auth/register -> 429
- [ ] Limits reset after the 1-minute window

### Account Deletion + Token
- [ ] Old token used AFTER DELETE /auth/account -> 404 on /auth/me (immediately invalidated)

### Reports Logic
- [ ] GET /reports/summary?period=nonsense -> 400 with allowed values (week, month, 3months, year)
- [ ] Notifications "Today" includes ALL of today's transactions (evening timestamps included)

### Account Management
- [ ] DELETE /auth/account → 204
- [ ] Login after account delete → 401
- [ ] Soft-deleted user cannot login

### Data — Backup/Restore/Clear (NEW)
- [ ] GET /data/backup returns JSON with settings/categories/customers/transactions counts matching user data
- [ ] Backup only includes own data (user B cannot see user A's data in export)
- [ ] POST /data/restore?replace=false merges: existing same-name categories are skipped (counted in categoriesSkipped)
- [ ] POST /data/restore?replace=true wipes data first then imports everything (counts match backup)
- [ ] Restored transactions keep amount/type/note/date and remapped customer/category names
- [ ] Restore is atomic: if import fails mid-way, no partial data remains
- [ ] DELETE /data/all → 204, then backup shows 0 categories/customers/transactions
- [ ] DELETE /data/all deletes attachment files from disk (uploads folder)
- [ ] After clear-all, account still works: can create new customers/categories/transactions

---

## 7. Known Limitations / Future Work

1. **Backup/Restore API** not implemented (mobile-only feature)
2. **CORS** configured via `AllowedOrigins` in `appsettings.json` — production uses whitelist, development allows any origin
3. **Email verification** not implemented
4. **Refresh tokens** not implemented (JWT only)
5. **Webhooks** not implemented
6. **Admin** cannot hard-delete users
7. **Swagger** only shows 200 responses for POST/PUT — add 4xx/5xx docs

---

## 8. Run Postman Collection

Import `docs/HisabDo-API.postman_collection.json`:
1. Run **Auth - Login** first (auto-saves token)
2. All requests use `{{token}}` variable
3. Create requests auto-save IDs: `catId`, `custId`, `txId`
4. Update/Delete/Upload use saved IDs

---

## 9. Key Files

| Area | Files |
|------|-------|
| Controllers | `src/HisabDo.API/Controllers/*.cs` |
| Services | `src/HisabDo.Application/Services/*.cs` |
| Repositories | `src/HisabDo.Infrastructure/Repositories/*.cs` |
| Entities | `src/HisabDo.Domain/Entities/*.cs` |
| DTOs | `src/HisabDo.Application/DTOs/*.cs` |
| Migrations | `src/HisabDo.Infrastructure/Migrations/*.cs` |
| Config | `src/HisabDo.API/Program.cs`, `appsettings.json` |
| Tests | `docs/HisabDo-API.postman_collection.json` |

---

**Ready for SQA testing.**