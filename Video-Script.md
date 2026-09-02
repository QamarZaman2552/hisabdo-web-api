# HisabDo Web API — Final Video Script (Verified)

---

## 1. INTRO (30 seconds)

```
"Assalam o Alaikum, mera naam Qamar Zaman hai aur main .NET track 
ka intern hoon. Aaj main aapko HisabDo Web API ka complete demo dikhaunga.

Ye ek khata management system hai — jaise HisabDo mobile app hai, 
ye uska cloud backend hai. Isko maine banaya hai ASP.NET Core Web API, 
Entity Framework Core, aur SQL Server use karke.

Is API mein 33 endpoints hain, 8 modules hain, aur ye production-ready hai.
SQA team ne test kiya hai — 49 out of 53 test cases pass hue hain, 
aur zero data leaks milay hain.

Chalein shuru karte hain."
```

---

## 2. ARCHITECTURE (1.5 minutes)

```
"Ye project hai Clean Architecture pe based — iska matlab hai 
ke code ko 4 alag layers mein divide kiya gaya hai, aur har layer 
ka apna kaam hai."
```

**VS Code mein `src/` folder kholo aur ye dikhao:**

```
src/
├── HisabDo.API/              ← Layer 1: Entry Point
│   ├── Controllers/          ← 8 controllers (API endpoints)
│   ├── Middleware/            ← Error handling
│   ├── Program.cs            ← App startup, DI, JWT, CORS
│   └── appsettings.json      ← Configuration
│
├── HisabDo.Application/      ← Layer 2: Business Logic
│   ├── Services/             ← 8 services (rules kahan lagengi)
│   ├── DTOs/                 ← 24 Data Transfer Objects
│   └── Repositories/         ← 7 interfaces (contracts)
│
├── HisabDo.Domain/           ← Layer 3: Core Entities
│   ├── Entities/             ← 5 entities (User, Customer, etc.)
│   ├── Enums/                ← TransactionType
│   └── Constants/            ← Roles, Defaults
│
└── HisabDo.Infrastructure/   ← Layer 4: Database
    ├── Data/                 ← DbContext + Migrations
    └── Repositories/         ← 7 implementations
```

**Connection explain karo:**

```
"Ab main batata hoon ke ye layers kaise kaam karti hain:

Jab koi user request bhejta hai — jaise POST /transactions:

1. Pehle Controller aata hai (API Layer)
   → Ye request ko receive karta hai
   → Validate karta hai DTO se
   → Service ko call karta hai

2. Phir Service aata hai (Application Layer)
   → Business rules lagata hai
   → Repository ko call karta hai

3. Phir Repository aata hai (Infrastructure Layer)
   → Database se data leta hai ya save karta hai
   → DbContext use karta hai

4. Domain Layer — ye sabse neeche hai
   → Sirf entities aur types define karta hai
   → Kisi pe depend nahi karta

Dependency flow ye hai:
API → Application → Domain ← Infrastructure

Ye isliye hai ke agar kal SQL Server MongoDB mein change karna ho,
to sirf Infrastructure layer change hogi — baqi sab same rahega."
```

---

## 3. API DEMO (3 minutes)

**Server start karo:**
```
dotnet run --project src\HisabDo.API
```

**Swagger kholo:** `http://localhost:5181/swagger`

---

### Step 1: Login
```
POST /api/v1/auth/login

{
  "email": "demo@hisabdo.com",
  "password": "Demo@123"
}

→ Response: 200 OK
→ Token milega — copy karo
→ Swagger mein "Authorize" button pe jao
→ "Bearer <token>" daalo → Authorize karo
```

### Step 2: Get Profile
```
GET /api/v1/auth/me

→ Response: 200 OK
{
  "id": 1,
  "fullName": "Demo User",
  "businessName": "Demo Shop",
  "email": "demo@hisabdo.com",
  "phone": "03000000000",
  "role": "Admin",
  "currencyCode": "PKR",
  "languageCode": "en"
}
```

### Step 3: Get Categories
```
GET /api/v1/categories?page=1&pageSize=50

→ Response: 200 OK
{
  "items": [
    { "id": 1, "name": "Sales" },
    { "id": 2, "name": "Purchase" },
    { "id": 3, "name": "Rent" },
    { "id": 4, "name": "Food" },
    { "id": 5, "name": "Transport" },
    { "id": 6, "name": "Salary" },
    { "id": 7, "name": "Others" }
  ],
  "totalCount": 7
}

"Bol ke dikhao: Register karte hi 7 categories automatic create ho jati hain"
```

### Step 4: Create Customer
```
POST /api/v1/customers

{
  "name": "Ahmed Traders",
  "phone": "03009876543",
  "email": "ahmed@test.com",
  "notes": "Wholesale supplier"
}

→ Response: 200 OK
{
  "id": 1,
  "name": "Ahmed Traders",
  "phone": "03009876543",
  "email": "ahmed@test.com",
  "notes": "Wholesale supplier"
}
```

### Step 5: Create Receivable (Income)
```
POST /api/v1/transactions

{
  "customerId": 1,
  "categoryId": 1,
  "type": 1,
  "amount": 5000,
  "note": "Widget order payment",
  "transactionDate": "2026-08-28T10:00:00Z"
}

→ categoryId: 1 = Sales
→ type: 1 = Receivable (income)
→ Response: 200 OK
```

### Step 6: Create Payable (Expense)
```
POST /api/v1/transactions

{
  "customerId": 1,
  "categoryId": 3,
  "type": 2,
  "amount": 2000,
  "note": "Office rent August",
  "transactionDate": "2026-08-28T10:00:00Z"
}

→ categoryId: 3 = Rent
→ type: 2 = Payable (expense)
→ Response: 200 OK
```

### Step 7: Get All Transactions
```
GET /api/v1/transactions?page=1&pageSize=50

→ Response: 200 OK
{
  "items": [
    { "id": 1, "amount": 5000, "type": 1, "note": "Widget order payment" },
    { "id": 2, "amount": 2000, "type": 2, "note": "Office rent August" }
  ],
  "totalCount": 2
}
```

### Step 8: Search Transactions
```
GET /api/v1/transactions?Search=rent

→ Response: 200 OK
{
  "items": [
    { "id": 2, "amount": 2000, "type": 2, "note": "Office rent August" }
  ],
  "totalCount": 1
}
```

### Step 9: Filter by Type
```
GET /api/v1/transactions?type=1

→ Sirf Receivable (income) dikhenge
→ Response: 200 OK
{
  "items": [
    { "id": 1, "amount": 5000, "type": 1, "note": "Widget order payment" }
  ],
  "totalCount": 1
}
```

### Step 10: Reports Summary
```
GET /api/v1/reports/summary?period=month

→ Response: 200 OK
{
  "totalReceivable": 5000,
  "totalPayable": 2000,
  "netReceivable": 3000,
  "transactionCount": 2,
  "period": "month"
}

"Bol ke dikhao: Ye dashboard pe monthly overview dikhaata hai"
```

### Step 11: Reports By Category
```
GET /api/v1/reports/by-category

→ Response: 200 OK
[
  { "categoryName": "Sales", "totalAmount": 5000, "type": 1 },
  { "categoryName": "Rent", "totalAmount": 2000, "type": 2 }
]
```

### Step 12: Notifications
```
GET /api/v1/reports/notifications

→ Response: 200 OK
{
  "today": { "transactionCount": 2, "totalAmount": 7000 },
  "thisWeek": { "transactionCount": 2, "totalAmount": 7000 }
}
```

### Step 13: Backup
```
GET /api/v1/data/backup

→ Response: 200 OK
{
  "exportedAt": "2026-08-28T10:00:00Z",
  "users": [...],
  "categories": [...7 items],
  "customers": [...1 item],
  "transactions": [...2 items],
  "settings": [...]
}

"Bol ke dikhao: Ye user apna saara data JSON format mein download kar sakta hai"
```

---

## 4. SECURITY DEMO (1 minute)

### Test 1: No Token → 401
```
// Logout karo (Authorize button se token hatao)
GET /api/v1/auth/me (bina token ke)

→ Response: 401 Unauthorized
```

### Test 2: Invalid Token → 401
```
GET /api/v1/auth/me
Header: Authorization: Bearer abc123invalid

→ Response: 401 Unauthorized
```

### Test 3: BOLA — User A can't access User B
```
// Naya user register karo
POST /api/v1/auth/register
{
  "fullName": "User B",
  "businessName": "B Shop",
  "email": "userb@test.com",
  "phone": "03001111111",
  "password": "Admin@123"
}

// Uska token lo aur transaction banao
POST /api/v1/transactions (User B ke token se)
{
  "customerId": 2,
  "categoryId": 8,
  "type": 1,
  "amount": 3000,
  "note": "User B ka transaction",
  "transactionDate": "2026-08-28T10:00:00Z"
}

// Ab demo user ke token se User B ka transaction access karo
GET /api/v1/transactions/3 (Demo User ke token se)

→ Response: 404 Not Found
"Bol ke dikhao: User A ko User B ka data nahi dikhta — ye BOLA protection hai"
```

### Test 4: Rate Limiting → 429
```
// Login ko 10 baar jaldi jaldi karo
POST /api/v1/auth/login (11th time)

→ Response: 429 Too Many Requests
```

### Test 5: Admin Only → 403
```
// User B ke token se admin endpoint access karo
GET /api/v1/admin/users (User B ke token se)

→ Response: 403 Forbidden
```

---

## 5. TESTING EVIDENCE (30 seconds)

```
"Ye SQA handover document hai — ismein saari testing ka record hai.

39 APIs test huin, 53 test cases execute huaye:
- 49 Passed
- 4 Deferred (Backup/Restore partial, CORS, Swagger docs)

Zero data leaks mile hain — User A ka data User B ko nahi dikha.

SQA Lead ne GREEN SIGNAL diya hai deployment ke liye."
```

---

## 6. ALL 33 API ENDPOINTS

```
AUTH (6):
  POST   /api/v1/auth/register
  POST   /api/v1/auth/login
  GET    /api/v1/auth/me
  PUT    /api/v1/auth/me
  POST   /api/v1/auth/change-password
  DELETE /api/v1/auth/account

CATEGORIES (6):
  GET    /api/v1/categories
  GET    /api/v1/categories/{id}
  POST   /api/v1/categories
  PUT    /api/v1/categories/{id}
  DELETE /api/v1/categories/{id}
  GET    /api/v1/categories/{id}/transactions

CUSTOMERS (5):
  GET    /api/v1/customers
  GET    /api/v1/customers/{id}
  POST   /api/v1/customers
  PUT    /api/v1/customers/{id}
  DELETE /api/v1/customers/{id}

TRANSACTIONS (7):
  GET    /api/v1/transactions
  GET    /api/v1/transactions/{id}
  POST   /api/v1/transactions
  PUT    /api/v1/transactions/{id}
  DELETE /api/v1/transactions/{id}
  POST   /api/v1/transactions/{id}/attachment
  GET    /api/v1/categories/{categoryId}/transactions

REPORTS (3):
  GET    /api/v1/reports/summary
  GET    /api/v1/reports/by-category
  GET    /api/v1/reports/notifications

SETTINGS (3):
  GET    /api/v1/settings
  PUT    /api/v1/settings
  DELETE /api/v1/settings

ADMIN (1):
  GET    /api/v1/admin/users

DATA (3):
  GET    /api/v1/data/backup
  POST   /api/v1/data/restore
  DELETE /api/v1/data/all
```

---

## 7. ALL 12 SECURITY FEATURES

```
1.  JWT Bearer Authentication — HS256, 24h expiry
2.  BCrypt Password Hashing — plain text kabhi store nahi hota
3.  Role-Based Authorization — Admin/User roles
4.  BOLA Prevention — ownership verify hota hai har endpoint pe
5.  Global Query Filter — soft-deleted data dikhta nahi
6.  Rate Limiting — 100/min global, 10/min auth endpoints
7.  CORS Policy — production mein sirf allowed domains
8.  Upload Security — 10MB limit, sirf allowed extensions
9.  RFC 7807 Error Handling — proper error responses
10. Password Policy — min 8, uppercase, lowercase, digit, special char
11. Soft Delete — data permanently delete nahi hota
12. Cascade Delete — account delete pe saara data cleanup
```

---

## 8. CONCLUSION (30 seconds)

```
"To ye tha HisabDo Web API ka complete demo.

Summary:
- 33 API endpoints across 8 modules
- 12 security features
- Clean Architecture with 108 C# files
- 8 database migrations
- 90+ test scenarios
- SQA green signal for deployment

Code pushed to:
- github.com/QamarZaman2552/hisabdo-web-api
- github.com/usmankhalid172/hisabdo-dotnet

Thank you!"
```

---

## Quick Reference (Video record karte waqt dekho)

| Step | Method | Endpoint | Body |
|------|--------|----------|------|
| 1 | POST | `/auth/login` | `{"email":"demo@hisabdo.com","password":"Demo@123"}` |
| 2 | GET | `/auth/me` | — |
| 3 | GET | `/categories?page=1&pageSize=50` | — |
| 4 | POST | `/customers` | `{"name":"Ahmed Traders","phone":"03009876543","email":"ahmed@test.com","notes":"Wholesale supplier"}` |
| 5 | POST | `/transactions` | `{"customerId":1,"categoryId":1,"type":1,"amount":5000,"note":"Widget order payment","transactionDate":"2026-08-28T10:00:00Z"}` |
| 6 | POST | `/transactions` | `{"customerId":1,"categoryId":3,"type":2,"amount":2000,"note":"Office rent August","transactionDate":"2026-08-28T10:00:00Z"}` |
| 7 | GET | `/transactions?page=1&pageSize=50` | — |
| 8 | GET | `/transactions?Search=rent` | — |
| 9 | GET | `/transactions?type=1` | — |
| 10 | GET | `/reports/summary?period=month` | — |
| 11 | GET | `/reports/by-category` | — |
| 12 | GET | `/reports/notifications` | — |
| 13 | GET | `/data/backup` | — |

---

## Category ID Reference

| ID | Name | Use in Transaction |
|----|------|-------------------|
| 1 | Sales | categoryId: 1 (Receivable) |
| 2 | Purchase | categoryId: 2 (Payable) |
| 3 | Rent | categoryId: 3 (Payable) |
| 4 | Food | categoryId: 4 (Payable) |
| 5 | Transport | categoryId: 5 (Payable) |
| 6 | Salary | categoryId: 6 (Payable) |
| 7 | Others | categoryId: 7 (Either) |

---

## Transaction Type Reference

| Type | Value | Meaning |
|------|-------|---------|
| Receivable | 1 | Income (paise aaye) |
| Payable | 2 | Expense (paise gaye) |
