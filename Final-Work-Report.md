# Final Work Report — HisabDo Web API (.NET Backend)

**Intern:** Qamar Zaman Baloch
**Track:** .NET
**Role:** Team Lead (.NET Capstone Development Team)
**Company:** XICTEK Systems / HisabDo Internship
**Duration:** August 9 – August 27, 2026 (Active Development)
**Repository:** https://github.com/QamarZaman2552/hisabdo-web-api
**Deployment Repo:** https://github.com/usmankhalid172/hisabdo-dotnet

---

## 1. Work Summary

### 1.1 What I Built

I developed the complete backend API for **HisabDo** — a khata/ledger management application — using ASP.NET Core Web API with Clean Architecture, Entity Framework Core, and SQL Server. The API mirrors the HisabDo mobile app functionality and serves as the cloud backend for multi-device data sync.

### 1.2 Architecture

```
HisabDo.API (Controllers, Middleware, Config)
    ↓
HisabDo.Application (Services, DTOs, Interfaces)
    ↓
HisabDo.Domain (Entities, Enums, Base Class)
    ↑
HisabDo.Infrastructure (EF Core, DbContext, Repositories)
```

- **4-layer Clean Architecture** with proper dependency direction
- **108 C# source files** across all layers
- **8 controllers, 8 services, 7 repositories, 24 DTOs, 5 entities**

### 1.3 Day-by-Day Progress

| Day | Date | Focus Area | Deliverables |
|-----|------|-----------|-------------|
| 9 | Aug 9 | Project Setup | Clean Architecture scaffolding, Customer CRUD, Swagger/Postman/SQL screenshots |
| 10 | Aug 10 | Categories | Full CRUD with validation, status codes, unique names, default category protection |
| 11 | Aug 11 | Transactions | CRUD with filters (date, type, customer, category), Category relationship |
| 12 | Aug 12 | Settings + Auth | Settings module (1-to-1), JWT register/login, BCrypt, role-based auth |
| 13 | Aug 13 | Security | All APIs protected with JWT, admin endpoint, Swagger Bearer fix, password policy |
| 14 | Aug 14 | Auth Finalization | User profile APIs (GET/PUT me), change password, auth testing screenshots |
| 15-16 | Aug 17 | Reports | Reports module (summary + by-category), database performance indexes |
| 17 | Aug 17 | Documentation | Postman collection, progress report, README sections |
| 18-20 | Aug 20 | Validation | RFC 7807 ProblemDetails, future-date validation, CreatedAt in DTOs |
| 22-24 | Aug 24 | Stabilization | Pagination, search, rate limiting, file upload, default categories, attachment URL |
| 25 | Aug 25 | SQA Handover | BOLA fixes, soft-delete login block, delete account, notifications, CORS, SQA handover document |
| 26 | Aug 26 | Full Audit | 90+ test scenarios, 6 bug fixes (rate limiter, deleted account, orphan categories, atomic restore) |
| 27 | Aug 27 | Production Hardening | Global query filter (all 5 entities), cascade delete, validation improvements, security hardening |

### 1.4 API Endpoints Delivered (33 unique endpoints)

| Module | Endpoints | Features |
|--------|-----------|----------|
| **Auth** | 6 | Register, Login, Get/Update Profile, Change Password, Delete Account |
| **Categories** | 6 | Full CRUD + Transactions by Category |
| **Customers** | 5 | Full CRUD with Search |
| **Transactions** | 7 | CRUD + Filtered List + Attachment Upload |
| **Reports** | 3 | Summary (period filter), By-Category, Notifications |
| **Settings** | 3 | Get, Update (currency/language), Delete |
| **Admin** | 1 | User Management (Admin only) |
| **Data** | 3 | Backup (JSON export), Restore (with remap), Clear All |
| **Total** | **34** | |

### 1.5 Database Design

- **5 entities:** User, Customer, Category, Transaction, Setting
- **15 indexes** including composite and filtered indexes
- **8 migrations** applied
- **Soft delete** on all entities with global query filters
- **Restrict delete** on parent-child relationships, **Cascade** on User→Setting
- **Seed data:** 7 default categories, demo user, default settings

### 1.6 Security Features

- JWT Bearer authentication (HS256, 24h expiry)
- BCrypt password hashing (never plain text)
- Role-based authorization (Admin/User)
- BOLA prevention (ownership verification on all endpoints)
- Rate limiting (100 req/min global, 10 req/min auth)
- CORS (restricted in production)
- Upload security (10MB limit, allowed extensions only)
- RFC 7807 error handling (hides internal details in production)
- Password policy (min 8, max 64, uppercase, lowercase, digit, special char)

---

## 2. Issues & Challenges

### 2.1 Technical Challenges Faced

| # | Issue | When | Impact |
|---|-------|------|--------|
| 1 | **Swagger didn't send Authorization header** | Day 13 | OpenAPI security requirement serialized as empty object; couldn't test authenticated endpoints in Swagger |
| 2 | **Port conflicts during testing** | Day 13 | Previous API instance kept port 5181 occupied, blocking new instances |
| 3 | **Demo password accidentally changed** | Day 13 | Changed during testing, locked out of demo account |
| 4 | **Git index lock corruption** | Day 14 | Windows line endings caused git index.lock corruption; repository recovered from fresh clone |
| 5 | **Global rate limiter never attached** | Day 26 | `GlobalLimiter` was declared in DI but never called in pipeline; requests bypassed rate limiting entirely |
| 6 | **Deleted account remained usable via live token** | Day 26 | Soft-deleted user could still access API with existing JWT token |
| 7 | **Registration rollback left orphan categories** | Day 26 | Partial failure during registration left categories without user |
| 8 | **Restore could activate soft-deleted settings** | Day 26 | Backup restore ordered by ID instead of active row, potentially restoring deleted settings |
| 9 | **`[EmailAddress]` rejected empty strings in .NET 9** | Day 28 | Customer without email got 400 error; .NET 9 changed `EmailAddressAttribute` behavior |

### 2.2 Integration Challenges

- **Frontend team coordination:** API contract needed to match mobile app screens (Receivable/Payable pattern instead of traditional Income/Expense)
- **Cross-controller routes:** `GET /categories/{id}/transactions` had to be mounted in TransactionsController while appearing under Categories path
- **Multi-device sync:** Backup/Restore had to remap IDs to prevent conflicts across devices

---

## 3. Issues Fixed

### 3.1 Bug Fixes (14+ distinct fixes)

| # | Bug | Root Cause | Fix Applied | Commit |
|---|-----|-----------|------------|--------|
| 1 | **Rate limiter bypassed** | `GlobalLimiter` declared but not called in pipeline | Added `app.UseRateLimiter()` in Program.cs | `08d2bfb` |
| 2 | **Deleted account still accessible** | Live JWT token still valid after soft-delete | User fetch now checks `IsDeleted` flag; token invalidated on delete | `08d2bfb` |
| 3 | **Orphan categories on failed registration** | Partial failure left categories without user | Full cleanup on registration failure (delete categories + user) | `08d2bfb` |
| 4 | **Restore activates deleted settings** | Settings ordered by ID instead of active status | Added active-row-first ordering in restore | `08d2bfb` |
| 5 | **Notifications end-of-day window** | Timezone mismatch in day boundary calculation | Fixed window calculation to use local time | `08d2bfb` |
| 6 | **Invalid period returns 500** | Unhandled enum value in period filter | Added explicit 400 for invalid period values | `08d2bfb` |
| 7 | **Global query filter missing** | Only Category had `HasQueryFilter` | Added `HasQueryFilter(IsDeleted)` to all 5 entities | `528c989` |
| 8 | **Delete account didn't cascade** | Only soft-deleted user, left data behind | Added full cascade: customers, transactions, categories, settings, files | `528c989` |
| 9 | **ClearAll had no safety check** | One-click wiped all data without confirmation | Added `?confirm=true` requirement | `528c999` |
| 10 | **TransactionFilterDto no range validation** | Amount/page could be negative | Added `[Range]` and `[StringLength]` attributes | `528c989` |
| 11 | **Upload missing content-type validation** | Any file type could be uploaded | Added `[Consumes("multipart/form-data")]` and MIME validation | `528c989` |
| 12 | **JWT secret hardcoded in source** | Secret visible in appsettings.json | Moved to environment variables; blank in production config | `8681d99` |
| 13 | **Email validation rejected empty** | `[EmailAddress]` attribute on DTO | Removed from DTO; service-level validation handles it | `8681d99` |
| 14 | **Restore customer dedupe** | Duplicate customers created on repeated restores | Added name-based deduplication with `CustomersSkipped` tracking | `8681d99` |

### 3.2 Security Fixes

| # | Issue | Fix |
|---|-------|-----|
| 1 | BOLA on all GET endpoints | Added `UserId` ownership verification; returns 404 for unauthorized access |
| 2 | Soft-deleted users could login | Login now checks `IsDeleted` flag |
| 3 | No rate limiting on auth | Added 10 req/min rate limiter on login/register |
| 4 | CORS AllowAnyOrigin in production | Environment-aware CORS; restricted origins in production |
| 5 | Production error details exposed | RFC 7807 hides internal details for 500 errors in production |
| 6 | Upload path traversal | Extension whitelist + content-type validation |

---

## 4. Unresolved Issues

| # | Issue | Status | Reason |
|---|-------|--------|--------|
| 1 | Unit tests not written | Deferred | Out of scope for this phase; SQA manual testing covered all scenarios |
| 2 | Frontend integration | Pending | Frontend not yet developed by Flutter team |
| 3 | `/reports/export` endpoint not implemented | Not required | SQA handover doc listed it but spec doesn't require it; backup endpoint serves same purpose |
| 4 | `/settings/currencies` endpoint not implemented | Not required | Currency list is static; frontend handles currency display |

---

## 5. Screenshots & Evidence

All screenshots are stored in the repository:

| Category | Location | Count |
|----------|----------|-------|
| Swagger Testing | `screenshots/Day-*/Swagger_Day_*/` | 60+ |
| Postman Testing | `screenshots/Day-*/Postman_Day_*/` | 20+ |
| SQL Server | `screenshots/Day-*/SqlServer_Day_*/` | 15+ |
| User Review | `feedback/evidence/user-review-screenshots/` | 4 |
| Total | | **100+** |

### Key Evidence Files
- `docs/SQA-Handover.md` — Complete API specification, test scenarios, security features
- `docs/HisabDo-API.postman_collection.json` — 39 Postman requests with auto-token
- `docs/ERD.md` — Database entity relationship diagram
- `feedback/feedback-report.md` — Product analysis and user feedback
- `README.md` — Full project documentation with embedded screenshots

---

## 6. GitHub / Repository Details

### Repositories

| Repository | URL | Purpose |
|-----------|-----|---------|
| **Primary** | https://github.com/QamarZaman2552/hisabdo-web-api | Development repo |
| **Upstream** | https://github.com/usmankhalid172/hisabdo-web-api | Sir's repo |
| **Deployment** | https://github.com/usmankhalid172/hisabdo-dotnet | Deployment repo |

### Branch

- `main` — Production-ready code (sole developer, 39 commits)

### Key Commits

| Commit | Date | Description |
|--------|------|-------------|
| `b928c41` | Aug 9 | Initial project setup with Clean Architecture |
| `a409e84` | Aug 12 | JWT authentication implemented |
| `4be7a3e` | Aug 20 | RFC 7807 error handling |
| `45d5c8f` | Aug 24 | File upload + default categories |
| `4099537` | Aug 25 | BOLA fixes + SQA handover |
| `08d2bfb` | Aug 26 | Full audit — 6 bug fixes |
| `8681d99` | Aug 27 | Production hardening — 30 fixes |
| `b2be2f7` | Aug 28 | Pushed to deployment repo |

### Files Modified (Final Commit)

- **32 files changed** across all 4 architecture layers
- **1,292 additions, 74 deletions**
- **9 new files** created (constants, DTOs, config classes, migrations)

---

## 7. Demo Video Outline

### Video Structure (5-7 minutes)

1. **Introduction (30s)**
   - Project overview: HisabDo — Khata management API
   - Tech stack: .NET 9, EF Core, SQL Server, Clean Architecture

2. **Architecture Walkthrough (1 min)**
   - Show 4-layer structure
   - Explain dependency direction

3. **API Demo (3 min)**
   - Register → Login → Get Token
   - Create Customer → Create Transaction (Receivable/Payable)
   - Upload Attachment
   - View Reports (Summary, By-Category)
   - Backup & Restore

4. **Security Demo (1 min)**
   - BOLA test: User A cannot access User B's data (404)
   - Rate limiting: Show 429 after 10 auth requests
   - JWT validation: Invalid token → 401

5. **Testing Evidence (30s)**
   - Swagger screenshots
   - SQA results: 49/53 passed, zero data leaks

6. **Conclusion (30s)**
   - Summary of deliverables
   - Production readiness status

---

## 8. Summary Statistics

| Metric | Value |
|--------|-------|
| Git Commits | 39 |
| Active Development Days | 13 |
| C# Source Files | 108 |
| API Endpoints | 33 |
| Controllers | 8 |
| Services | 8 |
| Repositories | 7 |
| DTOs | 24 |
| Entities | 5 |
| Database Migrations | 8 |
| Bug Fixes | 14+ |
| Security Features | 12 |
| Test Scenarios | 90+ |
| Screenshots | 100+ |
| SQA Pass Rate | 49/53 (92.5%) |
| Zero Data Leaks | Yes |

---

*Report prepared by Qamar Zaman Baloch*
*Date: August 28, 2026*
