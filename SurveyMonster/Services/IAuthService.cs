using SurveyMonster.Models;
using SurveyMonster.Models.DTOs;
using SurveyMonster.Models.Requests;

namespace SurveyMonster.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request);
    void SetAuthToken(string token);
    string? GetAuthToken();
    void ClearAuthToken();
}
