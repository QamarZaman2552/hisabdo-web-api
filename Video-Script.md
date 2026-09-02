# HisabDo Web API — Video Script (13 APIs Only)

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
"Jab koi user request bhejta hai — jaise POST /transactions:

1. Pehle Controller aata hai (API Layer)
   → Ye request ko receive karta hai
   → Validate karta hai DTO se
   → Service ko call karta hai

2. Phir Service aata hai (Application Layer)
   → Business rules lagata hai
   → Repository ko call karta hai

3. Phir Repository aata hai (Infrastructure Layer)
   → Database se data leta hai ya save karta hai

4. Domain Layer — sabse neeche hai
   → Sirf entities define karta hai

Dependency: API → Application → Domain ← Infrastructure"
```

---

## 3. API DEMO (13 APIs)

**Server start karo:**
```
dotnet run --project src\HisabDo.API
```

**Swagger kholo:** `http://localhost:5181/swagger`

---

### API 1: Login
```
POST /api/v1/auth/login
{
  "email": "demo@hisabdo.com",
  "password": "Demo@123"
}
→ 200 OK → Token copy karo → Authorize mein daalo
```

### API 2: Get Profile
```
GET /api/v1/auth/me
→ 200 OK
{
  "id": 1,
  "fullName": "Demo User",
  "businessName": "Demo Shop",
  "email": "demo@hisabdo.com",
  "role": "Admin",
  "currencyCode": "PKR",
  "languageCode": "en"
}
```

### API 3: Get Categories
```
GET /api/v1/categories?page=1&pageSize=50
→ 200 OK → 7 categories:
  id=1 Sales, id=2 Purchase, id=3 Rent,
  id=4 Food, id=5 Transport, id=6 Salary, id=7 Others
```

### API 4: Create Customer
```
POST /api/v1/customers
{
  "name": "Ahmed Traders",
  "phone": "03009876543",
  "email": "ahmed@test.com",
  "notes": "Wholesale supplier"
}
→ 200 OK → id: 1
```

### API 5: Create Receivable (Income)
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
→ categoryId: 1 = Sales, type: 1 = Receivable
→ 200 OK
```

### API 6: Create Payable (Expense)
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
→ categoryId: 3 = Rent, type: 2 = Payable
→ 200 OK
```

### API 7: Get All Transactions
```
GET /api/v1/transactions?page=1&pageSize=50
→ 200 OK → 2 transactions:
  id=1 amount=5000 type=1 "Widget order payment"
  id=2 amount=2000 type=2 "Office rent August"
```

### API 8: Search Transactions
```
GET /api/v1/transactions?Search=rent
→ 200 OK → 1 result:
  id=2 amount=2000 type=2 "Office rent August"
```

### API 9: Filter by Type
```
GET /api/v1/transactions?type=1
→ 200 OK → 1 receivable:
  id=1 amount=5000 type=1 "Widget order payment"
```

### API 10: Reports Summary
```
GET /api/v1/reports/summary?period=month
→ 200 OK
{
  "totalReceivable": 5000,
  "totalPayable": 2000,
  "netReceivable": 3000,
  "transactionCount": 2
}
```

### API 11: Reports By Category
```
GET /api/v1/reports/by-category
→ 200 OK
[
  { "categoryName": "Sales", "totalAmount": 5000 },
  { "categoryName": "Rent", "totalAmount": 2000 }
]
```

### API 12: Notifications
```
GET /api/v1/reports/notifications
→ 200 OK
{
  "today": { "transactionCount": 2 },
  "thisWeek": { "transactionCount": 2 }
}
```

### API 13: Backup
```
GET /api/v1/data/backup
→ 200 OK → Full JSON export:
  7 categories, 1 customer, 2 transactions
```

---

## 4. SECURITY (30 seconds)

```
"Bol ke dikhao:

1. No token → 401 Unauthorized
2. User A ka token → User B ka data nahi dikhta (BOLA protection)
3. Login 10 baar → 429 Rate Limiting
4. User role → Admin endpoint nahi khulta (403)"
```

---

## 5. CONCLUSION (30 seconds)

```
"To ye tha HisabDo Web API ka complete demo.

- 33 API endpoints across 8 modules
- 12 security features
- Clean Architecture with 108 C# files
- SQA green signal for deployment

Code pushed to:
- github.com/QamarZaman2552/hisabdo-web-api
- github.com/usmankhalid172/hisabdo-dotnet

Thank you!"
```

---

## Quick Reference

| # | Method | Endpoint | Body |
|---|--------|----------|------|
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
