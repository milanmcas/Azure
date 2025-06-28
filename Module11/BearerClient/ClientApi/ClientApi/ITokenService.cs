using ClientApi;

namespace ClientApi
{
    public interface ITokenService
    {
        Task<TokenResponse> GetTokenAsync();
        Task<string> GetAccessTokenAsync();
    }
}
