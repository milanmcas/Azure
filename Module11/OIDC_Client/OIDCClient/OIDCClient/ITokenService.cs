namespace OIDCClient
{
    public interface ITokenService
    {
        Task<TokenResponse> GetTokenAsync();
        Task<string> GetAccessTokenAsync();
    }
}
