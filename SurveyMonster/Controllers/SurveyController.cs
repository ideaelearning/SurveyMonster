using Microsoft.AspNetCore.Mvc;
using SurveyMonster.Models.Requests;
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
    public async Task<IActionResult> Index(int? surveyId = null)
    {
        if (!CheckAuthentication())
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Index", "Survey") });
        }

        try
        {
            // Use default survey ID from configuration if not provided
            var targetSurveyId = surveyId ?? _configuration.GetValue<int>("Survey:DefaultSurveyId", 534);

            var survey = await _surveyService.GetSurveyAsync(targetSurveyId);
            if (survey == null)
            {
                TempData["Error"] = "Anket bulunamadı.";
                return View("Error");
            }

            // Check if user already has an active entry
            var existingEntryId = HttpContext.Session.GetInt32(SurveyEntryIdKey);
            if (existingEntryId.HasValue)
            {
                // User already started the survey
                return RedirectToAction("TakeSurvey", new { entryId = existingEntryId.Value });
            }

            var viewModel = new SurveyInfoViewModel
            {
                SurveyId = survey.Id.Value,
                Name = survey.Name,
                InformationText = survey.InformationText,
                ExpireDate = survey.ExpireDate,
                QuestionCount = survey.SurveySurveyQuestionOrders?.Count ?? 0
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading survey");
            TempData["Error"] = "Anket yüklenirken bir hata oluştu.";
            return View("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartSurvey(int surveyId)
    {
        if (!CheckAuthentication())
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            var isAnonymous = IsAnonymousUser();
            int? userId = null;

            if (!isAnonymous)
            {
                userId = HttpContext.Session.GetInt32(UserIdKey);
                if (!userId.HasValue)
                {
                    TempData["Error"] = "Kullanıcı bilgisi bulunamadı. Lütfen tekrar giriş yapın.";
                    return RedirectToAction("Login", "Auth");
                }
            }

            var tenantId = _configuration.GetValue<int>("Survey:DefaultTenantId", 1);

            var assignmentRequest = new CreateSurveyAssignmentRequest
            {
                SurveyId = surveyId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                EventCategoryId = 5,
                Title = isAnonymous ? "Anonim Anket Katılımı" : "Kullanıcı Anketi",
                Imperative = false,
                SurveyMaxTakeCount = 1,
                ExamSecurityType = 0,
                IsAnonymous = isAnonymous,
            };

            var assignmentId = await _surveyService.CreateSurveyAssignmentAsync(assignmentRequest);
            if (!assignmentId.HasValue)
            {
                TempData["Error"] = "Anket ataması oluşturulamadı.";
                return RedirectToAction("Index");
            }

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            var userAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "";
            string cleaned = Regex.Replace(ip + userAgent, @"\s+", "");
            string hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(cleaned)));

            var takerRequest = new CreateSurveyAssignmentTakerRequest
            {
                SurveyAssignmentId = assignmentId.Value,
                UserId = isAnonymous ? null : userId,
                AnonymousId = hash,
                AnonymousAgent= cleaned,
                IsAnonymous=true
                
            };

            var takerId = await _surveyService.CreateSurveyAssignmentTakerAsync(takerRequest);
            if (!takerId.HasValue)
            {
                TempData["Error"] = "Anket katılımcısı oluşturulamadı.";
                return RedirectToAction("Index");
            }

            // Step 3: Create Survey Entry
            var entryRequest = new CreateSurveyEntryRequest
            {
                SurveyAssignmentTakerId = takerId.Value,
                StartDate = DateTime.UtcNow,
                SurveyState = 1,
                FinishDate = DateTime.UtcNow.AddHours(1),
                Score = 0

            };

            var entryId = await _surveyService.CreateSurveyEntryAsync(entryRequest);
            if (!entryId.HasValue)
            {
                TempData["Error"] = "Anket girişi oluşturulamadı.";
                return RedirectToAction("Index");
            }

            // Store entry ID in session
            HttpContext.Session.SetInt32(SurveyEntryIdKey, entryId.Value);

            return RedirectToAction("TakeSurvey", new { entryId = entryId.Value });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting survey");
            TempData["Error"] = "Anket başlatılırken bir hata oluştu.";
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public async Task<IActionResult> TakeSurvey(int entryId)
    {
        if (!CheckAuthentication())
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            // Get survey ID from session or default
            var surveyId = _configuration.GetValue<int>("Survey:DefaultSurveyId", 534);

            var survey = await _surveyService.GetSurveyAsync(surveyId);
            if (survey == null)
            {
                TempData["Error"] = "Anket bulunamadı.";
                return RedirectToAction("Index");
            }

            var viewModel = new SurveyTakingViewModel
            {
                SurveyId = survey.Id.Value,
                SurveyName = survey.Name,
                EntryId = entryId,
                Questions = survey.SurveySurveyQuestionOrders?
                .OrderBy(q => q.Order)
             .Select(q => new QuestionViewModel
             {
                 QuestionId = q.SurveyQuestion.Id.Value,
                 QuestionText = q.SurveyQuestion.Text,
                 Order = q.Order.Value,
                 Options = q.SurveyQuestion.SurveySurveyQuestionOptions
                   .Select(o => new OptionViewModel
                   {
                       OptionId = o.Id.Value,
                       OptionText = o.Text,
                       Value = o.Value
                    }).ToList()
             }).OrderBy(a=>a.Order).ToList() ?? new List<QuestionViewModel>()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading survey for taking");
            TempData["Error"] = "Anket yüklenirken bir hata oluştu.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitSurvey(int entryId, Dictionary<int, string> answers)
    {
        if (!CheckAuthentication())
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            var tenantId = _configuration.GetValue<int>("Survey:DefaultTenantId", 1);

            // Save all answers
            foreach (var answer in answers)
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
                }
            }

            // Finish survey entry
            await _surveyService.FinishSurveyEntryAsync(entryId);

            // Clear session
            HttpContext.Session.Remove(SurveyEntryIdKey);

            TempData["Success"] = "Anket başarıyla tamamlandı. Katılımınız için teşekkür ederiz!";
            return RedirectToAction("Completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting survey");
            TempData["Error"] = "Anket gönderilirken bir hata oluştu.";
            return RedirectToAction("TakeSurvey", new { entryId });
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
            TempData["Error"] = "Anonim kullanıcılar anket geçmişini görüntüleyemez.";
            return RedirectToAction("Index", "Survey");
        }

        try
        {
            var userId = HttpContext.Session.GetInt32(UserIdKey);
            if (!userId.HasValue)
            {
                TempData["Error"] = "Kullanıcı bilgisi bulunamadı. Lütfen tekrar giriş yapın.";
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user surveys");
            TempData["Error"] = "Anketler yüklenirken bir hata oluştu.";
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
            TempData["Error"] = "Anonim kullanıcılar rapor görüntüleyemez.";
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
                    QuestionOrder = r.QuestionOrder,
                    QuestionText = r.QuestionText ?? "",
                    Answer = r.Answer ?? r.AnswerOrj ?? "",
                    EntryDate = r.EntryStartDate.GetValueOrDefault(),
                    CompletionStatus = r.CompletionStatus
                }).ToList()
            };

            // Group by question for better display
            viewModel.GroupedByQuestion = viewModel.Answers
                .GroupBy(a => a.QuestionText)
                .ToDictionary(g => g.Key, g => g.ToList());

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading survey report");
            TempData["Error"] = "Rapor yüklenirken bir hata oluştu.";
            return RedirectToAction("MySurveys");
        }
    }
}
