using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyMonster.Helpers;
using SurveyMonster.Models;
using SurveyMonster.Models.Requests;
using SurveyMonster.Models.Response;
using SurveyMonster.Models.ViewModels;
using SurveyMonster.Services;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SurveyMonster.Controllers;

public class SurveyController : Controller
{
    private readonly ISurveyService _surveyService;
    private readonly IAuthService _authService;
    private readonly ILogger<SurveyController> _logger;
    private readonly IConfiguration _configuration;
    private const string SurveyEntryIdKey = "SurveyEntryId";
    private const string SurveyAssignmentId = "SurveyAssignmentId";
    private const string UserIdKey = "UserId";
    private const string IsAnonymousKey = "IsAnonymous";
    public SurveyController(
        ISurveyService surveyService,
        IAuthService authService,
        ILogger<SurveyController> logger,
     IConfiguration configuration)
    {
        _surveyService = surveyService;
        _authService = authService;
        _logger = logger;
        _configuration = configuration;
    }

    private bool CheckAuthentication()
    {
        var token = _authService.GetAuthToken();
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }
        return true;
    }

    private bool IsAnonymousUser()
    {
        var isAnonymous = HttpContext.Session.GetString(IsAnonymousKey);
        return isAnonymous == "true";
    }

    [HttpGet]
    public async Task<IActionResult> Index(long? surveyAssignmentId = null)
    {
        if (!CheckAuthentication())
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Index", "Survey") });
        }

        try
        {
          
            // Use default survey ID from configuration if not provided
            var targetSurveyId = surveyAssignmentId ?? _configuration.GetValue<int>("Survey:DefaultSurveyId", 30954);

            var survey = await _surveyService.GetSurveyAssignmentAsync(targetSurveyId, IsAnonymousUser());
            if (survey == null)
            {
                TempData["Error"] = ErrorMessages.SurveyNotFound;
                return View("Error");
            }

            // Check if user already has an active entry
            var existingEntryId = HttpContext.Session.GetInt32(SurveyEntryIdKey);
            var existingSurveyAssignmentId = HttpContext.Session.GetString(SurveyAssignmentId);
            if (existingEntryId.HasValue && !String.IsNullOrEmpty(existingSurveyAssignmentId))
            {
                // User already started the survey
                return RedirectToAction("TakeSurvey", new { entryId = existingEntryId.Value , assignmentId=Convert.ToInt64(existingSurveyAssignmentId) });
            }

            var isAnonymous = IsAnonymousUser();

            var viewModel = new SurveyInfoViewModel
            {
                SurveyId = survey.Id,
                Name = survey.Title,
                InformationText = survey.Details,
                ExpireDate = survey.EndDate,
                QuestionCount = survey.Survey.SurveySurveyQuestionOrders?.Count(a => a.SurveyQuestion != null) ?? 0,
                RequiresAnonymousInfo = isAnonymous,
                RequiredUserInformations = survey.RequiredUserInformationsList
            };

            return View(viewModel);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "API request failed while loading survey");
            TempData["Error"] = ErrorMessages.ApiConnectionError;
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading survey");
            TempData["Error"] = ErrorMessages.SurveyLoadError;
            return View("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartSurvey(long surveyAssignmetId, string? userInfoJson)
    {
        if (!CheckAuthentication())
        {
            return RedirectToAction("Login", "Auth");
        }
   
        try
        {
            var isAnonymous = IsAnonymousUser();
            int? userId = null;
            // Validate and store user info if provided (dynamic user information)
            if (isAnonymous && !string.IsNullOrEmpty(userInfoJson))
            {
                try
                {
                    // Validate JSON format
                    var userInfo = System.Text.Json.JsonSerializer.Deserialize<RequiredUserInformationsResponse>(userInfoJson);
                    
                    //if (userInfo == null)
                    //{
                    //    TempData["Error"] = ErrorMessages.AnonymousInfoRequired;
                    //    return RedirectToAction("Index", new { surveyAssignmetId });
                    //}

                    //// Store user info in session as JSON
                    //HttpContext.Session.SetString("AnonymousUserInfo", userInfoJson);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse user info JSON");
                    TempData["Error"] = ErrorMessages.DataProcessingError;
                    return RedirectToAction("Index", new { surveyAssignmetId });
                }
            }
            else if (!isAnonymous)
            {
                userId = HttpContext.Session.GetInt32(UserIdKey);
                if (!userId.HasValue)
                {
                    TempData["Error"] = ErrorMessages.UserInfoNotFound;
                    return RedirectToAction("Login", "Auth");
                }
            }

            var tenantId = _configuration.GetValue<int>("Survey:DefaultTenantId", 1);

            //var assignmentRequest = new CreateSurveyAssignmentRequest
            //{
            //    SurveyId = surveyId,
            //    StartDate = DateTime.UtcNow,
            //    EndDate = DateTime.UtcNow.AddDays(30),
            //    EventCategoryId = 5,
            //    Title = isAnonymous ? "Anonim Anket Katılımı" : "Kullanıcı Anketi",
            //    Imperative = false,
            //    SurveyMaxTakeCount = 1,
            //    ExamSecurityType = 0,
            //    IsAnonymous = isAnonymous,
            //};

            //var assignmentId = await _surveyService.CreateSurveyAssignmentAsync(assignmentRequest);
            //if (!assignmentId.HasValue)
            //{
            //    TempData["Error"] = "Anket ataması oluşturulamadı.";
            //    return RedirectToAction("Index");
            //}

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            var userAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "";
            string cleaned = Regex.Replace(ip + userAgent, @"\s+", "");
            string hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(cleaned)));

            /// Aynı ip giriş kontrolü

            //var result = await _surveyService.CheckSurveyAssignment(surveyAssignmetId, true, hash);
            //if (result)
            //{
            //    TempData["Error"] = "Anonim giriş sırasında bir hata oluştu.";
            //    return RedirectToAction("Login","Auth");
            //}
            var takerRequest = new CreateSurveyAssignmentTakerRequest
            {
                SurveyAssignmentId = surveyAssignmetId, ////////// BU ID URL'DEN GELECEK !!!!!!!!!!!
                UserId = isAnonymous ? null : userId,
                AnonymousId = hash,
                AnonymousAgent= cleaned,
                IsAnonymous=true,
                UserInformations = userInfoJson

            };

            var takerId = await _surveyService.CreateSurveyAssignmentTakerAsync(takerRequest);
            if (!takerId.HasValue)
            {
                TempData["Error"] = ErrorMessages.SurveyTakerCreateError;
                return RedirectToAction("Index");
            }

            // Step 3: Create Survey Entry
            var entryRequest = new CreateSurveyEntryRequest
            {
                SurveyAssignmentTakerId = takerId.Value,
                StartDate = DateTime.UtcNow,
                SurveyState = 1,
                FinishDate = DateTime.UtcNow.AddHours(1),
                Score = 0,
                IsAnonymous = true

            };

            var entryId = await _surveyService.CreateSurveyEntryAsync(entryRequest);
            if (!entryId.HasValue)
            {
                TempData["Error"] = ErrorMessages.SurveyEntryCreateError;
                return RedirectToAction("Index");
            }

            // Store entry ID in session
            try
            {
                HttpContext.Session.SetInt32(SurveyEntryIdKey, entryId.Value);
                HttpContext.Session.SetString(SurveyAssignmentId, surveyAssignmetId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store entry ID in session");
                TempData["Error"] = ErrorMessages.SessionExpired;
                return RedirectToAction("Index");
            }

            return RedirectToAction("TakeSurvey", new { entryId = entryId.Value, assignmentId = surveyAssignmetId });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "API request failed while starting survey");
            TempData["Error"] = ErrorMessages.ApiConnectionError;
            return RedirectToAction("Index");
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogError(ex, "JSON processing error while starting survey");
            TempData["Error"] = ErrorMessages.DataProcessingError;
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting survey");
            TempData["Error"] = ErrorMessages.SurveyStartError;
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public async Task<IActionResult> TakeSurvey(int entryId,long assignmentId)
    {
        if (!CheckAuthentication())
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            // Validate entry ID exists in session
            var sessionEntryId = HttpContext.Session.GetInt32(SurveyEntryIdKey);
            if (!sessionEntryId.HasValue || sessionEntryId.Value != entryId)
            {
                _logger.LogWarning("Entry ID mismatch or session expired. Session: {SessionId}, Requested: {EntryId}", 
                    sessionEntryId, entryId);
                TempData["Error"] = ErrorMessages.SessionTimeout;
                return RedirectToAction("Index");
            }


            var survey = await _surveyService.GetSurveyAssignmentAsync(assignmentId, IsAnonymousUser());
            if (survey == null)
            {
                TempData["Error"] = ErrorMessages.SurveyNotFound;
                return RedirectToAction("Index");
            }

            // Get all questions ordered - load once for client-side navigation
            var questions = survey.Survey.SurveySurveyQuestionOrders?
                .OrderBy(q => q.Order)
                .Select(q => new QuestionViewModel
                {
                    QuestionId = q.SurveyQuestion.Id.Value,
                    QuestionText = q.SurveyQuestion.Text,
                    Order = q.Order.Value,
                    QuestionTypeId = (int)q.SurveyQuestion.SurveyQuestionTypeId,
                    QuestionTypeName = q.SurveyQuestion.SurveyQuestionType?.Name ?? "",
                    Options = q.SurveyQuestion.SurveySurveyQuestionOptions
                        .Select(o => new OptionViewModel
                        {
                            OptionId = o.Id.Value,
                            OptionText = o.Text,
                            Value = o.Value,
                            IsOther = o.IsOther
                        }).ToList()
                }).ToList() ?? new List<QuestionViewModel>();

            if (questions.Count == 0)
            {
                TempData["Error"] = "Ankette soru bulunamadı.";
                return RedirectToAction("Index");
            }

            // Pass all questions to ViewBag for JavaScript
            ViewBag.AllQuestions = questions;

            var viewModel = new QuestionNavigationViewModel
            {
                SurveyId = survey.Id,
                SurveyName = survey.Survey.Name,
                EntryId = entryId,
                CurrentQuestionIndex = 0,
                TotalQuestions = questions.Count,
                CurrentQuestion = questions[0],
                SavedAnswers = new Dictionary<int, string>()
            };

            return View(viewModel);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "API request failed while loading survey for taking");
            TempData["Error"] = ErrorMessages.ApiConnectionError;
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading survey for taking");
            TempData["Error"] = ErrorMessages.SurveyLoadError;
            return RedirectToAction("Index");
        }
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitSurvey(int entryId, string answersJson)
    {
        if (!CheckAuthentication())
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            // Validate session
            var sessionEntryId = HttpContext.Session.GetInt32(SurveyEntryIdKey);
            if (!sessionEntryId.HasValue || sessionEntryId.Value != entryId)
            {
                _logger.LogWarning("Session expired or entry ID mismatch while submitting survey");
                TempData["Error"] = ErrorMessages.SessionTimeout;
                return RedirectToAction("Index");
            }

            var tenantId = _configuration.GetValue<int>("Survey:DefaultTenantId", 1);

            // Parse answers from JSON
            if (string.IsNullOrEmpty(answersJson))
            {
                TempData["Error"] = ErrorMessages.NoAnswersFound;
                return RedirectToAction("TakeSurvey", new { entryId });
            }

            Dictionary<int, string> savedAnswers;
            try
            {
                savedAnswers = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, string>>(answersJson) ?? new Dictionary<int, string>();
            }
            catch (System.Text.Json.JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize answers JSON during submit");
                TempData["Error"] = ErrorMessages.DataProcessingError;
                return RedirectToAction("TakeSurvey", new { entryId });
            }

            // Get anonymous user info from session if exists
            var anonymousInfoJson = HttpContext.Session.GetString("AnonymousUserInfo");
            
            // Save anonymous user info as a special answer (questionId = 0) if exists
            if (!string.IsNullOrEmpty(anonymousInfoJson))
            {
                try
                {
                    var userInfoRequest = new SaveAnswerRequest
                    {
                        SurveyAssignmentTakerEntryId = entryId,
                        SurveyQuestionId = 0, // Special ID for anonymous user info
                        Answer = anonymousInfoJson,
                        IsEmpty = false,
                        TenantId = tenantId
                    };

                    var savedUserInfo = await _surveyService.SaveAnswerAsync(userInfoRequest);
                    if (!savedUserInfo)
                    {
                        _logger.LogWarning("Failed to save anonymous user info for entry {EntryId}", entryId);
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "API request failed while saving anonymous user info");
                    TempData["Error"] = ErrorMessages.ApiConnectionError;
                    return RedirectToAction("TakeSurvey", new { entryId });
                }
            }

            // Save all answers
            var failedAnswers = new List<int>();
            foreach (var answer in savedAnswers)
            {
                try
                {
                    var saveRequest = new SaveAnswerRequest
                    {
                        SurveyAssignmentTakerEntryId = entryId,
                        SurveyQuestionId = answer.Key,
                        Answer = answer.Value,
                        IsEmpty = string.IsNullOrEmpty(answer.Value),
                        TenantId = tenantId
                    };

                    var saved = await _surveyService.SaveAnswerAsync(saveRequest);
                    if (!saved)
                    {
                        _logger.LogWarning("Failed to save answer for question {QuestionId}", answer.Key);
                        failedAnswers.Add(answer.Key);
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "API request failed while saving answer for question {QuestionId}", answer.Key);
                    failedAnswers.Add(answer.Key);
                }
            }

            // If some answers failed to save, notify user
            if (failedAnswers.Any())
            {
                _logger.LogError("Failed to save {Count} answers: {QuestionIds}", failedAnswers.Count, string.Join(", ", failedAnswers));
                TempData["Error"] = $"{ErrorMessages.ApiRequestError} Bazı cevaplar kaydedilemedi.";
                return RedirectToAction("TakeSurvey", new { entryId });
            }

            // Finish survey entry
            try
            {
                await _surveyService.FinishSurveyEntryAsync(entryId);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "API request failed while finishing survey entry");
                TempData["Error"] = ErrorMessages.ApiConnectionError;
                return RedirectToAction("TakeSurvey", new { entryId });
            }

            // Clear session
            try
            {
                HttpContext.Session.Remove(SurveyEntryIdKey);
                HttpContext.Session.Remove(SurveyAssignmentId);
                HttpContext.Session.Remove("AnonymousUserInfo");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clear session data after survey completion");
                // Continue anyway as survey is already submitted
            }

            TempData["Success"] = "Anket başarıyla tamamlandı. Katılımınız için teşekkür ederiz!";
            return RedirectToAction("Completed");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "API request failed while submitting survey");
            TempData["Error"] = ErrorMessages.ApiConnectionError;
            return RedirectToAction("TakeSurvey", new { entryId });
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogError(ex, "JSON processing error while submitting survey");
            TempData["Error"] = ErrorMessages.DataProcessingError;
            return RedirectToAction("TakeSurvey", new { entryId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting survey");
            TempData["Error"] = ErrorMessages.SurveySubmitError;
            return RedirectToAction("TakeSurvey", new { entryId });
        }
    }

    [HttpGet]
    public async Task<IActionResult> PreviewSurvey(int surveyId)
    {
        if (!CheckAuthentication())
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            var survey = await _surveyService.GetSurveyAssignmentAsync(surveyId,IsAnonymousUser());
            if (survey == null)
            {
                return StatusCode(404, ErrorMessages.SurveyNotFound);
            }

            var viewModel = new SurveyPreviewViewModel
            {
                SurveyId = survey.Id,
                SurveyName = survey.Title,
                Questions = survey.Survey.SurveySurveyQuestionOrders?
                    .OrderBy(q => q.Order)
                    .Select(q => new PreviewQuestionViewModel
                    {
                        QuestionId = q.SurveyQuestion.Id.Value,
                        QuestionText = q.SurveyQuestion.Text,
                        Order = q.Order.Value,
                        QuestionTypeId = (int)q.SurveyQuestion.SurveyQuestionTypeId,
                      //  QuestionTypeName = q.SurveyQuestion.SurveyQuestionType.Name,
                        Options = q.SurveyQuestion.SurveySurveyQuestionOptions
                            .Select(o => new OptionViewModel
                            {
                                OptionId = o.Id.Value,
                                OptionText = o.Text,
                                Value = o.Value,
                                IsOther = o.IsOther
                            }).ToList()
                    }).ToList() ?? new List<PreviewQuestionViewModel>()
            };

            return PartialView("_PreviewModal", viewModel);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "API request failed while loading survey preview");
            return StatusCode(500, ErrorMessages.ApiConnectionError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading survey preview");
            return StatusCode(500, ErrorMessages.PreviewLoadError);
        }
    }

    [HttpGet]
    public IActionResult Completed()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> MySurveys()
    {
        if (!CheckAuthentication())
        {
            return RedirectToAction("Login", "Auth");
        }

        // Anonymous users cannot view their survey history
        if (IsAnonymousUser())
        {
            TempData["Error"] = ErrorMessages.AnonymousUserRestriction;
            return RedirectToAction("Index", "Survey");
        }

        try
        {
            var userId = HttpContext.Session.GetInt32(UserIdKey);
            if (!userId.HasValue)
            {
                TempData["Error"] = ErrorMessages.UserInfoNotFound;
                return RedirectToAction("Login", "Auth");
            }

            var mySurveys = await _surveyService.GetMySurveysAsync(userId.Value);
            if (mySurveys == null || !mySurveys.Any())
            {
                var emptyViewModel = new MySurveysViewModel();
                return View(emptyViewModel);
            }

            var viewModel = new MySurveysViewModel
            {
                Surveys = mySurveys.Select(s => new MySurveyItemViewModel
                {
                    Id = s.Id,
                    SurveyAssignmentId = s.SurveyAssignmentId,
                    SurveyId = s.SurveyAssignment?.Survey?.Id ?? 0,
                    SurveyName = s.SurveyAssignment?.Survey?.Name ?? "Bilinmeyen Anket",
                    Title = s.SurveyAssignment?.Title ?? "",
                    InformationText = s.SurveyAssignment?.Survey?.InformationText ?? "",
                    StartDate = s.SurveyAssignment?.StartDate ?? DateTime.MinValue,
                    EndDate = s.SurveyAssignment?.EndDate ?? DateTime.MinValue,
                    CompletionStatus = s.CompletionStatus,
                    CompletionPercentage = s.CompletionPercentage,
                    EntryCount = s.EntryCount,
                    SurveyEntryCompletedCount = s.SurveyEntryCompletedCount,
                    MaxTakeCount = s.SurveyAssignment?.SurveyMaxTakeCount ?? 0,
                    IsExpired = s.SurveyAssignment?.IsExpired ?? false,
                    IsActive = s.SurveyAssignment?.IsActive ?? false
                }).ToList()
            };

            return View(viewModel);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "API request failed while loading user surveys");
            TempData["Error"] = ErrorMessages.ApiConnectionError;
            return View(new MySurveysViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user surveys");
            TempData["Error"] = ErrorMessages.SurveyLoadError;
            return View(new MySurveysViewModel());
        }
    }

    [HttpGet]
    public async Task<IActionResult> SurveyReport(long assignmentId)
    {
        if (!CheckAuthentication())
        {
            return RedirectToAction("Login", "Auth");
        }

        // Anonymous users cannot view reports
        if (IsAnonymousUser())
        {
            TempData["Error"] = ErrorMessages.AnonymousUserRestriction;
            return RedirectToAction("Index", "Survey");
        }

        try
        {
            var reportData = await _surveyService.GetSurveyReportAsync(assignmentId);
            if (reportData == null || !reportData.Any())
            {
                TempData["Error"] = "Rapor bulunamadı.";
                return RedirectToAction("MySurveys");
            }

            var firstEntry = reportData.First();
            var viewModel = new SurveyReportViewModel
            {
                AssignmentId = firstEntry.AsgId,
                AssignmentTitle = firstEntry.AsgTitle ?? "",
                SurveyId = (int)firstEntry.SurveyId,
                SurveyName = firstEntry.SurveyName ?? "",
                StartDate = firstEntry.AsgStartDate.GetValueOrDefault(),
                EndDate = firstEntry.AsgEndDate.GetValueOrDefault(),
                Answers = reportData.Select(r => new ReportAnswerViewModel
                {
                    AnswerId = r.AnswerId.GetValueOrDefault(),
                    UserName = r.UserName ?? "",
                    FullName = $"{r.Name} {r.Surname}".Trim(),
                    EmailAddress = r.EmailAddress ?? "",
                    PersonnelNo = r.PersonnelNo ?? "",
                    QuestionOrder = r.QuestionOrder,
                    SurveyQuestionId = r.SurveyQuestionId,
                    QuestionText = r.QuestionText ?? "",
                    QuestionTypeId = r.QuestionTypeId,
                    QuestionTypeName = r.QuestionTypeName ?? "",
                    Answer = r.Answer ?? r.AnswerOrj ?? "",
                    AnswerOrj = r.AnswerOrj ?? "",
                    AnswerOther = r.AnswerOther ?? "",
                    EntryId = r.EntryId,
                    EntryDate = r.EntryStartDate.GetValueOrDefault(),
                    EntryFinishDate = r.EntryFinishDate,
                    CompletionStatus = r.CompletionStatus
                }).ToList()
            };

            // Group by question for better display
            viewModel.GroupedByQuestion = viewModel.Answers
                .GroupBy(a => a.QuestionText)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Group by question and user for multiple choice display
            viewModel.GroupedByQuestionAndUser = viewModel.Answers
                .GroupBy(a => a.QuestionText)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(a => a.UserName)
                          .ToDictionary(ug => ug.Key, ug => ug.ToList())
                );

            return View(viewModel);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "API request failed while loading survey report");
            TempData["Error"] = ErrorMessages.ApiConnectionError;
            return RedirectToAction("MySurveys");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading survey report");
            TempData["Error"] = "Rapor yüklenirken bir hata oluştu.";
            return RedirectToAction("MySurveys");
        }
    }
}
