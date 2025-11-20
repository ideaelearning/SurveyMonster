using Lms.Shared.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using SurveyMonster.Models.Requests;
using SurveyMonster.Services;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SurveyMonster.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ISurveyService _surveyService;
    private readonly ILogger<AuthController> _logger;
    private const string IsAnonymousKey = "IsAnonymous";
    private const string enc_private_key = "6AmxCMy2y0C3z4tvH6lNBl39N6ZLoiBQ";

    public AuthController(IAuthService authService, ILogger<AuthController> logger, ISurveyService surveyService)
    {
        _authService = authService;
        _logger = logger;
        _surveyService = surveyService;
    }

    [HttpGet]
    public IActionResult Login(string? survey = null)
    {
        // If already logged in, redirect to survey
        if (!string.IsNullOrEmpty(_authService.GetAuthToken()))
        {
            return RedirectToAction("Index", "Survey");
        }

        if (/*!string.IsNullOrEmpty(survey)*/true)
        {
            ViewData["ReturnUrl"] = Uri.UnescapeDataString(CryptographyHelper.Encrypt("30954", enc_private_key)); // test amaçlı  30954 yerine survey gelecek
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest request, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl; // Preserve returnUrl when returning to view
            return View(request);
        }

        try
        {
            var result = await _authService.LoginAsync(request);

            if (result.Success)
            {
                _logger.LogInformation("User logged in successfully: {Username}", request.Username);

                HttpContext.Session.SetString(IsAnonymousKey, "false");
                // Check if returnUrl exists
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    try
                    {
                        var decoded = Uri.UnescapeDataString(returnUrl);
                        long assignmentId = Convert.ToInt64(CryptographyHelper.Decrypt(decoded, enc_private_key));

                        //aynı giriş kontrolü
                        //CheckAssignment(assignmentId,true,hash);

                        return RedirectToAction("Index", "Survey", new { surveyAssignmentId = assignmentId });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error decrypting returnUrl");
                        return RedirectToAction("Index", "Survey");
                    }
                }

                // If no returnUrl, redirect to survey index
                return RedirectToAction("Index", "Survey");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            ViewData["ReturnUrl"] = returnUrl; // Preserve returnUrl when returning to view
            return View(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login attempt");
            ModelState.AddModelError(string.Empty, "Giriş sırasında bir hata oluştu. Lütfen tekrar deneyin.");
            ViewData["ReturnUrl"] = returnUrl; // Preserve returnUrl when returning to view
            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> AnonymousEntry(string? returnUrl = null)
    {
        try
        {
            // Set a dummy token to pass authentication checks
            var anonymousToken = "ANONYMOUS_USER";
            _authService.SetAuthToken(anonymousToken);

            // Mark as anonymous user
            HttpContext.Session.SetString(IsAnonymousKey, "true");
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            var userAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "";
            string hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Regex.Replace(ip + userAgent, @"\s+", ""))));
            _logger.LogInformation("Anonymous user entered the survey");

            try
            {
                var decoded = Uri.UnescapeDataString(returnUrl);
                long assignmentId = Convert.ToInt64(CryptographyHelper.Decrypt(decoded, enc_private_key));

                //aynı giriş kontrolü
                //CheckAssignment(assignmentId,true,hash);

                return RedirectToAction("Index", "Survey", new { surveyAssignmentId = assignmentId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Auth");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during anonymous entry");
            TempData["Error"] = "Anonim giriş sırasında bir hata oluştu.";
            return RedirectToAction("Login");
        }
    }
    private async Task<bool> CheckAssignment(long assignmentId,bool anonymous,string anonymousId)
    {
        //aynı giriş kontrolü
        return await _surveyService.CheckSurveyAssignment(assignmentId, anonymous, anonymousId);

    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        _authService.ClearAuthToken();
        HttpContext.Session.Remove(IsAnonymousKey);
        _logger.LogInformation("User logged out");
        return RedirectToAction("Login");
    }
}
