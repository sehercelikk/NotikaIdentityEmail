namespace NotikaIdentityEmail.Models.JWTModels;

public class JwtSettingsModel
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpireMinutes { get; set; }
}
