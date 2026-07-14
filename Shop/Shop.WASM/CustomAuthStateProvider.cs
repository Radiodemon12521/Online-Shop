using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;
using Shop.Common;
using Shop.Common.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ApiClient _apiClient;
    private readonly IJwtService _jwtService;

    public CustomAuthStateProvider(ApiClient apiClient, IJwtService jwtService)
    {
        _apiClient = apiClient;
        _jwtService = jwtService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (string.IsNullOrWhiteSpace(_jwtService.Token))
        {
            await _apiClient.RefreshToken();
        }

        if (string.IsNullOrWhiteSpace(_jwtService.Token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(_jwtService.Token);
        var identity = new ClaimsIdentity(token.Claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyUserChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}