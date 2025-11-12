using SurveyMonster.Models;
using SurveyMonster.Models.DTOs;
using SurveyMonster.Models.Requests;
using System.IdentityModel.Tokens.Jwt;

namespace SurveyMonster.Services;

public class AuthService : IAuthService
{
    private readonly IApiClient _apiClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;
    private const string TokenSessionKey = "AuthToken";
    private const string UserIdSessionKey = "UserId";
    private const string AnonymousToken = "ANONYMOUS_USER";

    public AuthService(
      IApiClient apiClient,
     IHttpContextAccessor httpContextAccessor,
    ILogger<AuthService> logger)
    {
        _apiClient = apiClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Attempting login for user: {Username}", request.Username);

            var response = await _apiClient.PostAsync<ApiResponse<LoginResponseDto>>(
    "/api/identity/auth/login",
                request);

            if (response?.Data != null && !string.IsNullOrEmpty(response.Data.Token))
            {
                var token = response.Data.Token;

                // Parse JWT to extract userId
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var userIdClaim = jsonToken?.Claims?.FirstOrDefault(c => c.Type == "userid");

                int userId = 0;
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                // Store token and userId in session
                SetAuthToken(token);
                _httpContextAccessor.HttpContext?.Session.SetInt32(UserIdSessionKey, userId);

                // Set token in API client (only for real tokens)
                _apiClient.SetAuthToken(token);

                _logger.LogInformation("Login successful for user: {Username}, UserId: {UserId}",
                  request.Username, userId);

                return new AuthResult
                {
                    Success = true,
                    Token = token,
                    UserId = userId
                };
            }

            _logger.LogWarning("Login failed for user: {Username} - Invalid response", request.Username);
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Giriş başarısız. Lütfen bilgilerinizi kontrol edin."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Username}", request.Username);
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Giriş sırasında bir hata oluştu. Lütfen tekrar deneyin."
            };
        }
    }

    public void SetAuthToken(string token)
    {
        _httpContextAccessor.HttpContext?.Session.SetString(TokenSessionKey, token);
        
        // Only set in API client if it's a real token (not anonymous)
        if (token != AnonymousToken)
    {
  _apiClient.SetAuthToken(token);
   }
    }

    public string? GetAuthToken()
    {
  return _httpContextAccessor.HttpContext?.Session.GetString(TokenSessionKey);
    }

    public void ClearAuthToken()
    {
     _httpContextAccessor.HttpContext?.Session.Remove(TokenSessionKey);
  _httpContextAccessor.HttpContext?.Session.Remove(UserIdSessionKey);
    }
}
