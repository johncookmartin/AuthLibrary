namespace AuthLibrary.Settings;

public class AuthSettings
{
    public string? ConnectionString { get; set; }
    public JwtSettings Jwt { get; set; } = new JwtSettings();
    public GoogleSettings? Google { get; set; }
}
