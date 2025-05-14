using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Template.Core.Configs;

namespace Template.Service.Clients;

public class LineClient(HttpClient httpClient,
                        IOptions<LineConfiguration> lineConfiguration)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly LineConfiguration _lineClientConfig = lineConfiguration.Value;

    public async Task<string> GetLineIdAsync(string code)
    {
        var accessToken = await GetAccessTokenAsync(code);
        if (accessToken is null)
        {
            return string.Empty;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);

        var response = await _httpClient.GetAsync("v2/profile");
        response.EnsureSuccessStatusCode();
        
        var profile = JsonConvert.DeserializeObject<LineProfileResponse>(await response.Content.ReadAsStringAsync());
        return profile?.UserId ?? string.Empty;
    }

    private async Task<LineTokenResponse?> GetAccessTokenAsync(string code)
    {
        var response = await _httpClient.PostAsync("oauth2/v2.1/token", 
            new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", _lineClientConfig.RedirectUrl },
            { "client_id", _lineClientConfig.ClientId },
            { "client_secret", _lineClientConfig.ClientSecret }
        }));

        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<LineTokenResponse>(await response.Content.ReadAsStringAsync());
    }
}

public class LineProfileResponse { public string UserId { get; set; } = string.Empty; }
public class LineTokenResponse { public string AccessToken { get; set; } = string.Empty; }