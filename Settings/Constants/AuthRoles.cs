namespace AuthLibrary.Settings.Constants;

public static class AuthRoles
{
    public const string Admin = "Admin";
    public const string User = "User";

    public static IEnumerable<string> All =>
        typeof(AuthRoles)
            .GetFields()
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!);
}
