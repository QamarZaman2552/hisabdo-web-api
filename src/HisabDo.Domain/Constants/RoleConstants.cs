namespace HisabDo.Domain.Constants;

public static class Roles
{
    public const string User = "User";
    public const string Admin = "Admin";
}

public static class Defaults
{
    public const string CurrencyCode = "PKR";
    public const string LanguageCode = "en";

    public static readonly string[] DefaultCategories =
        ["Sales", "Purchase", "Rent", "Food", "Transport", "Salary", "Others"];
}
