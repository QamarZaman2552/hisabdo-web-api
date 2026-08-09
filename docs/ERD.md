# HisabDo Web API - Database ERD

Initial entity relationship diagram for the HisabDo web application (v1).

```mermaid
erDiagram
    USERS ||--o{ CUSTOMERS : owns
    USERS ||--o{ CATEGORIES : owns
    USERS ||--o{ TRANSACTIONS : owns
    USERS ||--o| SETTINGS : has
    CUSTOMERS ||--o{ TRANSACTIONS : has
    CATEGORIES ||--o{ TRANSACTIONS : has

    USERS {
        int UserId PK
        string FullName
        string BusinessName
        string Email UK
        string Phone
        string PasswordHash
        string CurrencyCode
        string LanguageCode
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
    }

    CUSTOMERS {
        int CustomerId PK
        int UserId FK
        string Name
        string Phone
        string Email
        string Notes
        datetime CreatedAt
        bool IsDeleted
    }

    CATEGORIES {
        int CategoryId PK
        int UserId FK
        string Name
        bool IsDefault
        datetime CreatedAt
    }

    TRANSACTIONS {
        int TransactionId PK
        int UserId FK
        int CustomerId FK
        int CategoryId FK
        int Type
        decimal Amount
        string Note
        datetime TransactionDate
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
    }

    SETTINGS {
        int SettingId PK
        int UserId FK
        string CurrencyCode
        string LanguageCode
    }
```

## Relationships

```text
User     1 --- * Customer        (one user has many customers)
User     1 --- * Transaction     (one user has many transactions)
User     1 --- * Category        (one user has many categories)
User     1 --- 1 Setting         (one user has one settings row)
Customer 1 --- * Transaction     (one customer has many transactions)
Category 1 --- * Transaction     (one category has many transactions)
```

## Notes

- Every business table has `UserId` so each user only sees their own data.
- `Type` in Transactions: 1 = Receivable (logha lena), 2 = Payable (logha dena).
- Soft delete: `IsDeleted` flag instead of hard delete.
- Reports are calculated from Transactions, not stored.
