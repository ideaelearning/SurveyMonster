using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SurveyMonster.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;
    private string? _authToken;

    public ApiClient(IHttpClientFactory httpClientFactory, ILogger<ApiClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("SurveyMonsterApi");
        _logger = logger;
    }

    public void SetAuthToken(string token)
    {
        _authToken = token;
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        _logger.LogInformation("Authentication token set for API client");
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            _logger.LogInformation("GET request to {Endpoint}", endpoint);
            
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

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            _logger.LogInformation("POST request to {Endpoint}", endpoint);
            
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
