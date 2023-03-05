using System.Net.Http.Headers;
using System.Security.Claims;
using Application.Constants.Web;
using Application.Services.System;
using Application.Settings.Identity;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Identity;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private readonly ISerializerService _serializer;
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient, ISerializerService serializer,
        IHttpContextAccessor contextAccessor)
    {
        _localStorage = localStorage;
        _serializer = serializer;
        _contextAccessor = contextAccessor;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // var savedToken = await _localStorage.GetItemAsync<string>(LocalStorageConstants.AuthToken);
            var savedToken = _contextAccessor.HttpContext!.Session.GetString(LocalStorageConstants.AuthToken);
            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
            var state = new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromJwt(savedToken), JwtBearerDefaults.AuthenticationScheme)));
            AuthenticationStateUser = state.User;
            await Task.CompletedTask;
            return state;
        }
        catch
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (AuthenticationStateUser is not null) return new AuthenticationState(AuthenticationStateUser);
            
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public ClaimsPrincipal AuthenticationStateUser { get; set; } = null!;

    public void IndicateUserAuthenticationSuccess(string userName)
    {
        var authenticatedUser = new ClaimsPrincipal(
            new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userName)
            }, JwtBearerDefaults.AuthenticationScheme));

        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

        NotifyAuthenticationStateChanged(authState);
    }

    public void IndicateUserAuthenticationSuccess(AuthenticationState authState)
    {
        var taskAuthState = Task.FromResult(authState);
        _contextAccessor.HttpContext!.User = new ClaimsPrincipal(authState.User.Identities);
        AuthenticationStateUser = authState.User;

        NotifyAuthenticationStateChanged(taskAuthState);
    }

    public void DeauthenticateUser()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));

        NotifyAuthenticationStateChanged(authState);
    }

    public async Task<ClaimsPrincipal> GetAuthenticationStateProviderUserAsync()
    {
        var state = await GetAuthenticationStateAsync();
        return state.User;
    }

    public IEnumerable<Claim> GetClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = _serializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (keyValuePairs is null) return claims;
        keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

        if (roles != null)
        {
            if (roles.ToString()!.Trim().StartsWith("["))
            {
                var parsedRoles = _serializer.Deserialize<string[]>(roles.ToString()!);

                claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
            }

            keyValuePairs.Remove(ClaimTypes.Role);
        }

        keyValuePairs.TryGetValue(ApplicationClaimTypes.Permission, out var permissions);
        if (permissions != null)
        {
            if (permissions.ToString()!.Trim().StartsWith("["))
            {
                var parsedPermissions = _serializer.Deserialize<string[]>(permissions.ToString()!);
                claims.AddRange(parsedPermissions.Select(permission => new Claim(ApplicationClaimTypes.Permission, permission)));
            }
            else
            {
                claims.Add(new Claim(ApplicationClaimTypes.Permission, permissions.ToString()!));
            }
            keyValuePairs.Remove(ApplicationClaimTypes.Permission);
        }

        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));
        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
}