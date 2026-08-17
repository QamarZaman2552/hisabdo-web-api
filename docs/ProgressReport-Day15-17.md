# Day 15-17 Progress Report - .NET Capstone Team

**Team:** .NET (Department 1 - Capstone Development Team)
**Intern:** Qamar Zaman (Team Lead)
**Period:** Day 15, Day 16, Day 17

## 1. Team Members List

| # | Name | Role | Assigned Module |
|---|------|------|-----------------|
| 1 | Qamar Zaman | Team Lead | Authentication & Security, API Development, Documentation & Testing |

## 2. Assigned Modules (work division)

| Module | Responsibility | Status |
|--------|---------------|--------|
| Authentication & Security | JWT login/register, roles, password policy, profile APIs | Completed |
| Database & Entity Design | SQL Server schema, relationships, indexes, migrations | Completed |
| API Development | Customers, Categories, Transactions, Settings, Reports, Admin | Completed |
| Documentation & Testing | README, Postman collection, Swagger/Postman testing, screenshots | Completed |

## 3. Progress Report

| Area | Progress (Day 9-17) |
|------|---------------------|
| Project setup (Clean Architecture: API / Application / Domain / Infrastructure) | 100% |
| Customers module (CRUD) | 100% |
| Categories module (CRUD, default-category protection, unique names) | 100% |
| Transactions module (CRUD, filters, category relationship) | 100% |
| Settings module (1-to-1 with user, upsert, soft delete) | 100% |
| Reports / Dashboard module (summary, by-category) | 100% |
| JWT authentication (register, login, BCrypt hashing) | 100% |
| Role-based authorization (Admin / User) | 100% |
| User profile APIs (GET/PUT me, change password) | 100% |
| Password validation policy | 100% |
| Database relationships & indexes (unique email, unique settings per user, report indexes, unique category names) | 100% |
| API documentation (README + Swagger + Postman collection) | 100% |
| Testing (Swagger, Postman, SQL Server screenshots) | 100% |

**Overall capstone progress estimate:** 60%+ (authentication, all core modules, database design and documentation complete; remaining: deployment and frontend integration).

## 4. GitHub Repository

- Repository: https://github.com/QamarZaman2552/hisabdo-web-api
- Branch: main
- Total commits: 25+ (daily commits Day 8 - Day 17)

## 5. Challenges & Blockers

1. **Swagger did not send the Authorization header** - the OpenAPI security requirement serialized as an empty object because the Bearer scheme reference was not bound to the document. Fixed by passing the document to `OpenApiSecuritySchemeReference` and verified in `swagger.json`. No blockers remain.
2. **Port conflicts during testing** - a previously started API instance kept port 5181 occupied and answered with old code, causing confusing 401 errors. Resolved by stopping all instances and testing against a fresh build.
3. **Demo password accidentally changed during password-change testing** - restored by re-running the change-password flow back to the documented credentials.
4. **Git index lock corruption** (line endings on Windows) - recovered the repository from a fresh clone of the pushed state; no commits lost.

## 6. Deliverables Checklist

- [x] Team Members List
- [x] Assigned Modules
- [x] Progress Report
- [x] GitHub Repository Link
- [x] Challenges & Blockers
- [x] Postman collection (docs/HisabDo-API.postman_collection.json)
- [x] Swagger/Postman testing results (screenshots in README)