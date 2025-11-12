using SurveyMonster.Models.Response;

namespace SurveyMonster.Services;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string endpoint, string baseUrl = "");
    Task<T?> PostAsync<T>(string endpoint, object data, string baseUrl = "");
    void SetAuthToken(string token);
}
