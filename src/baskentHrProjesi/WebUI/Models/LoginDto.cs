public class ApiLoginResponse
{
    public AccessTokenDto AccessToken { get; set; } = null!;
    public string? RequiredAuthenticatorType { get; set; }
}

public class AccessTokenDto
{
    public string Token { get; set; } = null!;
    public DateTime ExpirationDate { get; set; }
}
