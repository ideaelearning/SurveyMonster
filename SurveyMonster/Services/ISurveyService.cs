using SurveyMonster.Models;
using SurveyMonster.Models.DTOs;
using SurveyMonster.Models.Requests;
using SurveyMonster.Models.Response;

namespace SurveyMonster.Services;

public interface ISurveyService
{
    Task<SurveyDetailDto?> GetSurveyAsync(int surveyId);
    Task<int?> CreateSurveyAssignmentAsync(CreateSurveyAssignmentRequest request);
    Task<int?> CreateSurveyAssignmentTakerAsync(CreateSurveyAssignmentTakerRequest request);
    Task<int?> CreateSurveyEntryAsync(CreateSurveyEntryRequest request);
    Task<bool> SaveAnswerAsync(SaveAnswerRequest request);
    Task<bool> FinishSurveyEntryAsync(int entryId);
    Task<List<MySurveyResponse>?> GetMySurveysAsync(int userId, int pageNumber = 0, int pageSize = 100);
    Task<List<SurveyReportResponse>?> GetSurveyReportAsync(long assignmentId);
}
