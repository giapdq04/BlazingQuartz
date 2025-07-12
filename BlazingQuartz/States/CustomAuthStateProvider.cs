using BlazeQuartz.Core.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthStateProvider(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var result = await _sessionStorage.GetAsync<bool>("isLoggedIn");
            var roleResult = await _sessionStorage.GetAsync<string>("userRole");
            var username = await _sessionStorage.GetAsync<string>("username");
            var userId = await _sessionStorage.GetAsync<string>("userId");
            
            if (result.Success && result.Value)
            {
                // Tạo ClaimsPrincipal cho user đã đăng nhập
                var role = roleResult.Success ? roleResult.Value : "User";
                var name = username.Success ? username.Value : "UnknownUser";
                var id = userId.Success ? userId.Value : "0";
                
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.NameIdentifier, id)
                }, "CustomAuth");
                
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
            return new AuthenticationState(_anonymous);
        }
        catch (Exception ex)
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task MarkUserAsAuthenticated(User u)
    {
        await _sessionStorage.SetAsync("isLoggedIn", true);
        await _sessionStorage.SetAsync("username", u.Username);
        await _sessionStorage.SetAsync("userRole", u.Role);
        await _sessionStorage.SetAsync("userId", u.UserId.ToString());
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, u.Username),
            new Claim(ClaimTypes.Role, u.Role),
            new Claim(ClaimTypes.NameIdentifier, u.UserId.ToString())
        }, "CustomAuth");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _sessionStorage.DeleteAsync("isLoggedIn");
        await _sessionStorage.DeleteAsync("username");
        await _sessionStorage.DeleteAsync("userRole");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}