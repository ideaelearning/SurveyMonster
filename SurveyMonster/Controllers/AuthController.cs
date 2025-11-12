using Microsoft.AspNetCore.Mvc;
using SurveyMonster.Models.Requests;
using SurveyMonster.Services;

namespace SurveyMonster.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private const string IsAnonymousKey = "IsAnonymous";

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // If already logged in, redirect to survey
        if (!string.IsNullOrEmpty(_authService.GetAuthToken()))
        {
            return RedirectToAction("Index", "Survey");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest request, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            var result = await _authService.LoginAsync(request);

            if (result.Success)
            {
                _logger.LogInformation("User logged in successfully: {Username}", request.Username);

                // Mark as authenticated (not anonymous)
                HttpContext.Session.SetString(IsAnonymousKey, "false");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Survey");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login attempt");
            ModelState.AddModelError(string.Empty, "Giriş sırasında bir hata oluştu. Lütfen tekrar deneyin.");
            return View(request);
        }
    }

    [HttpGet]
    public IActionResult AnonymousEntry(string? returnUrl = null)
    {
        try
        {
            // Set a dummy token to pass authentication checks
            var anonymousToken = "ANONYMOUS_USER";
            _authService.SetAuthToken(anonymousToken);

            // Mark as anonymous user
            HttpContext.Session.SetString(IsAnonymousKey, "true");

            _logger.LogInformation("Anonymous user entered the survey");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Survey");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during anonymous entry");
            TempData["Error"] = "Anonim giriş sırasında bir hata oluştu.";
            return RedirectToAction("Login");
        }
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
