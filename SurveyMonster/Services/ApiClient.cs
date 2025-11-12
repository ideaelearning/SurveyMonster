using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SurveyMonster.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _authToken;
    private const string AnonymousToken = "ANONYMOUS_USER";

    public ApiClient(
      IHttpClientFactory httpClientFactory,
      ILogger<ApiClient> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClientFactory.CreateClient("SurveyMonsterApi");
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetAuthToken(string token)
    {
        _authToken = token;

        // Don't set bearer token for anonymous users
        if (token != AnonymousToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
           new AuthenticationHeaderValue("Bearer", token);
     
            _logger.LogInformation("Authentication token set for API client");
        }
        else
        {
            _logger.LogInformation("Anonymous user detected, skipping bearer token");
        }
    }

    private void SetAuthorizationHeader()
    {
        // Get token from session if not already set
        var token = _authToken ?? _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");

        // Only set authorization header if we have a valid (non-anonymous) token
        if (!string.IsNullOrEmpty(token) && token != AnonymousToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
      
        }
        else if (token == AnonymousToken)
        {
            // Remove authorization header for anonymous users
            // _httpClient.DefaultRequestHeaders.Authorization = null;
            _httpClient.DefaultRequestHeaders.Remove("tenantid");
            _httpClient.DefaultRequestHeaders.Add("tenantid", "1");
            _logger.LogDebug("Anonymous user - no authorization header set");
     
        }
    }

    public async Task<T?> GetAsync<T>(string endpoint, string b = "")
    {
        try
        {
            _logger.LogInformation("GET request to {Endpoint}", endpoint);

            SetAuthorizationHeader();

            if (!string.IsNullOrEmpty(b))
            {
                _httpClient.BaseAddress = new Uri(b);
            }

            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("GET request failed. Status: {StatusCode}, Response: {Response}",
             response.StatusCode, errorContent);
                throw new HttpRequestException(
                    $"API request failed with status code {response.StatusCode}: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("GET request successful for {Endpoint}", endpoint);

            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request exception during GET to {Endpoint}", endpoint);
            throw;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout during GET to {Endpoint}", endpoint);
            throw new HttpRequestException("Request timed out", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during GET to {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data, string baseUrl = "")
    {
        try
        {
            _logger.LogInformation("POST request to {Endpoint}", endpoint);

            try
            {
                if (!string.IsNullOrEmpty(baseUrl))
                {
                    _httpClient.BaseAddress = new Uri(baseUrl);
                }
            }
            catch (Exception)
            {
            }

            SetAuthorizationHeader();

            var jsonContent = JsonSerializer.Serialize(data);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("POST request failed. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, errorContent);
                throw new HttpRequestException(
               $"API request failed with status code {response.StatusCode}: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("POST request successful for {Endpoint}", endpoint);

            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request exception during POST to {Endpoint}", endpoint);
            throw;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout during POST to {Endpoint}", endpoint);
            throw new HttpRequestException("Request timed out", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during POST to {Endpoint}", endpoint);
            throw;
        }
    }
}
