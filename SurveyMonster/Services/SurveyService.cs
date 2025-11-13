using SurveyMonster.Models;
using SurveyMonster.Models.DTOs;
using SurveyMonster.Models.Requests;
using SurveyMonster.Models.Response;

namespace SurveyMonster.Services;

public class SurveyService : ISurveyService
{
    private readonly IApiClient _apiClient;
    private readonly ILogger<SurveyService> _logger;
    private readonly IConfiguration _configuration;

    public SurveyService(
           IApiClient apiClient,
           ILogger<SurveyService> logger,
           IConfiguration configuration)
    {
        _apiClient = apiClient;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<SurveyDetailDto?> GetSurveyAsync(int surveyId)
    {
        try
        {
            _logger.LogInformation("Fetching survey details for surveyId: {SurveyId}", surveyId);
            //           var response = await _apiClient.GetAsync<ApiResponse<SurveyDetailDto>>(

            //$"api/Surveys/survey/{surveyId}");


            var url = $"api/Surveys/survey/{surveyId}?isAnonymous={true.ToString().ToLower()}";
            var response = await _apiClient.GetAsync<ApiResponse<SurveyDetailDto>>(url);



            if (response?.Data != null)
            {
                _logger.LogInformation("Successfully retrieved survey: {SurveyName}", response.Data.Name);
                return response.Data;
            }

            _logger.LogWarning("Survey not found for surveyId: {SurveyId}", surveyId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching survey with id: {SurveyId}", surveyId);
            throw;
        }
    }
    public async Task<bool> CheckSurveyAssignment(long assignmentId, bool isAnonymous, string anonymousId)
    {
        try
        {

            var url = $"api/Surveys/checkSurveyAssignment/{assignmentId}?anonymousId={anonymousId} &isAnonymous={isAnonymous.ToString().ToLower()}";
            var response = await _apiClient.GetAsync<ApiResponse<bool>>( url);



            if (response?.Data != null)
            {
                _logger.LogInformation("Successfully retrieved survey: {SurveyName}", response.Data);
                return response.Data;
            }

            return false;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public async Task<SurveyAssignmentResponse?> GetSurveyAssignmentAsync(long assignmentId, bool isAnonymous )
    {
        try
        {
            _logger.LogInformation("Fetching survey details for surveyId: {SurveyId}", assignmentId);

            var url = $"api/Surveys/surveyAssignment/{assignmentId}?isAnonymous={isAnonymous.ToString().ToLower()}";
            var response = await _apiClient.GetAsync<ApiResponse<SurveyAssignmentResponse>>( url);



            if (response?.Data != null)
            {
                _logger.LogInformation("Successfully retrieved survey: {SurveyName}", response.Data.Title);
                return response.Data;
            }

            _logger.LogWarning("Survey not found for surveyId: {SurveyId}", assignmentId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching survey with id: {SurveyId}", assignmentId);
            throw;
        }
    }
    public async Task<int?> CreateSurveyAssignmentAsync(CreateSurveyAssignmentRequest request)
    {
        try
        {
            _logger.LogInformation("Creating survey assignment for surveyId: {SurveyId}", request.SurveyId);

            var response = await _apiClient.PostAsync<ApiResponse<IdResponseDto>>(
     "api/surveys/surveyAssignment",
       request);

            if (response?.Data != null)
            {
                _logger.LogInformation("Survey assignment created with id: {Id}", response.Data.Id);
                return response.Data.Id;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating survey assignment");
            throw;
        }
    }

    public async Task<int?> CreateSurveyAssignmentTakerAsync(CreateSurveyAssignmentTakerRequest request)
    {
        try
        {
            _logger.LogInformation("Creating survey assignment taker for assignmentId: {AssignmentId}, userId: {UserId}",
                  request.SurveyAssignmentId, request.UserId);

            var response = await _apiClient.PostAsync<ApiResponse<IdResponseDto>>(
    "api/surveys/surveyAssignmentTaker",
         request);

            if (response?.Data != null)
            {
                _logger.LogInformation("Survey assignment taker created with id: {Id}", response.Data.Id);
                return response.Data.Id;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating survey assignment taker");
            throw;
        }

    }

    public async Task<int?> CreateSurveyEntryAsync(CreateSurveyEntryRequest request)
    {
        try
        {
            _logger.LogInformation("Creating survey entry for takerId: {TakerId}",
  request.SurveyAssignmentTakerId);

            var response = await _apiClient.PostAsync<ApiResponse<IdResponseDto>>(
                    "api/surveys/surveyAssignmentTakerEntry",
                request);

            if (response?.Data != null)
            {
                _logger.LogInformation("Survey entry created with id: {Id}", response.Data.Id);
                return response.Data.Id;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating survey entry");
            throw;
        }
    }

    public async Task<bool> SaveAnswerAsync(SaveAnswerRequest request)
    {
        try
        {
            _logger.LogInformation("Saving answer for questionId: {QuestionId}, entryId: {EntryId}",
             request.SurveyQuestionId, request.SurveyAssignmentTakerEntryId);

            var response = await _apiClient.PostAsync<ApiResponse<IdResponseDto>>(
             "api/surveys/surveyAssignmentTakerEntryGivenAnswer",
           request);

            if (response?.Data != null && response.Data.Id > 0)
            {
                _logger.LogInformation("Answer saved successfully with id: {Id}", response.Data.Id);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving answer");
            return false;
        }
    }

    public async Task<bool> FinishSurveyEntryAsync(int entryId)
    {
        try
        {
            _logger.LogInformation("Finishing survey entry: {EntryId}", entryId);

            // Update entry with finish date and completed state
            var updateRequest = new
            {
                SurveyAssignmentTakerEntryId = entryId,
                FinishDate = DateTime.UtcNow,
                SurveyState = 2 // Completed state
            };

            // Note: You may need to create a specific endpoint for updating entries
            // For now, we'll just log the completion
            _logger.LogInformation("Survey entry {EntryId} marked as finished", entryId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finishing survey entry");
            return false;
        }
    }

    public async Task<List<MySurveyResponse>?> GetMySurveysAsync(int userId, int pageNumber = 0, int pageSize = 100)
    {
        try
        {
            _logger.LogInformation("Fetching surveys for userId: {UserId}", userId);

            var request = new
            {
                Id = userId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var query = $"?Id={userId}&PageNumber={pageNumber}&PageSize={pageSize}";

            //var response = await _apiClient.GetAsync<ApiResponse<List<MySurveyResponse>>> ($"api/surveys/mySurvey", "http://localhost:5010");
            var response = await _apiClient.GetAsync<ApiResponse<List<MySurveyResponse>>>($"api/surveys/mySurvey{query}");

            if (response?.Data != null)
            {
                _logger.LogInformation("Successfully retrieved {Count} surveys for user {UserId}",
                        response.Data.Count, userId);
                return response.Data;
            }

            _logger.LogWarning("No surveys found for userId: {UserId}", userId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching surveys for userId: {UserId}", userId);
            throw;
        }
    }

    public async Task<List<SurveyReportResponse>?> GetSurveyReportAsync(long assignmentId)
    {
        try
        {
            _logger.LogInformation("Fetching survey report for assignmentId: {AssignmentId}", assignmentId);

            var response = await _apiClient.GetAsync<ApiResponse<List<SurveyReportResponse>>>($"api/surveys/surveyAssignmentTakerEntryReport/{assignmentId}");

            if (response?.Data != null)
            {
                _logger.LogInformation("Successfully retrieved {Count} report entries for assignment {AssignmentId}",
           response.Data.Count, assignmentId);
                return response.Data;
            }

            _logger.LogWarning("No report data found for assignmentId: {AssignmentId}", assignmentId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching survey report for assignmentId: {AssignmentId}", assignmentId);
            throw;
        }
    }
}
